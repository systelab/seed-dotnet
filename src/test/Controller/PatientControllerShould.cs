namespace test_seed_dotnet.Controller
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using seed_dotnet.Controllers.Api;
    using seed_dotnet.Models;
    using seed_dotnet.Services;
    using seed_dotnet.ViewModels;

    using Xunit;

    [TestClass]
    public class PatientControllerShould
    {
        private readonly Mock<ISeed_dotnetRepository> _mockUserRepo;

        private PatientController _sut;

        private List<Patient> lpatient;

        public PatientControllerShould()
        {
            // InitContext();
            Mapper.Reset();
            Mapper.Initialize(config => { config.CreateMap<PatientViewModel, Patient>().ReverseMap(); });
            this._mockUserRepo = new Mock<ISeed_dotnetRepository>();

            this.InitializeData();
        }

        [TestMethod]
        [Fact]
        public async Task CreatePatient_ReturnsBadRequest_GivenInvalidPatient()
        {
            // Arrange & Act
            this._mockUserRepo.Setup(repo => repo.AddPatient(It.IsAny<Patient>()));
            this._sut = new PatientController(this._mockUserRepo.Object);
            this._sut.ModelState.AddModelError("error", "some error");

            // Act
            var result = await this._sut.createPatient(patient: null);

            // Assert
            Xunit.Assert.IsType<BadRequestObjectResult>(result);
        }

        [TestMethod]
        [Fact]
        public async Task CreatePatient_Should_Create_A_New_Patient()
        {
            this._mockUserRepo.Setup(repo => repo.AddPatient(It.IsAny<Patient>()));
            this._sut = new PatientController(this._mockUserRepo.Object);

            PatientViewModel nPatient =
                new PatientViewModel { Name = "Carlos", LastName = "Carmona", Email = "ccarmona@werfen.com" };

            // Act
            var result = this._sut.createPatient(nPatient);

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var model = Xunit.Assert.IsType<PatientViewModel>(viewResult.Value);
            this._mockUserRepo.Verify();
            Xunit.Assert.Equal(nPatient.Email, model.Email);
            Xunit.Assert.Equal(nPatient.Name, model.Name);
            Xunit.Assert.Equal(nPatient.LastName, model.LastName);
        }

        [TestMethod]
        [Fact]
        public async Task GetAllPatients_Should_Return_List_Of_Patients()
        {
            this._mockUserRepo.Setup(repo => repo.GetAllPatients()).Returns(this.lpatient);
            this._sut = new PatientController(this._mockUserRepo.Object);

            // Act
            var result = this._sut.getAllPatients();

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var model = Xunit.Assert.IsAssignableFrom<List<PatientViewModel>>(viewResult.Value);
            Xunit.Assert.Equal(3, model.Count());
        }

        [TestMethod]
        [Fact]
        public async Task GetPatients_Should_Return_Patient_Information()
        {
            this._mockUserRepo.Setup(repo => repo.GetPatient(It.IsAny<Patient>())).Returns(this.lpatient[1]);
            this._sut = new PatientController(this._mockUserRepo.Object);

            // Act
            var result = this._sut.getPatient(this.lpatient[1].id);

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var model = Xunit.Assert.IsAssignableFrom<PatientViewModel>(viewResult.Value);
            Xunit.Assert.Equal(this.lpatient[1].Email, model.Email);
            Xunit.Assert.Equal(this.lpatient[1].Name, model.Name);
            Xunit.Assert.Equal(this.lpatient[1].LastName, model.LastName);
            Xunit.Assert.Equal(this.lpatient[1].id, model.id);
        }

        [TestMethod]
        [Fact]
        public async Task RemovePatient_Should_Remove_A_Existing_Patient()
        {
            this._mockUserRepo.Setup(repo => repo.DeletePatient(It.IsAny<Patient>())).Returns(this.lpatient);
            this._sut = new PatientController(this._mockUserRepo.Object);

            PatientViewModel nPatient =
                new PatientViewModel { id = 2, Name = "Cerizo", LastName = "Remundo", Email = "cremundo@werfen.com" };

            // Act
            var result = this._sut.remove(nPatient.id);

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
                                            id = 1,
                                            Name = "Arturo",
                                            LastName = "Ciguendo",
                                            Email = "aciguendo@werfen.com"
                                        },
                                    new Patient
                                        {
                                            id = 2,
                                            Name = "Sofia",
                                            LastName = "Corona",
                                            Email = "scorona@werfen.com"
                                        },
                                    new Patient
                                        {
                                            id = 3,
                                            Name = "Marta",
                                            LastName = "Sanchez",
                                            Email = "msanchez@werfen.com"
                                        }
                                };
        }
    }
}