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
		private readonly Serilog.ILogger log = Log.ForContext<Memloc_Query>();


		[UseProjection]
		public IQueryable<Memloc> memlocs([Service] Demo_Context context, int? layout_id)
		{
			log.Information("Fetching memlocs");
			IQueryable<Memloc> q = context.memlocs.OrderBy(x => x.byteoffset);
			if (layout_id == null)
			{
				return q;
			}
			else
			{
				return q.Where(x => x.layout_id == layout_id);
			}
		}

	}
}
