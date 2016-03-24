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
			TimelineModel model = new TimelineModel
			{
				User = repository.GetUser(id),
				TimelineEntries = new List<TimelineEntryModel>()
			};

			foreach (var entry in timeLine)
			{
				TimelineEntryModel timelineEntryModel = new TimelineEntryModel(entry);

				// User has to be the requested user so no need to resolve
				timelineEntryModel.User = model.User;

				// Get the meme referenced by the time line entry
				if (entry.EntryType == TimeLineEntry.Post ||
				    entry.EntryType == TimeLineEntry.Like ||
				    entry.EntryType == TimeLineEntry.Dislike ||
				    entry.EntryType == TimeLineEntry.Repost ||
				    entry.EntryType == TimeLineEntry.Reply ||
					entry.EntryType == TimeLineEntry.Comment
					)
				{
					timelineEntryModel.Meme = new MemeLiteModel(repository, repository.GetMeme(entry.TimeLineRefId));
				}

				// Resolve the comment
				if (entry.EntryType == TimeLineEntry.Comment)
				{
					timelineEntryModel.UserComment = repository.GetUserComment(entry.TimeLineRefAlternateId);
				}

				// Resolve the alternative ref id (always a meme)
				if (entry.EntryType == TimeLineEntry.Reply)
				{
					timelineEntryModel.AlternateMeme = new MemeLiteModel(repository, repository.GetMeme(entry.TimeLineRefAlternateId));
				}

				// Add to time line model
				model.TimelineEntries.Add(timelineEntryModel);
			}
			return Ok(model);
		}
    }
}
