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

    [EnableCors("MyPolicy")]
    [Route("users")]
    public class AuthController : Controller
    {
        private readonly IConfigurationRoot config;

        private readonly SignInManager<UserManage> signInManager;

        private readonly UserManager<UserManage> userManager;

        private SeedDotnetContext context;

        public AuthController(
            SignInManager<UserManage> _signInManager,
            UserManager<UserManage> _userM,
            SeedDotnetContext _context,
            IConfigurationRoot _config)
        {
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
        public async Task<IActionResult> GetToken(LoginViewModel vm)
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByNameAsync(vm.UserName);
                if (user != null)
                {
                    var signInResult = await this.signInManager.CheckPasswordSignInAsync(user, vm.Password, false);
                    if (signInResult.Succeeded)
                    {
                        // Create the token
                        var claims = new[]
                                         {
                                             new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                                             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                             new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
                                         };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config["Tokens:Key"]));
                        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            this.config["Tokens:Issuer"],
                            this.config["Tokens:Audience"],
                            claims,
                            expires: DateTime.Now.AddMinutes(40),
                            signingCredentials: creds);
                        var results = new
                                          {
                                              token = new JwtSecurityTokenHandler().WriteToken(token),
                                              expiration = DateTime.Now.AddMinutes(40)
                                          };
                        return this.Created(string.Empty, results);
                    }
                }
            }

            return this.BadRequest("Username or password incorrect");
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