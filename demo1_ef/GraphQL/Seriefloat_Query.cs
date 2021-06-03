using System.Linq;
using System;


using Serilog;

using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Relay;


namespace Demo
{

	[ExtendObjectType("Seriefloat_Query")]
	public class Seriefloat_Resolver
	{
		public float times_20(Seriefloat s)
		{
			return s.value * 20;
		}

	}


	[ExtendObjectType("Query")]
	public class Seriefloat_Query
	{
		private readonly ILogger log = Log.ForContext<Seriefloat_Query>();

		[HotChocolate.Data.UseProjection]
		[HotChocolate.Data.UseFiltering]
		[HotChocolate.Data.UseSorting]
		public IQueryable<Seriefloat> seriefloats([Service] Demo_Context context)
		{
			log.Information("Fetching seriefloats");
			return context.seriefloats;
		}

/*
		public IQueryable<Seriefloat> seriefloats_by_id([Service] Demo_Context context, [ID(nameof(User))]int id) 
		{
			return context.users.Where(u => u.id == id);
		}
		*/

	}
}
