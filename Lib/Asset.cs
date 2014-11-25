using System;

namespace Lib
{
	public class Asset
	{
		public string TrackTitle { get; set; }
		public string Restriction { get; set; }
		public string AlbumOnly { get; set; }
		public string TrackNumber { get; set; }
		public string Volume { get; set; }
		public string UnambiguousTrackTitle { get; set; }
		public string PreOrderOnly { get; set; }
		public string SalesStartDate { get; set; }
		public DateTime? SalesStartDt { get; set; }
	}
}