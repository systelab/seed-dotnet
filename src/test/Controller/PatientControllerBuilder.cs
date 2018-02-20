namespace Test.Controller
{

    using Main.Controllers.Api;
    using Main.Services;

    using Microsoft.Extensions.Logging;

    using Moq;

    public class PatientControllerBuilder
    {
        private ISeedDotnetRepository repository;

        private ILogger<PatientController> logger;

        public PatientControllerBuilder()
        {
            this.repository = new Mock<ISeedDotnetRepository>().Object;
            this.logger = new Mock<ILogger<PatientController>>().Object;
        }

        public static implicit operator PatientController(PatientControllerBuilder instance)
        {
            return instance.Build();
        }

        public PatientController Build()
        {
            return new PatientController(this.repository, this.logger);
        }

        public PatientControllerBuilder WithRepository(ISeedDotnetRepository repository)
        {
            this.repository = repository;
            return this;
        }

        public PatientControllerBuilder WithLogger(ILogger<PatientController> logger)
        {
            this.logger = logger;
            return this;
        }
    }
}