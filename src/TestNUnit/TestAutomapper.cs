using System;
using System.Collections.Generic;
using System.Text;

namespace TestNUnit
{
    using main.Extensions;

    using NUnit.Framework;

    class TestAutomapper
    {
        [Test]
        public void AutomapperTest()
        {
            new SeedMapperConfiguration().AssertConfigurationIsValid();
        }
    }
}
