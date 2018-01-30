using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using seed_dotnet.Models;
using System.Web.Http.Cors;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using seed_dotnet.ViewModels;

using Microsoft.Extensions.Logging;
using AutoMapper;
using seed_dotnet.Services;
using Microsoft.EntityFrameworkCore;

namespace seed_dotnet.Controllers.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
    [Route("patients")]
    public class PatientController : Controller
    {

        private ISeed_dotnetRepository _repository;

        public PatientController(ISeed_dotnetRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get list of all the patients stored in the database
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Microsoft.AspNetCore.Mvc.HttpGet()]
        public IActionResult getAllPatients()
        {
            try
            {
                    var results = _repository.GetAllPatients();
                    return Ok(Mapper.Map<IEnumerable<PatientViewModel>>(results));
            }
            catch (Exception ex)
            {
               // _logger.LogError($"Failed to get the Project: {ex}");
                return BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Get Information of a patient
        /// </summary>
        /// <param name="uid">The id of the patient that you want to retrieve information</param>
        /// <returns></returns>
        [Route("{uid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Microsoft.AspNetCore.Mvc.HttpGet()]
        public IActionResult getPatient(int uid)
        {
            try
            {
                Patient nPatient = new Patient() { id = uid };
                Patient results = _repository.GetPatient(nPatient);
                return Ok(Mapper.Map<PatientViewModel>(results));
            }
            catch (Exception ex)
            {
               // _logger.LogError($"Failed to get the Project: {ex}");
                return BadRequest("Error Occurred");
            }
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
        [Microsoft.AspNetCore.Mvc.HttpPost]
        public async Task<IActionResult> createPatient(PatientViewModel patient)
        {


            if (ModelState.IsValid)
            {
                //Save to the database
                var newpatient = Mapper.Map<Patient>(patient);
                _repository.AddPatient(newpatient);
                if (await _repository.SaveChangesAsync())
                {
                    return Created($"patient/{patient.Name}", Mapper.Map<PatientViewModel>(newpatient));
                }
                else
                {
                    return BadRequest("Error saving data to the database");
                }


            }
            return BadRequest("Bad data");
        }

        /// <summary>
        /// Remove a specific patient
        /// </summary>
        /// <param name="uid">The id of the patient that you want to remove</param>
        /// <returns></returns>
        [Route("{uid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Microsoft.AspNetCore.Mvc.HttpDelete()]
        public IActionResult remove(int uid)
        {
            try
            {
                if (uid == 0)
                {
                    return BadRequest("Bad data");
                }
                else
                {
                    var nPatient = new Patient() { id = uid };
                    _repository.DeletePatient(nPatient);
                    var results = _repository.GetAllPatients();
                    return Ok(Mapper.Map<IEnumerable<PatientViewModel>>(results));
                }
            }
            catch (Exception ex)
            {
               // _logger.LogError($"Failed to get the Project: {ex}");
                return BadRequest("Error Occurred");
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
        [Microsoft.AspNetCore.Mvc.HttpPut()]
        public IActionResult updatePatient(PatientViewModel patient)
        {
            if (ModelState.IsValid)
            {
                //Save to the database

                var nPatient = new Patient() { id = patient.id };
                var results = _repository.GetPatient(nPatient);
                if (results.id == 0 || results == null)
                {
                    return BadRequest("User does not exist");
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
                    _repository.UpdatePatient(results);
                    return Ok(Mapper.Map<PatientViewModel>(results));
                }



            }
            return BadRequest("Bad data");
        }


    }
}