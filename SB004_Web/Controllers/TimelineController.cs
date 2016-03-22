using System.Collections.Generic;
using System.Web.Http;
using SB004.Data;
using SB004.Domain;
using SB004.Models;

namespace SB004.Controllers
{
	[RoutePrefix("api/timeline")]
    public class TimelineController : ApiController
    {
		readonly IRepository repository;

		public TimelineController(IRepository repository)
        {
            this.repository = repository;
        }

		/// <summary>
		/// Return the time line of the specified user
		/// </summary>
		/// <param name="id">user id</param>
		/// <param name="skip">number of items to skip</param>
		/// <param name="take">number of items to take</param>
		/// <param name="type">The type of entries. Allow null for all entry types</param>
		/// <returns></returns>
		[Route("{id}")]
		public IHttpActionResult Get(string id, int skip, int take, TimeLineEntry type)
		{
			List<ITimeLine> timeLine = repository.GetUserTimeLine(id, skip, take, type);
			List<TimelineModel> model = new List<TimelineModel>();
			foreach (var item in timeLine)
			{
				TimelineModel itemModel = new TimelineModel(item);

				// Resove the user object
				itemModel.User = repository.GetUser(item.UserId);

				// Get the meme referenced by the time line entry
				if (item.EntryType == TimeLineEntry.Post ||
				    item.EntryType == TimeLineEntry.Like ||
				    item.EntryType == TimeLineEntry.Dislike ||
				    item.EntryType == TimeLineEntry.Repost ||
				    item.EntryType == TimeLineEntry.Reply ||
					item.EntryType == TimeLineEntry.Comment
					)
				{
					itemModel.Meme = new MemeLiteModel(repository, repository.GetMeme(item.TimeLineRefId));
				}

				// Resolve the comment
				if (item.EntryType == TimeLineEntry.Comment)
				{
					itemModel.UserComment = repository.GetUserComment(item.TimeLineRefAlternateId);
				}

				// Resolve the alternative ref id (always a meme)
				if (item.EntryType == TimeLineEntry.Reply)
				{
					itemModel.AlternateMeme = new MemeLiteModel(repository, repository.GetMeme(item.TimeLineRefAlternateId));
				}

				// Add to time line model
				model.Add(itemModel);
			}
			return Ok(model);
		}
    }
}
