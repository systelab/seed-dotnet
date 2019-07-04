namespace main.Services
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using Contracts;
    using Entities;
    using Entities.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;

    internal class JwtHandler : IJwtHandler
    {
        private readonly IConfigurationRoot _config;
        private readonly SigningCredentials _signingCredentials;

        public JwtHandler(IConfigurationRoot config)
        {
            this._config = config;
            SecurityKey _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this._config["jwt:secretKey"]));
            this._signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256);
        }

        public JsonWebToken Create(UserManage user)
        {
            Claim[] claims =
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };
            DateTime nowUtc = DateTime.Now;

            SigningCredentials creds = this._signingCredentials;
            DateTime expires = nowUtc.AddMinutes(int.Parse(this._config["jwt:expiryMinutes"]));
            long exp = (long) new TimeSpan(expires.Ticks).TotalSeconds;


            JwtSecurityToken token = new JwtSecurityToken(this._config["jwt:issuer"], this._config["Tokens:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(int.Parse(this._config["jwt:expiryMinutes"])),
                signingCredentials: creds
            );
            string tokenx = new JwtSecurityTokenHandler().WriteToken(token);

            return new JsonWebToken
            {
                AccessToken = tokenx,
                Expires = exp,
                Expiration = expires
            };
        }
    }
}