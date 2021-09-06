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
	public class Location_Query
	{
		private readonly Serilog.ILogger log = Log.ForContext<Location_Query>();


		public IQueryable<Location> locations([Service] Demo_Context context)
		{
			log.Information("Fetching locations");
			return context.locations;
		}

	}
}
