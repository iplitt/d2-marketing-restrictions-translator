using System;

namespace Lib
{
	[Serializable]
	public class Product
  {
    public string Restriction { get; set; }
    public string AlbumOnly { get; set; }
		public string PreOrderOnly { get; set; }
		public string SalesStartDate { get; set; }
		public DateTime? SalesStartDt { get; set; }
	}
}
