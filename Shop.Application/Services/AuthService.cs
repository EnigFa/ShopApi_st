using AutoMapper;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.DTOs.AuthDTOs;
using Shop.Application.Interfaces.Helpers;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Enums;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Services;
public class AuthService(
    IMapper _mapper, 
    IAuthRepository _repository, 
    IHashHelper _hashHelper, 
    IJWTService _jwtService, 
    IRefreshTokenRepository _refreshTokenRepository,
    IProviderRepository _providerRepository,
    IQueueService _queueService) : IAuthService
 
{
    public async Task<(UserReadDTO? User, string? Token, string? RefreshToken)> RegisterAsync(UserCreateDTO dto)
    {
       
        var isExist = await _repository.IsExistEmailAsync(dto.Email);
        if(!isExist)
        {
            var hash = _hashHelper.Hash(dto.Password);
            var user = _mapper.Map<User>(dto);
            var registerUser = await _repository.RegisterUserAsync(user, hash);
            if (registerUser != null)
            {
              
                var token = _jwtService.GenerateAccessToken(_mapper.Map<UserLoginDTO>(user), user.Role.ToString());
                var refreshToken = _jwtService.GenerateRefreshToken();
                await _refreshTokenRepository.AddAsync(new RefreshToken
                {
                    Token = refreshToken.Item1,
                    UserId = user.Id,
                    IsRevoked = false,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(refreshToken.Item2)
                });
                try
                {
                    await _queueService.PublishAsync<UserCreateDTO>("Users", dto);
                }
                catch
                {
                    // Користувач уже збережений. Недоступна черга не скасовує реєстрацію.
                }

                return (_mapper.Map<UserReadDTO>(registerUser), token, refreshToken.Item1);
            }
        }
        return (null,null,null);
    }

    public async Task<(string? Token, string? RefreshToken)> LoginAsync(UserLoginDTO dto)
    {
        var user = await _repository.GetByEmailAsync(dto.Email);
        if (user == null) return (null, null);

        if (!_hashHelper.IsValidPassword(dto.Password, user.PasswordHash))
            return (null, null);
        await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id);
        var token = _jwtService.GenerateAccessToken(dto, user.Role.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken.Item1,
            UserId = user.Id,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshToken.Item2)
        });

        return (token, refreshToken.Item1);
    }

    public async Task<(UserReadDTO? User, string? Token, string? RefreshToken)> ExternalLoginAsync(
        string email,
        string providerName,
        string numberProvider)
    {
        var provider = await _providerRepository.GetByNameAsync(providerName);
        if (provider == null)
            return (null, null, null);

        var user = await _repository.GetByEmailAsync(email);
        if (user == null)
        {
            var created = new User
            {
                Email = email,
                Role = UserRole.User,
                IsActive = true
            };

            user = await _repository.RegisterUserAsync(created, _hashHelper.Hash(Guid.NewGuid().ToString("N")));
            if (user == null)
                return (null, null, null);
        }

        if (!string.IsNullOrWhiteSpace(numberProvider))
        {
            var alreadyLinked = await _providerRepository.ExistsForUserAsync(user.Id, provider.Id);
            var numberTaken = await _providerRepository.NumberTakenAsync(provider.Id, numberProvider);

            if (!alreadyLinked && !numberTaken)
            {
                await _providerRepository.AddUserProviderAsync(new UserProvider
                {
                    UserId = user.Id,
                    ProviderId = provider.Id,
                    NumberProvider = numberProvider,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await _refreshTokenRepository.RevokeAllRefreshTokensAsync(user.Id);
        var token = _jwtService.GenerateAccessToken(_mapper.Map<UserLoginDTO>(user), user.Role.ToString());
        var refreshToken = _jwtService.GenerateRefreshToken();
        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshToken.Item1,
            UserId = user.Id,
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(refreshToken.Item2)
        });

        return (_mapper.Map<UserReadDTO>(user), token, refreshToken.Item1);
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(string refreshToken)
    {
        var token = await _refreshTokenRepository
            .GetByTokenAsync(refreshToken);

        if (token == null || token.IsRevoked == true)
        {
            return null;
        }

        if (token.ExpiresAt <= DateTime.UtcNow)
        {
            return null;
        }

        var user = await _repository.GetUserById(token.UserId);
         

        if (user == null)
        {
            return null;
        }

        
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        token.Token = newRefreshToken.Item1;
        token.ExpiresAt = DateTime.UtcNow.AddDays(newRefreshToken.Item2);
       
        await _refreshTokenRepository.UpdateAsync(token);
        var newAccessToken = _jwtService.GenerateAccessToken(_mapper.Map<UserLoginDTO>(user), user.Role.ToString());

        return new AuthResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = token.Token
        };
    }
}
