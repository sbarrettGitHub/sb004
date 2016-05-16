
namespace SB004.Domain
{

	public interface IHashTag
	{
		string Id { get; set; }
		double TrendScoreOfAllMemes { get; set; }
	}

	public interface IHashTagMeme
	{
		string Id { get; set; }
		string HashTag { get; set; }
		string MemeId { get; set; }
		double TrendScore { get; set; }
	}

}
