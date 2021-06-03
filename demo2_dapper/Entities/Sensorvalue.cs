using System;
using Microsoft.EntityFrameworkCore;


namespace Demo
{
	[Keyless]
	public class Sensorvalue
	{
		//Low frequency
		public int device_id { get; set; }
		public int project_id { get; set; }
		public int location_id { get; set; }
		public int quantity_id { get; set; }

		//Medium frequency
		public float longitude { get; set; }
		public float latitude { get; set; }

		//High frequency
		public DateTime time { get; set; }
		public float value { get; set; }




		public virtual Device device { get; set; }
		public virtual Project project { get; set; }
		public virtual Location location { get; set; }
		public virtual Quantity quantity { get; set; }
	}
}