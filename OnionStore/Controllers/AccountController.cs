using API.Extensions;
using Core.Dtos;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
public class AccountController(IAccountService _accountService) : BaseController
{

	[HttpPost("register")]
	public async Task<ActionResult> Register(RegisterRequest model)
	{
        var result = await _accountService.Register(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemOrSuccessMessage();
	}

    [HttpPost("login")]
	public async Task<ActionResult<AppUserResponse>> Login(LoginRequest model)
	{
		var result = await _accountService.Login(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemOrSuccessMessage();
	}

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<AppUserResponse>> GetCurrentUser()
    {
        var result = await _accountService.GetCurrentUser(User);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemOrSuccessMessage();
    }

    [Authorize]
    [HttpGet("address")]
    public async Task<ActionResult<UserAddressResponse>> GetCurrentUserAddress()
    {
        var result = await _accountService.GetCurrentUserAddress(User);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemOrSuccessMessage();
    }

    [Authorize]
    [HttpPut("address")]
    public async Task<ActionResult<UserAddressResponse>> UpdateUserAddress(UserAddressResponse updatedAddress)
    {
        var result = await _accountService.UpdateUserAddress(updatedAddress, User);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemOrSuccessMessage();
    }

    [HttpPost("googlelogin")]
    public async Task<ActionResult<AppUserResponse>> GoogleLogin([FromBody] string credential)
    {
        var result = await _accountService.GoogleLogin(credential);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblemOrSuccessMessage();
    }

}