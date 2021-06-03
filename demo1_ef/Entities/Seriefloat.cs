using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo
{
	public class Seriefloat
	{
		//High frequency
		public DateTime time { get; set; }
		public float value { get; set; }

		//Medium frequency
		public float longitude { get; set; }
		public float latitude { get; set; }

		//Low frequency
		public int serie_id { get; set; }
		public virtual Serie serie { get; set; }
	}
}