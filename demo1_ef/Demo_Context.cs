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


#nullable disable

namespace Demo
{
	public class Demo_Context : DbContext
	{

		private readonly ILoggerFactory _loggerFactory;
		private readonly Serilog.ILogger log = Log.ForContext<Demo_Context>();

		public DbSet<User> users { get; set; }
		public DbSet<Book> books { get; set; }
		public DbSet<Floatval> floatvals { get; set; }
		public DbSet<Quantity> quantities { get; set; }
		public DbSet<Location> locations { get; set; }
		public DbSet<Device> devices { get; set; }
		public DbSet<Project> projects { get; set; }
		public DbSet<Producer> producers { get; set; }
		public DbSet<Layout> layouts { get; set; }
		public DbSet<Memloc> memlocs { get; set; }


		public Demo_Context(ILoggerFactory loggerFactory, DbContextOptions<Demo_Context> options)
			: base(options)
		{
			_loggerFactory = loggerFactory;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder options)
		{
			options.UseLoggerFactory(_loggerFactory);
			options.UseNpgsql(DB.npgsql_connection);
			log.Information("UseNpgsql {s}", DB.npgsql_connection);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<User>().HasIndex(u => u.email).IsUnique();
			builder.Entity<User>().HasMany(t => t.books).WithOne(t => t.author).HasForeignKey(t => t.author_id);
			builder.Entity<Device>().HasMany(t => t.producer).WithOne(t => t.device).HasForeignKey(t => t.device_id);
			builder.Entity<Project>().HasMany(t => t.producer).WithOne(t => t.project).HasForeignKey(t => t.project_id);
			builder.Entity<Quantity>().HasMany(t => t.producer).WithOne(t => t.quantity).HasForeignKey(t => t.quantity_id);
			builder.Entity<Location>().HasMany(t => t.producer).WithOne(t => t.location).HasForeignKey(t => t.location_id);
			builder.Entity<Producer>().HasMany(t => t.floatvals).WithOne(t => t.producer).HasForeignKey(t => t.producer_id);
			builder.Entity<Layout>().HasMany(t => t.memlocs).WithOne(t => t.layout).HasForeignKey(t => t.layout_id);
			builder.Entity<Floatval>().HasKey(u => new{u.time,u.producer_id});
			builder.Entity<Floatval>().Property(b => b.longitude).HasDefaultValueSql("0.0");
			builder.Entity<Floatval>().Property(b => b.latitude).HasDefaultValueSql("0.0");
		}
	}


}