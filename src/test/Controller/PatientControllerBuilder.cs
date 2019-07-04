namespace Test.Controller
{
    using AutoMapper;
    using main.Contracts;
    using main.Controllers.Api;
    using Microsoft.Extensions.Logging;
    using Moq;

    internal class PatientControllerBuilder
    {
        private readonly IMedicalRecordNumberService medicalRecordNumber;
        private ILogger<PatientController> logger;

        private IMapper mapper;
        private IUnitOfWork unitOfWork;

        public PatientControllerBuilder(IMapper mapper, IMedicalRecordNumberService medicalRecordNumber)
        {
            this.unitOfWork = new Mock<IUnitOfWork>().Object;
            this.logger = new Mock<ILogger<PatientController>>().Object;
            this.mapper = mapper;
            this.medicalRecordNumber = medicalRecordNumber;
        }

        public static implicit operator PatientController(PatientControllerBuilder instance)
        {
            return instance.Build();
        }

        public PatientController Build()
        {
            return new PatientController(this.unitOfWork, this.logger, this.mapper, this.medicalRecordNumber);
        }

        public PatientControllerBuilder WithRepository(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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