using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTest
{
    using Microsoft.AspNetCore.TestHost;

    internal static class RequestBuilderExtensions
    {
        public static RequestBuilder WithAuthorization(this RequestBuilder requestBuilder, string token)
        {
            requestBuilder.AddHeader("Authorization", $"Bearer  {token}");
            return requestBuilder;
        }
    }
}
