using SB004.Data;
using SB004.Domain;

namespace SB004.Business
{
	public class UserCommentBusiness : IUserCommentBusiness
	{
		readonly IRepository repository;
		public UserCommentBusiness(IRepository repository)
		{
		  this.repository = repository;
		}
		/// <summary>
		/// Inser the comment and record on user time line
		/// </summary>
		/// <param name="userComment"></param>
		/// <returns></returns>
		public IUserComment PostUserComment(IUserComment userComment)
		{
			// Post the comment
			userComment = repository.Save(userComment);
			
			// Record on time line
			repository.Save(new TimeLine(userComment.UserId, TimeLineEntry.Comment, userComment.MemeId, userComment.Id,null));
			
			// Update the number of comments added by this user
			IUser user = repository.GetUser(userComment.UserId);
			if (user != null)
			{
				user.Comments ++;

				// Save commentator
				repository.Save(user);
			}
			return userComment;
		}
	}
}
