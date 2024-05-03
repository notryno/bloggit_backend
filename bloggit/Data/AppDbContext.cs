using System;
using bloggit.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace bloggit.Data
{
	public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
	{
		public AppDbContext(DbContextOptions options) : base(options)
		{
		}
		public DbSet<Product> Products { get; set; }
	}
}

