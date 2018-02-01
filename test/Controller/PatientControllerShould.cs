using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using seed_dotnet.Controllers;
using seed_dotnet.Models;
using seed_dotnet.ViewModels;
using Moq;
using Xunit;
using seed_dotnet.Services;
using seed_dotnet.Controllers.Api;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace test_seed_dotnet.Controller
{
    [TestClass]
    public class PatientControllerShould
    {
        private List<Patient> lpatient;
        private readonly Mock<ISeed_dotnetRepository> _mockUserRepo;
        private  PatientController _sut;
        public PatientControllerShould()
        {
          //  InitContext();
            Mapper.Reset();
            Mapper.Initialize(config =>
            {
                config.CreateMap<PatientViewModel, Patient>().ReverseMap();
            });
            _mockUserRepo = new Mock<ISeed_dotnetRepository>();
           
        }

       

        [TestInitialize]
        public void TestInitialize()
        {
            lpatient = new List<Patient>
            {
                new Patient
                {
                    id = 1,
                    Name = "Arturo",
                    LastName = "Ciguendo",
                    Email ="aciguendo@werfen.com"
                },
                new Patient
                {
                    id = 2,
                    Name = "Sofia",
                    LastName = "Corona",
                    Email ="scorona@werfen.com"
                },
                new Patient
                {
                    id = 3,
                    Name = "Marta",
                    LastName = "Sanchez",
                    Email ="msanchez@werfen.com"
                }
            };
        }


        [TestMethod]
        [Fact]
        public async Task GetAllPatients_Should_Return_List_Of_Patients()
        {
            _mockUserRepo.Setup(repo => repo.GetAllPatients()).Returns(lpatient);
            _sut = new PatientController(_mockUserRepo.Object);

            // Act
            var result = _sut.getAllPatients();

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var model = Xunit.Assert.IsAssignableFrom<List<PatientViewModel>>(viewResult.Value);
            Xunit.Assert.Equal(3, model.Count());
        }

        [TestMethod]
        [Fact]
        public async Task GetPatients_Should_Return_Patient_Information()
        {

            _mockUserRepo.Setup(repo => repo.GetPatient(It.IsAny<Patient>())).Returns(lpatient[1]);
            _sut = new PatientController(_mockUserRepo.Object);

            // Act
            var result = _sut.getPatient(lpatient[1].id);

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result);
            var model = Xunit.Assert.IsAssignableFrom<PatientViewModel>(viewResult.Value);
            Xunit.Assert.Equal(lpatient[1].Email, model.Email);
            Xunit.Assert.Equal(lpatient[1].Name, model.Name);
            Xunit.Assert.Equal(lpatient[1].LastName, model.LastName);
            Xunit.Assert.Equal(lpatient[1].id, model.id);

        }

        [TestMethod]
        [Fact]
        public async Task CreatePatient_ReturnsBadRequest_GivenInvalidPatient()
        {
            // Arrange & Act
            _mockUserRepo.Setup(repo => repo.AddPatient(It.IsAny<Patient>()));
            _sut = new PatientController(_mockUserRepo.Object);
            _sut.ModelState.AddModelError("error", "some error");

            // Act
            var result = await _sut.createPatient(patient: null);

            // Assert
            Xunit.Assert.IsType<BadRequestObjectResult>(result);
        }

        [TestMethod]
        [Fact]
        public async Task CreatePatient_Should_Create_A_New_Patient()
        {

            _mockUserRepo.Setup(repo => repo.AddPatient(It.IsAny<Patient>()));
            _sut = new PatientController(_mockUserRepo.Object);

            PatientViewModel nPatient = new PatientViewModel
            {
               
                Name = "Carlos",
                LastName = "Carmona",
                Email = "ccarmona@werfen.com"
            };

            // Act
            var result = _sut.createPatient(nPatient);

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var model = Xunit.Assert.IsType<PatientViewModel>(viewResult.Value);
            _mockUserRepo.Verify();
            Xunit.Assert.Equal(nPatient.Email, model.Email);
            Xunit.Assert.Equal(nPatient.Name, model.Name);
            Xunit.Assert.Equal(nPatient.LastName, model.LastName);
        }
        [TestMethod]
        [Fact]
        public async Task RemovePatient_Should_Remove_A_Existing_Patient()
        {
            _mockUserRepo.Setup(repo => repo.DeletePatient(It.IsAny<Patient>())).Returns(lpatient);
            _sut = new PatientController(_mockUserRepo.Object);

            PatientViewModel nPatient = new PatientViewModel
            {
                id=2,
                Name = "Cerizo",
                LastName = "Remundo",
                Email = "cremundo@werfen.com"
            };

            // Act
            var result = _sut.remove(nPatient.id);

            // Assert
            var viewResult = Xunit.Assert.IsType<OkObjectResult>(result.Result);
            var model = Xunit.Assert.IsAssignableFrom<List<PatientViewModel>>(viewResult.Value);
            Xunit.Assert.Equal(3, model.Count());
        }



    }
}
