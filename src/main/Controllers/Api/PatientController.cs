namespace seed_dotnet.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;

    using seed_dotnet.Models;
    using seed_dotnet.Services;
    using seed_dotnet.ViewModels;

    [EnableCors("MyPolicy")]
    [Route("patients")]
    public class PatientController : Controller
    {
        private ISeed_dotnetRepository _repository;

        public PatientController(ISeed_dotnetRepository repository)
        {
            this._repository = repository;
        }

        /// <summary>
        /// Create a new patient in the database
        /// </summary>
        /// <param name="name">Name of the new patient</param>
        /// <param name="lastname">Lastname of the new patient</param>
        /// <param name="email">Email of the new patient</param>
        /// <returns></returns>
        [Route("patient")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> createPatient(PatientViewModel patient)
        {
            if (this.ModelState.IsValid)
            {
                // Save to the database
                var newpatient = Mapper.Map<Patient>(patient);
                this._repository.AddPatient(newpatient);
                return this.Ok(Mapper.Map<PatientViewModel>(newpatient));
            }

            return this.BadRequest("Bad data");
        }

        /// <summary>
        /// Get list of all the patients stored in the database
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet()]
        public IActionResult getAllPatients()
        {
            try
            {
                var results = this._repository.GetAllPatients();
                return this.Ok(Mapper.Map<IEnumerable<PatientViewModel>>(results));
            }
            catch (Exception ex)
            {
                // _logger.LogError($"Failed to get the Project: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Get Information of a patient
        /// </summary>
        /// <param name="uid">The id of the patient that you want to retrieve information</param>
        /// <returns></returns>
        [Route("{uid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet()]
        public IActionResult getPatient(int uid)
        {
            try
            {
                Patient nPatient = new Patient() { id = uid };
                Patient results = this._repository.GetPatient(nPatient);
                return this.Ok(Mapper.Map<PatientViewModel>(results));
            }
            catch (Exception ex)
            {
                // _logger.LogError($"Failed to get the Project: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Remove a specific patient
        /// </summary>
        /// <param name="uid">The id of the patient that you want to remove</param>
        /// <returns></returns>
        [Route("{uid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete()]
        public async Task<IActionResult> remove(int uid)
        {
            try
            {
                if (uid == 0)
                {
                    return this.BadRequest("Bad data");
                }
                else
                {
                    var nPatient = new Patient() { id = uid };

                    var results = this._repository.DeletePatient(nPatient);
                    
                    return this.Ok(Mapper.Map<IEnumerable<PatientViewModel>>(results));
                }
            }
            catch (Exception ex)
            {
                // _logger.LogError($"Failed to get the Project: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Update the information of an existing patient
        /// </summary>
        /// <param name="id">Id of the patient</param>
        /// <param name="name">Name of the new patient</param>
        /// <param name="lastname">Lastname of the new patient</param>
        /// <param name="email">Email of the new patient</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut()]
        public async Task<IActionResult> updatePatient(PatientViewModel patient)
        {
            if (this.ModelState.IsValid)
            {
                // Save to the database
                var nPatient = new Patient() { id = patient.id };
                var results = this._repository.GetPatient(nPatient);
                if (results.id == 0 || results == null)
                {
                    return this.BadRequest("User does not exist");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(patient.Name))
                    {
                        results.Name = patient.Name;
                    }

                    if (!string.IsNullOrWhiteSpace(patient.Email))
                    {
                        results.Email = patient.Email;
                    }

                    if (!string.IsNullOrWhiteSpace(patient.LastName))
                    {
                        results.LastName = patient.LastName;
                    }

                    this._repository.UpdatePatient(results);
                    return this.Ok(Mapper.Map<PatientViewModel>(results));
                }
            }

            return this.BadRequest("Bad data");
        }
    }
}