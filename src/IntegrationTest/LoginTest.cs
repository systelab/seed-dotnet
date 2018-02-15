namespace IntegrationTest
{
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using Main;
    using Main.Models;
    using Main.ViewModels;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Newtonsoft.Json;

    using Xunit;

    public class LoginTest
    {
        private readonly TestServer server;

        private readonly HttpClient client;

        public LoginTest()
        {
            this.server = new TestServer(
                new WebHostBuilder()
                    .UseEnvironment("Testing")
                    .UseStartup<Main.Startup>());
                
            this.server.Host.Seed();
            this.client = this.server.CreateClient();
        }

        [Fact]
        public async Task ValidLogin_ReturnToken()
        {
            var requestData = new LoginViewModel() { UserName = "admin", Password = "P@ssw0rd!" };            
            // Act
            var response = await this.client.PostAsJsonAsync("/users/login", requestData);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
        }

        public async Task InvalidLogin_BadRequest()
        {
        }

        public async Task TwoLogins_SameTokenUpdatedExpirationDate()
        {
        }

        public async Task GetListOfPatients_EmptyList()
        {
        }

        public async Task GetListOfPatients_PopulatedList()
        {
        }

        // TODO: do the following two for each call

        public async Task GetListOfPatients_Unauthorized()
        {
        }

        public async Task GetListOfPatients_Timedout()
        {
        }

        public async Task CreatePatient_CreationOK(PatientViewModel patient)
        {
        }
   
        public async Task CreatePatient_BadRequest(PatientViewModel patient)
        {
        }

        public async Task UpdatePatient_CreationOK(PatientViewModel patient)
        {
        }

        public async Task UpdatePatient_BadRequest(PatientViewModel patient)
        {
        }

        public async Task RemovePatient_Found(int id)
        {
        }

        public async Task RemovePatient_NotFound(int id)
        {
        }

        public async Task GetPatient_Found(int id)
        {
        }

        public async Task GetPatient_NotFound(int id)
        {
        }
    }    
}
