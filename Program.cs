using ForumNG.Data;
using ForumNG.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ForumNG
{
	public class Program
	{
		public static void Main(string[] args)
		{

			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.


			builder.Services.AddCors(options =>
			{
				options.AddDefaultPolicy(
					builder =>
					{
						builder.AllowAnyHeader()
						.AllowAnyMethod()
						.AllowAnyOrigin();
					});
			});

			builder.Services.AddControllers();

			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


			builder.Services.AddIdentity<IdentityUser, IdentityRole>()
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();

			// Dodaj konfiguracjê JWT Bearer
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ValidIssuer = builder.Configuration["Jwt:Issuer"],
					ValidAudience = builder.Configuration["Jwt:Audience"],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
				};
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseCors();
			app.UseHttpsRedirection();
			
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();
			
			app.Run();
		}
	}
}
