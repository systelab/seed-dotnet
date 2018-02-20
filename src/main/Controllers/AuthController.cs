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
    using AutoMapper;
 

    [EnableCors("MyPolicy")]
    [Route("users")]
    public class AuthController : Controller
    {
        private readonly IAccountService repository;
        private readonly UserManager<UserManage> userManager;

        public AuthController( UserManager<UserManage> _userM, IAccountService _repository)
        {
            this.repository = _repository;
            this.userManager = _userM;
        }

   /// <summary>
   /// 
   /// </summary>
   /// <typeparam name="IActionResult"></typeparam>
   /// <param name=""></param>
   /// <param name="vm"></param>
   /// <returns></returns>
        [Route("login")]
        [HttpPost, Consumes("application/x-www-form-urlencoded")]
        public async  Task<IActionResult>  GetToken([FromForm] LoginViewModel vm)
        {
            if (this.ModelState.IsValid)
            {
                var result = await this.repository.SignIn(vm.login, vm.password);
                if (result != null)
                {
                    Response.Headers.Add("Authorization", result.AccessToken);
                    Response.Headers.Add("Refresh", result.RefreshToken);
                    return this.Ok(result);
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
                return this.Ok(result);
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
                return this.Ok(Mapper.Map<UserViewModel>(user));
            }
            else
            {
                return this.BadRequest("Not logged");
            }
        }
    }
}