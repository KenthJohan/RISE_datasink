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