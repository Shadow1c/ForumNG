using ForumNG.Models.ForumModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ForumNG.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{

		}
		public DbSet<IdentityUser> Users { get; set; }
		public DbSet<ForumTitle> Titles { get; set; }
		public DbSet<ForumPost> Posts { get; set; }

	}
}
