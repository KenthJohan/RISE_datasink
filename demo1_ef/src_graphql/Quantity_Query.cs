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
	public class Quantity_Query
	{
		private readonly Serilog.ILogger log = Log.ForContext<Quantity_Query>();


		public IQueryable<Quantity> quantities([Service] Demo_Context context)
		{
			log.Information("Fetching quantities");
			return context.quantities;
		}

	}
}
