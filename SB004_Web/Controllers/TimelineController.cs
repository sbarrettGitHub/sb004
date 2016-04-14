using System.Collections.Generic;
using System.Linq;
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
				User = repository.GetUser(id,true),
				TimelineEntries = new List<TimelineEntryModel>()
			};

			foreach (ITimeLine entry in timeLine)
			{
				var timelineEntryModel = GetTimelineEntryModel(entry);

				// Add to time line model
				model.TimelineEntries.Add(timelineEntryModel);
			}
			return Ok(model);
		}

		private TimelineEntryModel GetTimelineEntryModel(ITimeLine entry)
		{
			TimelineEntryModel timelineEntryModel = new TimelineEntryModel(entry);

			timelineEntryModel.User = repository.GetUser(entry.UserId);

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
			return timelineEntryModel;
		}

		/// <summary>
		/// Return the combined time line of the specified user those he follows
		/// </summary>
		/// <param name="id">user id</param>
		/// <param name="skip">number of items to skip</param>
		/// <param name="take">number of items to take</param>
		/// <param name="type">The type of entries. Allow null for all entry types</param>
		/// <returns></returns>
		[Route("full/{id}")]
		public IHttpActionResult GetFull(string id, int skip, int take, TimeLineEntry type)
		{
			TimelineModel model = new TimelineModel
			{
				User = repository.GetUser(id, true),
				TimelineEntries = new List<TimelineEntryModel>()
			};

			// 
			List<string> userIds = new List<string>{id};
			userIds.AddRange(model.User.FollowingIds.Select(x=>x.Id));

			foreach (string userId in userIds)
			{
				List<ITimeLine> timeLine = repository.GetUserTimeLine(userId, skip, take, type);
				foreach (ITimeLine entry in timeLine)
				{
					var timelineEntryModel = GetTimelineEntryModel(entry);

					// Add to time line model
					model.TimelineEntries.Add(timelineEntryModel);
				}
			}

			// Order by date descending
			model.TimelineEntries = model.TimelineEntries.OrderByDescending(x => x.DateOfEntry).ToList();
			return Ok(model);
		}
    }
}
