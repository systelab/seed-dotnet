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
        private readonly Mock<ISeed_dotnetRepository> _mockRepository;
        private  PatientController _sut;
        public PatientControllerShould()
        {
          //  InitContext();
            Mapper.Reset();
            Mapper.Initialize(config =>
            {
                config.CreateMap<PatientViewModel, Patient>().ReverseMap();
            });
              _mockRepository = new Mock<ISeed_dotnetRepository>();
              _sut = new PatientController(_mockRepository.Object);
        }

       

        [TestInitialize]
        public void TestInitialize()
        {
            lpatient = new List<Patient>
            {
                new Patient
                {
                    id = 1,
                    Name = "Adidas",
                    LastName = "LAdidas",
                    Email ="a@a.com"
                },
                new Patient
                {
                    id = 2,
                    Name = "Nike",
                    LastName = "LNike",
                    Email ="a@a.com"
                },
                new Patient
                {
                    id = 3,
                    Name = "Puma",
                    LastName = "LPuma",
                    Email ="a@a.com"
                }
            };
        }

        [TestMethod]
        public async System.Threading.Tasks.Task IsAddingUsersAsync()
        {
            //Arrange
            PatientViewModel p = new PatientViewModel();
            p.Email = "aww@sad.com";
            p.Name = "Test1";
            p.LastName = "Test1lastname";
            var mock = new Mock<seed_dotnet.Services.ISeed_dotnetRepository>();
            var patientservice = new PatientController(mock.Object);

            //Act
            await patientservice.createPatient(p);

            //Assert
            mock.Verify(x => x.AddPatient(It.IsAny<Patient>()), Times.Exactly(1));
        }

        [TestMethod]
        [Fact]
        public async Task CheckModelState()
        {
            Mock<ISeed_dotnetRepository> mockUserRepo = new Mock<ISeed_dotnetRepository>();
            
            Patient p = lpatient[0];
            mockUserRepo.Setup(m => m.GetPatient(p)).Returns(p);
            PatientController controller = new PatientController(
                 mockUserRepo.Object);

            // Act
            var result = controller.getPatient(3);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsNotNull(result);

        }

        [TestMethod]
        public async System.Threading.Tasks.Task GetPatients()
        {
            // Arrange
            var product = lpatient[0];
            var mock = new Mock<seed_dotnet.Services.ISeed_dotnetRepository>();

            var patientservice = new PatientController(mock.Object);

            // Act
            var result = patientservice.getPatient(3);

            // Assert
           // Xunit.Assert.AreSame(product, result);
        }




    }
}
