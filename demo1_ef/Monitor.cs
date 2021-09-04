using System;
using System.Threading;
using Serilog;

namespace Demo
{
	public static class Monitor
	{

		public static void ThreadProc()
		{
			Proc proc = new Proc{};
			while(true)
			{
				Proc_Reader.read_stat("/proc/stat", ref proc);
				Proc_Reader.read_meminfo("/proc/meminfo", ref proc);
				Log.Information("{@Proc}", proc);
				Thread.Sleep(1000);
			}
		}

		public static void init()
		{
			Thread t = new Thread(new ThreadStart(ThreadProc));
			t.Start();
		}


	}
}