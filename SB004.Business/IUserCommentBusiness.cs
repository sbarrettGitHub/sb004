using SB004.Domain;

namespace SB004.Business
{
	public interface IUserCommentBusiness
	{
		/// <summary>
		/// Inser the comment and record on user time line
		/// </summary>
		/// <param name="userComment"></param>
		/// <returns></returns>
		IUserComment PostUserComment(IUserComment userComment);
	}
}