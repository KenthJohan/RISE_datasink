using System;
using System.IO;
using System.Text;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Demo
{
	public static class Datasink
	{
		private static readonly ILogger log = Log.ForContext(typeof(Datasink));
		public static List<WebSocket> socks { get; set; } = new List<WebSocket>();



		public static void send(WebSocket sock, string buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationTokene)
		{
			var bytes = Encoding.ASCII.GetBytes(buffer);
			var arraySegment = new ArraySegment<byte>(bytes);
			sock.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
		}

		public static void add(Demo_Context context, Floatval state)
		{
			context.floatvals.Add(state);
			context.SaveChanges();
			log.Information("Added new value {@Floatval}", state);
			foreach (WebSocket sock in socks)
			{
				switch(sock.State)
				{
					case WebSocketState.Open:
						string json = JsonSerializer.Serialize(state);
						send(sock, json, WebSocketMessageType.Text, true, CancellationToken.None);
						break;
					case WebSocketState.Closed:
						log.Information("ws Closed");
						break;
					case WebSocketState.CloseReceived:
						log.Information("ws CloseReceived");
						break;
				}
			}
		}

	};
}




