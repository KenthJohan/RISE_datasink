using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

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
			//services.AddTransient<Demo_Context>();
			
			services
				.AddGraphQLServer()
				.AddType<User_Resolver>()
				.AddType<Book_Resolver>()
				.AddQueryType(d => d.Name("Query"))
					.AddTypeExtension<User_Query>()
					.AddTypeExtension<Book_Query>()
				.AddMutationType(d => d.Name("Mutation"))
					.AddTypeExtension<User_Mutation>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			DB.init(app);

			/*
			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapGet("/", async context =>
				{
					await context.Response.WriteAsync("Hello World!");
				});
			});
			*/

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
