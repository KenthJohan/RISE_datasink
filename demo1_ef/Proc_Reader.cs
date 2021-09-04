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

		public static void read_stat(string path, ref Proc info)
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
					info.cpu = new long[words.Length-1];
					strv_to_longv(words, 1, info.cpu, 0);
					break;
				case "page":
					info.page = new long[words.Length-1];
					strv_to_longv(words, 1, info.page, 0);
					break;
				case "swap":
					info.swap = new long[words.Length-1];
					strv_to_longv(words, 1, info.swap, 0);
					break;
				case "intr":
					info.intr = long.TryParse(words[1], out v) ? v : info.intr;
					break;
				case "ctxt":
					info.ctxt = long.TryParse(words[1], out v) ? v : info.ctxt;
					break;
				case "btime":
					info.btime = long.TryParse(words[1], out v) ? v : info.btime;
					break;
				}
			}
		}

		public static void read_meminfo(string path, ref Proc info)
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


		public static void test()
		{
			Proc info = new Proc{};

			/*
			/proc/stat
			kernel/system statistics.  Varies with architecture.
			*/
			read_stat("/proc/stat", ref info);

			/*
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
			read_meminfo("/proc/meminfo", ref info);
		}

	}
}
