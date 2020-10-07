namespace main.Controllers.Api
{
    using System;
    using System.Threading.Tasks;

    using AutoMapper;

    using main.Contracts;
    using main.Entities.Common;
    using main.Entities.Models;
    using main.Entities.ViewModels;
    using main.Services;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using X.PagedList;

    [ApiVersion("1")]
    [EnableCors("MyPolicy")]
    [Route("seed/v{version:apiVersion}/allergies")]
    [Authorize]
    public class AllergyController : Controller
    {
        private readonly ILogger<AllergyController> logger;

        private readonly IMapper mapper;

        private readonly IUnitOfWork unitOfWork;

        public AllergyController(IUnitOfWork unitOfWork, ILogger<AllergyController> logger, IMapper mapper)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        ///     Create a new allergy in the database
        /// </summary>
        /// <param name="allergy">allergy model</param>
        /// <returns>Task returning the status of the action</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAllergy([FromBody] AllergyViewModel allergy)
        {
            try
            {
                // Save to the database
                Allergy newAllergy = this.mapper.Map<Allergy>(allergy);
                if (allergy.Id.Equals(Guid.Empty))
                {
                    newAllergy.Id = Guid.NewGuid();
                }

                await this.unitOfWork.Allergies.Add(newAllergy);
                return this.Ok(this.mapper.Map<AllergyViewModel>(newAllergy));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to create the allergy: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        ///     Get list of all the allergies stored in the database
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
        public async Task<IActionResult> GetAllAllergies([FromQuery(Name = "page")] int page, [FromQuery(Name = "size")] int elementsPerPage)
        {
            try
            {
                // PagedList is a one-based index. We offer a zero-based index, therefore we have to add 1 to the page number
                IPagedList<Allergy> results = this.unitOfWork.Allergies.GetAllWithPaginationAllergy(page + 1, elementsPerPage);
                return this.Ok(this.mapper.Map<ExtendedPagedList<AllergyViewModel>>(results));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the allergies for page {page} (number of elements per page {elementsPerPage}): {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        ///     Get Information of a allergy
        /// </summary>
        /// <param name="uid">The id of the allergy that you want to retrieve information</param>
        /// <returns>result of the action</returns>
        [Route("{uid}")]
        [HttpGet]
        public async Task<IActionResult> GetAllergy(Guid uid)
        {
            try
            {
                Allergy results = await this.unitOfWork.Allergies.Get(uid);
                return this.Ok(this.mapper.Map<AllergyViewModel>(results));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the allergy: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        ///     Remove a specific allergy
        /// </summary>
        /// <param name="uid">The id of the allergy that you want to remove</param>
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

                Allergy allergyToDelete = await this.unitOfWork.Allergies.Get(uid);
                if (allergyToDelete == null)
                    //TODO do here something general
                {
                    throw new PatientNotFoundException();
                }

                await this.unitOfWork.Allergies.Remove(allergyToDelete);
                return this.Ok();
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the allergy: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        ///     Update the information of an existing allergy
        /// </summary>
        /// <param name="uid">
        ///     unique identifier of the allergy
        /// </param>
        /// <param name="allergy">
        ///     allergy model
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [HttpPut]
        [Route("{uid}")]
        public async Task<IActionResult> UpdateAllergy(Guid uid, [FromBody] AllergyViewModel allergy)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            // Save to the database
            Allergy results = await this.unitOfWork.Allergies.Get(uid);
            if (results == null || results.Id.Equals(Guid.Empty))
            {
                return this.BadRequest("User does not exist");
            }

            if (!string.IsNullOrWhiteSpace(allergy.Name))
            {
                results.Name = allergy.Name;
            }

            if (!string.IsNullOrWhiteSpace(allergy.Signs))
            {
                results.Signs = allergy.Signs;
            }

            if (!string.IsNullOrWhiteSpace(allergy.Symptoms))
            {
                results.Symptoms = allergy.Symptoms;
            }

            await this.unitOfWork.Allergies.Update(results);
            return this.Ok(this.mapper.Map<AllergyViewModel>(results));
        }
    }
}