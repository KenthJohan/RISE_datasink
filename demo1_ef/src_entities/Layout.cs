using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Demo
{

	public class Layout
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public string name { get; set; }
		public bool enable_websock { get; set; } = false;
		public bool enable_mqtt { get; set; } = false;
		public bool enable_tcp { get; set; } = false;
		public bool enable_udp { get; set; } = false;
		public bool enable_storage { get; set; } = false;
		public virtual ICollection<Memloc> memlocs { get; set; }
	}
}