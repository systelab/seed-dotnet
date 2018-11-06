namespace main.Services
{
    using System;

    using Polly;
    using Polly.CircuitBreaker;

    using RestSharp;

    public class MedicalRecordNumberService : IMedicalRecordNumberService
    {
        private readonly RestClient client;

        private readonly ISyncPolicy policy;

        public MedicalRecordNumberService(string url, ISyncPolicy policy)
        {
            this.client = new RestClient(url);
            this.policy = policy;
        }

        public string GetMedicalRecordNumber()
        {
            try
            {
                return this.policy.Execute<string>(() => this.client.Execute(new RestRequest()).Content);
            }
            catch (Exception)
            {
                return GetDefaultMedicalRecordNumber();
            }            
        }

        private static string GetDefaultMedicalRecordNumber()
        {
            return "UNDEFINED";
        }
    }
}