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
	static public class Hub
	{
		static public HashSet<int> enable_mqtt = new HashSet<int>();
		static public HashSet<int> enable_reqget = new HashSet<int>();
		static public HashSet<int> enable_websock = new HashSet<int>();
		static public HashSet<int> enable_reqjson = new HashSet<int>();
		static public HashSet<int> enable_tcp = new HashSet<int>();
		static public HashSet<int> enable_udp = new HashSet<int>();
		static public HashSet<int> enable_storage = new HashSet<int>();
		public static Dictionary<int, List<Memloc>> dict_memlocs = new Dictionary<int, List<Memloc>>();

		public static void load(Demo_Context context)
		{
			List<Layout> layouts = context.layouts.ToList();
			foreach (Layout layout in layouts)
			{
				List<Memloc> memlocs = context.memlocs.Where(x => x.layout_id == layout.id).ToList();
				dict_memlocs.Add(layout.id, memlocs);
			}
			
			List<Producer> producers = context.producers.ToList();
			foreach (Producer p in producers)
			{
				if (p.enable_mqtt) {enable_mqtt.Add(p.id);}
				if (p.enable_reqget) {enable_reqget.Add(p.id);}
				if (p.enable_websock) {enable_websock.Add(p.id);}
				if (p.enable_reqjson) {enable_reqjson.Add(p.id);}
				if (p.enable_tcp) {enable_tcp.Add(p.id);}
				if (p.enable_udp) {enable_udp.Add(p.id);}
				if (p.enable_storage) {enable_storage.Add(p.id);}
			}
		}

		static public void send(int producer_id, float value)
		{
			if(enable_storage.Contains(producer_id))
			{
				DB.insert_value(producer_id, value);
			}
			if(enable_websock.Contains(producer_id))
			{
				Sublist_Producer.publish(producer_id, DateTime.Now, value);
			}
		}




	}
}