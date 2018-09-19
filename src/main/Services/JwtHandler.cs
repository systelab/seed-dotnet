using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Main.Models;

namespace Main.Services
{
    internal class JwtHandler : IJwtHandler
    {
        private readonly SigningCredentials _signingCredentials;
        private readonly IConfigurationRoot _config;

        public JwtHandler(IConfigurationRoot config)
        {
            _config = config;
            SecurityKey _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:secretKey"]));
            _signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
        }

        public JsonWebToken Create(UserManage user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };
            var nowUtc = DateTime.Now;

            var creds = _signingCredentials;
            var expires = nowUtc.AddMinutes(int.Parse(_config["jwt:expiryMinutes"]));
            var exp = (long)(new TimeSpan(expires.Ticks).TotalSeconds);


            var token = new JwtSecurityToken(
                _config["jwt:issuer"],
                 _config["Tokens:Audience"],
                 claims,
                 expires: DateTime.Now.AddMinutes(int.Parse(_config["jwt:expiryMinutes"])),
                 signingCredentials: creds
                );
            var tokenx = new JwtSecurityTokenHandler().WriteToken(token);

            return new JsonWebToken
            {
                AccessToken = tokenx,
                Expires = exp,
                Expiration = expires
            };
        }
    }
}
