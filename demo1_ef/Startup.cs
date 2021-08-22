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
			GraphQL_Services.init(services);
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
