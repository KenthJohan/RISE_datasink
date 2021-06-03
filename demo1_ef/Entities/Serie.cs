using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Demo
{
	public class Serie
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public string name { get; set; }
		
		//Low frequency
		public int device_id { get; set; }

		public int project_id { get; set; }

		public int location_id { get; set; }
		
		public int quantity_id { get; set; }


		public ICollection<Seriefloat> seriefloats { get; set; }

		
		//Map these with EF:
		public virtual Device device { get; set; }
		public virtual Project project { get; set; }
		public virtual Location location { get; set; }
		public virtual Quantity quantity { get; set; }
		
	}
}