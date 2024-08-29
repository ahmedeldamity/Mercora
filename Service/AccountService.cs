using AutoMapper;
using Core.Dtos;
using Core.Entities.IdentityEntities;
using Core.ErrorHandling;
using Core.Interfaces.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Service;
public class AccountService(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager, IAuthService _authService, IMapper _mapper) : IAccountService
{
    public async Task<Result<AppUserResponse>> Register(RegisterRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        // Check if the email is already registered and confirmed 
        if (user is not null && user.EmailConfirmed is true)
        {
            return Result.Failure<AppUserResponse>(400, "The email address you entered is already taken, Please try a different one.");
        }

        var newUser = new AppUser()
        {
            DisplayName = model.DisplayName,
            Email = model.Email,
            UserName = model.Email.Split('@')[0],
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(newUser, model.Password);

        if (result.Succeeded is false)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<AppUserResponse>(400, errors);
        }

        var token = await _authService.CreateTokenAsync(newUser, _userManager);

        var userResponse = new AppUserResponse
        {
            DisplayName = newUser.DisplayName,
            Email = newUser.Email,
            Token = token
        };

        return Result.Success(userResponse);
    }

    public async Task<Result<AppUserResponse>> Login(LoginRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null || model.Password is null)
            return Result.Failure<AppUserResponse>(400, "The email or password you entered is incorrect, Check your credentials and try again.");

        var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

        if (result.Succeeded is false)
        {
            var errors = string.Join(", ", "The email or password you entered is incorrect, Check your credentials and try again.");
            return Result.Failure<AppUserResponse>(400, errors);
        }

        var token = await _authService.CreateTokenAsync(user, _userManager);

        var userResponse = new AppUserResponse
        {
            DisplayName = user.DisplayName,
            Email = user.Email!,
            Token = token
        };

        return Result.Success(userResponse);
    }

    public async Task<Result<AppUserResponse>> GetCurrentUser(ClaimsPrincipal User)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.FindByEmailAsync(email!);

        var userResponse = new AppUserResponse
        {
            DisplayName = user!.DisplayName,
            Email = user.Email!,
            Token = await _authService.CreateTokenAsync(user, _userManager)
        };

        return Result.Success(userResponse);
    }

    public async Task<Result<UserAddressResponse>> GetCurrentUserAddress(ClaimsPrincipal User)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(u => u.Email == email);

        if (user!.Address is null)
            return Result.Failure<UserAddressResponse>(404, "The address is not available in our system.");

        var address = _mapper.Map<UserAddress, UserAddressResponse>(user.Address);

        return Result.Success(address);
    }

    public async Task<Result<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress, ClaimsPrincipal User)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);

        var address = _mapper.Map<UserAddressResponse, UserAddress>(updatedAddress);

        var userEmail = User.FindFirstValue(ClaimTypes.Email);

        var user = await _userManager.Users.Include(x => x.Address).SingleOrDefaultAsync(u => u.Email == userEmail);

        user!.Address = address;

        address.AppUserId = user.Id;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result.Failure<UserAddressResponse>(400, errors);
        }

        return Result.Success(updatedAddress);
    }

    public async Task<Result<AppUserResponse>> GoogleLogin(string credential)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings()
        {
            Audience = ["YOUR_CLIENT"]
        };

        var payload = await GoogleJsonWebSignature.ValidateAsync(credential, settings);

        var user = await _userManager.FindByEmailAsync(payload.Email);

        if (user is null)
        {
            user = new AppUser
            {
                UserName = payload.Email.Split('@')[0],
                Email = payload.Email,
                DisplayName = payload.Name,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded is false)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure<AppUserResponse>(400, errors);
            }
        }

        user.EmailConfirmed = true;

        await _userManager.UpdateAsync(user);

        var token = await _authService.CreateTokenAsync(user, _userManager);

        var userResponse = new AppUserResponse
        {
            DisplayName = user.DisplayName,
            Email = user.Email!,
            Token = token
        };

        return Result.Success(userResponse);
    }

}