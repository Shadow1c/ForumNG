using ForumNG.Data;
using ForumNG.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ForumNG.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class ForumController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _configuration;
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;

		public ForumController(ApplicationDbContext context, IConfiguration configuration, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
		{
			_context = context;
			_configuration = configuration;
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}

		[HttpGet("Title/GetAll")]
		public async Task<ActionResult<IEnumerable<ForumTitle>>> GetAll()
		{
			try
			{
				var result = await _context.Titles.ToListAsync();
				if (result.Count == 0)
				{
					return NotFound();
				}
				return Ok(result);
			}
			catch (Exception ex)
			{
				return Problem(ex.Message);
			}
		}

		[HttpGet("Title/GetAll/limit/{limit}")]
		public async Task<ActionResult<IEnumerable<ForumTitle>>> GetAll(int limit)
		{
			try
			{
				var result = await _context.Titles.Take(limit).ToListAsync();
				if (result.Count == 0)
				{
					return NotFound();
				}
				return Ok(result);
			}
			catch (Exception ex)
			{
				return Problem(ex.Message);
			}
		}

		[HttpDelete("Title/Delete/{id}")]
		public async Task<ActionResult> Delete(int id)
		{
			try
			{
				var title = await _context.Titles.FindAsync(id);
				if (title == null)
				{
					return NotFound();
				}

				_context.Titles.Remove(title);
				await _context.SaveChangesAsync();

				return Ok();
			}
			catch (Exception ex)
			{
				return Problem(ex.Message);
			}
		}
	}
}
