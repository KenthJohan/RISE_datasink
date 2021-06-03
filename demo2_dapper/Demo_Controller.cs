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
		public Demo_Controller()
		{
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
				await Demo_Sink.accept_ws(c);
			}
			else
			{
				c.Response.StatusCode = 400;
				await c.Response.WriteAsync("WebSocket does not work :(");
			}
		}




















	}
}





