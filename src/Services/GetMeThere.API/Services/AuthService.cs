﻿using GetMeThere.API.Models;
using GetMeThere.API.Models.DTO;
using GetMeThere.API.Repositories;
using System.Security.Claims;

namespace GetMeThere.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        public AuthService(IConfiguration configuration, IAuthRepository authRepository, ITokenService tokenService)
        {
            _configuration = configuration;
            _authRepository = authRepository;
            _tokenService = tokenService;
        }

        public async Task<AuthResultDto> Login(LoginRequest request)
        {
            var user = await _authRepository.GetUser(login: request.Username, password: request.Password);
            if (user != null)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.Username)
                };
                var accessToken = _tokenService.GetAccessToken(claims);
                var refreshToken = _authRepository.GetUserRefreshToken(request.Username);
                

                // sugar. #changeme
                if(refreshToken is not null && refreshToken.ExpiryTime < DateTime.UtcNow)
                {
                    refreshToken = _tokenService.GetRefreshToken(user.Id, DateTime.UtcNow);
                    await _authRepository.UpdateUserRefreshToken(user.Id, refreshToken);
                }
                else
                {
                    refreshToken = _tokenService.GetRefreshToken(user.Id, DateTime.UtcNow);
                    await _authRepository.InsertUserRefreshToken(refreshToken);
                }

                // change to AuthResultDto with AccessToken & RefreshToken only?
                return new AuthResultDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.TokenString,
                    
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<AuthResultDto> Register(RegisterRequest request)
        {
            var isUserExists = await _authRepository.IsUserExists(login: request.Login, password: request.Password);
            if (isUserExists)
            {
                return null;
            }
            else
            {
                var user = await _authRepository.CreateUser(request);
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Name)
                };
                
                var tokens = _tokenService.GetTokens(user, claims, DateTime.UtcNow);
                await _authRepository.InsertUserRefreshToken(tokens.RefreshToken);

                return new AuthResultDto
                {
                    AccessToken = tokens.AccessToken,
                    RefreshToken = tokens.RefreshToken.TokenString
                };
            }
        }
    }
}
