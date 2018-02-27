namespace IntegrationTest
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    using AutoMapper;

    using Main;
    using Main.Models;
    using Main.ViewModels;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using Newtonsoft.Json;

    using Xunit;

    public class Integration
    {
        private readonly HttpClient client;

        private readonly TestServer server;

        private string currentToken;

        public Integration()
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
                    new PatientViewModel() { Email = "valid@email.com", Id = 1, Surname = "Pérez", Name = "Silvio" },
                    new PatientViewModel()
                        {
                            Email = "another_valid@email.com",
                            Id = 2,
                            Surname = "Maragall",
                            Name = "Lluís"
                        },
                    new PatientViewModel()
                        {
                            Email = "third_email@email.com",
                            Id = 3,
                            Surname = "Malafont",
                            Name = "Andreu"
                        },
                    new PatientViewModel()
                        {
                            Email = "fourth_email@email.com",
                            Id = 4,
                            Surname = "De la Cruz",
                            Name = "Penélope"
                        },
                };

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", 1)]
        [InlineData("joe", "", "email@valid.com", 10)]
        [InlineData("joe", "doe", "", 10)]
        [InlineData("joe", "doe", null, 10)]
        [InlineData("joe", null, null, 10)]
        [InlineData("", null, null, 10)]
        [InlineData(null, null, null, 10)]
        public async Task CreatePatient_BadRequest(string name, string lastname, string email, int id)
        {
            // Arrange
            await this.Authorize();
            PatientViewModel patientToUpdate = new PatientViewModel() { Email = email, Id = id, Surname = lastname, Name = name };

            // Act
            var response = await this.CallCreatePatient(patientToUpdate);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", 10)]
        public async Task CreatePatient_CreationOK(string name, string lastname, string email, int id)
        {
            // Arrange
            await this.Authorize();
            PatientViewModel patientToUpdate = new PatientViewModel() { Email = email, Id = id, Surname = lastname, Name = name };

            // Act
            var response = await this.CallCreatePatient(patientToUpdate);
            response.EnsureSuccessStatusCode();
            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotNull(patient);
            Assert.Equal(patientToUpdate, patient, new JsonEqualityComparer());
        }

        [Fact]
        public async Task GetListOfPatients_EmptyList()
        {
            // Arrange
            await this.Authorize();
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
            // Arrange
            await this.Authorize();
            // Act
            var response = await this.CallGetPatientList();
            response.EnsureSuccessStatusCode();

            var patientList = JsonConvert.DeserializeObject<List<PatientViewModel>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotEmpty(patientList);
            Assert.Equal(this.KnownPatients, patientList, new JsonEqualityComparer());
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
            // Arrange
            await this.Authorize();
            
            // Act
            var response = await this.CallGetPatient(id);
            response.EnsureSuccessStatusCode();

            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotNull(patient);
            Assert.Equal(this.KnownPatients.First(p => p.Id == id), patient, new JsonEqualityComparer());
        }

        [Theory]
        [InlineData(5)]
        [InlineData(999)]
        [InlineData(-1)]
        public async Task GetPatient_NotFound(int id)
        {
            // Arrange
            await this.Authorize();
            
            // Act
            var response = await this.CallGetPatient(id);
            response.EnsureSuccessStatusCode();

            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.Null(patient);
        }

        [Fact]
        public async Task InvalidLogin_BadRequest()
        {
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("login", "admin"));
            var req = new HttpRequestMessage(HttpMethod.Post, "/users/login")
                          {
                              Content = new FormUrlEncodedContent(
                                  nvc)
                          };
            var response = await this.client.SendAsync(req);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
            // Arrange
            await this.Authorize();
            
            // Act
            var response = await this.CallDeletePatient(id);
            response.EnsureSuccessStatusCode();
            var patients = JsonConvert.DeserializeObject<IEnumerable<PatientViewModel>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotNull(patients);
            Assert.Equal(this.KnownPatients.Count() - 1, patients.Count());
        }

        [Theory]
        [InlineData(5)]
        [InlineData(999)]
        [InlineData(-1)]
        public async Task RemovePatient_NotFound(int id)
        {
            // Arrange
            await this.Authorize();
            
            // Act
            var response = await this.CallDeletePatient(id);
            response.EnsureSuccessStatusCode();
            var patients = JsonConvert.DeserializeObject<IEnumerable<PatientViewModel>>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotNull(patients);
            Assert.Equal(this.KnownPatients.Count(), patients.Count());
        }

        [Fact]
        public async Task UpdatePatient_Unauthorized()
        {
            var response = await this.server.CreateRequest("/patients/1").SendAsync("PUT");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", 1)]        
        public async Task UpdatePatient_CreationOk(string name, string lastname, string email, int id)
        {
            // Arrange
            await this.Authorize();
            PatientViewModel patientToUpdate = new PatientViewModel() { Email = email, Id = id, Surname = lastname, Name = name };

            // Act
            var response = await this.CallUpdatePatient(patientToUpdate);
            response.EnsureSuccessStatusCode();
            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync());

            // Assert
            Assert.NotNull(patient);
            Assert.NotEqual(this.KnownPatients.First(p => p.Id == id), patient, new JsonEqualityComparer());
            Assert.Equal(patientToUpdate, patient, new JsonEqualityComparer());
        }

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", 0)]
        [InlineData("joe", "", "email@valid.com", 1)]
        [InlineData("joe", "doe", "", 1)]
        [InlineData("joe", "doe", null, 1)]
        [InlineData("joe", null, null, 1)]
        [InlineData("", null, null, 1)]
        [InlineData(null, null, null, 1)]
        public async Task UpdatePatient_BadRequest(string name, string lastname, string email, int id)
        {
            // Arrange
            await this.Authorize();
            PatientViewModel patientToUpdate = new PatientViewModel() { Email = email, Id = id, Surname = lastname, Name = name };

            // Act
            var response = await this.CallUpdatePatient(patientToUpdate);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ValidLogin_ReturnToken()
        {
            var response = await this.RequestToken();
            response.EnsureSuccessStatusCode();
            var token = response.Headers.GetValues("Authorization").FirstOrDefault();
            Assert.NotNull(token);
        }

        private async Task<HttpResponseMessage> RequestToken()
        {
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("login", "admin"));
            nvc.Add(new KeyValuePair<string, string>("password", "P@ssw0rd!"));
            var req = new HttpRequestMessage(HttpMethod.Post, "/users/login")
                          {
                              Content = new FormUrlEncodedContent(
                                  nvc)
                          };
            return await this.client.SendAsync(req);
        }

        private async Task Authorize()
        {
            var response = await this.RequestToken();
            response.EnsureSuccessStatusCode();
            this.currentToken = response.Headers.GetValues("Authorization").First();
        }

        private RequestBuilder BuildAuthorizedRequest(string uri)
        {
            return this.server.CreateRequest(uri).WithAuthorization(this.currentToken).WithAppJsonContentType();
        }

        private async Task<HttpResponseMessage> CallCreatePatient(PatientViewModel patient)
        {
            return await this.BuildAuthorizedRequest($"/patients/patient").WithJsonContent(patient).PostAsync();
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
            return await this.BuildAuthorizedRequest($"/patients/{patient.Id}").WithJsonContent(patient).SendAsync("PUT");
        }

        private void ClearPatientData()
        {
            using (var scope = this.server.Host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<SeedDotnetContext>())
                {
                    var patients = context.Patients.ToList();

                    foreach (var patient in patients )
                    {
                        context.Patients.Remove(patient);
                    }

                    context.SaveChanges();
                }
            }
        }

        private void SeedPatient(PatientViewModel[] patientList)
        {
            using (var scope = this.server.Host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                var mapper = this.server.Host.Services.GetService<IMapper>();
                using (var context = scope.ServiceProvider.GetRequiredService<SeedDotnetContext>())
                {
                    foreach (var patient in patientList)
                    {
                        context.Patients.Add(mapper.Map<Patient>(patient));
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}