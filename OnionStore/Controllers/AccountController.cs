using Shared.Dtos;
using API.Errors;
using Core.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class AccountController : BaseController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly SignInManager<AppUser> _signInManager;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[HttpPost("register")]
		[ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		public async Task<ActionResult> Register(RegisterRequestDto model)
		{
			if (model is null || !IsValidEmail(model.Email))
			{
				return BadRequest(new ApiResponse(400, "Invalid registration data."));
			}

			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user is not null)
			{
				return BadRequest(new ApiResponse(400, "This email has already been used."));
			}

			var newUser = new AppUser()
			{
				DisplayName = model.DisplayName,
				Email = model.Email,
				UserName = model.Email.Split('@')[0],
				PhoneNumber = model.PhoneNumber
			};

			var result = await _userManager.CreateAsync(newUser, model.Password);

			if (result.Succeeded is false)
			{
				var error = result.Errors.Select(e => e.Description).FirstOrDefault();
				return BadRequest(new ApiResponse(400, error));
			}

			return Ok(new AppUserDto
			{
				DisplayName = newUser.DisplayName,
				Email = newUser.Email,
				Token = ""
			});
		}

		[HttpPost("login")]
		[ProducesResponseType(typeof(AppUserDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<AppUserDto>> Login(LoginRequestDto model)
		{
			if (model is null || !IsValidEmail(model.Email))
				return BadRequest(new ApiResponse(400, "Invalid login data."));

			var user = await _userManager.FindByEmailAsync(model.Email);

			if (user is null || model.Password is null)
				return BadRequest(new ApiResponse(400, "Invalid email or password."));

			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

			if (result.Succeeded is false)
				return BadRequest(new ApiResponse(400, "Invalid email or password."));

			return Ok(new AppUserDto
			{
				DisplayName = user.DisplayName,
				Email = model.Email,
				Token = ""
			});
		}

		private bool IsValidEmail(string email)
		{
			if (string.IsNullOrEmpty(email))
				return false;

			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				return addr.Address == email;
			}
			catch
			{
				return false;
			}
		}

		private async Task<bool> CheckEmailExist(string email)
		{
			return await _userManager.FindByEmailAsync(email) is not null;
		}

	}
}
