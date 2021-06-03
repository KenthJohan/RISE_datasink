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
		public DbSet<Seriefloat> seriefloats { get; set; }
		public DbSet<Quantity> quantities { get; set; }
		public DbSet<Location> locations { get; set; }
		public DbSet<Device> devices { get; set; }
		public DbSet<Project> projects { get; set; }
		public DbSet<Serie> series { get; set; }


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

			builder.Entity<User>()
				.HasMany(t => t.books)
				.WithOne(t => t.author)
				.HasForeignKey(t => t.author_id);

			//builder.Entity<Sensorvalue>().HasOne(t => t.timeserie).WithMany(t => t.sensorvalues);
			//builder.Entity<Timeserie>().HasOne(t => t.device).WithMany(t => t.timeseries);

			builder.Entity<Serie>().HasMany(t => t.seriefloats).WithOne(t => t.serie).HasForeignKey(t => t.serie_id);
			builder.Entity<Device>().HasMany(t => t.series).WithOne(t => t.device).HasForeignKey(t => t.device_id);
			builder.Entity<Project>().HasMany(t => t.series).WithOne(t => t.project).HasForeignKey(t => t.project_id);
			builder.Entity<Quantity>().HasMany(t => t.series).WithOne(t => t.quantity).HasForeignKey(t => t.quantity_id);
			builder.Entity<Location>().HasMany(t => t.series).WithOne(t => t.location).HasForeignKey(t => t.location_id);


			builder.Entity<Seriefloat>().HasKey(u => new
			{
				u.time,
				u.serie_id
			});





		}
	}


	public static class DbContextExtensions
	{
		public static void config_hypertables(this Demo_Context context)
		{
			int r;
			r = context.Database.ExecuteSqlRaw($"CREATE EXTENSION IF NOT EXISTS timescaledb;");
			Log.Information("CREATE timescaledb {r}", r);
			r = context.Database.ExecuteSqlRaw($"SELECT create_hypertable('seriefloats', 'time', 'serie_id', 1);");
			//r = context.Database.ExecuteSqlRaw($"SELECT create_distributed_hypertable('sensorvalues', 'time', 'device_id');");
			Log.Information("create_hypertable {r}", r);
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

			List<Serie> t = new List<Serie>
			{
				new Serie{id = 1, name="My timeserie 1", location_id = 1, project_id = 1, quantity_id = 1, device_id = 1},
				new Serie{id = 2, name="My timeserie 2", location_id = 1, project_id = 1, quantity_id = 1, device_id = 1},
			};

			List<Seriefloat> v = new List<Seriefloat>
			{
				new Seriefloat { time = DateTime.Now.AddMilliseconds(1), value = 1.2f, serie_id = 1 },
				new Seriefloat { time = DateTime.Now.AddMilliseconds(2), value = 1.3f, serie_id = 1 },
				new Seriefloat { time = DateTime.Now.AddMilliseconds(3), value = 1.4f, serie_id = 1 },
				new Seriefloat { time = DateTime.Now.AddMilliseconds(4), value = 1.5f, serie_id = 1 },
				new Seriefloat { time = DateTime.Now.AddMilliseconds(5), value = 1.6f, serie_id = 1 }
			};

			context.devices.AddRange(d);
			context.projects.AddRange(p);
			context.locations.AddRange(l);
			context.quantities.AddRange(q);
			context.series.AddRange(t);
			context.seriefloats.AddRange(v);

			int r = context.SaveChanges();
			Log.Information("SaveChanges {r}", r);
		}

	}

}