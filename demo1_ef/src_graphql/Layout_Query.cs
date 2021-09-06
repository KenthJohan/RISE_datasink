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
	public class Layout_Query
	{
		private readonly Serilog.ILogger log = Log.ForContext<Layout_Query>();

		[UseProjection]
		public IQueryable<Layout> layouts([Service] Demo_Context context)
		{
			log.Information("Fetching layouts");
			return context.layouts;
		}

	}
}
