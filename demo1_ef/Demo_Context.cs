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
			{
				List<Device> records = new List<Device>
				{
					new Device { id = 1, name = "Unknown" },
					new Device { id = 2, name = "BME280" },
					new Device { id = 3, name = "BME280" },
					new Device { id = 4, name = "BME280" },
					new Device { id = 5, name = "BME280" }
				};
				context.devices.AddRange(records);
			}

			{
				List<Project> records = new List<Project>
				{
					new Project { id = 1, name = "Unknown" },
					new Project { id = 2, name = "MiLo" },
					new Project { id = 3, name = "Arena" },
					new Project { id = 4, name = "Johan Home" },
				};
				context.projects.AddRange(records);
			}

			{
				List<Location> records = new List<Location>
				{
					new Location { id = 1, name = "Unknown" },
					new Location { id = 2, name = "MiLo ATV" },
					new Location { id = 3, name = "Johan home" },
				};
				context.locations.AddRange(records);
			}

			{
				List<Quantity> records = new List<Quantity>
				{
					new Quantity { id = 1, name = "Unknown" },
					new Quantity { id = 2, name = "Temperature" },
					new Quantity { id = 3, name = "Humidity" },
					new Quantity { id = 4, name = "Wind" },
					new Quantity { id = 5, name = "Pressure" },
					new Quantity { id = 6, name = "Force" },
					new Quantity { id = 7, name = "Current" },
					new Quantity { id = 8, name = "Power" },
					new Quantity { id = 9, name = "Voltage" },
				};
				context.quantities.AddRange(records);
			}


			{
				List<Producer> records = new List<Producer>
				{
					new Producer { id = 1, name = "Unknown", quantity_id = 1 },
					new Producer { id = 2, name = "Producer 2", quantity_id = 2 },
					new Producer { id = 3, name = "Producer 3", quantity_id = 3 },
					new Producer { id = 4, name = "Producer 4", quantity_id = 4 },
					new Producer { id = 5, name = "Producer 5", quantity_id = 5 },
					new Producer { id = 6, name = "Producer 6", quantity_id = 6 },
					new Producer { id = 7, name = "Producer 7", quantity_id = 7 },
					new Producer { id = 8, name = "Producer 8", quantity_id = 8 },
					new Producer { id = 9, name = "Producer 9", quantity_id = 9 },
					new Producer { id = 10, name = "Producer 10", quantity_id = 9 },
					new Producer { id = 11, name = "Producer 11", quantity_id = 4 },
					new Producer { id = 12, name = "Producer 12", quantity_id = 4 },
				};
				context.producers.AddRange(records);
			}
			//humidity, barometric pressure and ambient temperature

			{
				List<Floatval> records = new List<Floatval>
				{
					new Floatval { time = DateTime.Now.AddMilliseconds(1), value = 1.2f},
					new Floatval { time = DateTime.Now.AddMilliseconds(2), value = 1.3f},
					new Floatval { time = DateTime.Now.AddMilliseconds(3), value = 1.4f},
					new Floatval { time = DateTime.Now.AddMilliseconds(4), value = 1.5f},
					new Floatval { time = DateTime.Now.AddMilliseconds(5), value = 1.6f}
				};
				context.floatvals.AddRange(records);
			}



			int r = context.SaveChanges();
			Log.Information("SaveChanges {r}", r);
		}

	}

}