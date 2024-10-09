using Asp.Versioning;
using BlazorEcommerce.Application.Dtos;
using BlazorEcommerce.Application.Interfaces.Services;
using BlazorEcommerce.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace BlazorEcommerce.Server.Controllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
[ApiVersion("2.1")]
[EnableRateLimiting("FixedWindowPolicy")]
public class AccountController(IAccountService accountService) : ControllerBase
{
    [HttpPost("register")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<AppUserResponseV20>> RegisterV20(RegisterRequest model)
    {
        var result = await accountService.RegisterV20(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("register")]
    [MapToApiVersion("2.1")]
    public async Task<ActionResult<AppUserResponseV21>> RegisterV21(RegisterRequest model)
    {
        var result = await accountService.RegisterV21(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("login")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<AppUserResponseV20>> LoginV20(LoginRequest model)
    {
        var result = await accountService.LoginV20(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}