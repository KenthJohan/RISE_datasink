using System;
using System.Threading;
using Serilog;

namespace Demo
{
	public static class Monitor
	{

		public static void worker()
		{
			Proc_Stat stat0 = new Proc_Stat{};
			Proc_Stat stat1 = new Proc_Stat{};
			Proc_Meminfo meminfo = new Proc_Meminfo{};
			while(true)
			{
				Proc_Reader.read_meminfo("/proc/meminfo", ref meminfo);
				Proc_Reader.read_stat("/proc/stat", ref stat0);
				Thread.Sleep(1000);
				Proc_Reader.read_stat("/proc/stat", ref stat1);
				float load = (float)Proc_Reader.calc_load(stat0.cpu, stat1.cpu);
				Sublist_Producer.publish(13, DateTime.Now, load);
				//Log.Information("load: {load}", load);
				//Log.Information("{@Proc_Stat}", stat0);
				//Log.Information("{@Proc_Meminfo}", meminfo);
			}
		}

		public static void init()
		{
			Thread t = new Thread(new ThreadStart(worker));
			t.Start();
		}


	}
}