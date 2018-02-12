namespace Main.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AutoMapper;

    using Main.Models;
    using Main.Services;
    using Main.ViewModels;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [EnableCors("MyPolicy")]
    [Route("patients")]
    public class PatientController : Controller
    {
        private readonly ISeedDotnetRepository repository;

        private readonly ILogger<PatientController> logger;

        public PatientController(ISeedDotnetRepository repository, ILogger<PatientController> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        /// <summary>
        /// Create a new patient in the database
        /// </summary>
        /// <param name="patient">patient model</param>
        /// <returns>Task returning the status of the action</returns>
        [Route("patient")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> CreatePatient(PatientViewModel patient)
        {
            if (this.ModelState.IsValid)
            {
                // Save to the database
                var newpatient = Mapper.Map<Patient>(patient);
                this.repository.AddPatient(newpatient);
                return this.Ok(Mapper.Map<PatientViewModel>(newpatient));
            }

            return this.BadRequest("Bad data");
        }

        /// <summary>
        /// Get list of all the patients stored in the database
        /// </summary>
        /// <returns>result of the action</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public IActionResult GetAllPatients()
        {
            try
            {
                var results = this.repository.GetAllPatients();
                return this.Ok(Mapper.Map<IEnumerable<PatientViewModel>>(results));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the Project: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Get Information of a patient
        /// </summary>
        /// <param name="uid">The id of the patient that you want to retrieve information</param>
        /// <returns>result of the action</returns>
        [Route("{uid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public IActionResult GetPatient(int uid)
        {
            try
            {
                Patient nPatient = new Patient { Id = uid };
                Patient results = this.repository.GetPatient(nPatient);
                return this.Ok(Mapper.Map<PatientViewModel>(results));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the Project: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Remove a specific patient
        /// </summary>
        /// <param name="uid">The id of the patient that you want to remove</param>
        /// <returns>Task with the result of the action</returns>
        [Route("{uid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        public async Task<IActionResult> Remove(int uid)
        {
            try
            {
                if (uid == 0)
                {
                    return this.BadRequest("Bad data");
                }
                else
                {
                    var nPatient = new Patient { Id = uid };

                    var results = this.repository.DeletePatient(nPatient);
                    
                    return this.Ok(Mapper.Map<IEnumerable<PatientViewModel>>(results));
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the Project: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Update the information of an existing patient
        /// </summary>
        /// <param name="patient">patient model</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]
        public async Task<IActionResult> UpdatePatient(PatientViewModel patient)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            // Save to the database
            var nPatient = new Patient { Id = patient.Id };
            var results = this.repository.GetPatient(nPatient);
            if (results == null || results.Id == 0)
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

                this.repository.UpdatePatient(results);
                return this.Ok(Mapper.Map<PatientViewModel>(results));
            }

        }
    }
}