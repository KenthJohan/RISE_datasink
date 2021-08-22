using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

namespace Demo
{
	public static class GraphQL_Services
	{
		private static readonly ILogger log = Log.ForContext(typeof(GraphQL_Services));
		public static void init(IServiceCollection services)
		{
			log.Information("Init graphql service");
			services
				.AddGraphQLServer()
				.AddType<User_Resolver>()
				.AddType<Book_Resolver>()
				.AddType<Seriefloat_Resolver>()
				.AddType<Producer_Resolver>()
				.AddQueryType(d => d.Name("Query"))
					.AddTypeExtension<User_Query>()
					.AddTypeExtension<Book_Query>()
					.AddTypeExtension<Floatval_Query>()
					.AddTypeExtension<Device_Query>()
					.AddTypeExtension<Location_Query>()
					.AddTypeExtension<Project_Query>()
					.AddTypeExtension<Quantity_Query>()
					.AddTypeExtension<Producer_Query>()
				.AddMutationType(d => d.Name("Mutation"))
					.AddTypeExtension<User_Mutation>()
					.AddTypeExtension<Floatval_Mutation>()
				.AddProjections()
				.AddFiltering()
				.AddSorting();
		}
	}
}