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
			builder.Entity<Floatval>().HasKey(u => new{u.time,u.producer_id});
		}
	}


	public static class DbContextExtensions
	{
		public static void config_hypertables(this Demo_Context context)
		{
			int r;
			r = context.Database.ExecuteSqlRaw($"CREATE EXTENSION IF NOT EXISTS timescaledb;");
			Log.Information("CREATE timescaledb {r}", r);
			r = context.Database.ExecuteSqlRaw($"SELECT create_hypertable('floatvals', 'time');");
			Log.Information("create_hypertable {r}", r);
			r = context.Database.ExecuteSqlRaw($"SELECT add_dimension('floatvals', 'producer_id', number_partitions => 4);");
			Log.Information("add_dimension {r}", r);
		}


		
		public static void add_test_data(this Demo_Context context)
		{
			List<Device> d = new List<Device>{
				new Device { id = 1, name = "Unknown" },
				new Device { id = 2, name = "BME280" },
				new Device { id = 3, name = "BME280" },
				new Device { id = 4, name = "BME280" },
				new Device { id = 5, name = "BME280" }
			};

			List<Project> p = new List<Project>{
				new Project { id = 1, name = "Unknown" },
				new Project { id = 2, name = "MiLo" },
				new Project { id = 3, name = "Arena" },
				new Project { id = 4, name = "Johan Home" },
			};

			List<Location> l = new List<Location>{
				new Location { id = 1, name = "Unknown" },
				new Location { id = 2, name = "MiLo ATV" },
				new Location { id = 3, name = "Johan home" },
			};

			List<Quantity> q = new List<Quantity>{
				new Quantity { id = 1, name = "Unknown" },
				new Quantity { id = 2, name = "Temperature" },
				new Quantity { id = 3, name = "Humidity" },
				new Quantity { id = 4, name = "Wind" },
			};

			{
				List<Producer> producers = new List<Producer>
				{
					new Producer { id = 1, name = "Unknown" },
					new Producer { id = 2, name = "Temperature" },
					new Producer { id = 3, name = "Humidity" },
					new Producer { id = 4, name = "Wind" },
				};
				context.producers.AddRange(producers);
			}
			//humidity, barometric pressure and ambient temperature

			List<Floatval> v = new List<Floatval>
			{
				new Floatval { time = DateTime.Now.AddMilliseconds(1), value = 1.2f},
				new Floatval { time = DateTime.Now.AddMilliseconds(2), value = 1.3f},
				new Floatval { time = DateTime.Now.AddMilliseconds(3), value = 1.4f},
				new Floatval { time = DateTime.Now.AddMilliseconds(4), value = 1.5f},
				new Floatval { time = DateTime.Now.AddMilliseconds(5), value = 1.6f}
			};

			context.devices.AddRange(d);
			context.projects.AddRange(p);
			context.locations.AddRange(l);
			context.quantities.AddRange(q);
			context.floatvals.AddRange(v);

			int r = context.SaveChanges();
			Log.Information("SaveChanges {r}", r);
		}

	}

}