namespace IntegrationTest
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Main;
    using Main.Models;
    using Main.ViewModels;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using Newtonsoft.Json;

    using Xunit;

    public class LoginTest
    {
        private readonly HttpClient client;

        private readonly TestServer server;

        private string currentToken;

        public LoginTest()
        {
            this.server = new TestServer(new WebHostBuilder().UseEnvironment("Testing").UseStartup<Startup>());

            this.server.Host.Seed();
            this.client = this.server.CreateClient();

            this.ClearPatientData();
            this.SeedPatient(this.KnownPatients);
        }

        public PatientViewModel[] KnownPatients =>
            new[]
                {
                    new PatientViewModel() { Email = "valid@email.com", Id = 1, LastName = "Pérez", Name = "Silvio" },
                    new PatientViewModel()
                        {
                            Email = "another_valid@email.com",
                            Id = 2,
                            LastName = "Maragall",
                            Name = "Lluís"
                        },
                    new PatientViewModel()
                        {
                            Email = "third_email@email.com",
                            Id = 3,
                            LastName = "Malafont",
                            Name = "Andreu"
                        },
                    new PatientViewModel()
                        {
                            Email = "fourth_email@email.com",
                            Id = 4,
                            LastName = "De la Cruz",
                            Name = "Penélope"
                        },
                };

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", 1)]
        public async Task CreatePatient_BadRequest(string name, string lastname, string email, int id)
        {
            PatientViewModel patientToUpdate = new PatientViewModel() { Email = email, Id = id, LastName = lastname, Name = name };

            // Act
            var response = await this.CallCreatePatient(patientToUpdate);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", 10)]
        [InlineData("joe", "", "email@valid.com", 10)]
        [InlineData("joe", "doe", "", 10)]
        [InlineData("joe", "doe", null, 10)]
        [InlineData("joe", null, null, 10)]
        [InlineData("", null, null, 10)]
        [InlineData(null, null, null, 10)]
        public async Task CreatePatient_CreationOK(string name, string lastname, string email, int id)
        {
            PatientViewModel patientToUpdate = new PatientViewModel() { Email = email, Id = id, LastName = lastname, Name = name };

            // Act
            var response = await this.CallCreatePatient(patientToUpdate);
            response.EnsureSuccessStatusCode();
            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotNull(patient);
            Assert.Equal(this.KnownPatients.First(p => p.Id == id), patient);
        }

        [Fact]
        public async Task GetListOfPatients_EmptyList()
        {
            // Arrange
            this.ClearPatientData();

            // Act
            var response = await this.CallGetPatientList();
            response.EnsureSuccessStatusCode();      
            var patientList = JsonConvert.DeserializeObject<List<PatientViewModel>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Empty(patientList);
        }

        [Fact]
        public async Task GetListOfPatients_PopulatedList()
        {
            // Act
            var response = await this.CallGetPatientList();
            response.EnsureSuccessStatusCode();

            var patientList = JsonConvert.DeserializeObject<List<PatientViewModel>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotEmpty(patientList);
            Assert.Equal(this.KnownPatients, patientList);
        }

        // TODO: do the following two for each call
        public async Task GetListOfPatients_Timedout()
        {
        }

        [Fact]
        public async Task GetListOfPatients_Unauthorized()
        {
            var response = await this.server.CreateRequest("/patients").GetAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetPatient_Unauthorized()
        {
            var response = await this.server.CreateRequest("/patients/1").GetAsync();

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        public async Task GetPatient_Found(int id)
        {
            // Act
            var response = await this.CallGetPatient(id);
            response.EnsureSuccessStatusCode();

            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotNull(patient);
            Assert.Equal(this.KnownPatients.First(p => p.Id == id), patient);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(999)]
        [InlineData(-1)]
        public async Task GetPatient_NotFound(int id)
        {
            // Act
            var response = await this.CallGetPatient(id);
            response.EnsureSuccessStatusCode();

            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Null(patient);
        }

        [Fact(Skip = "The login interface is about to change")]
        public async Task InvalidLogin_BadRequest()
        {
        }

        [Fact]
        public async Task RemovePatient_Unauthorized()
        {
            var response = await this.server.CreateRequest("/patients/1").SendAsync("DELETE");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        public async Task RemovePatient_Found(int id)
        {
            // Act
            var response = await this.CallDeletePatient(id);
            response.EnsureSuccessStatusCode();
            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotNull(patient);
            Assert.Equal(this.KnownPatients.First(p => p.Id == id), patient);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(999)]
        [InlineData(-1)]
        public async Task RemovePatient_NotFound(int id)
        {
            // Act
            var response = await this.CallDeletePatient(id);
            response.EnsureSuccessStatusCode();
            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Null(patient);
        }

        [Fact(Skip = "The login interface is about to change")]
        public async Task TwoLogins_SameTokenUpdatedExpirationDate()
        {
        }

        [Fact]
        public async Task UpdatePatient_Unauthorized()
        {
            var response = await this.server.CreateRequest("/patients/1").SendAsync("PUT");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", 1)]
        [InlineData("joe", "", "email@valid.com", 1)]
        [InlineData("joe", "doe", "", 1)]
        [InlineData("joe", "doe", null, 1)]
        [InlineData("joe", null, null, 1)]
        [InlineData("", null, null, 1)]
        [InlineData(null, null, null, 1)]
        public async Task UpdatePatient_CreationOk(string name, string lastname, string email, int id)
        {
            PatientViewModel patientToUpdate = new PatientViewModel() { Email = email, Id = id, LastName = lastname, Name = name };

            // Act
            var response = await this.CallUpdatePatient(patientToUpdate);
            response.EnsureSuccessStatusCode();
            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotNull(patient);
            Assert.Equal(this.KnownPatients.First(p => p.Id == id), patient);
        }

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", 0)]
        public async Task UpdatePatient_BadRequest(string name, string lastname, string email, int id)
        {
            PatientViewModel patientToUpdate = new PatientViewModel() { Email = email, Id = id, LastName = lastname, Name = name };

            // Act
            var response = await this.CallUpdatePatient(patientToUpdate);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact(Skip = "The login interface is about to change")]
        public async Task ValidLogin_ReturnToken()
        {
            var requestData = new LoginViewModel() { UserName = "admin", Password = "P@ssw0rd!" };

            // Act
            var response = await this.client.PostAsJsonAsync("/users/login", requestData);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
        }

        private RequestBuilder BuildAuthorizedRequest(string uri)
        {
            return this.server.CreateRequest(uri).WithAuthorization(this.currentToken);
        }

        private async Task<HttpResponseMessage> CallCreatePatient(PatientViewModel patient)
        {
            return await this.BuildAuthorizedRequest($"/patients/patient").PostAsync();
        }

        private async Task<HttpResponseMessage> CallDeletePatient(int id)
        {
            return await this.BuildAuthorizedRequest($"/patients/{id}").SendAsync("DELETE");
        }

        private async Task<HttpResponseMessage> CallGetPatient(int id)
        {
            return await this.BuildAuthorizedRequest($"/patients/{id}").GetAsync();
        }

        private async Task<HttpResponseMessage> CallGetPatientList()
        {
            return await this.BuildAuthorizedRequest("/patients").GetAsync();
        }

        private Task<HttpResponseMessage> CallUnauthorized(string uri)
        {
            return this.server.CreateRequest(uri).GetAsync();
        }

        private async Task<HttpResponseMessage> CallUpdatePatient(PatientViewModel patient)
        {
            return await this.BuildAuthorizedRequest($"/patients").SendAsync("PUT");
        }

        private void ClearPatientData()
        {
            using (var scope = this.server.Host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<SeedDotnetContext>())
                {
                    context.Patients.FromSql("DELETE TABLE Patients");
                    context.SaveChanges();
                }
            }
        }

        private void SeedPatient(PatientViewModel[] patientList)
        {
            using (var scope = this.server.Host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<SeedDotnetContext>())
                {
                    foreach (var patient in patientList)
                    {
                        context.Patients.Add(AutoMapper.Mapper.Map<Patient>(patient));
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}