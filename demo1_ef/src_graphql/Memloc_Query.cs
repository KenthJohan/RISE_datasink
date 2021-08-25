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
		public IQueryable<Memloc> memlocs([Service] Demo_Context context, int? layout_id)
		{
			log.Information("Fetching memlocs");
			if (layout_id == null)
			{
				return context.memlocs;
			}
			else
			{
				return context.memlocs.Where(x => x.layout_id == layout_id);
			}
		}

	}
}
