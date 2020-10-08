namespace Main.Services
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    using Main.Contracts;
    using Main.Entities;
    using Main.Entities.Models;

    using Microsoft.IdentityModel.Tokens;

    using JsonWebToken = Entities.JsonWebToken;
    using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

    internal class JwtHandler : IJwtHandler
    {
        private readonly AppSettingsModel config;

        public JwtHandler(AppSettingsModel config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public JsonWebToken Create(UserManage user)
        {
            Claim[] claims = { new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName) };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(this.config.Secret);
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
                                                          {
                                                              Subject = new ClaimsIdentity(claims),
                                                              Expires = DateTime.UtcNow.AddMinutes(double.Parse(this.config.ExpiryMinutes)),
                                                              SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                                                          };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return new JsonWebToken { AccessToken = tokenHandler.WriteToken(token), Expiration = tokenDescriptor.Expires.Value };
        }
    }
}