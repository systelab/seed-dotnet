namespace Test.Controller
{
    using AutoMapper;

    using main.Contracts;
    using main.Controllers.Api;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Abstractions;

    internal class PatientControllerBuilder
    {
        private ILogger<PatientController> logger;

        private IMapper mapper;

        private IPatientService patientService;

        public PatientControllerBuilder(IMapper mapper, IPatientService patientService)
        {
            this.patientService = patientService;
            this.logger = new NullLogger<PatientController>();
            this.mapper = mapper;
        }

        public static implicit operator PatientController(PatientControllerBuilder instance)
        {
            return instance.Build();
        }

        public PatientController Build()
        {
            return new PatientController(this.patientService, this.logger, this.mapper);
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

        public PatientControllerBuilder WithPatientService(IPatientService patientService)
        {
            this.patientService = patientService;
            return this;
        }
    }
}