using ForumNG.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ForumNG.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;

		public AccountController(IConfiguration configuration, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
		{
			_configuration = configuration;
			_userManager = userManager;
			_signInManager = signInManager;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			var user = await _userManager.FindByEmailAsync(model.Login);
			if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
			{
				var claims = new[]
				{
					new Claim(ClaimTypes.NameIdentifier, user.UserName),
					new Claim(ClaimTypes.Email, user.Email),
					//new Claim(ClaimTypes.Role, user.)
				};

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

				var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
					_configuration["Jwt:Audience"],
					claims,
					expires: DateTime.Now.AddMinutes(30),
					signingCredentials: creds);

				return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
			}

			return Unauthorized();
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] LoginModel model)
		{
			if (await _userManager.FindByEmailAsync(model.Login) != null)
			{
				return ValidationProblem();
			}
			var user = new IdentityUser { Email = model.Login, UserName = model.Login };
			var result = await _userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				return Ok();
			}

			return BadRequest(result.Errors);
		}
	}
}
