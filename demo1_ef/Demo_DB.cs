using System;
using System.Data;
using System.Data.Common;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Serilog;

//https://www.codeproject.com/Articles/5263745/Return-DataTable-Using-Entity-Framework

namespace Demo
{
	public static class DB
	{


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
			Log.Information("DB Init");
			using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
			{
				var context = serviceScope.ServiceProvider.GetRequiredService<Demo_Context>();
				//If all tables are dropped then they will be created here:
				//dropall(context);
				//context.Database.ExecuteSqlRaw("DROP TABLE course_user_edges CASCADE");
				var deleted = context.Database.EnsureDeleted();
				var created = context.Database.EnsureCreated();
				if (deleted){Log.Information("Database delted");}
				else{Log.Information("Database not delted, does not exist");}
				if (created){Log.Information("Database created");}
				else{Log.Information("Database not created, already exist");}
				//Testing.db_add_example(context);
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

			//Testing.db_add_example(context);
		}



	}
}