namespace Main.Controllers
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;

    using Main.Models;
    using Main.ViewModels;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Main.Services;

    [EnableCors("MyPolicy")]
    [Route("users")]
    public class AuthController : Controller
    {
        private readonly IConfigurationRoot config;
        private readonly IAccountService repository;

        private readonly SignInManager<UserManage> signInManager;

        private readonly UserManager<UserManage> userManager;

        private SeedDotnetContext context;

        public AuthController(
            SignInManager<UserManage> _signInManager,
            UserManager<UserManage> _userM,
            SeedDotnetContext _context,
            IConfigurationRoot _config, IAccountService _repository)
        {
            this.repository = _repository;
            this.signInManager = _signInManager;
            this.context = _context;
            this.userManager = _userM;
            this.config = _config;
        }

        /// <summary>
        /// Providing a correct credentials (username and password), returns a valid session token for 40 minutes
        /// </summary>
        /// <param name="vm">Login model</param>
        /// <returns></returns>
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody] LoginViewModel vm)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.repository.SignIn(vm.login, vm.password);
                return Ok(result);
            }

            return this.BadRequest("Username or password incorrect");
        }

        /// <summary>
        /// Providing a refresh token, the system will return a not expired access token.
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpPost("login/refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshAccessToken(string refreshToken)
        {
            var result = await this.repository.RefreshAccessToken(refreshToken);
            return Ok(result);
        }

        /// <summary>
        /// With a valid authentication token, returns information of the owner of the token.
        /// </summary>
        /// <param name="uid">User unique identifier</param>
        /// <returns></returns>
        [Route("{uid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> GetUserInformation(int uid)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var user = await this.userManager.FindByNameAsync(this.User.Identity.Name);
                var userw = new { Email = user.Email, Name = user.Name, LastName = user.LastName };
                return this.Created(string.Empty, userw);
            }
            else
            {
                return this.BadRequest("Not logged");
            }
        }
    }
}