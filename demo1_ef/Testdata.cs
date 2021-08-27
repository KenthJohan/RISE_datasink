using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.Common;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Serilog;

using Npgsql;
using NpgsqlTypes;

namespace Demo
{

	public static class Testdata
	{
		private static readonly ILogger log = Log.ForContext(typeof(Testdata));
		public static void add(this Demo_Context context)
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

			{
				List<Layout> records = new List<Layout>
				{
					new Layout { id = 1, name = "Test 1" },
					new Layout { id = 2, name = "Test 2" },
					new Layout { id = 3, name = "Test 3" },
					new Layout { id = 4, name = "MiLo ATV sensors" },
				};
				context.layouts.AddRange(records);
			}

			{
				List<Memloc> records = new List<Memloc>
				{
					new Memloc { id = 1, layout_id = 1, producer_id = 5, byteoffset = 8 },
					new Memloc { id = 2, layout_id = 1, producer_id = 6, byteoffset = 12 },
					new Memloc { id = 3, layout_id = 1, producer_id = 7, byteoffset = 16 },
					new Memloc { id = 4, layout_id = 1, producer_id = 8, byteoffset = 20 },

					new Memloc { id = 5, layout_id = 4, producer_id = 1, byteoffset = 8 },
					new Memloc { id = 6, layout_id = 4, producer_id = 2, byteoffset = 12 },
					new Memloc { id = 7, layout_id = 4, producer_id = 3, byteoffset = 16 },
					new Memloc { id = 8, layout_id = 4, producer_id = 4, byteoffset = 20 },
					new Memloc { id = 9, layout_id = 4, producer_id = 11, byteoffset = 24 },
					new Memloc { id = 10, layout_id = 4, producer_id = 12, byteoffset = 28 },
				};
				context.memlocs.AddRange(records);
			}

			int r = context.SaveChanges();
			Log.Information("SaveChanges {r}", r);
		}

	}
}