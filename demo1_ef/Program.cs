using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;

namespace Demo
{
	public class Program
	{
		private static readonly Serilog.ILogger log = Log.ForContext(typeof(Program));

		public static void Main(string[] args)
		{
			Password.assert_salthash();

			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console(applyThemeToRedirectedOutput: true)
				.WriteTo.Demo_Sink()
				.CreateBootstrapLogger();


			DB.test_psql();
			Monitor.init();
			Proc_Reader.test();

			CreateHostBuilder(args)
				.UseSerilog((context, services, configuration) => configuration
					.ReadFrom.Configuration(context.Configuration)
					.ReadFrom.Services(services)
					//.MinimumLevel.Verbose()
					.WriteTo.Console()
					.WriteTo.Demo_Sink())
				.Build().Run();

		}

		public static IHostBuilder CreateHostBuilder(string[] args)
		{

			return Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
		}

	}
}
