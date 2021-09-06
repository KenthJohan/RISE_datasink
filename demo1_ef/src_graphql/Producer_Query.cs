using System.Linq;
using System;


using Serilog;

using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Relay;


namespace Demo
{

	[ExtendObjectType("Producer")]
	public class Producer_Resolver
	{
		public string quantity_name(Producer producer, [Service] Demo_Context context)
		{
			return context.quantities.Where(u => u.id == producer.quantity_id).Select(x => x.name).FirstOrDefault();
		}
	}


	[ExtendObjectType("Query")]
	public class Producer_Query
	{
		private readonly ILogger log = Log.ForContext<Producer_Query>();


		[UseProjection]
		public IQueryable<Producer> producers([Service] Demo_Context context)
		{
			log.Information("Fetching producers");
			return context.producers.OrderBy(x => x.id);
		}

	}
}
