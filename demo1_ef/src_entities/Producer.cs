using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Demo
{
	public class Producer
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public string name { get; set; }
		public virtual ICollection<Floatval> floatvals { get; set; }
		public virtual ICollection<Memloc> memlocs { get; set; }

		public bool enable_mqtt { get; set; } = false;
		public bool enable_reqget { get; set; } = false;
		public bool enable_websock { get; set; } = false;
		public bool enable_reqjson { get; set; } = false;
		public bool enable_tcp { get; set; } = false;
		public bool enable_udp { get; set; } = false;
		public bool enable_storage { get; set; } = false;

		public int device_id { get; set; } = 1;
		public int project_id { get; set; } = 1;
		public int location_id { get; set; } = 1;
		public int quantity_id { get; set; } = 1;
		public virtual Device device { get; set; }
		public virtual Project project { get; set; }
		public virtual Location location { get; set; }
		public virtual Quantity quantity { get; set; }
	}
}