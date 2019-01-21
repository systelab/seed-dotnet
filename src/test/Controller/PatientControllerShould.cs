namespace Test.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Allure.builder;
    using Allure.builder.attributes;
    using Allure.Commons;

    using AutoFixture;

    using AutoMapper;
    using main.Contracts;
    using main.Contracts.Repository;
    using main.Controllers.Api;
    using main.Entities.Common;
    using main.Entities.Models;
    using main.Entities.ViewModels;
    using main.Extensions;
    using main.Services;




    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Newtonsoft.Json;

    using PagedList.Core;

    using Xunit;
    using Xunit.Abstractions;

    using Assert = Xunit.Assert;

    public class PatientControllerShould
    {
        private const string MockedMedicalRecordNumber = "MR_";
        private readonly IMapper mapper;

        private readonly Mock<IUnitOfWork> mockRepositories;

        private readonly Mock<IPatientRepository> mockPatientRepository;

        private readonly Mock<IAllergyRepository> mockAllergyRepository;

        private readonly Mock<IMedicalRecordNumberService> medicalRecordMock;

        private readonly PatientControllerBuilder sutBuilder;

        private List<Patient> listOfPatients;

        private readonly ILogger<PatientController> xunitLogger;

        public PatientControllerShould(ITestOutputHelper testOutputHelper)
        {
            Mapper.Reset();
            var automapConfiguration = new SeedMapperConfiguration();
            this.medicalRecordMock = new Mock<IMedicalRecordNumberService>();
            this.medicalRecordMock.Setup(p => p.GetMedicalRecordNumber(It.IsAny<string>())).Returns(MockedMedicalRecordNumber);
            this.xunitLogger = new XunitLogger<PatientController>(testOutputHelper);


            this.mapper = automapConfiguration.CreateMapper();
            this.sutBuilder = new PatientControllerBuilder(this.mapper, this.medicalRecordMock.Object).WithLogger(this.xunitLogger);
            this.mockRepositories = new Mock<IUnitOfWork>();
            this.mockPatientRepository = new Mock<IPatientRepository>();
            this.mockAllergyRepository = new Mock<IAllergyRepository>();


            this.mockRepositories.Setup(p => p.Patients).Returns(this.mockPatientRepository.Object);
            this.mockRepositories.Setup(p => p.Allergies).Returns(this.mockAllergyRepository.Object);

            this.InitializeData();
            Test.createInstance();


        }

        [Fact(DisplayName = "Create Patient Return Bad Request Given Invalid Patient")]
        [Trait("Category", "Unit")]
        public async Task CreatePatient_ReturnsBadRequest_GivenInvalidPatient()
        {
            string testId = string.Empty;
            try
            {
                testId = Test.addTest(
                    new testDefinition
                        {
                            name = "Create Patient Return Bad Request Given Invalid Patient",
                            description = "Return a bad request if you provide a invalid Patient",
                            storyName = "Patient Creation",
                            featureName = "Negative Tests",
                            epicName = "Unit Tests"
                        });

                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });

                // Arrange
                PatientController sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
                sut.ModelState.AddModelError("error", "some error");
                Test.stopStep(Status.passed);

                Test.addStep(
                    new step
                        {
                            description = "Act",
                            name = "Step 2: Act",
                            listParamenters =
                                new List<Parameter> { { new Parameter { name = "Patient", value = null } } }
                        });

                // Act
                var result = await sut.CreatePatient(patient: null);
                Test.stopStep(Status.passed);

                Test.addStep(new step { description = "Assert", name = "Step 3: Assert" });

                // Assert
                Assert.IsType<BadRequestObjectResult>(result);
                Test.stopStep(Status.passed);

                Test.stopTest(testId, Status.passed, "Test success", "Passed");
            }
            catch (Exception ex)
            {
                Test.stopStep(Status.failed);
                Test.stopTest(testId, Status.passed, "Test failed", ex.ToString());
                Assert.True(false);
            }
        }

        [Fact(DisplayName = "Create a new Patient")]
        [Trait("Category", "Unit")]
        public async Task CreatePatient_Should_Create_A_New_Patient()
        {
            string testId = string.Empty;
            try
            {
                testId = Test.addTest(
                    new testDefinition
                        {
                            name = "Create a new Patient",
                            description = "This test should create a new patient",
                            storyName = "Patient Creation",
                            featureName = "Positive Tests",
                            epicName = "Unit Tests"
                        });

                // Arrange
                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                PatientController sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
                PatientViewModel patient = new PatientViewModel
                                               {
                                                   Name = "Carlos", Surname = "Carmona", Email = "ccarmona@werfen.com"
                                               };
                
                Test.stopStep(Status.passed);

                // Act
                Test.addStep(
                    new step
                        {
                            description = "Act",
                            name = "Step 2: Act",
                            listParamenters = new List<Parameter>
                                                  {
                                                      {
                                                          new Parameter
                                                              {
                                                                  name = "Email", value = "ccarmona@werfen.com"
                                                              }
                                                      },
                                                      { new Parameter { name = "Name", value = "Carlos" } },
                                                      { new Parameter { name = "Surname", value = "Carmona" } },
                                                  }
                        });

                var result = await sut.CreatePatient(patient);

                Test.stopStep(Status.passed);

                // Assert
                Test.addStep(new step { description = "Assert", name = "Step 3: Assert" });
                var viewResult = Assert.IsType<OkObjectResult>(result);
                var model = Assert.IsType<PatientViewModel>(viewResult.Value);
                this.mockPatientRepository.Verify(repo => repo.Add(It.IsAny<Patient>()));
                Assert.Equal(patient.Email, model.Email);
                Assert.Equal(patient.Name, model.Name);
                Assert.Equal(patient.Surname, model.Surname);
                Assert.Equal(MockedMedicalRecordNumber, model.MedicalNumber);

                Test.stopStep(Status.passed);
                Test.stopTest(testId, Status.passed, "Test success", "Passed");
            }
            catch (Exception ex)
            {
                Test.stopStep(Status.failed);
                Test.stopTest(testId, Status.passed, "Test failed", ex.ToString());
                Assert.True(false, ex.ToString());
            }
        }

        [Fact(DisplayName = "Create Patient Constructor Null Parameters")]
        [Trait("Category", "Unit")]
        public void CreatePatientConstructor_NullParameters_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            string testId = string.Empty;
            try
            {
                testId = Test.addTest(
                    new testDefinition
                        {
                            name = "Create Patient Constructor Null Parameters",
                            description =
                                "Create Patient Constructor with Null Parameters should Throws ArgumentNullException",
                            storyName = "Patient Creation",
                            featureName = "Negative Tests",
                            epicName = "Unit Tests",
                            listLinks = new List<link>
                                            {
                                                new link
                                                    {
                                                        isIssue = false, name = "RQS-62356", url = "http://google.com"
                                                    }
                                            }
                        });

                Assert.Throws<ArgumentNullException>(() => { this.sutBuilder.WithRepository(null).Build(); });
                Assert.Throws<ArgumentNullException>(() => { this.sutBuilder.WithLogger(null).Build(); });
                Assert.Throws<ArgumentNullException>(() => { this.sutBuilder.WithMapper(null).Build(); });

                Test.stopTest(testId, Status.passed, "Test success", "This was fantastic");
            }
            catch (Exception ex)
            {
                Test.stopTest(testId, Status.passed, "Test failed", ex.ToString());
                Assert.True(false);
            }
        }

        [Fact(DisplayName = "Get All Patients (after last page)")]
        [Trait("Category", "Unit")]
        public async Task GetAllPatients_Paged_Should_Return_List_Of_Patients_After_Last_Page()
        {
            int totalElements = 90;
            int page = 5;
            int elementsPerPage = 18;

            string testId = string.Empty;
            IQueryable<Patient> list = this.GetPatientList(totalElements);
            try
            {
                testId = Test.addTest(
                    new testDefinition
                        {
                            name = "Get list of patients",
                            description = "This test should return a list of patients",
                            storyName = "Patient Retrieve",
                            featureName = "Positive Tests",
                            epicName = "Unit Tests"
                        });

                // Arrange
                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                this.mockPatientRepository.Setup(repo => repo.GetAllWithPaginationPatients(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(
                    new PagedList<Patient>(list, page + 1, elementsPerPage));
                PatientController sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
                Test.stopStep(Status.passed);

                // Act
                Test.addStep(
                    new step
                        {
                            description = $" Get all the patients of page {page}, just {elementsPerPage} items",
                            name = "Step 2: Act"
                        });
                var result = await sut.GetAllPatients(page, 18);
                Test.stopStep(Status.passed);

                // Assert
                Test.addStep(
                    new step
                        {
                            description = "Check if the number the patients returned is correct",
                            name = "Step 3: Assert"
                        });
                var viewResult = Assert.IsType<OkObjectResult>(result);
                var model = Assert.IsAssignableFrom<ExtendedPagedList<PatientViewModel>>(viewResult.Value);
                Assert.Equal(0, model.NumberOfElements);
                Assert.Equal(page, model.Number);
                Assert.Equal(totalElements, model.TotalElements);

                // the list shall be empty
                Assert.False(model.Content.Any());
                Test.stopStep(Status.passed);
                Test.stopTest(testId, Status.passed, "Test success", "Passed");
            }
            catch (Exception ex)
            {
                Test.stopStep(Status.failed);
                Test.stopTest(testId, Status.passed, "Test failed", ex.ToString());
                Assert.True(false, ex.ToString());
            }
        }

        [Fact(DisplayName = "Get All Patients (first page)")]
        [Trait("Category", "Unit")]
        public async Task GetAllPatients_Paged_Should_Return_List_Of_Patients_First_Page()
        {
            await this.GetPagedPatient(0, 90, 18);
        }

        [Fact(DisplayName = "Get All Patients (last page)")]
        [Trait("Category", "Unit")]
        public async Task GetAllPatients_Paged_Should_Return_List_Of_Patients_Last_Page()
        {
            await this.GetPagedPatient(4, 90, 18);
        }

        [Fact(DisplayName = "Get All Patients (middle page)")]
        [Trait("Category", "Unit")]
        public async Task GetAllPatients_Paged_Should_Return_List_Of_Patients_Middle_Page()
        {
            await this.GetPagedPatient(3, 90, 18);
        }

        [Fact(DisplayName = "Get Patient")]
        [Trait("Category", "Unit")]
        public async Task GetPatients_Should_Return_Patient_Information()
        {
            string testId = string.Empty;
            try
            {
                testId = Test.addTest(
                    new testDefinition
                        {
                            name = "Get list of patients",
                            description = "This test should return a patient information",
                            storyName = "Patient Retrieve",
                            featureName = "Positive Tests",
                            epicName = "Unit Tests"
                        });

                // Arrange
                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                this.mockPatientRepository.Setup(repo => repo.Get(It.IsAny<Guid>()))
                    .ReturnsAsync(this.listOfPatients[1]);
                PatientController sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
                Test.stopStep(Status.passed);

                // Act
                Test.addStep(
                    new step
                        {
                            description = " Get patient information",
                            name = "Step 2: Act",
                            listParamenters = new List<Parameter>
                                                  {
                                                      {
                                                          new Parameter
                                                              {
                                                                  name = "Patient ID",
                                                                  value = this.listOfPatients[1].Id.ToString()
                                                              }
                                                      }
                                                  }
                        });
                var result = await sut.GetPatient(this.listOfPatients[1].Id);
                Test.stopStep(Status.passed);

                // Assert
                Test.addStep(
                    new step
                        {
                            description = "Check if the patient information returned is correct",
                            name = "Step 3: Assert"
                        });
                var viewResult = Assert.IsType<OkObjectResult>(result);
                var model = Assert.IsAssignableFrom<PatientViewModel>(viewResult.Value);
                Assert.Equal(this.listOfPatients[1].Email, model.Email);
                Assert.Equal(this.listOfPatients[1].Name, model.Name);
                Assert.Equal(this.listOfPatients[1].Surname, model.Surname);
                Assert.Equal(this.listOfPatients[1].MedicalNumber, model.MedicalNumber);
                Assert.Equal(this.listOfPatients[1].Id, model.Id);
                Test.stopStep(Status.passed);
                Test.stopTest(testId, Status.passed, "Test success", "Passed");
            }
            catch (Exception ex)
            {
                Test.stopStep(Status.failed);
                Test.stopTest(testId, Status.passed, "Test failed", ex.ToString());
                Assert.True(false);
            }
        }

        [Theory(DisplayName = "Insert Patient")]
        [InlineData("joe", "doe", "email@valid.com")]
        [InlineData("joe", "", "email@valid.com")]
        [InlineData("joe", "doe", "")]
        [InlineData("joe", "doe", null)]
        [InlineData("joe", null, null)]
        [InlineData("", null, null)]
        [InlineData(null, null, null)]
        [Trait("Category", "Unit")]
        public async void InsertPatient_ValidPatient_InsertionOK(string name, string lastname, string email)
        {
            Guid id = Guid.NewGuid();
            string testId = string.Empty;
            try
            {
                testId = Test.addTest(
                    new testDefinition
                        {
                            name = "Insert a Patient",
                            description = "This test should insert a patient",
                            storyName = "Insert a Patient",
                            featureName = "Positive Tests",
                            epicName = "Unit Tests"
                        });

                // Arrange
                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                PatientViewModel patientToInsert =
                    new PatientViewModel { Email = email, Id = id, Surname = lastname, Name = name};
                Patient mappedPatientToInsert = this.mapper.Map<Patient>(patientToInsert);
                this.mockPatientRepository.Setup(repo => repo.Add(It.Is<Patient>(p => p.Equals(mappedPatientToInsert))));
                PatientController sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
                Test.stopStep(Status.passed);

                // Act
                Test.addStep(
                    new step
                        {
                            description = "Insert patient",
                            name = "Step 2: Act",
                            listParamenters = new List<Parameter>
                                                  {
                                                      {
                                                          new Parameter
                                                              {
                                                                  name = "Email", value = patientToInsert.Email
                                                              }
                                                      },
                                                      { new Parameter { name = "Name", value = patientToInsert.Name } },
                                                      {
                                                          new Parameter
                                                              {
                                                                  name = "Surname", value = patientToInsert.Surname
                                                              }
                                                      },
                                                      {
                                                          new Parameter
                                                              {
                                                                  name = "Id", value = patientToInsert.Id.ToString()
                                                              }
                                                      }
                                                  }
                        });
                var result = await sut.CreatePatient(patientToInsert);
                Test.stopStep(Status.passed);

                // Assert
                Test.addStep(new step { description = "Check if the patient was inserted", name = "Step 3: Assert" });
                this.mockRepositories.Verify();
                var viewResult = Assert.IsType<OkObjectResult>(result);
                var model = Assert.IsAssignableFrom<PatientViewModel>(viewResult.Value);
                // we expect the MRN to be this string
                patientToInsert.MedicalNumber = MockedMedicalRecordNumber;
                Assert.Equal(JsonConvert.SerializeObject(patientToInsert), JsonConvert.SerializeObject(model));
                Test.stopStep(Status.passed);
                Test.stopTest(testId, Status.passed, "Test success", "Passed");
            }
            catch (Exception ex)
            {
                Test.stopStep(Status.failed);
                Test.stopTest(testId, Status.passed, "Test failed", ex.ToString());
                Assert.True(false);
            }
        }

        [Fact(DisplayName = "Remove Patient")]
        [Trait("Category", "Unit")]
        public async Task RemovePatient_Should_Remove_A_Existing_Patient()
        {
            string testId = string.Empty;
            try
            {
                testId = Test.addTest(
                    new testDefinition
                        {
                            name = "Remove a Patient",
                            description = "This test should remove a patient",
                            storyName = "Patient Remove",
                            featureName = "Positive Tests",
                            epicName = "Unit Tests"
                        });

                // Arrange
                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                this.mockPatientRepository.Setup(repo => repo.Get(It.IsAny<Guid>()))
                    .ReturnsAsync(this.listOfPatients[0]);
                PatientController sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);

                PatientViewModel patient = new PatientViewModel
                                               {
                                                   Id = Guid.NewGuid(),
                                                   Name = "Cerizo",
                                                   Surname = "Remundo",
                                                   Email = "cremundo@werfen.com",
                                                   MedicalNumber = "8899"
                                               };
                Test.stopStep(Status.passed);

                // Act
                Test.addStep(
                    new step
                        {
                            description = " Remove the patient",
                            name = "Step 2: Act",
                            listParamenters = new List<Parameter>
                                                  {
                                                      {
                                                          new Parameter
                                                              {
                                                                  name = "Patient ID", value = patient.Id.ToString()
                                                              }
                                                      }
                                                  }
                        });
                var result = await sut.Remove(patient.Id);
                Test.stopStep(Status.passed);

                // Assert
                Test.addStep(new step { description = "Check if the patient is removed", name = "Step 3: Assert" });
                var viewResult = Assert.IsType<OkResult>(result);
                Test.stopStep(Status.passed);
                Test.stopTest(testId, Status.passed, "Test success", "Passed");
            }
            catch (Exception ex)
            {
                Test.stopStep(Status.failed);
                Test.stopTest(testId, Status.passed, "Test failed", ex.ToString());
                Assert.True(false, ex.ToString());
            }
        }

        private async Task GetPagedPatient(int page, int totalElements, int elementsPerPage)
        {
            string testId = string.Empty;
            IQueryable<Patient> list = this.GetPatientList(totalElements);
            try
            {
                testId = Test.addTest(
                    new testDefinition
                        {
                            name = "Get list of patients",
                            description = "This test should return a list of patients",
                            storyName = "Patient Retrieve",
                            featureName = "Positive Tests",
                            epicName = "Unit Tests"
                        });

                // Arrange
                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                this.mockPatientRepository.Setup(repo => repo.GetAllWithPaginationPatients(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(
                    new PagedList<Patient>(list, page + 1, elementsPerPage));
                PatientController sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
                Test.stopStep(Status.passed);

                // Act
                Test.addStep(
                    new step
                        {
                            description = $" Get all the patients of page {page}, just {elementsPerPage} items",
                            name = "Step 2: Act"
                        });
                var result = await sut.GetAllPatients(page, 18);
                Test.stopStep(Status.passed);

                // Assert
                Test.addStep(
                    new step
                        {
                            description = "Check if the number the patients returned is correct",
                            name = "Step 3: Assert"
                        });
                var viewResult = Assert.IsType<OkObjectResult>(result);
                var model = Assert.IsAssignableFrom<ExtendedPagedList<PatientViewModel>>(viewResult.Value);
                Assert.Equal(elementsPerPage, model.NumberOfElements);
                Assert.Equal(page, model.Number);
                Assert.Equal(totalElements, model.TotalElements);

                // compare the Ids of the patient and patientVM
                CollectionAssert.AreEqual(
                    list.Skip(page * elementsPerPage).Take(elementsPerPage).Select(p => p.Id).ToArray(),
                    model.Content.Select(p => p.Id).ToArray());
                Test.stopStep(Status.passed);
                Test.stopTest(testId, Status.passed, "Test success", "Passed");
            }
            catch (Exception ex)
            {
                Test.stopStep(Status.failed);
                Test.stopTest(testId, Status.passed, "Test failed", ex.ToString());
                Assert.True(false, ex.ToString());
            }
        }

        private IQueryable<Patient> GetPatientList(int totalElements)
        {
            Fixture autoFixture = new Fixture();
            return autoFixture.CreateMany<Patient>(totalElements).AsQueryable();
        }

        private void InitializeData()
        {
            this.listOfPatients = new List<Patient>
                                      {
                                          new Patient
                                              {
                                                  Id = new Guid("35321823-4f70-40a7-8135-f7f2e9b5ea90"),
                                                  Name = "Arturo",
                                                  Surname = "Ciguendo",
                                                  Email = "aciguendo@werfen.com"
                                              },
                                          new Patient
                                              {
                                                  Id = new Guid("4ab9c588-6177-4c1e-93da-694ebb034c07"),
                                                  Name = "Sofia",
                                                  Surname = "Corona",
                                                  Email = "scorona@werfen.com"
                                              },
                                          new Patient
                                              {
                                                  Id = new Guid("7780fa2f-628e-472d-8503-e0e48e5a4875"),
                                                  Name = "Marta",
                                                  Surname = "Sanchez",
                                                  Email = "msanchez@werfen.com"
                                              }
                                      };
        }
    }

    public class XunitLogger<T> : ILogger<T>
    {
        private readonly ITestOutputHelper testOutputHelper;
        
        public XunitLogger(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        public IDisposable BeginScope<TState>(TState state)
            => NoopDisposable.Instance;

        public bool IsEnabled(LogLevel logLevel)
            => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this.testOutputHelper.WriteLine($"[{eventId}] {formatter(state, exception)}");
            if (exception != null)
            {
                this.testOutputHelper.WriteLine(exception.ToString());
            }
        }

        private class NoopDisposable : IDisposable
        {
            public static NoopDisposable Instance = new NoopDisposable();

            public void Dispose() { }
        }
    }
}