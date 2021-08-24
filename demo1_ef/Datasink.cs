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

	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public unsafe struct Float_Message
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
		[FieldOffset(0)]
		public fixed byte data[16];
		[FieldOffset(0)] public Int32 producer_id;
		[FieldOffset(4)] public Int64 time;
		[FieldOffset(12)] public float value;
		public unsafe static byte[] GetBytes(Float_Message value)
		{
			byte[] arr = new byte[16];
			Marshal.Copy((IntPtr)value.data, arr, 0, 16);
			return arr;
		}
		public ArraySegment<byte> GetArraySegment()
		{
			return new ArraySegment<byte>(Float_Message.GetBytes(this));
		}
	}

	[StructLayout(LayoutKind.Explicit, Pack = 1)]
	public unsafe struct Subscription_Message
	{
		[FieldOffset(0)]
		public fixed byte data[4];

		[FieldOffset(0)]
		public Int32 producer_id;

		public unsafe static Subscription_Message FromBytes(byte[] data)
		{
			Subscription_Message msg = new Subscription_Message{};
			Marshal.Copy(data, 0, (IntPtr)msg.data, Marshal.SizeOf(typeof(Subscription_Message)));
			return msg;
		}

		public Subscription_Message FromArraySegment(ArraySegment<byte> data)
		{
			return FromBytes(data.Array);
		}

	}


	//https://stackoverflow.com/questions/2404247/datetime-to-javascript-date
	public static class DateTimeJavaScript
	{
		private static readonly long DatetimeMinTimeTicks =
		   (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

		public static long ToJavaScriptMilliseconds(this DateTime dt)
		{
			return (long)((dt.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
		}
	}


	public static class Datasink
	{
		private static readonly ILogger log = Log.ForContext(typeof(Datasink));
		public static Dictionary<WebSocket, HashSet<int>> subscriptions = new Dictionary<WebSocket, HashSet<int>>();



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
			foreach (WebSocket sock in subscriptions.Keys)
			{
				switch (sock.State)
				{
					case WebSocketState.Open:
						//Should be O(1):
						if (subscriptions[sock].Contains(state.producer_id))
						{
							Float_Message message = new Float_Message { };
							message.producer_id = state.producer_id;
							//message.time = state.time.ToBinary();
							message.time = DateTimeJavaScript.ToJavaScriptMilliseconds(state.time);
							message.value = state.value;
							sock.SendAsync(message.GetArraySegment(), WebSocketMessageType.Binary, true, CancellationToken.None);
							//string json = JsonSerializer.Serialize(state);
							//send(sock, json, WebSocketMessageType.Text, true, CancellationToken.None);
							//subscriptions[sock].Remove(state.producer_id);
						}
						else
						{
							//subscriptions[sock].Add(state.producer_id);
						}
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


		//https://csharp.hotexamples.com/examples/System.Net.WebSockets/WebSocket/ReceiveAsync/php-websocket-receiveasync-method-examples.html
		//https://csharp.hotexamples.com/site/file?hash=0xf067416689828e386db477c574790970910f8d1e74cad5bae935fe4eb22e6f75&fullName=src/Microsoft.AspNet.SignalR.Hosting.AspNet45/WebSocketMessageReader.cs&project=TerenceLewis/SignalR
		public static async Task accept(WebSocket ws)
		{
			subscriptions.Add(ws, new HashSet<int>());
			log.Information("Adding new websocket {ws} to Dictionary. Websockets count {count}", ws.GetHashCode(), subscriptions.Count);
			var buffer = new byte[1024 * 4];
			WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			while (!result.CloseStatus.HasValue)
			{
				Subscription_Message message = Subscription_Message.FromBytes(buffer);
				if (subscriptions[ws].Contains(message.producer_id))
				{
					log.Information("WebSocket {ws} unsubscribes {producer_id}", ws.GetHashCode(), message.producer_id);
					subscriptions[ws].Remove(message.producer_id);
				}
				else
				{
					log.Information("WebSocket {ws} subscribes {producer_id}", ws.GetHashCode(), message.producer_id);
					subscriptions[ws].Add(message.producer_id);
				}
				//log.Information("WebSocket {ws} loop. WebSocket count {count}. {@Subscription_Message}", ws.GetHashCode(), subscriptions.Count, message);
				//Echo websocket message:
				//await ws.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
				result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}
			await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
			subscriptions.Remove(ws);
			log.Information("WebSocket {ws} closed. WebSocket count {count}", ws.GetHashCode(), subscriptions.Count);
		}


	};
}




