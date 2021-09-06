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
		public float times_20(Floatval s)
		{
			return s.value * 20;
		}
	}




	[ExtendObjectType("Query")]
	public class Floatval_Query
	{
		private readonly Serilog.ILogger log = Log.ForContext<Floatval_Query>();

		[HotChocolate.Data.UseProjection]
		[HotChocolate.Data.UseFiltering]
		[HotChocolate.Data.UseSorting]
		public IQueryable<Floatval> floatvals([Service] Demo_Context context)
		{
			log.Information("Fetching seriefloats");
			return context.floatvals;
		}

		public Serie_Float_Chunk floatvals_chunk1([Service] Demo_Context context, int producer_id)
		{
			Serie_Float_Chunk chunk = new Serie_Float_Chunk{};
			var q = context.floatvals.Where(t => t.producer_id == producer_id);
			chunk.time = q.Select(t => t.time).ToList();
			chunk.value = q.Select(t => t.value).ToList();
			chunk.longitude = q.Select(t => t.longitude).ToList();
			chunk.latitude = q.Select(t => t.latitude).ToList();
			return chunk;
		}
















		/*
		public Serie_Float_Chunk floatvals_chunk2([Service] Demo_Context context, int producer_id)
		{
			NpgsqlConnection conn = new NpgsqlConnection(DB.npgsql_connection);
			NpgsqlCommand cmd = new NpgsqlCommand("SELECT time,value,longitude,latitude FROM floatvals WHERE @producer_id", conn);
			cmd.Parameters.AddWithValue("producer_id", producer_id);
			cmd.Prepare();
			//NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
			//DataTable table = new DataTable();
			//da.Fill(table);
			NpgsqlDataReader reader = cmd.ExecuteReader();
			Serie_Float_Chunk chunk = new Serie_Float_Chunk{};
			while (reader.Read())
			{
				chunk.time.Add(reader.GetDateTime(0));
				chunk.value.Add(reader.GetFloat(1));
				chunk.longitude.Add(reader.GetFloat(2));
				chunk.latitude.Add(reader.GetFloat(3));
				//for(int i = 0; i < reader.FieldCount; i++)
				{
					//Console.Write("{0} \t \n", reader[i].ToString());
				}
			}
			if(conn.State == ConnectionState.Open){conn.Close();}else{conn.Open();}
			return chunk;
		}
		*/

	}
}
