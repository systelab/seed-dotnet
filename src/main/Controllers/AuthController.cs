
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

namespace seed_dotnet.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
    [Route("users")]
    public class AuthController : Controller
    {

        private seed_dotnetContext _context;
        private readonly SignInManager<UserManage> _signInManager;
        private readonly UserManager<UserManage> _userManager;
        private readonly IConfigurationRoot _config;
        public AuthController(SignInManager<UserManage> signInManager, UserManager<UserManage> userM, seed_dotnetContext context, IConfigurationRoot config)
        {
            _signInManager = signInManager;
            _context = context;
            _userManager = userM;
            _config = config;
        }
        /// <summary>
        /// Providing a correct credentials (username and password), returns a valid session token for 40 minutes
        /// </summary>
        /// <param name="UserName">This is the username of the user</param>
        /// <param name="Password">This is the pasword of the user</param>
        /// <returns></returns>
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> getToken(LoginViewModel vm)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(vm.UserName);
                if (user != null)
                {
                   
                    var signInResult = await _signInManager.CheckPasswordSignInAsync(user, vm.Password, false);
                    if (signInResult.Succeeded)
                    {
                        //Create the token
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                        };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            _config["Tokens:Issuer"],
                             _config["Tokens:Audience"],
                             claims,
                             expires: DateTime.Now.AddMinutes(40),
                             signingCredentials: creds
                            );
                        var results = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = DateTime.Now.AddMinutes(40)
                        };
                        return Created("", results);
                    }
                }


            }
            return BadRequest("Username or password incorrect");
        }

        /// <summary>
        /// With a valid authentication token, returns information of the owner of the token.
        /// </summary>
        /// <returns></returns>
        [Route("{uid}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<ActionResult> GetUserInformation(int uid)
        {
            if (this.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(this.User.Identity.Name);
                var userw = new
                {
                    Email = user.Email,
                    Name = user.Name,
                    LastName = user.LastName
                };
                return Created("", userw);
            }
            else
            {
                return BadRequest("Not logged");
            }
        }

    }
}