namespace Test.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using Main.Controllers.Api;
    using Main.Models;
    using Main.Services;
    using Main.ViewModels;
    using Microsoft.AspNetCore.Mvc;
    using Moq;

    using Newtonsoft.Json;

    using Xunit;
    using Allure.Commons;
    using Allure.builder;
    using Allure.builder.attributes;
    using System;

    public partial class PatientControllerShould
    {
        private readonly Mock<ISeedDotnetRepository> mockUserRepo;

        private readonly PatientControllerBuilder sutBuilder;

        private readonly IMapper mapper;

        private List<Patient> lpatient;

        public PatientControllerShould()
        {
            Mapper.Reset();
            var automapConfiguration = new AutoMapper.MapperConfiguration(cfg =>
                    { cfg.CreateMap<PatientViewModel, Patient>().ReverseMap(); });
            this.mapper = automapConfiguration.CreateMapper();
            this.sutBuilder = new PatientControllerBuilder(mapper);
            this.mockUserRepo = new Mock<ISeedDotnetRepository>();

            this.InitializeData();
            Test.createInstance();
        }

        [Fact(DisplayName = "Create Patient Constructor Null Parameters")]
        [Trait("Category", "Unit")]
        public async Task CreatePatientConstructor_NullParameters_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            string testId = "";
            try
            {
                testId = Test.addTest(new testDefinition
                {
                    name = "Create Patient Constructor Null Parameters",
                    description = "Create Patient Constructor with Null Parameters should Throws ArgumentNullException",
                    storyName = "Patient Creation",
                    featureName = "Negative Tests",
                    epicName = "Unit Tests",
                    listLinks = new List<link>{
                                new link {isIssue=false, name ="RQS-62356", url="http://google.com" } }
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

        [Fact(DisplayName = "Create Patient Return Bad Request Given Invalid Patient")]
        public async Task CreatePatient_ReturnsBadRequest_GivenInvalidPatient()
        {
            string testId = "";
            try
            {
                testId = Test.addTest(new testDefinition
                {
                    name = "Create Patient Return Bad Request Given Invalid Patient",
                    description = "Return a bad request if you provide a invalid Patient",
                    storyName = "Patient Creation",
                    featureName = "Negative Tests",
                    epicName = "Unit Tests" 
                });

                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                // Arrange
                this.mockUserRepo.Setup(repo => repo.AddPatient(It.IsAny<Patient>()));
                PatientController sut = this.sutBuilder.WithRepository(this.mockUserRepo.Object);
                sut.ModelState.AddModelError("error", "some error");
                Test.stopStep(Status.passed);

                Test.addStep(new step { description = "Act", name = "Step 2: Act", listParamenters = new List<Allure.Commons.Parameter> { { new Allure.Commons.Parameter { name = "Patient", value = null } } } });
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
        public async Task CreatePatient_Should_Create_A_New_Patient()
        {
            string testId = "";
            try
            {
                testId = Test.addTest(new testDefinition
                {
                    name = "Create a new Patient",
                    description = "This test should create a new patient",
                    storyName = "Patient Creation",
                    featureName = "Positive Tests",
                    epicName = "Unit Tests"
                });

                // Arrange
                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                this.mockUserRepo.Setup(repo => repo.AddPatient(It.IsAny<Patient>()));
                PatientController sut = this.sutBuilder.WithRepository(this.mockUserRepo.Object);
                PatientViewModel patient =
                    new PatientViewModel { Name = "Carlos", Surname = "Carmona", Email = "ccarmona@werfen.com" };

                Test.stopStep(Status.passed);

                // Act
                Test.addStep(new step { description = "Act", name = "Step 2: Act", listParamenters = new List<Allure.Commons.Parameter> {
                    { new Allure.Commons.Parameter { name = "Email", value = "ccarmona@werfen.com" }},
                    { new Allure.Commons.Parameter { name = "Name", value = "Carlos"}},
                    { new Allure.Commons.Parameter { name = "Surname", value = "Carmona" }},
                } });

                var result = sut.CreatePatient(patient);

                Test.stopStep(Status.passed);

                // Assert
                Test.addStep(new step { description = "Assert", name = "Step 3: Assert" });
                var viewResult = Assert.IsType<OkObjectResult>(result.Result);
                var model = Assert.IsType<PatientViewModel>(viewResult.Value);
                this.mockUserRepo.Verify();
                Xunit.Assert.Equal(patient.Email, model.Email);
                Xunit.Assert.Equal(patient.Name, model.Name);
                Xunit.Assert.Equal(patient.Surname, model.Surname);

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

        [Fact(DisplayName = "Get All Patients")]
        public async Task GetAllPatients_Should_Return_List_Of_Patients()
        {

            string testId = "";
            try
            {
                testId = Test.addTest(new testDefinition()
                {
                    name = "Get list of patients",
                    description = "This test should return a list of patients",
                    storyName = "Patient Retrieve",
                    featureName = "Positive Tests",
                    epicName = "Unit Tests"
                });

                // Arrange
                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                this.mockUserRepo.Setup(repo => repo.GetAllPatients()).Returns(this.lpatient);
                PatientController sut = this.sutBuilder.WithRepository(this.mockUserRepo.Object);
                Test.stopStep(Status.passed);

                // Act
                Test.addStep(new step { description = " Get all the patients", name = "Step 2: Act" });
                var result = sut.GetAllPatients();
                Test.stopStep(Status.passed);

                // Assert
                Test.addStep(new step { description = "Check if the number the clients returned is correct", name = "Step 3: Assert" });
                var viewResult = Assert.IsType<OkObjectResult>(result);
                var model = Assert.IsAssignableFrom<List<PatientViewModel>>(viewResult.Value);
                Assert.Equal(3, model.Count());
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

        [Fact(DisplayName = "Get Patient")]
        public async Task GetPatients_Should_Return_Patient_Information()
        {
            string testId = "";
            try
            {
                testId = Test.addTest(new testDefinition
                {
                    name = "Get list of patients",
                    description = "This test should return a patient information",
                    storyName = "Patient Retrieve",
                    featureName = "Positive Tests",
                    epicName = "Unit Tests"
                });

                // Arrange
                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                this.mockUserRepo.Setup(repo => repo.GetPatient(It.IsAny<Patient>())).Returns(this.lpatient[1]);
                PatientController sut = this.sutBuilder.WithRepository(this.mockUserRepo.Object);
                Test.stopStep(Status.passed);

                // Act
                Test.addStep(new step
                {
                    description = " Get patient information",
                    name = "Step 2: Act",
                    listParamenters = new List<Allure.Commons.Parameter> {
                    { new Allure.Commons.Parameter { name = "Patient ID", value = this.lpatient[1].Id.ToString() }} }
                });
                var result = sut.GetPatient(this.lpatient[1].Id);
                Test.stopStep(Status.passed);

                // Assert
                Test.addStep(new step { description = "Check if the patient information returned is correct", name = "Step 3: Assert" });
                var viewResult = Xunit.Assert.IsType<OkObjectResult>(result);
                var model = Xunit.Assert.IsAssignableFrom<PatientViewModel>(viewResult.Value);
                Xunit.Assert.Equal(this.lpatient[1].Email, model.Email);
                Xunit.Assert.Equal(this.lpatient[1].Name, model.Name);
                Xunit.Assert.Equal(this.lpatient[1].Surname, model.Surname);
                Xunit.Assert.Equal(this.lpatient[1].Id, model.Id);
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
        public async Task RemovePatient_Should_Remove_A_Existing_Patient()
        {
            string testId = "";
            try
            {
                testId = Test.addTest(new testDefinition
                {
                    name = "Remove a Patient",
                    description = "This test should remove a patient",
                    storyName = "Patient Remove",
                    featureName = "Positive Tests",
                    epicName = "Unit Tests"
                });

                // Arrange
                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                this.mockUserRepo.Setup(repo => repo.GetPatient(It.IsAny<Patient>())).Returns(this.lpatient[0]);
                this.mockUserRepo.Setup(repo => repo.DeletePatient(It.IsAny<Patient>())).Returns(this.lpatient);
                PatientController sut = this.sutBuilder.WithRepository(this.mockUserRepo.Object);

                PatientViewModel patient =
                    new PatientViewModel { Id = 2, Name = "Cerizo", Surname = "Remundo", Email = "cremundo@werfen.com" };
                Test.stopStep(Status.passed);

                // Act
                Test.addStep(new step
                {
                    description = " Remove the patient",
                    name = "Step 2: Act",
                    listParamenters = new List<Allure.Commons.Parameter> {
                    { new Allure.Commons.Parameter { name = "Patient ID", value = patient.Id.ToString() }} }
                });
                var result = sut.Remove(patient.Id);
                Test.stopStep(Status.passed);

                // Assert
                Test.addStep(new step { description = "Check if the patient is removed", name = "Step 3: Assert" });
                var viewResult = Assert.IsType<OkObjectResult>(result.Result);
                var model = Assert.IsAssignableFrom<List<PatientViewModel>>(viewResult.Value);
                Assert.Equal(3, model.Count());
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
        [InlineData("joe", "doe", "email@valid.com", 99)]
        [InlineData("joe", "", "email@valid.com", 99)]
        [InlineData("joe", "doe", "", 99)]
        [InlineData("joe", "doe", null, 99)]
        [InlineData("joe", null, null, 99)]
        [InlineData("", null, null, 99)]
        [InlineData(null, null, null, 99)]
        public async Task InsertPatient_ValidPatient_InsertionOK(string name, string lastname, string email, int id)
        {
            string testId = "";
            try
            {
                testId = Test.addTest(new testDefinition
                {
                    name = "Insert a Patient",
                    description = "This test should insert a patient",
                    storyName = "Insert a Patient",
                    featureName = "Positive Tests",
                    epicName = "Unit Tests"
                });

                // Arrange
                Test.addStep(new step { description = "Arrange", name = "Step 1: Arrange" });
                PatientViewModel patientToInsert = new PatientViewModel { Email = email, Id = id, Surname = lastname, Name = name };
                Patient mappedPatientToInsert = this.mapper.Map<Patient>(patientToInsert);
                this.mockUserRepo.Setup(repo => repo.AddPatient(It.Is<Patient>(p => p.Equals(mappedPatientToInsert))));
                PatientController sut = this.sutBuilder.WithRepository(this.mockUserRepo.Object);
                Test.stopStep(Status.passed);

                // Act
                Test.addStep(new step
                {
                    description = "Insert patient",
                    name = "Step 2: Act",
                    listParamenters = new List<Allure.Commons.Parameter> {
                    { new Allure.Commons.Parameter { name = "Email", value = patientToInsert.Email }},
                    { new Allure.Commons.Parameter { name = "Name", value = patientToInsert.Name}},
                    { new Allure.Commons.Parameter { name = "Surname", value = patientToInsert.Surname }},
                    { new Allure.Commons.Parameter { name = "Id", value = patientToInsert.Id.ToString() }}}
                });
                var result = await sut.CreatePatient(patientToInsert);
                Test.stopStep(Status.passed);

                // Assert
                Test.addStep(new step { description = "Check if the patient was inserted", name = "Step 3: Assert" });
                this.mockUserRepo.Verify();
                var viewResult = Assert.IsType<OkObjectResult>(result);
                var model = Assert.IsAssignableFrom<PatientViewModel>(viewResult.Value);
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

        private void InitializeData()
        {
            this.lpatient = new List<Patient>
                                {
                                    new Patient
                                        {
                                            Id = 1,
                                            Name = "Arturo",
                                            Surname = "Ciguendo",
                                            Email = "aciguendo@werfen.com"
                                        },
                                    new Patient
                                        {
                                            Id = 2,
                                            Name = "Sofia",
                                            Surname = "Corona",
                                            Email = "scorona@werfen.com"
                                        },
                                    new Patient
                                        {
                                            Id = 3,
                                            Name = "Marta",
                                            Surname = "Sanchez",
                                            Email = "msanchez@werfen.com"
                                        }
                                };
        }
    }
}