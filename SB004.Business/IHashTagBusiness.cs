
using SB004.Domain;

namespace SB004.Business
{
	public interface IHashTagBusiness
	{
		IHashTag SaveMemeTags(string hashTag, string memeId, double previousMemeTrendScore, double newMemeTrendScore);
	}
}
