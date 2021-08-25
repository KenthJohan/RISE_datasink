using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Security.Claims;
using System.IO.Compression;
using System.Linq;
using System.Net;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Npgsql;

using System.Net.WebSockets;
using Serilog;


//https://stackoverflow.com/questions/33940903/multiple-response-writeasync-calls
//https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/intro?view=aspnetcore-5.0
//https://developers.redhat.com/blog/2020/01/29/api-login-and-jwt-token-generation-using-keycloak/

namespace Demo
{
	public class Demo_Controller : Controller
	{
		private readonly ILogger log = Log.ForContext<Demo_Controller>();
		private readonly Demo_Context _context;
		public Demo_Controller(Demo_Context context)
		{
			_context = context;
		}

		[HttpGet("/table")]
		public IActionResult tables()
		{
			var t = _context.Database.GetDbConnection().GetSchema("Tables");
			return Content(HTML.create_table(t), "text/html");
		}


		[HttpGet("/table/{tablename}")]
		public IActionResult table(String tablename)
		{
			var t = DB.export_table(_context, "SELECT * FROM \"" + tablename + "\"");
			return Content(HTML.create_table(t), "text/html");
		}


		[HttpGet("/column")]
		public IActionResult column()
		{
			string fields = "table_name,column_name,is_nullable,data_type,character_octet_length,udt_name,is_identity";
			string db = Environment.GetEnvironmentVariable("POSTGRES_DB");
			var t = DB.export_table(_context, "SELECT " + fields + " FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = '" + db + "' and table_schema = 'public' ORDER BY table_name");
			//var t = DB.export_table(_context, "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = 'postgres' and table_schema = 'public' ORDER BY table_name");
			return Content(HTML.create_table(t), "text/html");
		}

		[HttpGet("/test_npgsql_command")]
		public IActionResult test_npgsql_command()
		{
			using var con = new NpgsqlConnection(DB.npgsql_connection);
			con.Open();
			using var cmd = new NpgsqlCommand("SELECT value from @colname where serie_id = @serie_id", con);
			cmd.Parameters.AddWithValue("serie_id", 1);
			cmd.Parameters.AddWithValue("colname", "seriefloat");
			//cmd.Prepare();
			using NpgsqlDataReader rdr = cmd.ExecuteReader();
			while (rdr.Read())
			{
				Console.WriteLine("{0}", rdr.GetFloat(0));
			}
			return Content("paratest", "text/html");
		}



		[HttpGet("/wslog")]
		public async Task wslog()
		{
			var c = ControllerContext.HttpContext;
			/*
			log.Information("is_siteadmin: {is_siteadmin}", _context.is_siteadmin());
			log.Information("IsWebSocketRequest: {IsWebSocketRequest}", c.WebSockets.IsWebSocketRequest);
			log.Information("WebSocketRequestedProtocols: {WebSocketRequestedProtocols}", c.WebSockets.WebSocketRequestedProtocols);
			log.Information("IHeaderDictionary: {@IHeaderDictionary}", c.Request.Headers);
			*/
			if (c.WebSockets.IsWebSocketRequest)
			{
				await Serilog_Websocket_Sink.accept_ws(c);
			}
			else
			{
				c.Response.StatusCode = 400;
				await c.Response.WriteAsync("WebSocket does not work :(");
			}
		}






		[HttpGet("/ws/sub")]
		public async Task ws_sub()
		{
			var c = ControllerContext.HttpContext;
			if (c.WebSockets.IsWebSocketRequest)
			{
				await Subs.accept(await c.WebSockets.AcceptWebSocketAsync());
			}
			else
			{
				c.Response.StatusCode = 400;
				await c.Response.WriteAsync("WebSocket does not work :(");
			}
		}


		[HttpGet("/ws/pub")]
		public async Task ws_pub()
		{
			var c = ControllerContext.HttpContext;
			if (c.WebSockets.IsWebSocketRequest)
			{
				await Pubs.accept(await c.WebSockets.AcceptWebSocketAsync());
			}
			else
			{
				c.Response.StatusCode = 400;
				await c.Response.WriteAsync("WebSocket does not work :(");
			}
		}










	}
}





