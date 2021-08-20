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
		public static Dictionary<int, List<WebSocket>> socks = new Dictionary<int, List<WebSocket>>();



		public static void send(WebSocket sock, string buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationTokene)
		{
			var bytes = Encoding.ASCII.GetBytes(buffer);
			var arraySegment = new ArraySegment<byte>(bytes);
			sock.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
		}


		//High frequency function.
		//This must be perfomant.
		public static void add(Demo_Context context, Floatval state)
		{
			context.floatvals.Add(state);
			context.SaveChanges();
			log.Information("Producer {producer_id} adds value {@Floatval}", state.producer_id, state);
			foreach (WebSocket sock in socks[state.producer_id])
			{
				switch(sock.State)
				{
					case WebSocketState.Open:
						//TODO: Send binary values instead of json:
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


		public static async Task accept(int producer_id, WebSocket ws)
		{
			if (socks.ContainsKey(producer_id) == false)
			{
				socks[producer_id] = new List<WebSocket>{};
			}
			socks[producer_id].Add(ws);
			log.Information("Adding new websocket {ws} to list {producer_id}. Websockets count {count}", ws.GetHashCode(), producer_id, socks[producer_id].Count);
			var buffer = new byte[1024 * 4];
			WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			while (!result.CloseStatus.HasValue)
			{
				log.Information("WebSocket {ws} loop. WebSocket count {count}", ws.GetHashCode(), socks[producer_id].Count);
				//Echo websocket message:
				await ws.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
				result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}
			await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
			socks[producer_id].Remove(ws);
			log.Information("WebSocket {ws} closed. WebSocket count {count}", ws.GetHashCode(), socks[producer_id].Count);
		}


	};
}




