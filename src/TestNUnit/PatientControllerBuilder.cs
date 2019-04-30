using AutoMapper;
using main.Contracts;
using main.Controllers.Api;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.Controller
{
    internal class PatientControllerBuilder
    {
        private ILogger<PatientController> logger;

        private IMapper mapper;

        private readonly IMedicalRecordNumberService medicalRecordNumber;
        private IUnitOfWork unitOfWork;

        public PatientControllerBuilder(IMapper mapper, IMedicalRecordNumberService medicalRecordNumber)
        {
            unitOfWork = new Mock<IUnitOfWork>().Object;
            logger = new Mock<ILogger<PatientController>>().Object;
            this.mapper = mapper;
            this.medicalRecordNumber = medicalRecordNumber;
        }

        public static implicit operator PatientController(PatientControllerBuilder instance)
        {
            return instance.Build();
        }

        public PatientController Build()
        {
            return new PatientController(unitOfWork, logger, mapper, medicalRecordNumber);
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