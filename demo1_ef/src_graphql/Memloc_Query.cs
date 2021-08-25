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
	public class Memloc_Query
	{
		private readonly ILogger log = Log.ForContext<Memloc_Query>();


		[UseProjection]
		public IQueryable<Memloc> memlocs([Service] Demo_Context context)
		{
			log.Information("Fetching memlocs");
			return context.memlocs;
		}

	}
}
