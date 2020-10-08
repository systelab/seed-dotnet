namespace Main.Controllers
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Main.Contracts;
    using Main.Entities;
    using Main.Entities.Models;
    using Main.Entities.ViewModels;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    [Authorize]
    [ApiController]
    [ApiVersion("1")]
    //[EnableCors("MyPolicy")]
    [Route("seed/v{version:apiVersion}/users")]
    public class AuthController : Controller
    {
        private readonly ILogger<AuthController> logger;

        private readonly IMapper mapper;

        private readonly IAccountService repository;

        private readonly UserManager<UserManage> userManager;

        /// <inheritdoc />
        public AuthController(UserManager<UserManage> userManager, IAccountService repository, IMapper mapper,
            ILogger<AuthController> logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     Do the login and get the Token of the session
        /// </summary>
        /// <param name="userLogin"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        [Produces("application/json")]
        [SwaggerConsumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> GetToken([FromForm] UserLogin userLogin)
        {
            JsonWebToken result = await this.repository.SignIn(userLogin.Login, userLogin.Password);
            if (result != null)
            {
                this.Response.Headers.Add("Authorization", "Bearer " + result.AccessToken);
                this.Response.Headers.Add("Refresh", result.RefreshToken);
                this.logger.LogDebug($"User logged: {userLogin.Login}");
                return this.Ok("Done");
            }

            this.logger.LogWarning($"Bad request, username {userLogin.Login} or password incorrect");

            return this.Unauthorized("Username or password incorrect");
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
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetUserInformation(int uid)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                UserManage user = await this.userManager.FindByNameAsync(this.User.Identity.Name);
                return this.Ok(this.mapper.Map<UserViewModel>(user));
            }

            return this.Unauthorized("Not logged");
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
                this.Response.Headers.Add("Access-Control-Expose-Headers",
                    "origin, content-type, accept, authorization, ETag, if-none-match");
                return this.Ok();
            }

            this.logger.LogDebug($"Refreshed token {refreshToken}");

            return this.BadRequest("Refresh token was not found.");
        }
    }
}