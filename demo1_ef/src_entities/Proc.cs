using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;


namespace Demo
{

	//https://man7.org/linux/man-pages/man5/proc.5.html
	public class Proc
	{
		public long MemTotal { get; set; }
		public long MemFree { get; set; }
		public long HighTotal { get; set; }
		public long HighFree { get; set; }
		public long LowTotal { get; set; }
		public long LowFree { get; set; }
		public long SwapTotal { get; set; }
		public long SwapFree { get; set; }


		/*
		The amount of time, measured in units of USER_HZ
		(1/100ths of a second on most architectures, use
		sysconf(_SC_CLK_TCK) to obtain the right value),
		that the system ("cpu" line) or the specific CPU
		("cpuN" line) spent in various states:

		user   (1) Time spent in user mode.

		nice   (2) Time spent in user mode with low
		priority (nice).

		system (3) Time spent in system mode.

		idle   (4) Time spent in the idle task.  This value
		should be USER_HZ times the second entry in
		the /proc/uptime pseudo-file.

		iowait (since Linux 2.5.41)
		(5) Time waiting for I/O to complete.  This
		value is not reliable, for the following
		reasons:

		1. The CPU will not wait for I/O to
			complete; iowait is the time that a task
			is waiting for I/O to complete.  When a
			CPU goes into idle state for outstanding
			task I/O, another task will be scheduled
			on this CPU.

		2. On a multi-core CPU, the task waiting for
			I/O to complete is not running on any
			CPU, so the iowait of each CPU is
			difficult to calculate.

		3. The value in this field may decrease in
			certain conditions.

		irq (since Linux 2.6.0)
		(6) Time servicing interrupts.

		softirq (since Linux 2.6.0)
		(7) Time servicing softirqs.

		steal (since Linux 2.6.11)
		(8) Stolen time, which is the time spent in
		other operating systems when running in a
		virtualized environment

		guest (since Linux 2.6.24)
		(9) Time spent running a virtual CPU for
		guest operating systems under the control of
		the Linux kernel.

		guest_nice (since Linux 2.6.33)
		(10) Time spent running a niced guest
		(virtual CPU for guest operating systems
		under the control of the Linux kernel).
		*/
		public long[] cpu { get; set; }

		/*
		The number of pages the system paged in and the
		number that were paged out (from disk).
		*/
		public long[] page { get; set; }

		/*
		The number of swap pages that have been brought in
		and out.
		*/
		public long[] swap { get; set; }
		public long intr { get; set; }
		public long ctxt { get; set; }
		public long btime { get; set; }
	}
}
