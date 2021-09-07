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
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Demo
{

	public static class Sublist_Producer
	{
		private static readonly Serilog.ILogger log = Log.ForContext(typeof(Sublist_Producer));

		// Used for O(1) complexity
		// [websocket][producer_id]
		private static Dictionary<WebSocket, HashSet<int>> subs0 = new Dictionary<WebSocket, HashSet<int>>();

		// Used for O(1) complexity
		// [producer_id][websocket]
		private static Dictionary<int, HashSet<WebSocket>> subs1 = new Dictionary<int, HashSet<WebSocket>>();



		private static void send(WebSocket sock, string buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationTokene)
		{
			var bytes = Encoding.ASCII.GetBytes(buffer);
			var arraySegment = new ArraySegment<byte>(bytes);
			sock.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
		}


		private static void send(WebSocket sock, int producer_id, DateTime time, float value)
		{
			Msg_Float message = new Msg_Float { };
			message.producer_id = producer_id;
			message.time = DateTimeJavaScript.ToJavaScriptMilliseconds(time);
			message.value = value;
			sock.SendAsync(message.GetArraySegment(), WebSocketMessageType.Binary, true, CancellationToken.None);
		}

		//High frequency function.
		//This must be perfomant.
		public static void publish(int producer_id, DateTime time, float value)
		{
			log.Information("Publish {producer_id} {time} {value}", producer_id, time, value);
			if (subs1.ContainsKey(producer_id))
			{
				foreach (WebSocket ws in subs1[producer_id])
				{
					switch (ws.State)
					{
						case WebSocketState.Open:
							send(ws, producer_id, time, value);
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
		}


		//https://csharp.hotexamples.com/examples/System.Net.WebSockets/WebSocket/ReceiveAsync/php-websocket-receiveasync-method-examples.html
		//https://csharp.hotexamples.com/site/file?hash=0xf067416689828e386db477c574790970910f8d1e74cad5bae935fe4eb22e6f75&fullName=src/Microsoft.AspNet.SignalR.Hosting.AspNet45/WebSocketMessageReader.cs&project=TerenceLewis/SignalR
		public static async Task accept(WebSocket ws)
		{
			subs0.Add(ws, new HashSet<int>());
			log.Information("Adding new websocket {ws} to Dictionary. Websockets count {count}", ws.GetHashCode(), subs0.Count);
			var buffer = new byte[1024 * 4];
			WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			while (!result.CloseStatus.HasValue)
			{
				Msg_Sub message = Msg_Sub.FromBytes(buffer);
				if (subs1.ContainsKey(message.producer_id) == false)
				{
					subs1[message.producer_id] = new HashSet<WebSocket>();
				}

				switch(message.mode)
				{
					case 0:
						log.Information("WebSocket {ws} unsubscribes {producer_id}", ws.GetHashCode(), message.producer_id);
						subs0[ws].Remove(message.producer_id);
						subs1[message.producer_id].Remove(ws);
						break;
					case 1:
						log.Information("WebSocket {ws} subscribes {producer_id}", ws.GetHashCode(), message.producer_id);
						subs0[ws].Add(message.producer_id);
						subs1[message.producer_id].Add(ws);
						break;
					default:
						log.Warning("This mode {mode} is not implemented", message.mode);
						break;
				}


				//log.Information("WebSocket {ws} loop. WebSocket count {count}. {@Subscription_Message}", ws.GetHashCode(), subscriptions.Count, message);
				//Echo websocket message:
				//await ws.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
				result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}
			await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
			subs0.Remove(ws);
			log.Information("WebSocket {ws} closed. WebSocket count {count}", ws.GetHashCode(), subs0.Count);
		}


	};
}




