using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTest
{
    using System.IO;

    using Allure.Commons;

    using NUnit.Framework;

    [SetUpFixture]
    public class AllureConfigSetUp
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Environment.SetEnvironmentVariable(
                "ALLURE_CONFIG",
                Path.Combine(Environment.CurrentDirectory, AllureConstants.CONFIG_FILENAME));
        }
    }
}
