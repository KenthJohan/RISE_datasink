using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Npgsql;


namespace Demo
{
	public class Program
	{

		public static string npgsql_connection = "Host=localhost:5432;Database=demo;Username=postgres;Password=secret";
		public static void test_psql()
		{
			Log.Information("NpgsqlConnection {s}", npgsql_connection);
			NpgsqlConnection conn = new NpgsqlConnection(npgsql_connection); 
			conn.Open(); 
			if (conn.State == System.Data.ConnectionState.Open)
			{
				Log.Information("Success open postgreSQL connection. {@ConnectionState}", conn.State); 
			}
			else
			{
				Log.Information("Failed open postgreSQL connection. {@ConnectionState}", conn.State); 
			}
			conn.Close(); 
		}

		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.CreateBootstrapLogger();

			test_psql();

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
