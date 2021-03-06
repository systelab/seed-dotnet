namespace TestNUnit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
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
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Newtonsoft.Json;
    using NUnit.Allure.Attributes;
    using NUnit.Allure.Core;
    using NUnit.Framework;
    using PagedList.Core;
    using Test.Controller;

    [AllureNUnit]
    public class PatientControllerShould
    {
        private const string MockedMedicalRecordNumber = "MR_";

        private List<Patient> listOfPatients;
        private IMapper mapper;

        private Mock<IMedicalRecordNumberService> medicalRecordMock;

        private Mock<IPatientRepository> mockPatientRepository;

        private Mock<IUnitOfWork> mockRepositories;

        private ILogger<PatientController> nuniLogger;

        private PatientControllerBuilder sutBuilder;


        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Environment.SetEnvironmentVariable("ALLURE_CONFIG",
                Path.Combine(Environment.CurrentDirectory, AllureConstants.CONFIG_FILENAME));

            Mapper.Reset();
            SeedMapperConfiguration automapConfiguration = new SeedMapperConfiguration();
            this.medicalRecordMock = new Mock<IMedicalRecordNumberService>();
            this.medicalRecordMock.Setup(p => p.GetMedicalRecordNumber(It.IsAny<string>()))
                .Returns(MockedMedicalRecordNumber);
            this.nuniLogger = new NunitLogger<PatientController>();

            this.mapper = automapConfiguration.CreateMapper();
            this.mockRepositories = new Mock<IUnitOfWork>();
            this.mockPatientRepository = new Mock<IPatientRepository>();
            Mock<IAllergyRepository> mockAllergyRepository = new Mock<IAllergyRepository>();

            this.mockRepositories.Setup(p => p.Patients).Returns(this.mockPatientRepository.Object);
            this.mockRepositories.Setup(p => p.Allergies).Returns(mockAllergyRepository.Object);

            this.InitializeData();
        }

        [SetUp]
        public void SetUp()
        {
            this.sutBuilder =
                new PatientControllerBuilder(this.mapper, this.medicalRecordMock.Object).WithLogger(this.nuniLogger);
        }

        [AllureEpic("Unit Tests")]
        [AllureFeature("Negative Tests")]
        [AllureStory("Patient Creation")]
        [Description("Provide a invalid Patient")]
        [AllureTms("Create Patient Return Bad Request Given Invalid Patient")]
        [Test]
        public async Task CreatePatient_ReturnsBadRequest_GivenInvalidPatient()
        {
            PatientController sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
            sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
            PatientViewModel patient = new PatientViewModel();
            IActionResult result = await sut.CreatePatient(patient);
            await AllureLifecycle.Instance.WrapInStep(async () =>
            {
                // Arrange

                sut.ModelState.AddModelError("error", "some error");

                //Act
                result = await sut.CreatePatient(null);
            }, "Action: Create patient error model");

            AllureLifecycle.Instance.WrapInStep(() =>
            {
                // Assert
                Assert.IsInstanceOf<BadRequestObjectResult>(result);
            }, "Expected: Returns BadRequest");
        }

        [AllureEpic("Unit Tests")]
        [AllureFeature("Positive Tests")]
        [AllureStory("Patient Creation")]
        [Description("Create a new Patient")]
        [AllureTms("Create a new Patient")]
        [Test]
        public async Task CreatePatient_Should_Create_A_New_Patient()
        {
            PatientViewModel patient = new PatientViewModel();
            PatientController sut = null;

            AllureLifecycle.Instance.WrapInStep(() =>
                {
                    sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
                    patient = new PatientViewModel
                    {
                        Name = "Carlos",
                        Surname = "Carmona",
                        Email = "ccarmona@werfen.com"
                    };
                },
                $"Action: Create a new Patient with values: Name: {patient.Name}, Surname: {patient.Surname} and email: {patient.Email}");

            // Arrange
            await AllureLifecycle.Instance.WrapInStep(async () =>
            {
                IActionResult result = await sut.CreatePatient(patient);

                // Assert
                PatientViewModel model = AssertAndGetModel<PatientViewModel>(result);
                this.mockPatientRepository.Verify(repo => repo.Add(It.IsAny<Patient>()));
                Assert.AreEqual(patient.Email, model.Email);
                Assert.AreEqual(patient.Name, model.Name);
                Assert.AreEqual(patient.Surname, model.Surname);
                Assert.AreEqual(MockedMedicalRecordNumber, model.MedicalNumber);
            }, " The emal, name, surname and medical numer are the injected values.");
        }

        private static T AssertAndGetModel<T>(IActionResult result)
        {
            Assert.IsInstanceOf<OkObjectResult>(result);
            OkObjectResult okResult = (OkObjectResult) result;
            Assert.IsInstanceOf<T>(okResult.Value);
            return (T) okResult.Value;
        }

        [AllureEpic("Unit Tests")]
        [AllureFeature("Negative Tests")]
        [AllureStory("Patient Creation")]
        [Description("RQS-62356 Create Patient Constructor with Null Parameters should Throws ArgumentNullException")]
        [AllureTms("Create Patient Constructor Null Parameters")]
        [Test]
        public void CreatePatientConstructor_NullParameters_ThrowsArgumentNullException()
        {
            AllureLifecycle.Instance.WrapInStep(() => { }, "Action: Create Patient Constructor with Null Parameters");
            AllureLifecycle.Instance.WrapInStep(() =>
            {
                // Arrange & Act & Assert
                Assert.Throws<ArgumentNullException>(() => { this.sutBuilder.WithRepository(null).Build(); });
                Assert.Throws<ArgumentNullException>(() => { this.sutBuilder.WithLogger(null).Build(); });
                Assert.Throws<ArgumentNullException>(() => { this.sutBuilder.WithMapper(null).Build(); });
            }, "Throws ArgumentNullException");
        }

        [AllureEpic("Unit Tests")]
        [AllureFeature("Positive Tests")]
        [AllureStory("Patient Creation")]
        [Description("This test should return a list of patients")]
        [AllureTms("Get All Patients")]
        [Test]
        public async Task GetAllPatients_Paged_Should_Return_List_Of_Patients_After_Last_Page()
        {
            int totalElements = 90;
            int page = 5;
            int elementsPerPage = 18;
            IActionResult result = null;

            IQueryable<Patient> list = GetPatientList(totalElements);
            PatientController sut = null;

            AllureLifecycle.Instance.WrapInStep(() =>
            {
                // Arrange
                this.mockPatientRepository
                    .Setup(repo => repo.GetAllWithPaginationPatients(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(
                        new PagedList<Patient>(list, page + 1, elementsPerPage));
                sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
            }, "Action: Create a Patient List");

            await AllureLifecycle.Instance.WrapInStep(async () =>
            {
                // Act
                result = await sut.GetAllPatients(page, 18);
            }, $"Action: Get all the patients of page {page}, just {elementsPerPage} items");

            AllureLifecycle.Instance.WrapInStep(() =>
            {
                ExtendedPagedList<PatientViewModel> model =
                    AssertAndGetModel<ExtendedPagedList<PatientViewModel>>(result);
                Assert.AreEqual(0, model.NumberOfElements);
                Assert.AreEqual(page, model.Number);
                Assert.AreEqual(totalElements, model.TotalElements);

                // the list shall be empty
                Assert.False(model.Content.Any());
            }, "Check if the number the patients returned is correct");
        }

        [AllureEpic("Unit Tests")]
        [Description("Get All Patients (first page)")]
        [AllureTms("Get All Patients")]
        [Test]
        public async Task GetAllPatients_Paged_Should_Return_List_Of_Patients_First_Page()
        {
            await this.GetPagedPatient(0, 90, 18).ConfigureAwait(false);
        }

        [AllureEpic("Unit Tests")]
        [Description("Get All Patients (last page)")]
        [AllureTms("Get All Patients")]
        [Test]
        public async Task GetAllPatients_Paged_Should_Return_List_Of_Patients_Last_Page()
        {
            await this.GetPagedPatient(4, 90, 18).ConfigureAwait(false);
        }

        [AllureEpic("Unit Tests")]
        [Description("Get All Patients (middle page)")]
        [AllureTms("Get All Patients")]
        [Test]
        public async Task GetAllPatients_Paged_Should_Return_List_Of_Patients_Middle_Page()
        {
            await this.GetPagedPatient(3, 90, 18).ConfigureAwait(false);
        }

        [AllureEpic("Unit Tests")]
        [AllureFeature("Positive Tests")]
        [AllureStory("Patient Retrieve")]
        [Description("This test should return a patient information")]
        [AllureTms("Get list of patients")]
        [Test]
        public async Task GetPatients_Should_Return_Patient_Information()
        {
            IActionResult result = null;
            PatientController sut = null;
            await AllureLifecycle.Instance.WrapInStep(async () =>
            {
                // Arrange
                this.mockPatientRepository.Setup(repo => repo.Get(It.IsAny<Guid>()))
                    .ReturnsAsync(this.listOfPatients[1]);
                sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);

                // Act
                result = await sut.GetPatient(this.listOfPatients[1].Id);
            }, $"Action: Step 1: Arrange. Get patient information with id: �{this.listOfPatients[1].Id}");


            AllureLifecycle.Instance.WrapInStep(() =>
            {
                // Assert
                PatientViewModel model = AssertAndGetModel<PatientViewModel>(result);
                Assert.AreEqual(this.listOfPatients[1].Email, model.Email);
                Assert.AreEqual(this.listOfPatients[1].Name, model.Name);
                Assert.AreEqual(this.listOfPatients[1].Surname, model.Surname);
                Assert.AreEqual(this.listOfPatients[1].MedicalNumber, model.MedicalNumber);
                Assert.AreEqual(this.listOfPatients[1].Id, model.Id);
            }, "Check if the patient information returned is correct");
        }

        [AllureEpic("Unit Tests")]
        [AllureFeature("Positive Tests")]
        [AllureStory("Insert a Patient")]
        [Description("This test should insert a patient")]
        [AllureTms("Insert a Patient")]
        [TestCase("joe", "doe", "email@valid.com")]
        [TestCase("joe", "", "email@valid.com")]
        [TestCase("joe", "doe", "")]
        [TestCase("joe", "doe", null)]
        [TestCase("joe", null, null)]
        [TestCase("", null, null)]
        [TestCase(null, null, null)]
        public async Task InsertPatient_ValidPatient_InsertionOK(string name, string lastname, string email)
        {
            IActionResult result = null;
            PatientController sut = null;
            Guid id = Guid.NewGuid();
            PatientViewModel patientToInsert = new PatientViewModel();
            AllureLifecycle.Instance.WrapInStep(() =>
            {
                // Arrange

                patientToInsert =
                    new PatientViewModel {Email = email, Id = id, Surname = lastname, Name = name};
                Patient mappedPatientToInsert = this.mapper.Map<Patient>(patientToInsert);
                this.mockPatientRepository.Setup(repo =>
                    repo.Add(It.Is<Patient>(p => p.Equals(mappedPatientToInsert))));
                sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
            }, "Action: Step 1: Arrange");

            await AllureLifecycle.Instance.WrapInStep(async () =>
                {
                    // Act
                    result = await sut.CreatePatient(patientToInsert);
                },
                $"Action: Step 2: Insert patient with Name:{patientToInsert.Name}, surname: {patientToInsert.Surname}, id: {patientToInsert.Id} and email: {patientToInsert.Email} ");

            AllureLifecycle.Instance.WrapInStep(() =>
            {
                // Assert
                this.mockRepositories.Verify();
                PatientViewModel model = AssertAndGetModel<PatientViewModel>(result);

                // we expect the MRN to be this string
                patientToInsert.MedicalNumber = MockedMedicalRecordNumber;
                Assert.AreEqual(JsonConvert.SerializeObject(patientToInsert), JsonConvert.SerializeObject(model));
            }, "Check if the patient was inserted");
        }

        [AllureEpic("Unit Tests")]
        [AllureFeature("Positive Tests")]
        [AllureStory("Patient Remove")]
        [Description("This test should remove a patient")]
        [AllureTms("Remove a Patient")]
        [Test]
        public async Task RemovePatient_Should_Remove_A_Existing_Patient()
        {
            IActionResult result = null;
            PatientController sut = null;
            PatientViewModel patient = new PatientViewModel();
            AllureLifecycle.Instance.WrapInStep(() =>
            {
                // Arrange
                this.mockPatientRepository.Setup(repo => repo.Get(It.IsAny<Guid>()))
                    .ReturnsAsync(this.listOfPatients[0]);
                sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);

                patient = new PatientViewModel
                {
                    Id = Guid.NewGuid(),
                    Name = "Cerizo",
                    Surname = "Remundo",
                    Email = "cremundo@werfen.com",
                    MedicalNumber = "8899"
                };
            }, "Action: Step 1: Arrange");

            await AllureLifecycle.Instance.WrapInStep(async () =>
            {
                // Act
                result = await sut.Remove(patient.Id);
            }, $"Action: Step 2: Act, Remove the patient:{patient.Id}");

            AllureLifecycle.Instance.WrapInStep(() =>
            {
                // Assert
                Assert.IsInstanceOf<OkResult>(result);
            }, "Check if the patient is removed");
        }

        [AllureEpic("Unit Tests")]
        [AllureFeature("Positive Tests")]
        [AllureStory("Patient Remove")]
        [Description("This test should return a list of patients")]
        [AllureTms("Get list of patients")]
        private async Task GetPagedPatient(int page, int totalElements, int elementsPerPage)
        {
            IActionResult result = null;
            PatientController sut = null;
            IQueryable<Patient> list = GetPatientList(totalElements);
            AllureLifecycle.Instance.WrapInStep(() =>
            {
                // Arrange
                this.mockPatientRepository
                    .Setup(repo => repo.GetAllWithPaginationPatients(It.IsAny<int>(), It.IsAny<int>()))
                    .ReturnsAsync(
                        new PagedList<Patient>(list, page + 1, elementsPerPage));
                sut = this.sutBuilder.WithRepository(this.mockRepositories.Object);
            }, "Action: Step 1: Arrange");

            await AllureLifecycle.Instance.WrapInStep(async () =>
            {
                // Act
                result = await sut.GetAllPatients(page, 18);
            }, $"Action: Step 2: Act. Get all the patients of page {page}, just {elementsPerPage} items");

            AllureLifecycle.Instance.WrapInStep(() =>
            {
                // Assert
                ExtendedPagedList<PatientViewModel> model =
                    AssertAndGetModel<ExtendedPagedList<PatientViewModel>>(result);
                Assert.AreEqual(elementsPerPage, model.NumberOfElements);
                Assert.AreEqual(page, model.Number);
                Assert.AreEqual(totalElements, model.TotalElements);

                // compare the Ids of the patient and patientVM
                CollectionAssert.AreEqual(
                    list.Skip(page * elementsPerPage).Take(elementsPerPage).Select(p => p.Id).ToArray(),
                    model.Content.Select(p => p.Id).ToArray());
            }, "Step 3: Assert. Check if the number the patients returned is correct");
        }

        private static IQueryable<Patient> GetPatientList(int totalElements)
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

    public class NunitLogger<T> : ILogger<T>, IDisposable
    {
        private readonly bool isEnabled = true;

        public void Dispose()
        {
            //Do nothing
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return this.isEnabled;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (this.isEnabled)
            {
                TestContext.WriteLine($"[{eventId}] {formatter(state, exception)}");
                if (exception != null)
                {
                    TestContext.WriteLine(exception.ToString());
                }
            }
        }
    }
}