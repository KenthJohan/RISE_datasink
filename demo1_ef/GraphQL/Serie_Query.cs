using System.Linq;
using System;


using Serilog;

using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Relay;


namespace Demo
{

/*
	[ExtendObjectType("Serie_Query")]
	public class Serie_Resolver
	{

	}
	*/


	[ExtendObjectType("Query")]
	public class Serie_Query
	{
		private readonly ILogger log = Log.ForContext<Serie_Query>();

		[HotChocolate.Data.UseProjection]
		[HotChocolate.Data.UseFiltering]
		[HotChocolate.Data.UseSorting]
		public IQueryable<Serie> series([Service] Demo_Context context)
		{
			log.Information("Fetching series");
			return context.series;
		}

		public IQueryable<Serie> serie_by_id([Service] Demo_Context context, [ID(nameof(Serie))]int id) 
		{
			return context.series.Where(u => u.id == id);
		}


		public string hello([Service] Demo_Context context)
		{
			return "hello";
		}


	}
}
