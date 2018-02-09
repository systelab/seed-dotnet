namespace test.Controller
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Main.Controllers.Api;
    using Main.Models;
    using Main.Services;
    using Main.ViewModels;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Xunit;

    [TestClass]
    public class PatientControllerShould
    {
        private readonly Mock<ISeedDotnetRepository> mockUserRepo;

        private PatientController sut;

        private List<Patient> lpatient;

        public PatientControllerShould()
        {
            // InitContext();
            Mapper.Reset();
            Mapper.Initialize(config => { config.CreateMap<PatientViewModel, Patient>().ReverseMap(); });
            this.mockUserRepo = new Mock<ISeedDotnetRepository>();

            this.InitializeData();
        }

        [TestMethod]
        [Fact]
        public async Task CreatePatient_ReturnsBadRequest_GivenInvalidPatient()
        {
            // Arrange & Act
            this.mockUserRepo.Setup(repo => repo.AddPatient(It.IsAny<Patient>()));
            this.sut = new PatientController(this.mockUserRepo.Object);
            this.sut.ModelState.AddModelError("error", "some error");

            // Act
            var result = await this.sut.CreatePatient(patient: null);

            // Assert
            Xunit.Assert.IsType<BadRequestObjectResult>(result);
        }

        [TestMethod]
        [Fact]
        public async Task CreatePatient_Should_Create_A_New_Patient()
        {
            this.mockUserRepo.Setup(repo => repo.AddPatient(It.IsAny<Patient>()));
            this.sut = new PatientController(this.mockUserRepo.Object);

            PatientViewModel nPatient =
                new PatientViewModel { Name = "Carlos", LastName = "Carmona", Email = "ccarmona@werfen.com" };

            // Act
            var result = this.sut.CreatePatient(nPatient);

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var model = Xunit.Assert.IsType<PatientViewModel>(viewResult.Value);
            this.mockUserRepo.Verify();
            Xunit.Assert.Equal(nPatient.Email, model.Email);
            Xunit.Assert.Equal(nPatient.Name, model.Name);
            Xunit.Assert.Equal(nPatient.LastName, model.LastName);
        }

        [TestMethod]
        [Fact]
        public async Task GetAllPatients_Should_Return_List_Of_Patients()
        {
            this.mockUserRepo.Setup(repo => repo.GetAllPatients()).Returns(this.lpatient);
            this.sut = new PatientController(this.mockUserRepo.Object);

            // Act
            var result = this.sut.GetAllPatients();

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var model = Xunit.Assert.IsAssignableFrom<List<PatientViewModel>>(viewResult.Value);
            Xunit.Assert.Equal(3, model.Count());
        }

        [TestMethod]
        [Fact]
        public async Task GetPatients_Should_Return_Patient_Information()
        {
            this.mockUserRepo.Setup(repo => repo.GetPatient(It.IsAny<Patient>())).Returns(this.lpatient[1]);
            this.sut = new PatientController(this.mockUserRepo.Object);

            // Act
            var result = this.sut.GetPatient(this.lpatient[1].Id);

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var model = Xunit.Assert.IsAssignableFrom<PatientViewModel>(viewResult.Value);
            Xunit.Assert.Equal(this.lpatient[1].Email, model.Email);
            Xunit.Assert.Equal(this.lpatient[1].Name, model.Name);
            Xunit.Assert.Equal(this.lpatient[1].LastName, model.LastName);
            Xunit.Assert.Equal(this.lpatient[1].Id, model.Id);
        }

        [TestMethod]
        [Fact]
        public async Task RemovePatient_Should_Remove_A_Existing_Patient()
        {
            this.mockUserRepo.Setup(repo => repo.DeletePatient(It.IsAny<Patient>())).Returns(this.lpatient);
            this.sut = new PatientController(this.mockUserRepo.Object);

            PatientViewModel nPatient =
                new PatientViewModel { Id = 2, Name = "Cerizo", LastName = "Remundo", Email = "cremundo@werfen.com" };

            // Act
            var result = this.sut.Remove(nPatient.Id);

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var model = Xunit.Assert.IsAssignableFrom<List<PatientViewModel>>(viewResult.Value);
            Xunit.Assert.Equal(3, model.Count());
        }

        private void InitializeData()
        {
            this.lpatient = new List<Patient>
                                {
                                    new Patient
                                        {
                                            Id = 1,
                                            Name = "Arturo",
                                            LastName = "Ciguendo",
                                            Email = "aciguendo@werfen.com"
                                        },
                                    new Patient
                                        {
                                            Id = 2,
                                            Name = "Sofia",
                                            LastName = "Corona",
                                            Email = "scorona@werfen.com"
                                        },
                                    new Patient
                                        {
                                            Id = 3,
                                            Name = "Marta",
                                            LastName = "Sanchez",
                                            Email = "msanchez@werfen.com"
                                        }
                                };
        }
    }
}