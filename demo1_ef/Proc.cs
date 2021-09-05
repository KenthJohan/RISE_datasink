using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;



//https://github.com/dotnet/orleans/blob/main/src/TelemetryConsumers/Orleans.TelemetryConsumers.Linux/LinuxEnvironmentStatistics.cs
namespace Demo
{


	/*
	https://man7.org/linux/man-pages/man5/proc.5.html
	/proc/meminfo
	This file reports statistics about memory usage on the
	system.  It is used by free(1) to report the amount of
	free and used memory (both physical and swap) on the
	system as well as the shared memory and buffers used by
	the kernel.  Each line of the file consists of a parameter
	name, followed by a colon, the value of the parameter, and
	an option unit of measurement (e.g., "kB").  The list
	below describes the parameter names and the format
	specifier required to read the field value.  Except as
	noted below, all of the fields have been present since at
	least Linux 2.6.0.  Some fields are displayed only if the
	kernel was configured with various options; those
	dependencies are noted in the list.
	*/
	public class Proc_Meminfo
	{


		public long MemTotal { get; set; }
		//Total usable RAM (i.e., physical RAM minus a few reserved bits and the kernel binary code).

		public long MemFree { get; set; }
		//The sum of LowFree+HighFree.

		public long MemAvailable { get; set; }
		//An estimate of how much memory is available for starting new applications, without swapping.

		public long Buffers { get; set; }
		//Relatively temporary storage for raw disk blocks that shouldn't get tremendously large (20 MB or so).

		public long Cached { get; set; }
		//In-memory cache for files read from the disk (the page cache).  Doesn't include SwapCached.

		public long HighTotal { get; set; }
		//Total amount of highmem.
		//Highmem is all memory above ~860 MB of physical memory.
		//Highmem areas are for use by user-space programs, or for the page cache.
		//The kernel must use tricks to access this memory, making it slower to access than lowmem.

		public long HighFree { get; set; }
		//Amount of free highmem.

		public long LowTotal { get; set; }
		//Total amount of lowmem.
		//Lowmem is memory which can be used for everything that highmem can be used for, 
		//but it is also available for the kernel's use for its own data structures.
		//Among many other things, it is where everything from Slab is allocated. 
		//Bad things happen when you're out of lowmem.

		public long LowFree { get; set; }
		//Amount of free lowmem.

		public long SwapTotal { get; set; }
		//Total amount of swap space available.

		public long SwapFree { get; set; }
		//Amount of swap space that is currently unused.


	}


	public enum Statcpu : int
	{
		user, //Time spent in user mode.
		nice, //Time spent in user mode with low priority (nice).
		system, //Time spent in system mode.
		idle, //Time spent in the idle task. This value should be USER_HZ times the second entry in the /proc/uptime pseudo-file.
		iowait, //Time waiting for I/O to complete. This value is not reliable
		irq, //Time servicing interrupts.
		softirq, //softirq
		steal, //Stolen time, which is the time spent in other operating systems when running in a virtualized environment
		guest, //Time spent running a virtual CPU for guest operating systems under the control of the Linux kernel.
		guest_nice, //Time spent running a niced guest (virtual CPU for guest operating systems under the control of the Linux kernel).
	};

	/*
	/proc/stat
	kernel/system statistics.  Varies with architecture.
	*/
	public class Proc_Stat
	{
		/*
		The amount of time, measured in units of USER_HZ
		(1/100ths of a second on most architectures, use
		sysconf(_SC_CLK_TCK) to obtain the right value),
		that the system ("cpu" line) or the specific CPU
		("cpuN" line) spent in various states:
		*/
		public ulong[] cpu { get; set; }

		//The number of pages the system paged in and the number that were paged out (from disk).
		public ulong[] page { get; set; }

		//The number of swap pages that have been brought in and out.
		public ulong[] swap { get; set; }
		public long intr { get; set; }
		public long ctxt { get; set; }
		public long btime { get; set; }
	}

	public static class Proc_Reader
	{
		public static int strv_to_longv(string[] src, int src_offset, long[] dst, int dst_offset)
		{
			int i = src_offset;
			int j = dst_offset;
			int ni = src.Length;
			int nj = dst.Length;
			while ((i < ni) && (j < nj))
			{
				j += long.TryParse(src[i], out dst[j]) ? 1 : 0;
				i++;
			}
			return j;
		}

		public static int strv_to_ulongv(string[] src, int src_offset, ulong[] dst, int dst_offset)
		{
			int i = src_offset;
			int j = dst_offset;
			int ni = src.Length;
			int nj = dst.Length;
			while ((i < ni) && (j < nj))
			{
				j += ulong.TryParse(src[i], out dst[j]) ? 1 : 0;
				i++;
			}
			return j;
		}

		public static void read_stat(string path, ref Proc_Stat stat)
		{
			using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 512, FileOptions.SequentialScan);
			using var r = new StreamReader(fs, Encoding.ASCII);
			string line;
			long v;
			while ((line = r.ReadLine()) != null)
			{
				string[] words = line.Split(' ');
				switch (words[0])
				{
				case "cpu":
					stat.cpu = new ulong[10];//last enum value of Statcpu is 9
					strv_to_ulongv(words, 1, stat.cpu, 0);
					break;
				case "page":
					stat.page = new ulong[words.Length-1];
					strv_to_ulongv(words, 1, stat.page, 0);
					break;
				case "swap":
					stat.swap = new ulong[words.Length-1];
					strv_to_ulongv(words, 1, stat.swap, 0);
					break;
				case "intr":
					stat.intr = long.TryParse(words[1], out v) ? v : stat.intr;
					break;
				case "ctxt":
					stat.ctxt = long.TryParse(words[1], out v) ? v : stat.ctxt;
					break;
				case "btime":
					stat.btime = long.TryParse(words[1], out v) ? v : stat.btime;
					break;
				}
			}
		}

		public static void read_meminfo(string path, ref Proc_Meminfo info)
		{
			using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 512, FileOptions.SequentialScan);
			using var r = new StreamReader(fs, Encoding.ASCII);
			string line;
			long v;
			while ((line = r.ReadLine()) != null)
			{
				string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				switch (words[0])
				{
				case "MemTotal:":
					info.MemTotal = long.TryParse(words[1], out v) ? v : info.MemTotal;
					break;
				case "MemFree:":
					info.MemFree = long.TryParse(words[1], out v) ? v : info.MemFree;
					break;
				case "HighTotal:":
					info.HighTotal = long.TryParse(words[1], out v) ? v : info.HighTotal;
					break;
				case "HighFree:":
					info.HighFree = long.TryParse(words[1], out v) ? v : info.HighFree;
					break;
				case "LowTotal:":
					info.LowTotal = long.TryParse(words[1], out v) ? v : info.LowTotal;
					break;
				case "LowFree:":
					info.LowFree = long.TryParse(words[1], out v) ? v : info.LowFree;
					break;
				case "SwapTotal:":
					info.SwapTotal = long.TryParse(words[1], out v) ? v : info.SwapTotal;
					break;
				case "SwapFree:":
					info.SwapFree = long.TryParse(words[1], out v) ? v : info.SwapFree;
					break;
				}
			}
		}

		//https://www.kgoettler.com/post/proc-stat/
		//https://stackoverflow.com/questions/23367857/accurate-calculation-of-cpu-usage-given-in-percentage-in-linux
		public static double calc_load(ulong[] prev, ulong[] cur)
		{
			ulong idle_prev = prev[(int)Statcpu.idle] + prev[(int)Statcpu.iowait];
			ulong idle_cur = cur[(int)Statcpu.idle] + cur[(int)Statcpu.iowait];
			ulong nidle_prev = prev[(int)Statcpu.user] + prev[(int)Statcpu.nice]+ prev[(int)Statcpu.system] + prev[(int)Statcpu.softirq];
			ulong nidle_cur = cur[(int)Statcpu.user] + cur[(int)Statcpu.nice]+ cur[(int)Statcpu.system] + cur[(int)Statcpu.softirq];
			ulong total_prev = idle_prev + nidle_prev;
			ulong total_cur = idle_cur + nidle_cur;
			double totald = (double) total_cur - (double) total_prev;
			double idled = (double) idle_cur - (double) idle_prev;
			double cpu_perc = (1000 * (totald - idled) / totald + 1) / 10;
			return cpu_perc;
		}


		public static void test()
		{
			Proc_Stat stat = new Proc_Stat{};
			Proc_Meminfo meminfo = new Proc_Meminfo{};
			read_stat("/proc/stat", ref stat);
			read_meminfo("/proc/meminfo", ref meminfo);
		}

	}
}
