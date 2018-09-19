namespace Test.Controller
{
    using AutoMapper;

    using Main.Controllers.Api;
    using Main.Services;

    using Microsoft.Extensions.Logging;

    using Moq;

    internal class PatientControllerBuilder
    {
        private ISeedDotnetRepository repository;

        private ILogger<PatientController> logger;

        private IMapper mapper;

        public PatientControllerBuilder(IMapper mapper)
        {
            this.repository = new Mock<ISeedDotnetRepository>().Object;
            this.logger = new Mock<ILogger<PatientController>>().Object;
            this.mapper = mapper;
        }

        public static implicit operator PatientController(PatientControllerBuilder instance)
        {
            return instance.Build();
        }

        public PatientController Build()
        {
            return new PatientController(this.repository, this.logger, this.mapper);
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

        public PatientControllerBuilder WithMapper(IMapper mapper)
        {
            this.mapper = mapper;
            return this;
        }
    }
}