namespace Main.Services
{
    using System;

    using Main.Contracts;

    using Microsoft.Extensions.Logging;

    using Polly;

    using RestSharp;

    public class MedicalRecordNumberService : IMedicalRecordNumberService
    {
        private readonly ILogger<MedicalRecordNumberService> logger;

        private readonly ISyncPolicy policy;

        public MedicalRecordNumberService(ISyncPolicy policy, ILogger<MedicalRecordNumberService> logger)
        {
            this.policy = policy ?? throw new ArgumentNullException(nameof(policy));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string GetMedicalRecordNumber(string url)
        {
            try
            {
                RestClient client = new RestClient(url);

                return this.policy.Execute(
                    () =>
                        {
                            IRestResponse response = client.Execute(new RestRequest("/identity/v1/medical-record-number", Method.GET, DataFormat.Json));
                            if (!response.IsSuccessful)
                            {
                                throw response.ErrorException;
                            }

                            return response.Content;
                        });
            }
            catch (Exception)
            {
                this.logger.LogWarning("Unable to retrieve medical record number from external service, returning default");
                return GetDefaultMedicalRecordNumber();
            }
        }

        private static string GetDefaultMedicalRecordNumber()
        {
            return "UNDEFINED";
        }
    }
}