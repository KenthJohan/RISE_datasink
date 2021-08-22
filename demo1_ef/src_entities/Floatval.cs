using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Demo
{
	public class Floatval
	{
		//High frequency
		public DateTime time { get; set; }
		public float value { get; set; }

		//Medium frequency
		public float longitude { get; set; } = 0.0f;
		public float latitude { get; set; } = 0.0f;

		//Low frequency
		public int producer_id { get; set; } = 1;
		public virtual Producer producer { get; set; }
	}

	public class Serie_Float_Chunk
	{
		public List<DateTime> time { get; set; }
		public List<float> value { get; set; }
		public List<float> longitude { get; set; }
		public List<float> latitude { get; set; }
	}

}