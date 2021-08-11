using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Serilog;

namespace Demo
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			//https://entityframeworkcore.com/knowledge-base/53150930/how-to-avoid-not-safe-context-operations-in-ef-core
			services.AddControllers();
			services.AddDbContext<Demo_Context>(ServiceLifetime.Transient);
			services.AddHttpContextAccessor();
			//services.AddTransient<Demo_Context>();
			
			services
				.AddGraphQLServer()
				.AddType<User_Resolver>()
				.AddType<Book_Resolver>()
				.AddType<Seriefloat_Resolver>()
				.AddQueryType(d => d.Name("Query"))
					.AddTypeExtension<User_Query>()
					.AddTypeExtension<Book_Query>()
					.AddTypeExtension<Seriefloat_Query>()
					.AddTypeExtension<Serie_Query>()
					.AddTypeExtension<Device_Query>()
					.AddTypeExtension<Location_Query>()
					.AddTypeExtension<Project_Query>()
					.AddTypeExtension<Quantity_Query>()
				.AddMutationType(d => d.Name("Mutation"))
					.AddTypeExtension<User_Mutation>()
				.AddProjections()
				.AddFiltering()
				.AddSorting();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			DB.init(app);
			loggerFactory.AddSerilog();
			app.UseSerilogRequestLogging();
			app.UseWebSockets();
			app.UseDefaultFiles();
			app.UseStaticFiles();

			app
				.UseRouting()
				.UseEndpoints(endpoints =>
				{
					endpoints.MapGraphQL();
					endpoints.MapDefaultControllerRoute();
				});
		}
	}
}
