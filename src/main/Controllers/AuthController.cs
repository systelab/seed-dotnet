namespace Main.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Main.Models;
    using Main.ViewModels;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Main.Services;
    using AutoMapper;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using main.Models;

    [EnableCors("MyPolicy")]
    [Route("seed/v1/users")]
    public class AuthController : Controller
    {
        private readonly IAccountService repository;
        private readonly UserManager<UserManage> userManager;
		private readonly IMapper mapper;

        public AuthController( UserManager<UserManage> _userM, IAccountService _repository, IMapper _mapper)
        {
            this.repository = _repository ?? throw new ArgumentNullException(nameof(_repository));
            this.userManager = _userM ?? throw new ArgumentNullException(nameof(_userM));
			this.mapper = _mapper ?? throw new ArgumentNullException(nameof(_mapper));
        }

        /// <summary>
        /// Do the login and get the Token of the session
        /// </summary>
        /// <param name="vm"></param>
        /// <returns></returns>

        [Route("login")]
        [HttpPost]
        [SwaggerConsumes("application/x-www-form-urlencoded")]
        public async  Task<IActionResult> GetToken([FromForm] LoginViewModel vm)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.repository.SignIn(vm.login, vm.password);
                if (result != null)
                {
                    Response.Headers.Add("Authorization", "Bearer " + result.AccessToken);
                    Response.Headers.Add("Refresh", result.RefreshToken);
                    Response.Headers.Add("Access-Control-Expose-Headers", "origin, content-type, accept, authorization, ETag, if-none-match");
                    return this.Ok("Done");
                }
                else
                {
                    return this.BadRequest("Username or password incorrect");
                }
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
            if(result != null)
            {
                Response.Headers.Add("Authorization", "Bearer " + result.AccessToken);
                Response.Headers.Add("Refresh", result.RefreshToken);
                Response.Headers.Add("Access-Control-Expose-Headers", "origin, content-type, accept, authorization, ETag, if-none-match");
                return this.Ok();
            }
            else
            {
               return this.BadRequest("Refresh token was not found.");
            }
            
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
                return this.Ok(this.mapper.Map<UserViewModel>(user));
            }
            else
            {
                return this.BadRequest("Not logged");
            }
        }
    }
}