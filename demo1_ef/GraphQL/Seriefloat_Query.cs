using System.Linq;
using System;
using System.Collections.Generic;


using Serilog;

using HotChocolate;
using HotChocolate.Types;


using Npgsql;
using System.Data;

namespace Demo
{

	[ExtendObjectType("Seriefloat_Query")]
	public class Seriefloat_Resolver
	{
		public float times_20(Seriefloat s)
		{
			return s.value * 20;
		}
	}




	[ExtendObjectType("Query")]
	public class Seriefloat_Query
	{
		private readonly ILogger log = Log.ForContext<Seriefloat_Query>();

		[HotChocolate.Data.UseProjection]
		[HotChocolate.Data.UseFiltering]
		[HotChocolate.Data.UseSorting]
		public IQueryable<Seriefloat> seriefloats([Service] Demo_Context context)
		{
			log.Information("Fetching seriefloats");
			return context.seriefloats;
		}

		public List<float> seriefloats_column1([Service] Demo_Context context, int serie_id)
		{
			return context.seriefloats.Where(t => t.serie_id == serie_id).Select(t => t.value).ToList();
		}
		public int seriefloats_column2([Service] Demo_Context context, int serie_id)
		{
			NpgsqlConnection conn = new NpgsqlConnection(DB.npgsql_connection);
			NpgsqlCommand com = new NpgsqlCommand("SELECT * from seriefloats", conn);
			NpgsqlDataAdapter ad = new NpgsqlDataAdapter(com);
			if(conn != null && conn.State == ConnectionState.Open){conn.Close();}
			else{conn.Open();}
			DataTable dt = new DataTable();
			ad.Fill(dt);
			NpgsqlDataReader dRead = com.ExecuteReader();
			while (dRead.Read())
			{
				for(int i = 0; i < dRead.FieldCount; i++)
				{
					Console.Write("{0} \t \n", dRead[i].ToString());
				}
			}
			return 0;
		}

/*
		public IQueryable<Seriefloat> seriefloats_by_id([Service] Demo_Context context, [ID(nameof(User))]int id) 
		{
			return context.users.Where(u => u.id == id);
		}
		*/

	}
}
