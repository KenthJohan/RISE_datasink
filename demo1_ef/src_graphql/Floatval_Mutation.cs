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
	public class Floatval_Mutation
	{
		public int floatval_add([Service] Demo_Context context, int producer_id, float value)
		{
			Floatval state = new Floatval{};
			state.value = value;
			state.time = DateTime.Now;
			state.producer_id = producer_id;
			context.floatvals.Add(state);
			context.SaveChanges();
			Sublist_Producer.publish(state.producer_id, state.time, state.value);
			return 0;
		}

	}
}