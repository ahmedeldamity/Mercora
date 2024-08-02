using API.Dtos;
using API.Errors;
using Core.Entities.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class AccountController : BaseController
	{
		private readonly UserManager<AppUser> _userManager;

		public AccountController(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
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



	}
}
