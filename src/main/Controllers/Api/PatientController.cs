namespace Main.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AutoMapper;

    using Main.Contracts;
    using Main.Entities.Common;
    using Main.Entities.Models;
    using Main.Entities.Models.Relations;
    using Main.Entities.ViewModels;
    using Main.Entities.ViewModels.Relations;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using X.PagedList;

    /// <summary>
    /// 
    /// </summary>
    [ApiVersion("1")]
    [ApiController]
    [EnableCors("MyPolicy")]
    [Route("seed/v{version:apiVersion}/patients")]
    [Authorize]
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> logger;

        private readonly IMapper mapper;

        private readonly IMedicalRecordNumberService medicalRecordNumberService;

        private readonly IUnitOfWork unitOfWork;

        private IPatientService patientService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public PatientController(IPatientService patientService, ILogger<PatientController> logger, IMapper mapper)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
        }

        /// <summary>
        ///     Create a new patient in the database
        /// </summary>
        /// <param name="patient">patient model</param>
        /// <returns>Task returning the status of the action</returns>
        [Route("patient")]
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientViewModel patient)
        {
            Patient newPatient = this.mapper.Map<Patient>(patient);
            PatientViewModel newPatientViewModel = this.mapper.Map<PatientViewModel>(await this.patientService.Create(newPatient));
            this.logger.LogDebug($"Patient created {newPatientViewModel.Id}");
            return this.Ok(newPatientViewModel);
        }

        /// <summary>
        ///     Assign Allergy to patient
        /// </summary>
        /// <param name="patientAllergy">PatientAllergy model</param>
        /// <param name="uid">uid of the patient</param>
        /// <returns>Task returning the status of the action</returns>
        [Route("{uid}/allergies")]
        [HttpPost]
        public async Task<IActionResult> CreatePatientAllergy([FromBody] PatientAllergyViewModel patientAllergy, Guid uid)
        {
            PatientAllergy allergy = await this.patientService.AddAllergy(this.mapper.Map<PatientAllergy>(patientAllergy), uid);
            return this.Ok(patientAllergy);
        }

        /// <summary>
        ///     Get list of all the allergies of the patient
        /// </summary>
        /// <param name="uid">
        ///     The patient uid
        /// </param>
        /// <returns>
        ///     result of the action
        /// </returns>
        [HttpGet]
        [Route("{uid}/allergies")]
        public async Task<IActionResult> GetAllPatientAllergies(Guid uid)
        {
            IList<PatientAllergy> results = await this.patientService.GetAllergies(uid);
            return this.Ok(this.mapper.Map<List<PatientAllergyViewModel>>(results));
        }

        /// <summary>
        ///     Get list of all the patients stored in the database
        /// </summary>
        /// <param name="page">
        ///     The page Number.
        /// </param>
        /// <param name="elementsPerPage">
        ///     The elements Per Page.
        /// </param>
        /// <returns>
        ///     result of the action
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAllPatients([FromQuery(Name = "page")] int page, [FromQuery(Name = "size")] int elementsPerPage)
        {
            // PagedList is a one-based index. We offer a zero-based index, therefore we have to add 1 to the page number
            IPagedList<Patient> results = await this.patientService.Get(page + 1, elementsPerPage);
            IEnumerable<PatientViewModel> resultViewModel = mapper.Map<IEnumerable<Patient>, IEnumerable<PatientViewModel>>(results.AsEnumerable());
            PagedList<PatientViewModel> patientViewModels = new PagedList<PatientViewModel>(results, resultViewModel);

            return this.Ok(new ExtendedPagedList<PatientViewModel>(patientViewModels));
        }

        /// <summary>
        ///     Get Information of a patient
        /// </summary>
        /// <param name="uid">The id of the patient that you want to retrieve information</param>
        /// <returns>result of the action</returns>
        [Route("{uid}")]
        [HttpGet]
        public async Task<IActionResult> GetPatient(Guid uid)
        {
            Patient results = await this.patientService.Get(uid);
            return this.Ok(this.mapper.Map<PatientViewModel>(results));
        }

        /// <summary>
        ///     Remove a specific patient
        /// </summary>
        /// <param name="uid">The id of the patient that you want to remove</param>
        /// <returns>Task with the result of the action</returns>
        [Route("{uid}")]
        [HttpDelete]
        public async Task<IActionResult> Remove(Guid uid)
        {
            Patient patient = await this.patientService.Get(uid);
            if (patient == null)
            {
                return this.NotFound(uid);
            }

            await this.patientService.Delete(uid);
            return this.Ok();
        }

        /// <summary>
        ///     Get list of all the allergies of the patient
        /// </summary>
        /// <param name="uid">
        ///     The patient uid
        /// </param>
        /// <param name="uidAllergy">
        ///     The allergy uid
        /// </param>
        /// <returns>
        ///     result of the action
        /// </returns>
        [HttpDelete]
        [Route("{uid}/allergies/{uidAllergy}")]
        public async Task<IActionResult> RemovePatientAllergy(Guid uid, Guid uidAllergy)
        {
            await this.patientService.RemoveAllergy(uid, uidAllergy);
            return this.Ok();
        }

        /// <summary>
        ///     Update the information of an existing patient
        /// </summary>
        /// <param name="uid">
        ///     unique identifier of the patient
        /// </param>
        /// <param name="patient">
        ///     patient model
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [HttpPut]
        [Route("{uid}")]
        public async Task<IActionResult> UpdatePatient(Guid uid, [FromBody] PatientViewModel patient)
        {
            Patient result = await this.patientService.Update(uid, this.mapper.Map<Patient>(patient));
            return this.Ok(this.mapper.Map<PatientViewModel>(result));
        }

        /// <summary>
        ///     Update the information of an existing patient
        /// </summary>
        /// <param name="uid">
        ///     unique identifier of the patient
        /// </param>
        /// <param name="patient">
        ///     patient model
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [HttpPut]
        [Route("{uid}/allergies/{uidAllergy}")]
        public async Task<IActionResult> UpdatePatientAllergy(Guid uid, Guid uidAllergy, [FromBody] PatientAllergyViewModel patientAllergy)
        {
            PatientAllergy result = await this.patientService.UpdateAllergy(uid, uidAllergy, this.mapper.Map<PatientAllergy>(patientAllergy));
            return this.Ok(this.mapper.Map<PatientAllergyViewModel>(result));
        }
    }
}