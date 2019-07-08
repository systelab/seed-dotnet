namespace main.Services
{
    using System;
    using Contracts;
    using Polly;
    using RestSharp;

    public class MedicalRecordNumberService : IMedicalRecordNumberService
    {
        private readonly ISyncPolicy policy;

        public MedicalRecordNumberService(ISyncPolicy policy)
        {
            this.policy = policy;
        }

        public string GetMedicalRecordNumber(string url)
        {
            try
            {
                RestClient client = new RestClient(url);

                return this.policy.Execute(() =>
                {
                    IRestResponse response = client.Execute(
                        new RestRequest("/identity/v1/medical-record-number", Method.GET, DataFormat.Json));
                    if (!response.IsSuccessful)
                    {
                        throw response.ErrorException;
                    }

                    return response.Content;
                });
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