namespace main.Services
{
    using System;
    using System.Threading.Tasks;
    using Contracts;
    using Entities;
    using Entities.Models;
    using Microsoft.AspNetCore.Identity;

    internal class AccountService : IAccountService
    {
        private readonly IJwtHandler jwtHandler;

        private readonly IPasswordHasher<UserManage> passwordHasher;

        private readonly ISeedDotnetRepository repository;

        private readonly SignInManager<UserManage> signInManager;

        private readonly UserManager<UserManage> userManager;

        public AccountService(
            SignInManager<UserManage> signInManager,
            UserManager<UserManage> userM,
            IJwtHandler jwtHandler,
            IPasswordHasher<UserManage> passwordHasher,
            ISeedDotnetRepository repository)
        {
            this.repository = repository;
            this.signInManager = signInManager;
            this.jwtHandler = jwtHandler;
            this.passwordHasher = passwordHasher;
            this.userManager = userM;
        }

        public async Task<JsonWebToken> RefreshAccessToken(string token)
        {
            UserManage refreshToken = await this.GetRefreshToken(token).ConfigureAwait(false);
            if (refreshToken == null)
            {
                return null;
            }

            JsonWebToken jwt = this.jwtHandler.Create(refreshToken);
            jwt.RefreshToken = refreshToken.RefreshToken;

            return jwt;
        }

        public async Task<JsonWebToken> SignIn(string username, string password)
        {
            UserManage user = await this.userManager.FindByNameAsync(username);
            if (user != null && !string.IsNullOrEmpty(password))
            {
                SignInResult signInResult = await this.signInManager.CheckPasswordSignInAsync(user, password, false);
                if (signInResult.Succeeded)
                {
                    JsonWebToken jwt = this.jwtHandler.Create(user);
                    string refreshToken = this.passwordHasher.HashPassword(user, Guid.NewGuid().ToString())
                        .Replace("+", string.Empty).Replace("=", string.Empty).Replace("/", string.Empty);
                    jwt.RefreshToken = refreshToken;
                    user.RefreshToken = refreshToken;
                    await this.repository.UpdateRefreshToken(user);
                    return jwt;
                }
            }

            return null;
        }

        private async Task<UserManage> GetRefreshToken(string token)
        {
            return await this.repository.GetUserManageWithRefreshToken(token);
        }
    }
}