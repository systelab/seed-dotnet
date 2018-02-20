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

    public partial class PatientControllerShould
    {
        private readonly Mock<ISeedDotnetRepository> mockUserRepo;

        private List<Patient> lpatient;

        public PatientControllerShould()
        {
            Mapper.Reset();
            Mapper.Initialize(config => { config.CreateMap<PatientViewModel, Patient>().ReverseMap(); });
            this.mockUserRepo = new Mock<ISeedDotnetRepository>();

            this.InitializeData();
        }

        [Fact]
        public async Task CreatePatientConstructor_NullParameters_ThrowsArgumentNullException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentNullException>(() => { new PatientControllerBuilder().WithRepository(null); });
            Assert.Throws<ArgumentNullException>(() => { new PatientControllerBuilder().WithLogger(null); });
        }

        [Fact]
        public async Task CreatePatient_ReturnsBadRequest_GivenInvalidPatient()
        {
            // Arrange & Act
            this.mockUserRepo.Setup(repo => repo.AddPatient(It.IsAny<Patient>()));
            
            PatientController sut = new PatientControllerBuilder().WithRepository(this.mockUserRepo.Object);
            sut.ModelState.AddModelError("error", "some error");

            // Act
            var result = await sut.CreatePatient(patient: null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreatePatient_Should_Create_A_New_Patient()
        {
            this.mockUserRepo.Setup(repo => repo.AddPatient(It.IsAny<Patient>()));
            PatientController sut = new PatientControllerBuilder().WithRepository(this.mockUserRepo.Object);

            PatientViewModel patient =
                new PatientViewModel { Name = "Carlos", Surname = "Carmona", Email = "ccarmona@werfen.com" };

            // Act
            var result = sut.CreatePatient(patient);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsType<PatientViewModel>(viewResult.Value);
            this.mockUserRepo.Verify();
            Xunit.Assert.Equal(patient.Email, model.Email);
            Xunit.Assert.Equal(patient.Name, model.Name);
            Xunit.Assert.Equal(patient.Surname, model.Surname);
        }

        [Fact]
        public async Task GetAllPatients_Should_Return_List_Of_Patients()
        {
            this.mockUserRepo.Setup(repo => repo.GetAllPatients()).Returns(this.lpatient);
            PatientController sut = new PatientControllerBuilder().WithRepository(this.mockUserRepo.Object);

            // Act
            var result = sut.GetAllPatients();

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<PatientViewModel>>(viewResult.Value);
            Assert.Equal(3, model.Count());
        }

        [Fact]
        public async Task GetPatients_Should_Return_Patient_Information()
        {
            this.mockUserRepo.Setup(repo => repo.GetPatient(It.IsAny<Patient>())).Returns(this.lpatient[1]);
            PatientController sut = new PatientControllerBuilder().WithRepository(this.mockUserRepo.Object);

            // Act
            var result = sut.GetPatient(this.lpatient[1].Id);

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var model = Xunit.Assert.IsAssignableFrom<PatientViewModel>(viewResult.Value);
            Xunit.Assert.Equal(this.lpatient[1].Email, model.Email);
            Xunit.Assert.Equal(this.lpatient[1].Name, model.Name);
            Xunit.Assert.Equal(this.lpatient[1].Surname, model.Surname);
            Xunit.Assert.Equal(this.lpatient[1].Id, model.Id);
        }

        [Fact]
        public async Task RemovePatient_Should_Remove_A_Existing_Patient()
        {
            this.mockUserRepo.Setup(repo => repo.DeletePatient(It.IsAny<Patient>())).Returns(this.lpatient);
            PatientController sut = new PatientControllerBuilder().WithRepository(this.mockUserRepo.Object);

            PatientViewModel patient =
                new PatientViewModel { Id = 2, Name = "Cerizo", Surname = "Remundo", Email = "cremundo@werfen.com" };

            // Act
            var result = sut.Remove(patient.Id);

            // Assert
            var viewResult = Assert.IsType<OkObjectResult>(result.Result);
            var model = Assert.IsAssignableFrom<List<PatientViewModel>>(viewResult.Value);
            Assert.Equal(3, model.Count());
        }

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", 99)]
        [InlineData("joe", "", "email@valid.com", 99)]
        [InlineData("joe", "doe", "", 99)]
        [InlineData("joe", "doe", null, 99)]
        [InlineData("joe", null, null, 99)]
        [InlineData("", null, null, 99)]
        [InlineData(null, null, null, 99)]
        public async Task InsertPatient_ValidPatient_InsertionOK(string name, string lastname, string email, int id)
        {
            PatientViewModel patientToInsert = new PatientViewModel() { Email = email, Id = id, LastName = lastname, Name = name };
            Patient mappedPatientToInsert = Mapper.Map<Patient>(patientToInsert);
            this.mockUserRepo.Setup(repo => repo.AddPatient(It.Is<Patient>(p => p.Equals(mappedPatientToInsert))));
            PatientController sut = new PatientControllerBuilder().WithRepository(this.mockUserRepo.Object);

            // Act
            var result = await sut.CreatePatient(patientToInsert);

            // Assert
            this.mockUserRepo.Verify();
            var viewResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<PatientViewModel>(viewResult.Value);
            Assert.Equal(JsonConvert.SerializeObject(patientToInsert), JsonConvert.SerializeObject(model));
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