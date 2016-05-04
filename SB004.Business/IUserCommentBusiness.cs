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

		/// <summary>
		/// Record the like of a user comment on the comment and in the time line
		/// </summary>
		/// <param name="userCommentId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		IUserComment LikeComment(string userCommentId, string userId);

		/// <summary>
		/// Record the dislike of a user comment on the comment and in the time line
		/// </summary>
		/// <param name="userCommentId"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		IUserComment DislikeComment(string userCommentId, string userId);
	}
}