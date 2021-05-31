using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;

using System;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using System.Security.Claims;
using System.Net;
using System.Net.WebSockets;
using System.Net.Http;
using System.Net.Http.Headers;


using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using Serilog;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


#nullable disable

namespace Demo
{
	public class Demo_Context : DbContext
	{
		public DbSet<User> users { get; set; }
		public DbSet<Book> books { get; set; }

		public Demo_Context(DbContextOptions<Demo_Context> options)
			: base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			string s = "Host=localhost:5432;Database=demo;Username=postgres;Password=secret";
			options.UseNpgsql(s);
			Log.Information("Connection string {s}", s);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<User>().HasIndex(u => u.email).IsUnique();

			builder.Entity<User>()
				.HasMany(t => t.books)
				.WithOne(t => t.author)
				.HasForeignKey(t => t.author_id);

		}
	}
}