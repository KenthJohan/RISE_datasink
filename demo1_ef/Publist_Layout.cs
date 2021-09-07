using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Buffers.Binary;
using Serilog;


namespace Demo
{

	static class Publist_Layout
	{
		private static readonly Serilog.ILogger log = Log.ForContext(typeof(Publist_Layout));
		public static Dictionary<int, List<Memloc>> dict_memlocs = new Dictionary<int, List<Memloc>>();

		private static HashSet<WebSocket> subs = new HashSet<WebSocket>();


		public static void load(Demo_Context context, int layout_id)
		{
			List<Memloc> memlocs = context.memlocs.Where(x => x.layout_id == layout_id).ToList();
			dict_memlocs.Add(layout_id, memlocs);
		}
		public static void load(Demo_Context context)
		{
			List<Layout> layouts = context.layouts.ToList();
			foreach (Layout layout in layouts)
			{
				load(context, layout.id);
			}
		}


		// |byte0|byte1|byte2|byte3|byte4|byte5|byte6|byte7|etc    |
		// |        layout_id      |                               |
		// https://github.com/npgsql/npgsql/issues/2779
		// High frequency function, must be perfomant.
		// 1000 sensors * 100hz
		// 1Mhz = 100Hz * (10000 floats)
		public static void recv(ReadOnlySpan<byte> buffer)
		{
			//"INSERT INTO floatvals (producer_id,time,value) VALUES (@producer_id, @time, @value)"
			using var importer = DB.connection.BeginBinaryImport("COPY floatvals (producer_id, value) FROM STDIN (FORMAT binary)");
			int layout_id = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(0));
			foreach(Memloc memloc in dict_memlocs[layout_id])
			{
				//Debug.Assert(memloc.byteoffset > 3);
				importer.StartRow();
				//DateTime time = DateTime.Now;
				float value = BinaryPrimitives.ReadSingleBigEndian(buffer.Slice(memloc.byteoffset));
				importer.Write(memloc.producer_id);
				//importer.Write(time);
				importer.Write(value);
				//importer.Write(0.0f);
				//importer.Write(0.0f);
				Sublist_Producer.publish(memloc.producer_id, DateTime.Now, value);
			}
			ulong r = importer.Complete();
			log.Information("importer.Complete(): {r}", r);
		}



		public static async Task accept(WebSocket ws)
		{
			subs.Add(ws);
			log.Information("Adding new websocket {ws} to Dictionary. Websockets count {count}", ws.GetHashCode(), subs.Count);
			var buffer = new byte[1024 * 4];
			WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			while (!result.CloseStatus.HasValue)
			{
				recv(buffer);
				result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}
			await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
			subs.Remove(ws);
			log.Information("WebSocket {ws} closed. WebSocket count {count}", ws.GetHashCode(), subs.Count);
		}





	}
}



