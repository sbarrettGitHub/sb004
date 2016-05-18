using System.Collections.Generic;

namespace SB004.Models
{
	public class HashTagMemeModel
	{
		public string HashTag { get; set; }
		public List<MemeLiteModel> MemeLiteModels { get; set; }
	}

	public class TrendingHashTagRequestModel
	{
		public int TakeHashTags { get; set; }
		public int TakeMemes { get; set; }
		public List<string> FilterList { get; set; }
	}
}