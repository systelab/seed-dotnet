namespace UnitTest.Extensions
{
    using AutoMapper;

    using Main.Extensions;

    using NUnit.Framework;

    internal class SeedMapperConfigurationTest
    {
        [Test]
        public void AutomapperTest()
        {
            new MapperConfiguration(cfg => { cfg.AddProfile<AppMapperProfile>(); }).AssertConfigurationIsValid();
        }
    }
}