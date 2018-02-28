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
    [Route("seed/v1/patients")]
    public class PatientController : Controller
    {
        private readonly ISeedDotnetRepository repository;

        private readonly ILogger<PatientController> logger;

        private readonly IMapper mapper;

        public PatientController(ISeedDotnetRepository repository, ILogger<PatientController> logger, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Create a new patient in the database
        /// </summary>
        /// <param name="patient">patient model</param>
        /// <returns>Task returning the status of the action</returns>
        [Route("patient")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
                var newpatient = this.mapper.Map<Patient>(patient);
                this.repository.AddPatient(newpatient);
                return this.Ok(this.mapper.Map<PatientViewModel>(newpatient));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to create the patient: {ex}");
                return this.BadRequest("Error Occurred");
            }


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
                return this.Ok(this.mapper.Map<IEnumerable<PatientViewModel>>(results));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the patient: {ex}");
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
                Patient lookupPatient = new Patient { Id = uid };
                Patient results = this.repository.GetPatient(lookupPatient);
                return this.Ok(this.mapper.Map<PatientViewModel>(results));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the patient: {ex}");
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
                if (uid == 0 )
                {
                    return this.BadRequest("Bad data");
                }
                else
                {
                    var lookupPatient = new Patient { Id = uid };
                    Patient patientToDelete = this.repository.GetPatient(lookupPatient);                    
                    if (patientToDelete == null)
                    {
                        return this.GetAllPatients();
                    }

                    var results = this.repository.DeletePatient(patientToDelete);
                    return this.Ok(this.mapper.Map<IEnumerable<PatientViewModel>>(results));

                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the patient: {ex}");
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
        [Route("{uid}")]
        public async Task<IActionResult> UpdatePatient(int uid,[FromBody] PatientViewModel patient)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            // Save to the database
            var lookupPatient = new Patient { Id = uid };
            var results = this.repository.GetPatient(lookupPatient);
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

                if (!string.IsNullOrWhiteSpace(patient.Surname))
                {
                    results.Surname = patient.Surname;
                }

                this.repository.UpdatePatient(results);
                return this.Ok(this.mapper.Map<PatientViewModel>(results));
            }

        }
    }
}