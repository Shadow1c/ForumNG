using ForumNG.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
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
		private readonly RoleManager<IdentityRole> _roleManager;

		public AccountController(IConfiguration configuration, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
		{
			_configuration = configuration;
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}
		
		[HttpGet("AddRole")]
		public async Task<IActionResult> AddRole()
		{
			if(!await _roleManager.RoleExistsAsync("User"))
			{
				await _roleManager.CreateAsync(new IdentityRole("User"));
				await _roleManager.CreateAsync(new IdentityRole("Admin"));
				return Ok();
			}

			return NotFound();
		}


		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginModel model)
		{
			
			var user = await _userManager.FindByEmailAsync(model.Login);

			if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
			{
				var roles = await _userManager.GetRolesAsync(user);

				if (roles == null)
				{
					return NotFound("Error, User without role assigned! Contact with application Administrator.");
				}

				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, user.UserName),
					new Claim(ClaimTypes.Email, user.Email)
				
				};

				foreach (var role in roles)
				{
					claims.Add(new Claim(ClaimTypes.Role, role));
				}

				var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
				var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
				var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
					_configuration["Jwt:Audience"],
					claims,
					expires: DateTime.Now.AddDays(2),
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
				if (user != null)
				{
					await _userManager.AddToRoleAsync(user, "User");
					return Ok();
				}
			}

			return BadRequest(result.Errors);
		}

		[HttpPost("role")]
		public async Task<IActionResult> GetRole()
		{
			
			var jwtToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

			try
			{
				var principal = tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(key),
					ValidateIssuer = false,
					ValidateAudience = false,
					ClockSkew = TimeSpan.Zero
				}, out SecurityToken validatedToken);

				var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

				if (roleClaim == null)
				{
					return Unauthorized();
				}

				return Ok(new { role = roleClaim.Value });
			}

			catch (SecurityTokenExpiredException)
			{
				return Unauthorized("Token is expired");
			}

			catch (Exception e)
			{
				return Unauthorized("Invalid token" + e);
			}
		}
	}
}
