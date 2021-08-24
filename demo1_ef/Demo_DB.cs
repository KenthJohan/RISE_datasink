using System;
using System.Data;
using System.Data.Common;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Serilog;

using Npgsql;
using NpgsqlTypes;


//https://www.codeproject.com/Articles/5263745/Return-DataTable-Using-Entity-Framework

namespace Demo
{
	public static class DB
	{

		public static string npgsql_connection = "Server=localhost; Port=5432; UserId=datasink; Password=datasink; Database=datasink";
		//public static string npgsql_connection = "Host=localhost:5432;Database=datasink;Username=postgres;Password=secret";
		public static NpgsqlConnection connection;
		public static NpgsqlCommand command_insert_floatval;

		private static readonly ILogger log = Log.ForContext(typeof(DB));

		public static void dropall(DbContext context)
		{
			Log.Information("Dropping all tables");
			//var context = serviceScope.ServiceProvider.GetRequiredService<SchoolContext>();
			//(Development) Drop all tables so we can create new ones:
			foreach (System.Data.DataRow row in context.Database.GetDbConnection().GetSchema("Tables").Rows)
			{
				String s = "DROP TABLE \"" + row["table_name"] + "\" CASCADE";
				Console.WriteLine(s);
				context.Database.ExecuteSqlRaw(s);
			}
		}

		public static DataTable export_table(this DbContext context, string sqlQuery, params DbParameter[] parameters)
		{
			DataTable dataTable = new DataTable();
			DbConnection connection = context.Database.GetDbConnection();
			DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);
			using (var cmd = dbFactory.CreateCommand())
			{
				cmd.Connection = connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sqlQuery;
				if (parameters != null)
				{
					foreach (var item in parameters)
					{
						cmd.Parameters.Add(item);
					}
				}
				using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
				{
					adapter.SelectCommand = cmd;
					adapter.Fill(dataTable);
				}
			}
			return dataTable;
		}



		public static void init(IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
			{
				var context = serviceScope.ServiceProvider.GetRequiredService<Demo_Context>();
				init1(context);
			}
		}


		public static void init1(Demo_Context context)
		{
			Log.Information("DB Init");
			//If all tables are dropped then they will be created here:
			//dropall(context);
			//context.Database.ExecuteSqlRaw("DROP TABLE course_user_edges CASCADE");
			var deleted = context.Database.EnsureDeleted();
			var created = context.Database.EnsureCreated();
			if (deleted){Log.Information("Database deleted");}
			else{Log.Information("Database not deleted, does not exist");}
			if (created){Log.Information("Database created");}
			else{Log.Information("Database not created, already exist");}

			context.config_hypertables();
			context.add_test_data();

			connection = new NpgsqlConnection(DB.npgsql_connection);
			connection.Open();
			command_insert_floatval = new NpgsqlCommand("INSERT INTO floatvals (producer_id,value) VALUES (@producer_id, @value)", connection);
			command_insert_floatval.Parameters.Add("producer_id", NpgsqlDbType.Integer);
			command_insert_floatval.Parameters.Add("value", NpgsqlDbType.Real);
			command_insert_floatval.Prepare();
			//Testing.db_add_example(context);
		}



		public static void test_psql()
		{
			log.Information("NpgsqlConnection {s}", npgsql_connection);
			NpgsqlConnection conn = new NpgsqlConnection(npgsql_connection); 
			conn.Open();
			if (conn.State == System.Data.ConnectionState.Open)
			{
				log.Information("Success open postgreSQL connection. {@ConnectionState}", conn.State); 
			}
			else
			{
				log.Information("Failed open postgreSQL connection. {@ConnectionState}", conn.State); 
			}
			conn.Close(); 
		}



	}
}