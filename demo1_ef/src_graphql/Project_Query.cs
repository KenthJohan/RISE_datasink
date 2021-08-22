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
	public class Project_Query
	{
		private readonly ILogger log = Log.ForContext<Project_Query>();


		public IQueryable<Project> projects([Service] Demo_Context context)
		{
			log.Information("Fetching projects");
			return context.projects;
		}

	}
}
