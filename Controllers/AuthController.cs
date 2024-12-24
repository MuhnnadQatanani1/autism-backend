using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Autsim.Models;
using Autsim.Models.AuthenticationModels;
using Autsim.Models.settingModel;
using System.Data;
using System.Threading.Tasks;
using Autsim.Services;

namespace Autsim.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly TokenService _tokenService;

		public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, TokenService tokenService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenService = tokenService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			model.Email = model.Email.Trim();

			var existingUser = await _userManager.FindByEmailAsync(model.Email);
			if (existingUser != null)
			{
				ModelState.AddModelError("Email", "Email is already in use.");
				return BadRequest(ModelState);
			}

			var emailParts = model.Email.Split('@');
			string userName = emailParts[0];

			var user = new ApplicationUser
			{
				Firstname = model.Firstname,
				Lastname = model.Lastname,
				Email = model.Email,
				UserName = model.Email + userName
			};

			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, model.Role.ToString());
				var response = new RegisterResponseModel
				{
					UserId = user.Id,
					Firstname = model.Firstname,
					Lastname = model.Lastname,
					Email = model.Email,
					Message = "User registered successfully.",
					Status = 200,
				};
				return Ok(response);
			}

			return BadRequest(result.Errors);
		}


		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
				return Unauthorized(new { Message = "Invalid email or password." });

			var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
			if (!result.Succeeded)
				return Unauthorized(new { Message = "Invalid email or password." });

			var roles = await _userManager.GetRolesAsync(user);
			var token = _tokenService.CreateToken(user, roles);
			var refreshToken = _tokenService.CreateRefreshToken();

			var response = new LoginResponseModel
			{
				UserId = user.Id,
                Firstname = user.Firstname,
				Lastname=user.Lastname,
                Token = token,
				RefreshToken = refreshToken,
				Email = user.Email,
				Message = "Login successful.",
				Status = 200,
				Roles = roles.ToList(),
			};

			return Ok(response);
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout()
		{
			// Sign out the user
			await _signInManager.SignOutAsync();

			return Ok(new { Message = "User logged out successfully.", Status = 200 });
		}


		[HttpGet("getUserById/{id}")]
		public async Task<IActionResult> GetUserById(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			if (user == null)
				return NotFound(new { Message = "User not found." });

			var roles = await _userManager.GetRolesAsync(user);
			var response = new getUserByIdResponseModel
			{
				UserId = user.Id,
				Firstname = user.Firstname,
				LastName = user.Lastname,
				Email = user.Email,
				Roles = roles.ToList(),
				ProfilePictureUrl = user.ProfilePictureUrl
			};

			return Ok(response);
		}



		[HttpPut("update-email")]
		public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequestModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null)
				return NotFound(new { Message = "User not found." });

			var existingUser = await _userManager.FindByEmailAsync(model.NewEmail);
			if (existingUser != null)
			{
				ModelState.AddModelError("Email", "Email is already in use.");
				return BadRequest(ModelState);
			}

			user.Email = model.NewEmail;
			user.UserName = model.NewEmail;

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
				return BadRequest(result.Errors);

			return Ok(new { Message = "Email updated successfully." });
		}

		[HttpPut("update-password")]
		public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequestModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null)
				return NotFound(new { Message = "User not found." });

			var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
			if (!result.Succeeded)
				return BadRequest(result.Errors);

			return Ok(new { Message = "Password updated successfully." });
		}
		[HttpPut("update-photo")]
		public async Task<IActionResult> UpdatePhoto([FromForm] UpdatePhotoRequestModel model)
		{
			var user = await _userManager.FindByIdAsync(model.UserId);
			if (user == null)
				return NotFound(new { Message = "User not found." });

			if (model.Photo != null && model.Photo.Length > 0)
			{
				var filePath = Path.Combine("wwwroot/images", $"{user.Id}_{model.Photo.FileName}");
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await model.Photo.CopyToAsync(stream);
				}

				user.ProfilePictureUrl = $"/images/{user.Id}_{model.Photo.FileName}";
				var result = await _userManager.UpdateAsync(user);
				if (!result.Succeeded)
					return BadRequest(result.Errors);

				return Ok(new { Message = "Profile photo updated successfully.", ImageUrl = user.ProfilePictureUrl });
			}

			return BadRequest(new { Message = "Invalid photo." });
		}



	}
}
