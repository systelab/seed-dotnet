namespace IntegrationTest
{
    using System.Net.Http;
    using System.Text;

    using Microsoft.AspNetCore.TestHost;

    using Newtonsoft.Json;

    internal static class RequestBuilderExtensions
    {
        public static RequestBuilder WithAppJsonContentType(this RequestBuilder requestBuilder)
        {
            requestBuilder.AddHeader("Content-Type", "application/json");
            return requestBuilder;
        }

        public static RequestBuilder WithAuthorization(this RequestBuilder requestBuilder, string token)
        {
            requestBuilder.AddHeader("Authorization", $"{token}");
            return requestBuilder;
        }

        public static RequestBuilder WithJsonContent<T>(this RequestBuilder requestBuilder, T data)
        {
            string dataAsString = JsonConvert.SerializeObject(data);
            StringContent content = new StringContent(dataAsString, Encoding.UTF8, "application/json");
            return requestBuilder.And(p => p.Content = content);
        }
    }
}