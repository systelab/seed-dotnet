namespace main.Controllers
{
    using System;
    using System.Threading.Tasks;

    using AutoMapper;
    using main.Contracts;
    using main.Entities;
    using main.Entities.Models;
    using main.Entities.ViewModels;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    /// <inheritdoc />
    [EnableCors("MyPolicy")]
    [Route("seed/v1/users")]
    public class AuthController : Controller
    {
        private readonly IMapper mapper;

        private readonly IAccountService repository;

        private readonly UserManager<UserManage> userManager;

        /// <inheritdoc />
        public AuthController(UserManager<UserManage> userManager, IAccountService repository, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Do the login and get the Token of the session
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>        
        /// <returns></returns>
        [Route("login")]
        [HttpPost]
        [SwaggerConsumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> GetToken(string login, string password)
        {   
                var result = await this.repository.SignIn(login, password);
                if (result != null)
                {
                    this.Response.Headers.Add("Authorization", "Bearer " + result.AccessToken);
                    this.Response.Headers.Add("Refresh", result.RefreshToken);
                    this.Response.Headers.Add(
                        "Access-Control-Expose-Headers",
                        "origin, content-type, accept, authorization, ETag, if-none-match");
                    return this.Ok("Done");
                }
                else
                {
                    return this.BadRequest("Username or password incorrect");
                }            
        }

        /// <summary>
        /// With a valid authentication token, returns information of the owner of the token.
        /// </summary>
        /// <param name="uid">
        /// User unique identifier
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Route("{uid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> GetUserInformation(int uid)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var user = await this.userManager.FindByNameAsync(this.User.Identity.Name);
                return this.Ok(this.mapper.Map<UserViewModel>(user));
            }
            else
            {
                return this.BadRequest("Not logged");
            }
        }

        /// <summary>
        /// Providing a refresh token, the system will return a not expired access token.
        /// </summary>
        /// <param name="refreshToken">
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpPost("login/refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshAccessToken(string refreshToken)
        {
            var result = await this.repository.RefreshAccessToken(refreshToken);
            if (result != null)
            {
                this.Response.Headers.Add("Authorization", "Bearer " + result.AccessToken);
                this.Response.Headers.Add("Refresh", result.RefreshToken);
                this.Response.Headers.Add(
                    "Access-Control-Expose-Headers",
                    "origin, content-type, accept, authorization, ETag, if-none-match");
                return this.Ok();
            }
            else
            {
                return this.BadRequest("Refresh token was not found.");
            }
        }
    }
}