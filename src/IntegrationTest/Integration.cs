namespace IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using AutoMapper;
    using main;
    using main.Entities;
    using main.Entities.Common;
    using main.Entities.Models;
    using main.Entities.ViewModels;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
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

        public  PatientViewModel[] KnownPatients =>
            new[]
                {
                    new PatientViewModel { Email = "valid@email.com", Id = new Guid("89aca2d6-f261-4a2c-a927-2ab029fb7959"), Surname = "Pérez", Name = "Silvio" },
                    new PatientViewModel
                        {
                            Email = "another_valid@email.com",
                            Id = new Guid("bd134275-c944-49aa-923b-a13b4a1ab54b"),
                            Surname = "Maragall",
                            Name = "Lluís",
                            MedicalNumber = "5656"
                        },
                    new PatientViewModel
                        {
                            Email = "third_email@email.com",
                            Id = new Guid("b8ac6260-2b95-4b7d-bd09-0653e2ac1fb6"),
                            Surname = "Malafont",
                            Name = "Andreu",
                            MedicalNumber = "5656"
                        },
                    new PatientViewModel
                        {
                            Email = "fourth_email@email.com",
                            Id = new Guid("6667cc4b-0efb-45c1-a57b-0d3089103e7b"),
                            Surname = "De la Cruz",
                            Name = "Penélope",
                            MedicalNumber = "5656"
                        },
                };

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", "bd134275-c944-49aa-923b-a13b4a1ab54b", "valid")]
        [InlineData("joe", "", "email@valid.com", "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [InlineData("joe", "doe", "", "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [InlineData("joe", "doe", null, "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [InlineData("joe", null, null, "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [InlineData("", null, null, "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [InlineData(null, null, null, "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [InlineData("joe", "doe", "@invalid.com", "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [InlineData("_joe_is_longer_than_255_characters_01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890", 
            "doe", "email@valid.com", "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [InlineData("joe", 
            "_doe_is_longer_than_255_characters_01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",
            "email@valid.com", "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [InlineData("joe", "doe", "email@valid.com", "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e",
            "_medicalNumber_is_longer_than_255_characters_01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
        public async Task CreatePatient_BadRequest(string name, string lastname, string email, string idGuid, string medicalNumber)
        {
            Guid id = new Guid(idGuid);
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            PatientViewModel patientToUpdate = new PatientViewModel { Email = email, Id = id, Surname = lastname, Name = name, MedicalNumber = medicalNumber};

            // Act
            var response = await this.CallCreatePatient(patientToUpdate).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "medicalNumber")]
        public async Task CreatePatient_CreationOK(string name, string lastname, string email, string idGuid, string medicalNumber)
        {
            Guid id = new Guid(idGuid);
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            PatientViewModel patientToUpdate = new PatientViewModel { Email = email, Id = id, Surname = lastname, Name = name, MedicalNumber = medicalNumber};

            // Act
            var response = await this.CallCreatePatient(patientToUpdate).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            Assert.NotNull(patient);
            // We expect to have the MedicalNumber service to fail
            patientToUpdate.MedicalNumber = "UNDEFINED";
            Assert.Equal(patientToUpdate, patient, new JsonEqualityComparer());
        }

        [Fact]
        public async Task GetListOfPatients_EmptyList()
        {
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            this.ClearPatientData();

            // Act
            var response = await this.CallGetPatientList(0, 23).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();      
            var patientList = JsonConvert.DeserializeObject<ExtendedPagedList<PatientViewModel>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            Assert.Empty(patientList.Content);
        }

        [Fact]
        public async Task GetListOfPatients_PopulatedList()
        {
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            // Act
            var response = await this.CallGetPatientList(0, 23).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var patientList = JsonConvert.DeserializeObject<ExtendedPagedList<PatientViewModel>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            Assert.NotEmpty(patientList.Content);
            Assert.Equal(this.KnownPatients, patientList.Content, new JsonEqualityComparer());
        }

        [Fact]
        public async Task GetListOfPatients_PopulatedList_Paged()
        {
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            for (int i = 0; i < 3; i++)
            {
                // Act
                var response = await this.CallGetPatientList(i, 1).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var patientList = JsonConvert.DeserializeObject<ExtendedPagedList<PatientViewModel>>(
                    await response.Content.ReadAsStringAsync().ConfigureAwait(false));

                // Assert
                Assert.NotEmpty(patientList.Content);
                Assert.Equal(this.KnownPatients.Skip(i).Take(1), patientList.Content, new JsonEqualityComparer());
            }
        }

        [Fact]
        public async Task GetListOfPatients_Unauthorized()
        {
            var response = await this.server.CreateRequest("seed/v1/patients").GetAsync().ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetPatient_Unauthorized()
        {
            var response = await this.server.CreateRequest("seed/v1/patients/1").GetAsync().ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("bd134275-c944-49aa-923b-a13b4a1ab54b")]
        [InlineData("b8ac6260-2b95-4b7d-bd09-0653e2ac1fb6")]
        [InlineData("6667cc4b-0efb-45c1-a57b-0d3089103e7b")]
        public async Task GetPatient_Found(string idGuid)
        {
            Guid id = new Guid(idGuid);
            // Arrange
            await this.Authorize().ConfigureAwait(false);

            // Act
            var response = await this.CallGetPatient(id).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            Assert.NotNull(patient);
            Assert.Equal(this.KnownPatients.First(p => p.Id == id), patient, new JsonEqualityComparer());
        }

        [Fact]
        public async Task GetPatient_NotFound()
        {
            Guid id = Guid.NewGuid();
            // Arrange
            await this.Authorize().ConfigureAwait(false);

            // Act
            var response = await this.CallGetPatient(id).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            Assert.Null(patient);
        }

        [Fact]
        public async Task InvalidLogin_BadRequest()
        {
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("login", "admin"));
            var req = new HttpRequestMessage(HttpMethod.Post, "seed/v1/users/login")
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
            var response = await this.server.CreateRequest("seed/v1/patients/1").SendAsync("DELETE");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("bd134275-c944-49aa-923b-a13b4a1ab54b")]
        [InlineData("b8ac6260-2b95-4b7d-bd09-0653e2ac1fb6")]
        [InlineData("6667cc4b-0efb-45c1-a57b-0d3089103e7b")]
        public async Task RemovePatient_Found(string idGuid)
        {
            Guid id = new Guid(idGuid);
            // Arrange
            await this.Authorize().ConfigureAwait(false);

            // Act
            var response = await this.CallDeletePatient(id).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();            
        }

        [Theory]
        [InlineData("bd134225-c944-49aa-923b-a13b4a1ab54b")]
        [InlineData("b8ac6230-2b95-4b7d-bd09-0653e2ac1fb6")]
        [InlineData("6667ce4b-0efb-45c1-a57b-0d3089103e7b")]
        public async Task RemovePatient_NotFound(string idGuid)
        {
            Guid id = new Guid(idGuid);
            // Arrange
            await this.Authorize().ConfigureAwait(false);

            // Act
            var response = await this.CallDeletePatient(id).ConfigureAwait(false);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task UpdatePatient_Unauthorized()
        {
            var response = await this.server.CreateRequest("seed/v1/patients/1").SendAsync("PUT").ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("joe", "doe", "email@valid.com", "5657")]        
        public async Task UpdatePatient_CreationOk(string name, string lastname, string email, string medicalNumber)
        {
            Guid id = new Guid("6667cc4b-0efb-45c1-a57b-0d3089103e7b");
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            PatientViewModel patientToUpdate = new PatientViewModel { Email = email, Id = id, Surname = lastname, Name = name, MedicalNumber = medicalNumber };

            // Act
            var response = await this.CallUpdatePatient(patientToUpdate).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            Assert.NotNull(patient);
            Assert.NotEqual(this.KnownPatients.First(p => p.Id == id), patient, new JsonEqualityComparer());
            Assert.Equal(patientToUpdate, patient, new JsonEqualityComparer());
        }

        [Theory]
        [InlineData("joe", "doe", "email@valid.com")]
        [InlineData("joe", "", "email@valid.com")]
        [InlineData("joe", "doe", "")]
        [InlineData("joe", "doe", null)]
        [InlineData("joe", null, null)]
        [InlineData("", null, null)]
        [InlineData(null, null, null)]
        public async Task UpdatePatient_BadRequest(string name, string lastname, string email)
        {
            Guid id = Guid.NewGuid();
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            PatientViewModel patientToUpdate = new PatientViewModel { Email = email, Id = id, Surname = lastname, Name = name };

            // Act
            var response = await this.CallUpdatePatient(patientToUpdate).ConfigureAwait(false);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ValidLogin_ReturnToken()
        {
            var response = await this.RequestToken().ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var token = response.Headers.GetValues("Authorization").FirstOrDefault();
            Assert.NotNull(token);
        }

        private async Task<HttpResponseMessage> RequestToken()
        {
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("login", "Systelab"));
            nvc.Add(new KeyValuePair<string, string>("password", "Systelab"));
            var req = new HttpRequestMessage(HttpMethod.Post, "seed/v1/users/login")
                          {
                              Content = new FormUrlEncodedContent(
                                  nvc)
                          };
            return await this.client.SendAsync(req).ConfigureAwait(false);
        }

        private async Task Authorize()
        {
            var response = await this.RequestToken().ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            this.currentToken = response.Headers.GetValues("Authorization").First();
        }

        private RequestBuilder BuildAuthorizedRequest(string uri)
        {
            return this.server.CreateRequest(uri).WithAuthorization(this.currentToken).WithAppJsonContentType();
        }

        private async Task<HttpResponseMessage> CallCreatePatient(PatientViewModel patient)
        {
            return await this.BuildAuthorizedRequest($"seed/v1/patients/patient").WithJsonContent(patient).PostAsync().ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> CallDeletePatient(Guid id)
        {
            return await this.BuildAuthorizedRequest($"seed/v1/patients/{id}").SendAsync("DELETE").ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> CallGetPatient(Guid id)
        {
            return await this.BuildAuthorizedRequest($"seed/v1/patients/{id}").GetAsync().ConfigureAwait(false);
        }

        private async Task<HttpResponseMessage> CallGetPatientList(int page, int size)
        {
            return await this.BuildAuthorizedRequest($"seed/v1/patients?page={page}&size={size}").GetAsync().ConfigureAwait(false);
        }

        private Task<HttpResponseMessage> CallUnauthorized(string uri)
        {
            return this.server.CreateRequest(uri).GetAsync();
        }

        private async Task<HttpResponseMessage> CallUpdatePatient(PatientViewModel patient)
        {
            return await this.BuildAuthorizedRequest($"seed/v1/patients/{patient.Id}").WithJsonContent(patient).SendAsync("PUT").ConfigureAwait(false);
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