namespace main.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using Contracts;
    using Entities.Common;
    using Entities.Models;
    using Entities.Models.Relations;
    using Entities.ViewModels;
    using Entities.ViewModels.Relations;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using PagedList.Core;

    /// <summary>
    /// 
    /// </summary>
    [EnableCors("MyPolicy")]
    [Route("seed/v1/patients")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PatientController : Controller
    {
        private readonly ILogger<PatientController> logger;

        private readonly IMapper mapper;

        private readonly IMedicalRecordNumberService medicalRecordNumberService;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        /// <param name="medicalRecordNumberService"></param>
        public PatientController(IUnitOfWork unitOfWork, ILogger<PatientController> logger, IMapper mapper,
            IMedicalRecordNumberService medicalRecordNumberService)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.medicalRecordNumberService = medicalRecordNumberService ??
                                              throw new ArgumentNullException(nameof(medicalRecordNumberService));
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
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            try
            {
                // Save to the database
                Patient newPatient = this.mapper.Map<Patient>(patient);
                newPatient.MedicalNumber =
                    this.medicalRecordNumberService.GetMedicalRecordNumber("http://localhost:9090");
                if (patient.Id.Equals(Guid.Empty))
                {
                    newPatient.Id = Guid.NewGuid();
                }

                await this.unitOfWork.Patients.Add(newPatient);
                return this.Ok(this.mapper.Map<PatientViewModel>(newPatient));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to create the patient: {ex}");
                return this.BadRequest("Error Occurred");
            }
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
        public async Task<IActionResult> GetAllPatients([FromQuery(Name = "page")] int page,
            [FromQuery(Name = "size")] int elementsPerPage)
        {
            try
            {
                // PagedList is a one-based index. We offer a zero-based index, therefore we have to add 1 to the page number
                PagedList<Patient> results =
                    await this.unitOfWork.Patients.GetAllWithPaginationPatients(page + 1, elementsPerPage);
                return this.Ok(this.mapper.Map<ExtendedPagedList<PatientViewModel>>(results));
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    $"Failed to get the patients for page {page} (number of elements per page {elementsPerPage}): {ex}");
                return this.BadRequest("Error Occurred");
            }
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
            try
            {
                Patient results = await this.unitOfWork.Patients.Get(uid);
                return this.Ok(this.mapper.Map<PatientViewModel>(results));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the patient: {ex}");
                return this.BadRequest("Error Occurred");
            }
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
            try
            {
                if (uid.Equals(Guid.Empty))
                {
                    return this.BadRequest("Bad data");
                }

                Patient patientToDelete = await this.unitOfWork.Patients.Get(uid);
                if (patientToDelete == null)
                {
                    throw new PatientNotFoundException();
                }

                await this.unitOfWork.Patients.Remove(patientToDelete);
                return this.Ok();
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the patient: {ex}");
                return this.BadRequest("Error Occurred");
            }
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
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            // Save to the database
            Patient results = await this.unitOfWork.Patients.Get(uid);
            if (results == null || results.Id.Equals(Guid.Empty))
            {
                return this.BadRequest("User does not exist");
            }

            if (!string.IsNullOrWhiteSpace(patient.Name))
            {
                results.Name = patient.Name;
            }

            if (!string.IsNullOrWhiteSpace(patient.Email))
            {
                results.Email = patient.Email;
            }

            if (!string.IsNullOrWhiteSpace(patient.Surname))
            {
                results.Surname = patient.Surname;
            }

            if (!string.IsNullOrWhiteSpace(patient.MedicalNumber))
            {
                results.MedicalNumber = patient.MedicalNumber;
            }

            await this.unitOfWork.Patients.Update(results);
            return this.Ok(this.mapper.Map<PatientViewModel>(results));
        }

        #region Relation with Allergies

        /// <summary>
        ///     Assign Allergy to patient
        /// </summary>
        /// <param name="patientAllergy">PatientAllergy model</param>
        /// <param name="uid">uid of the patient</param>
        /// <returns>Task returning the status of the action</returns>
        [Route("{uid}/allergies")]
        [HttpPost]
        public async Task<IActionResult> CreatePatientAllergy([FromBody] PatientAllergyViewModel patientAllergy,
            Guid uid)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            try
            {
                // Save to the database
                patientAllergy.IdPatient = uid;
                patientAllergy.Id = Guid.NewGuid();
                return this.Ok(patientAllergy);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to create the patient: {ex}");
                return this.BadRequest("Error Occurred");
            }
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
            try
            {
                List<PatientAllergy> results = this.unitOfWork.Patients.GetAllergies(uid);
                return this.Ok(this.mapper.Map<List<PatientAllergyViewModel>>(results));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the allergies of the patient {uid}: {ex}");
                return this.BadRequest("Error Occurred");
            }
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
            try
            {
                bool results = this.unitOfWork.Patients.RemoveAllergy(uid, uidAllergy);
                return this.Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to remove the allergies of the patient {uid} {uidAllergy}: {ex}");
                return this.BadRequest("Error Occurred");
            }
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
        public async Task<IActionResult> UpdatePatientAllergy(Guid uid, Guid uidAllergy,
            [FromBody] PatientAllergyViewModel patientAllergy)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            // Save to the database
            PatientAllergy results = this.unitOfWork.Patients.GetPatientAllergy(uid, uidAllergy);
            if (results == null || results.Id.Equals(Guid.Empty))
            {
                return this.BadRequest("User does not exist");
            }

            if (!string.IsNullOrWhiteSpace(patientAllergy.Note))
            {
                results.Note = patientAllergy.Note;
            }

            if (patientAllergy.LastOcurrence != DateTime.MinValue)
            {
                results.LastOcurrence = patientAllergy.LastOcurrence;
            }

            if (patientAllergy.AssertedDate != DateTime.MinValue)
            {
                results.AssertedDate = patientAllergy.AssertedDate;
            }


            this.unitOfWork.Patients.UpdatePatientAllergy(results);
            return this.Ok(this.mapper.Map<PatientAllergyViewModel>(results));
        }

        #endregion
    }
}