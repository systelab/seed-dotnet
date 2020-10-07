namespace IntegrationTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    using AutoMapper;

    using FluentAssertions;

    using main;
    using main.Entities;
    using main.Entities.Common;
    using main.Entities.Models;
    using main.Entities.ViewModels;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.DependencyInjection;

    using Newtonsoft.Json;

    using NUnit.Allure.Attributes;
    using NUnit.Allure.Core;
    using NUnit.Allure.Steps;
    using NUnit.Framework;

    [AllureEpic("TestAPI")]
    [AllureNUnit]
    [AllureFeature("The scope of this scenario is to verify that the API response as expected. In addition, this scenario also interacts with the database.")]
    public class IntegrationNunit
    {
        private readonly HttpClient client;

        private readonly TestServer server;

        private string currentToken;

        public IntegrationNunit()
        {
            this.server = new TestServer(new WebHostBuilder().UseEnvironment("Testing").UseStartup<Startup>());

            this.server.Host.Seed();
            this.client = this.server.CreateClient();

            this.ClearPatientData();
            this.SeedPatient(KnownPatients);
        }

        public static PatientViewModel[] KnownPatients =>
            new[]
                {
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
                            Email = "another_valid@email.com",
                            Id = new Guid("bd134275-c944-49aa-923b-a13b4a1ab54b"),
                            Surname = "Maragall",
                            Name = "Lluís",
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
                    new PatientViewModel { Email = "valid@email.com", Id = new Guid("89aca2d6-f261-4a2c-a927-2ab029fb7959"), Surname = "Pérez", Name = "Silvio" }
                };

        [AllureTms("CreatePatient_BadRequest")]
        [Description("Create patients with differents values to invalid request")]
        [TestCase("joe", "doe", "email@valid.com", "bd134275-c944-49aa-923b-a13b4a1ab54b", "valid")]
        [TestCase("joe", "", "email@valid.com", "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [TestCase("joe", "doe", "", "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [TestCase("joe", "doe", null, "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [TestCase("joe", null, null, "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [TestCase("", null, null, "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [TestCase(null, null, null, "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [TestCase("joe", "doe", "@invalid.com", "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "valid")]
        [TestCase(
            "_joe_is_longer_than_255_characters_01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",
            "doe",
            "email@valid.com",
            "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e",
            "valid")]
        [TestCase(
            "joe",
            "_doe_is_longer_than_255_characters_01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",
            "email@valid.com",
            "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e",
            "valid")]
        [TestCase(
            "joe",
            "doe",
            "email@valid.com",
            "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e",
            "_medicalNumber_is_longer_than_255_characters_01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
        [NonParallelizable]
        public async Task CreatePatient_BadRequest(string name, string surname, string email, string idGuid, string medicalnumber)
        {
            PatientViewModel patientToUpdate = this.CreatePatienWithIncorrectData(name, surname, email, idGuid, medicalnumber);
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            // Act
            HttpResponseMessage response = await this.CallCreatePatient(patientToUpdate).ConfigureAwait(false);
            this.StatusCode(response);
        }

        [AllureTms("CreatePatient_CreationOK")]
        [Description("Create patients with differents values")]
        [TestCase("joe", "doe", "email@valid.com", "c4c9b6b2-17ce-4b03-9d3e-db6b6856d99e", "medicalNumber")]
        public async Task CreatePatient_CreationOK(string name, string lastname, string email, string idGuid, string medicalNumber)
        {
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            PatientViewModel patientToUpdate = this.CreatePatienWithIncorrectData(name, lastname, email, idGuid, medicalNumber);

            // Act
            HttpResponseMessage response = await this.CallCreatePatient(patientToUpdate).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            PatientViewModel patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            Assert.NotNull(patient);
            this.DataIsUploadExceptEmail(patientToUpdate, patient);
        }

        [AllureStep("Create patients with differents values")]
        public PatientViewModel CreatePatienWithIncorrectData(string name, string surname, string email, string idGuid, string medicalnumber)
        {
            Guid id = new Guid(idGuid);
            PatientViewModel patientToUpdate = new PatientViewModel
                                                   {
                                                       Email = email,
                                                       Id = id,
                                                       Surname = surname,
                                                       Name = name,
                                                       MedicalNumber = medicalnumber
                                                   };
            return patientToUpdate;
        }

        [AllureStep("The data is upload except the email, that is undefined.")]
        public void DataIsUploadExceptEmail(PatientViewModel patientToUpdate, PatientViewModel patient)
        {
            patientToUpdate.MedicalNumber = "UNDEFINED";
            patient.MedicalNumber.Should().NotBeSameAs(patientToUpdate.MedicalNumber);
            patient.Should().BeEquivalentTo(patientToUpdate, config => config.Excluding(p => p.MedicalNumber));
        }

        [AllureTms("GetListOfPatients_EmptyList")]
        [Description("Get a list of patient that are empty.")]
        [Test]
        public async Task GetListOfPatients_EmptyList()
        {
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            this.ClearPatientData();

            // Act
            HttpResponseMessage response = await this.CallGetPatientList(0, 23).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            ExtendedPagedList<PatientViewModel> patientList = JsonConvert.DeserializeObject<ExtendedPagedList<PatientViewModel>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            patientList.Content.Should().BeEmpty();
        }

        [AllureTms("GetListOfPatients_PopulatedList")]
        [Description("Get a list of patients for populated list.")]
        [Test]
        public async Task GetListOfPatients_PopulatedList()
        {
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            // Act
            HttpResponseMessage response = await this.CallGetPatientList(0, 23).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            ExtendedPagedList<PatientViewModel> patientList = JsonConvert.DeserializeObject<ExtendedPagedList<PatientViewModel>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            patientList.Content.Should().NotBeEmpty();
            patientList.Content.Should().BeEquivalentTo(KnownPatients);
        }

        [AllureTms("GetListOfPatients_PopulatedList_Paged")]
        [Description("Get a list of patients for populated list paged.")]
        [Test]
        public async Task GetListOfPatients_PopulatedList_Paged()
        {
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            for (int i = 0; i < 3; i++)
            {
                // Act
                HttpResponseMessage response = await this.CallGetPatientList(i, 1).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                ExtendedPagedList<PatientViewModel> patientList = JsonConvert.DeserializeObject<ExtendedPagedList<PatientViewModel>>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                // Assert
                patientList.Content.Should().NotBeEmpty();
                patientList.Content.Should().BeEquivalentTo(KnownPatients.Skip(i).Take(1));
            }
        }

        [AllureTms("GetListOfPatients_Unauthorized")]
        [Description("Get a list of patients Unauthorized.")]
        [Test]
        public async Task GetListOfPatients_Unauthorized()
        {
            HttpResponseMessage response = await this.server.CreateRequest("seed/v1/patients").GetAsync().ConfigureAwait(false);

            response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [AllureTms("GetPatient_Found")]
        [Description("Get a different patients by id and found it.")]
        [TestCase("bd134275-c944-49aa-923b-a13b4a1ab54b")]
        [TestCase("b8ac6260-2b95-4b7d-bd09-0653e2ac1fb6")]
        [TestCase("6667cc4b-0efb-45c1-a57b-0d3089103e7b")]
        public async Task GetPatient_Found(string idGuid)
        {
            Guid id = new Guid(idGuid);
            // Arrange
            await this.Authorize().ConfigureAwait(false);

            // Act
            HttpResponseMessage response = await this.CallGetPatient(id).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            PatientViewModel patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            Assert.NotNull(patient);
            patient.Should().BeEquivalentTo(KnownPatients.First(p => p.Id == id));
        }

        [AllureTms("GetPatient_NotFound")]
        [Description("Get a list of patients that not exists.")]
        [Test]
        public async Task GetPatient_NotFound()
        {
            Guid id = Guid.NewGuid();
            // Arrange
            await this.Authorize().ConfigureAwait(false);

            // Act
            HttpResponseMessage response = await this.CallGetPatient(id).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            PatientViewModel patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            patient.Should().BeNull();
        }

        [AllureTms("GetPatient_Unauthorized")]
        [Description("Get a Unauthorized patient.")]
        [Test]
        public async Task GetPatient_Unauthorized()
        {
            HttpResponseMessage response = await this.server.CreateRequest("seed/v1/patients/1").GetAsync().ConfigureAwait(false);

            response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [AllureTms("InvalidLogin_BadRequest")]
        [Description("Login with invalid user.")]
        [Test]
        public async Task InvalidLogin_BadRequest()
        {
            List<KeyValuePair<string, string>> nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("login", "admin"));
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "seed/v1/users/login") { Content = new FormUrlEncodedContent(nvc) };
            HttpResponseMessage response = await this.client.SendAsync(req);

            response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.BadRequest);
        }

        [AllureTms("RemovePatient_Found")]
        [Description("Remove a different patients by id and found it.")]
        [TestCase("bd134275-c944-49aa-923b-a13b4a1ab54b")]
        [TestCase("b8ac6260-2b95-4b7d-bd09-0653e2ac1fb6")]
        [TestCase("6667cc4b-0efb-45c1-a57b-0d3089103e7b")]
        public async Task RemovePatient_Found(string idGuid)
        {
            Guid id = new Guid(idGuid);
            // Arrange
            await this.Authorize().ConfigureAwait(false);

            // Act
            HttpResponseMessage response = await this.CallDeletePatient(id).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        [AllureTms("RemovePatient_NotFound")]
        [Description("Not found Patient deleted with different IDs.")]
        [TestCase("bd134225-c944-49aa-923b-a13b4a1ab54b")]
        [TestCase("b8ac6230-2b95-4b7d-bd09-0653e2ac1fb6")]
        [TestCase("6667ce4b-0efb-45c1-a57b-0d3089103e7b")]
        public async Task RemovePatient_NotFound(string idGuid)
        {
            Guid id = new Guid(idGuid);
            // Arrange
            await this.Authorize().ConfigureAwait(false);

            // Act
            HttpResponseMessage response = await this.CallDeletePatient(id).ConfigureAwait(false);
            Assert.False(response.IsSuccessStatusCode);
        }

        [AllureTms("RemovePatient_Unauthorized")]
        [Description("Remove a patient without authorization.")]
        [Test]
        public async Task RemovePatient_Unauthorized()
        {
            HttpResponseMessage response = await this.server.CreateRequest("seed/v1/patients/1").SendAsync("DELETE");

            response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [SetUp]
        public void SetUp()
        {
            this.ClearPatientData();
            this.SeedPatient(KnownPatients);
        }

        [AllureStep("Response to bad Request as expected")]
        public void StatusCode(HttpResponseMessage response)
        {
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [AllureTms("UpdatePatient_BadRequest")]
        [Description("Update a patient with invalid data and the API response with a bad request.")]
        [TestCase("joe", "doe", "email@valid.com")]
        [TestCase("joe", "", "email@valid.com")]
        [TestCase("joe", "doe", "")]
        [TestCase("joe", "doe", null)]
        [TestCase("joe", null, null)]
        [TestCase("", null, null)]
        [TestCase(null, null, null)]
        public async Task UpdatePatient_BadRequest(string name, string lastname, string email)
        {
            Guid id = Guid.NewGuid();
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            PatientViewModel patientToUpdate = new PatientViewModel { Email = email, Id = id, Surname = lastname, Name = name };

            // Act
            HttpResponseMessage response = await this.CallUpdatePatient(patientToUpdate).ConfigureAwait(false);
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [AllureTms("UpdatePatient_CreationOk")]
        [Description("Update a patient and create it correctly")]
        [TestCase("joe", "doe", "email@valid.com", "5657")]
        public async Task UpdatePatient_CreationOk(string name, string lastname, string email, string medicalNumber)
        {
            Guid id = new Guid("6667cc4b-0efb-45c1-a57b-0d3089103e7b");
            // Arrange
            await this.Authorize().ConfigureAwait(false);
            PatientViewModel patientToUpdate = new PatientViewModel
                                                   {
                                                       Email = email,
                                                       Id = id,
                                                       Surname = lastname,
                                                       Name = name,
                                                       MedicalNumber = medicalNumber
                                                   };

            // Act
            HttpResponseMessage response = await this.CallUpdatePatient(patientToUpdate).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            PatientViewModel patient = JsonConvert.DeserializeObject<PatientViewModel>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            // Assert
            Assert.NotNull(patient);
            patient.Should().NotBeEquivalentTo(KnownPatients.First(p => p.Id == id));
            patient.Should().BeEquivalentTo(patientToUpdate);
        }

        [AllureTms("UpdatePatient_Unauthorized")]
        [Description("Update a patient without authorization.")]
        [Test]
        public async Task UpdatePatient_Unauthorized()
        {
            HttpResponseMessage response = await this.server.CreateRequest("seed/v1/patients/1").SendAsync("PUT").ConfigureAwait(false);

            response.StatusCode.Should().BeEquivalentTo(HttpStatusCode.Unauthorized);
        }

        [AllureTms("ValidLogin_ReturnToken")]
        [Description("Login with a valid user.")]
        [Test]
        public async Task ValidLogin_ReturnToken()
        {
            HttpResponseMessage response = await this.RequestToken().ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            string token = response.Headers.GetValues("Authorization").FirstOrDefault();
            token.Should().NotBeNullOrEmpty();
        }

        private async Task Authorize()
        {
            HttpResponseMessage response = await this.RequestToken().ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            this.currentToken = response.Headers.GetValues("Authorization").First();
        }

        private RequestBuilder BuildAuthorizedRequest(string uri)
        {
            return this.server.CreateRequest(uri).WithAuthorization(this.currentToken).WithAppJsonContentType();
        }

        private async Task<HttpResponseMessage> CallCreatePatient(PatientViewModel patient)
        {
            return await this.BuildAuthorizedRequest("seed/v1/patients/patient").WithJsonContent(patient).PostAsync().ConfigureAwait(false);
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

        private async Task<HttpResponseMessage> CallUpdatePatient(PatientViewModel patient)
        {
            return await this.BuildAuthorizedRequest($"seed/v1/patients/{patient.Id}").WithJsonContent(patient).SendAsync("PUT").ConfigureAwait(false);
        }

        [AllureTms("UpdatePatient_Unauthorized")]
        [Description("Update a patient without authorization.")]
        private void ClearPatientData()
        {
            using (IServiceScope scope = this.server.Host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (SeedDotnetContext context = scope.ServiceProvider.GetRequiredService<SeedDotnetContext>())
                {
                    List<Patient> patients = context.Patients.ToList();

                    foreach (Patient patient in patients)
                    {
                        context.Patients.Remove(patient);
                    }

                    context.SaveChanges();
                }
            }
        }

        private async Task<HttpResponseMessage> RequestToken()
        {
            List<KeyValuePair<string, string>> nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("login", "Systelab"));
            nvc.Add(new KeyValuePair<string, string>("password", "Systelab"));
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "seed/v1/users/login") { Content = new FormUrlEncodedContent(nvc) };
            return await this.client.SendAsync(req).ConfigureAwait(false);
        }

        private void SeedPatient(PatientViewModel[] patientList)
        {
            using (IServiceScope scope = this.server.Host.Services.GetService<IServiceScopeFactory>().CreateScope())
            {
                IMapper mapper = this.server.Host.Services.GetService<IMapper>();
                using (SeedDotnetContext context = scope.ServiceProvider.GetRequiredService<SeedDotnetContext>())
                {
                    foreach (PatientViewModel patient in patientList)
                    {
                        context.Patients.Add(mapper.Map<Patient>(patient));
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}