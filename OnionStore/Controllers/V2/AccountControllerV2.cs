using API.Extensions;
using Asp.Versioning;
using Core.Dtos;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.V2;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("2.0")]
[ApiVersion("2.1")]
public class AccountControllerV2(IAccountService _accountService) : BaseController
{

    [HttpPost("register")]
    [MapToApiVersion("2.0")]
    public async Task<ActionResult<AppUserResponseV20>> RegisterV20(RegisterRequest model)
    {
        var result = await _accountService.RegisterV20(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost("register")]
    [MapToApiVersion("2.1")]
    public async Task<ActionResult<AppUserResponseV21>> RegisterV21(RegisterRequest model)
    {
        var result = await _accountService.RegisterV21(model);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

}