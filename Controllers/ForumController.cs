using ForumNG.Data;
using ForumNG.Models.ForumModel;
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
		public async Task<ActionResult<IEnumerable<ForumTitle>>> GetAllTitle()
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

		[HttpGet("Title/GetAll/limit/{limit}/{preset}")]
		public async Task<ActionResult<IEnumerable<ForumTitle>>> GetAllTitle(int limit,int preset)
		{
			try
			{
				var result = await _context.Titles.Take(limit).Skip(preset).ToListAsync();
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

		[HttpGet("Title/{id}")]
		public async Task<ActionResult<ForumTitle>> GetAllTitle(int id)
		{
			try
			{
				var result = await _context.Titles.FirstOrDefaultAsync(x => x.Id == id);
				if (result == null)
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
		public async Task<ActionResult> DeleteTitle(int id)
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

		[HttpGet("Post/GetAll")]
		public async Task<ActionResult<IEnumerable<ForumTitle>>> GetAllPost()
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

		[HttpGet("Post/GetAll/{id}/limit/{limit}/{preset}")]
		public async Task<ActionResult<IEnumerable<ForumTitle>>> GetAllPost(int id, int limit,int preset)
		{
			try
			{
				var result = await _context.Titles.Where(x => x.Id == id).Skip(preset).Take(limit).ToListAsync();
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

		[HttpDelete("Post/Delete/{id}")]
		public async Task<ActionResult> DeletePost(int id)
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
