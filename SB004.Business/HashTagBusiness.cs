using SB004.Data;
using SB004.Domain;

namespace SB004.Business
{
	public class HashTagBusiness:IHashTagBusiness
	{
		readonly IRepository repository;

		public HashTagBusiness(IRepository repository)
		{
			this.repository = repository;
		}

		/// <summary>
		/// Calculate the hash tags trend score and save. Maintain the link between a has tag and meme
		/// NOTE: A meme's hash tag list never changes.  
		/// </summary>
		/// <param name="hashTag"></param>
		/// <param name="memeId"></param>
		/// <param name="previousMemeTrendScore"></param>
		/// <param name="newMemeTrendScore"></param>
		/// <returns></returns>
		public IHashTag SaveMemeTags(string hashTag, string memeId, double previousMemeTrendScore, double newMemeTrendScore)
		{
			// Get from repository or create new if doesn't exist
			IHashTag tag = repository.GetHashTag(hashTag) ?? new HashTag{Id = hashTag};

			// Update the score of the hash tag to account for the memes new score
			tag.TrendScoreOfAllMemes += newMemeTrendScore - previousMemeTrendScore;
			
			// Save hash tag in repository
		    var savedHashTag = repository.Save(tag);

			// Maintain a link between the hash tag and meme
			repository.Save(new HashTagMeme(tag.Id, memeId, newMemeTrendScore));

			return savedHashTag;
		}
	}
}
