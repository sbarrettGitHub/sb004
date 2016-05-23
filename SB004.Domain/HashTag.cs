namespace SB004.Domain
{
	public class HashTag : IHashTag
	{
		public string Id { get; set; }

		public string Name { get; set; }

		public double TrendScoreOfAllMemes { get; set; }
	}
	public class HashTagMeme : IHashTagMeme
	{

		public HashTagMeme(string hashTag, string memeId, double trendScore)
		{
			TrendScore = trendScore;
			HashTag = hashTag;
			MemeId = memeId;
			Id = hashTag + memeId;
		}

		public string Id { get; set; }
		public string HashTag { get; set; }
		public string MemeId { get; set; }
		public double TrendScore { get; set; }
	}
}
