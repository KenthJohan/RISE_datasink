using System;
using System.Threading.Tasks;
using System.Linq;


using Microsoft.EntityFrameworkCore;

using Serilog;

using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Types;

namespace Demo
{
	[ExtendObjectType("Mutation")]
	public class Producer_Mutation
	{
		public int producers_update([Service] Demo_Context context, int id, 
		bool? enable_mqtt, 
		bool? enable_reqget, 
		bool? enable_websock, 
		bool? enable_storage
		)
		{
			Producer p = context.producers.Where(t => t.id == id).SingleOrDefault();
			if (p == null) {return 0;}
			p.enable_mqtt = enable_mqtt ?? p.enable_mqtt;
			p.enable_reqget = enable_reqget ?? p.enable_reqget;
			p.enable_websock = enable_websock ?? p.enable_websock;
			p.enable_storage = enable_storage ?? p.enable_storage;
			context.producers.Update(p);
			return context.SaveChanges();
		}

	}
}