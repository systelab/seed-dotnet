using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTest
{
    using System.Net.Http;
    using System.Net.Http.Headers;

    using Microsoft.AspNetCore.TestHost;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    using Newtonsoft.Json;

    internal static class RequestBuilderExtensions
    {
        public static RequestBuilder WithAuthorization(this RequestBuilder requestBuilder, string token)
        {
            requestBuilder.AddHeader("Authorization", $"Bearer  {token}");
            return requestBuilder;
        }

        public static RequestBuilder WithAppJsonContentType(this RequestBuilder requestBuilder)
        {
            requestBuilder.AddHeader("Content-Type", "application/json");
            return requestBuilder;
        }

        public static RequestBuilder WithJsonContent<T>(this RequestBuilder requestBuilder, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString, Encoding.UTF8, "application/json");
            return requestBuilder.And(p => p.Content = content);
        }
    }
}
