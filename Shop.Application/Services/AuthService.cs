using AutoMapper;
using Shop.Application.DTOs.UserDTOs;
using Shop.Application.Interfaces.Helpers;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Enums;
using Shop.Domain.Models;
using System.Security.Cryptography;
using System.Text;

namespace Shop.Application.Services;

public class AuthService(
    IMapper _mapper,
    IAuthRepository _repository,
    IHashHelper _hashHelper,
    IJWTService _jwtService,
    IEmailService _emailService,
    IUserQueuePublisher _userQueuePublisher) : IAuthService
{

    public async Task<(
        UserReadDTO? User,
        string? Token,
        string? RefreshToken)> RegisterAsync(
            UserCreateDTO dto)
    {
        var isExist = await _repository.IsExistEmailAsync(dto.Email);

        if (!isExist)
        {
            var hash = _hashHelper.Hash(dto.Password);
            var user = _mapper.Map<User>(dto);
            user.Role = UserRole.User;

            var registerUser = await _repository.RegisterUserAsync(
                user,
                hash);

            if (registerUser != null)
            {
                var token = _jwtService.GenerateAccessToken(
                    _mapper.Map<UserLoginDTO>(registerUser),
                    registerUser.Role.ToString());

                var refreshToken =
                    _jwtService.GenerateRefreshToken(
                        registerUser.Id);

                await _repository.AddRefreshTokenAsync(
                    refreshToken);

                await _userQueuePublisher.PublishRegisteredUserAsync(
                    registerUser.Id,
                    registerUser.Email,
                    registerUser.Role.ToString());

                return (
                    _mapper.Map<UserReadDTO>(registerUser),
                    token,
                    refreshToken.Token);
            }
        }

        return (null, null, null);
    }

    public async Task<(
        UserReadDTO? User,
        string? Token,
        string? RefreshToken)> LoginAsync(
            UserLoginDTO dto)
    {
        var user = await _repository.GetByEmailAsync(dto.Email);

        if (user == null || !user.IsActive)
        {
            return (null, null, null);
        }

        var isValidPassword = _hashHelper.IsValidPassword(
            dto.Password,
            user.PasswordHash);

        if (!isValidPassword)
        {
            return (null, null, null);
        }

        var token = _jwtService.GenerateAccessToken(
            dto,
            user.Role.ToString());

        var refreshToken =
            _jwtService.GenerateRefreshToken(user.Id);

        await _repository.AddRefreshTokenAsync(
            refreshToken);

        return (
            _mapper.Map<UserReadDTO>(user),
            token,
            refreshToken.Token);
    }

    public async Task<string?> RefreshAccessTokenAsync(
        string refreshToken)
    {
        var savedRefreshToken =
            await _repository.GetValidRefreshTokenAsync(
                refreshToken);

        if (savedRefreshToken == null)
        {
            return null;
        }

        var userLoginDto = _mapper.Map<UserLoginDTO>(
            savedRefreshToken.User);

        return _jwtService.GenerateAccessToken(
            userLoginDto,
            savedRefreshToken.User.Role.ToString());
    }

    public async Task EnsureFirstAdminAsync(string email, string password)
    {
        if (await _repository.HasAdminAsync())
        {
            return;
        }

        var existing = await _repository.GetByEmailAsync(email);

        if (existing != null)
        {
            existing.Role = UserRole.Admin;
            existing.IsActive = true;
            await _repository.UpdateUserAsync(existing);
            return;
        }

        var admin = new User
        {
            Email = email,
            Role = UserRole.Admin,
            IsActive = true
        };

        await _repository.RegisterUserAsync(
            admin,
            _hashHelper.Hash(password));
    }

    public async Task<UserReadDTO?> CreateStaffAsync(StaffCreateDTO dto)
    {
        if (dto.Role is not (UserRole.Admin or UserRole.Moderator))
        {
            return null;
        }

        if (await _repository.IsExistEmailAsync(dto.Email))
        {
            return null;
        }

        var user = _mapper.Map<User>(dto);
        var created = await _repository.RegisterUserAsync(
            user,
            _hashHelper.Hash(dto.Password));

        return created == null
            ? null
            : _mapper.Map<UserReadDTO>(created);
    }

    public async Task RequestPasswordResetAsync(string email)
    {
        var user = await _repository.GetByEmailAsync(email);

        if (user == null || !user.IsActive)
        {
            return;
        }

        await _repository.InvalidatePasswordResetTokensAsync(user.Id);

        var rawToken = CreateResetToken();
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = HashToken(rawToken),
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        await _repository.AddPasswordResetTokenAsync(resetToken);

        await _emailService.SendPasswordResetEmailAsync(
            user.Email,
            rawToken);
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDTO dto)
    {
        var savedToken = await _repository.GetValidPasswordResetTokenAsync(
            HashToken(dto.Token));

        if (savedToken == null)
        {
            return false;
        }

        savedToken.User.PasswordHash = _hashHelper.Hash(dto.NewPassword);
        savedToken.IsUsed = true;

        await _repository.InvalidatePasswordResetTokensAsync(savedToken.UserId);
        await _repository.UpdateUserAsync(savedToken.User);
        await _repository.ExpireRefreshTokensAsync(savedToken.UserId);

        return true;
    }

    private static string CreateResetToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }

    private static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
}
