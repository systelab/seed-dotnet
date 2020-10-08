namespace UnitTest.Extensions
{
    using Main.Extensions;

    using NUnit.Framework;

    internal class SeedMapperConfigurationTest
    {
        [Test]
        public void AutomapperTest()
        {
            new SeedMapperConfiguration().AssertConfigurationIsValid();
        }
    }
}
