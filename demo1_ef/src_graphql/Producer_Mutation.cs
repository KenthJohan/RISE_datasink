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
			if (enable_storage != null)
			{
				p.enable_storage = enable_storage.Value;
				if (p.enable_storage) {Hub.enable_storage.Add(id);}
				else {Hub.enable_storage.Remove(id);}
			}
			if (enable_websock != null)
			{
				p.enable_websock = enable_websock.Value;
				if (p.enable_websock) {Hub.enable_websock.Add(id);}
				else {Hub.enable_websock.Remove(id);}
			}
			if (enable_reqget != null)
			{
				p.enable_reqget = enable_reqget.Value;
				if (p.enable_reqget) {Hub.enable_reqget.Add(id);}
				else {Hub.enable_reqget.Remove(id);}
			}
			if (enable_mqtt != null)
			{
				p.enable_mqtt = enable_mqtt.Value;
				if (p.enable_mqtt) {Hub.enable_mqtt.Add(id);}
				else {Hub.enable_mqtt.Remove(id);}
			}
			context.producers.Update(p);
			return context.SaveChanges();
		}

	}
}