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
using NpgsqlTypes;

using System.Net.WebSockets;
using Serilog;
using System.Buffers.Binary;
using System.Diagnostics;


namespace Demo
{




	class Door
	{

		private static void msg_receive(NpgsqlCommand cmd, List<Memloc> memlocs, ReadOnlySpan<byte> buffer)
		{
			foreach(var memloc in memlocs)
			{
				//Debug.Assert(memloc.byteoffset > 3);
				Floatval v = new Floatval{};
				v.value = BinaryPrimitives.ReadSingleBigEndian(buffer.Slice(memloc.byteoffset));
				v.producer_id = memloc.producer_id;
				v.time = DateTime.Now;
				cmd.Parameters.AddWithValue("producer_id", v.producer_id);
				cmd.Parameters.AddWithValue("time", v.time);
				cmd.Parameters.AddWithValue("value", v.value);
				Websock_Subs.publish(v);
			}
			int r = cmd.ExecuteNonQuery();
			Debug.Assert(r == memlocs.Count);
		}


		// |byte0|byte1|byte2|byte3|byte4|byte5|byte6|byte7|byte8|
		// |        layout_id      |                             |
		private static void msg_receive(NpgsqlCommand cmd, Dictionary<int, List<Memloc>> layouts, ReadOnlySpan<byte> buffer)
		{
			int layout_id = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(0));
			msg_receive(cmd, layouts[layout_id], buffer);
		}



	};
}




