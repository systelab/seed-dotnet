namespace main.Controllers
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Contracts;
    using Entities;
    using Entities.Models;
    using Entities.ViewModels;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    [ApiVersion("1")]
    [EnableCors("MyPolicy")]
    [Route("seed/v{version:apiVersion}/users")]
    public class AuthController : Controller
    {
        private readonly IMapper mapper;

        private readonly ILogger<AuthController> logger;

        private readonly IAccountService repository;

        private readonly UserManager<UserManage> userManager;

        /// <inheritdoc />
        public AuthController(UserManager<UserManage> userManager, IAccountService repository, IMapper mapper, ILogger<AuthController> logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Do the login and get the Token of the session
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [Route("login")]
        [HttpPost]
        [SwaggerConsumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> GetToken(string login, string password)
        {
            JsonWebToken result = await this.repository.SignIn(login, password);
            if (result != null)
            {
                this.Response.Headers.Add("Authorization", "Bearer " + result.AccessToken);
                this.Response.Headers.Add("Refresh", result.RefreshToken);
                return this.Ok("Done");
            }

            this.logger.LogWarning($"Bad request, username {login} or password incorrect");

            return this.BadRequest("Username or password incorrect");
        }

        /// <summary>
        ///     With a valid authentication token, returns information of the owner of the token.
        /// </summary>
        /// <param name="uid">
        ///     User unique identifier
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [Route("{uid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> GetUserInformation(int uid)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                UserManage user = await this.userManager.FindByNameAsync(this.User.Identity.Name);
                return this.Ok(this.mapper.Map<UserViewModel>(user));
            }

            return this.BadRequest("Not logged");
        }

        /// <summary>
        ///     Providing a refresh token, the system will return a not expired access token.
        /// </summary>
        /// <param name="refreshToken">
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [HttpPost("login/refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshAccessToken(string refreshToken)
        {
            JsonWebToken result = await this.repository.RefreshAccessToken(refreshToken);
            if (result != null)
            {
                this.Response.Headers.Add("Authorization", "Bearer " + result.AccessToken);
                this.Response.Headers.Add("Refresh", result.RefreshToken);
                this.Response.Headers.Add(
                    "Access-Control-Expose-Headers",
                    "origin, content-type, accept, authorization, ETag, if-none-match");
                return this.Ok();
            }

            this.logger.LogDebug($"Refreshed token {refreshToken}");

            return this.BadRequest("Refresh token was not found.");
        }
    }
}