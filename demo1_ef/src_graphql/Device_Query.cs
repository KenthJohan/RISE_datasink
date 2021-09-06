using System.Linq;
using System;


using Serilog;

using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Relay;


namespace Demo
{

	[ExtendObjectType("Query")]
	public class Device_Query
	{
		private readonly Serilog.ILogger log = Log.ForContext<Device_Query>();


		public IQueryable<Device> devices([Service] Demo_Context context)
		{
			log.Information("Fetching devices");
			return context.devices;
		}

	}
}
