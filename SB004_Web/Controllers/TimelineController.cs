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

		/// <summary>
		/// Return the combined time line of the specified user those he follows
		/// </summary>
		/// <param name="id">user id</param>
		/// <param name="days"></param>
		/// <returns></returns>
		[Route("comprehensive/{id}")]
		public IHttpActionResult GetComprehensive(string id, int days)
		{
			// Get all memes posted or reposted by the user with any activite in the last X days
			List<ITimeLine> activityOnUserPostedMemes = repository.GetUserMemeTimeLine(id, days);

			TimelineGroupModel timelineGroupModel = new TimelineGroupModel
			{
				User = repository.GetUser(id, true),
				TimelineGroups = new List<TimelineGroup>()
			};
			foreach (ITimeLine activityOnUserPostedMeme in activityOnUserPostedMemes)
			{
				AddActivitytoTimeLineGroups(ref timelineGroupModel, activityOnUserPostedMeme);
			}

			TimelineModel model = new TimelineModel
			{
				User = repository.GetUser(id, true),
				TimelineEntries = new List<TimelineEntryModel>()
			};

			// Get the activity of the user and all his/her followers (grouped by meme)
			List<string> userIds = new List<string> { id };
			userIds.AddRange(model.User.FollowingIds.Select(x => x.Id));

			foreach (string userId in userIds)
			{
				// Get the activity of the user for the last x number of days
				List<ITimeLine> activity = repository.GetUserTimeLine(userId, days);
				foreach (ITimeLine entry in activity)
				{
					AddActivitytoTimeLineGroups(ref timelineGroupModel, entry);
				}
			}

			// Order by date descending
			timelineGroupModel.TimelineGroups = timelineGroupModel.TimelineGroups.OrderByDescending(x => x.TimeStamp).ToList();

			return Ok(timelineGroupModel);
		}

		/// <summary>
		/// Add the supplied time line activity to the time lien groups
		/// </summary>
		/// <param name="timelineGroupModel">timline group model updated</param>
		/// <param name="activityOnUserPostedMeme"></param>
		private void AddActivitytoTimeLineGroups(ref TimelineGroupModel timelineGroupModel, ITimeLine activityOnUserPostedMeme)
		{
			// Find a time line group has has activity on thie meme already
			TimelineGroup existingTimelineGroup =
				timelineGroupModel.TimelineGroups.FirstOrDefault(x => x.Meme.Id == activityOnUserPostedMeme.TimeLineRefId);
			if (existingTimelineGroup != null)
			{
				// Don't add more than 10
				if (existingTimelineGroup.TimelineEntries.Count < 10)
				{
					// Don't re-add an timeline entry that is already there 
					if (existingTimelineGroup.TimelineEntries.Any(x => x.TimelineId == activityOnUserPostedMeme.Id) == false)
					{
						existingTimelineGroup.TimelineEntries.Add(new TimelineEntryModel(activityOnUserPostedMeme));
					}
				}
				else
				{
					// If more than 10 just flag as having more
					existingTimelineGroup.HasMore = true;
				}
			}
			else
			{
				var timeLineGroup = new TimelineGroup();
				timeLineGroup.TimeStamp = activityOnUserPostedMeme.DateOfEntry;
				timeLineGroup.Meme = new MemeLiteModel(repository, repository.GetMeme(activityOnUserPostedMeme.TimeLineRefId));
				timeLineGroup.TimelineEntries = new List<TimelineEntryModel> { new TimelineEntryModel(activityOnUserPostedMeme) };

				timelineGroupModel.TimelineGroups.Add(timeLineGroup);
			}
		}
    }
}
