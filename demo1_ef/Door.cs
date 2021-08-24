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
	public class Layout
	{
		public int id;
	}

	public class Memloc
	{
		public int layout_id;
		public int producer_id;
		public int byteoffset;
	};

	class Door
	{

		private static void insert(NpgsqlCommand cmd, List<Memloc> memlocs, ReadOnlySpan<byte> buffer)
		{
			foreach(var memloc in memlocs)
			{
				float value = BinaryPrimitives.ReadSingleBigEndian(buffer.Slice(memloc.byteoffset));
				cmd.Parameters.AddWithValue("producer_id", memloc.producer_id);
				cmd.Parameters.AddWithValue("value", value);
			}
			int r = cmd.ExecuteNonQuery();
			Debug.Assert(r == memlocs.Count);
		}



		private static void insert(NpgsqlCommand cmd, Dictionary<int, List<Memloc>> layouts, ReadOnlySpan<byte> buffer)
		{
			int layout_id = BinaryPrimitives.ReadInt32BigEndian(buffer.Slice(0));
			insert(cmd, layouts[layout_id], buffer);
		}



	};
}




