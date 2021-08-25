using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Demo
{

	public class Memloc
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id { get; set; }
		public int layout_id { get; set; }
		public int producer_id { get; set; }
		public int byteoffset { get; set; }
		public virtual Layout layout { get; set; }
	};
}