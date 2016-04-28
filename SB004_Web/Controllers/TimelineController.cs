using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using SB004.Data;
using SB004.Domain;
using SB004.Models;
using SB004.User;

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
		/// 1. Get latest 10 entries from ANY user on all quotes posted or reposted by the this user. 
		/// 2. Get latest 10 entries of the last X days of this user and all the users he/she follows on any quote. 
		/// 
		/// 1.	For every quote this user ever posted or reposted that has any activitiy in the last X days, retrieve the latest 
		///		maxCount (e.g. 10) entries (regardless of date) from ANY user (even users unrelated to this user).
		///		So if a user posted a quote last year and there a single timeline even by anyone in the last X days retrieve the last maxCount (e.g. 10) entries from the 
		///		time line even if only one entry falls within the last X days.
		/// 
		/// 2.	Taking this user and all the users he/she follows:
		///		For each 
		///			Retrieve latest maxCount (e.g. 10) entries by this user on any quote performed in teh last X days
		///  
		/// </summary>
		/// <param name="id">id of the user</param>
		/// <param name="days">Days to limit </param>
		/// <param name="maxCount">max entries per quote</param>
		/// <returns>For all of the above ensure no quote is repeated (i.e. activity is grouped by quote) and entries are orderd by date of entry ascending</returns>
		[Authorize]
		[Route("home/{id}")]
		public IHttpActionResult GetComprehensive(string id, int days, int maxCount)
		{
			// Get all memes posted or reposted by the user with any activity by ANY user in the last X days
			List<ITimeLine> activityOnUserPostedMemes = repository.GetUserMemeTimeLine(id, 30000).OrderByDescending(x => x.DateOfEntry).ToList(); 

			TimelineGroupModel timelineGroupModel = new TimelineGroupModel
			{
				User = repository.GetUser(id, true),
				TimelineGroups = new List<TimelineGroup>()
			};
			foreach (ITimeLine activityOnUserPostedMeme in activityOnUserPostedMemes)
			{
				AddActivitytoTimeLineGroups(ref timelineGroupModel, activityOnUserPostedMeme, maxCount);
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
				List<ITimeLine> activity = repository.GetUserTimeLine(userId, days).OrderByDescending(x => x.DateOfEntry).ToList(); 
				foreach (ITimeLine entry in activity)
				{
					AddActivitytoTimeLineGroups(ref timelineGroupModel, entry, maxCount);
				}
			}
            
            // Ensure the timeline entries in each group are ordered descending by date of entry
            timelineGroupModel.TimelineGroups.ForEach(x =>
            {
                x.TimelineEntries = x.TimelineEntries.OrderByDescending(y => y.DateOfEntry).ToList();
            });

			// Order by date descending
			timelineGroupModel.TimelineGroups = timelineGroupModel.TimelineGroups.OrderByDescending(x => x.TimeStamp).ToList();

			return Ok(timelineGroupModel);
		}
		/// <summary>
		/// Return the time line of the specified meme
		/// If the current user is created or reposted this then return activity for ALL users
		/// Otherwise return only the activity on the meme of the current user and those he/she follows 
		/// </summary>
		/// <param name="id">user id</param>
		/// <param name="days"></param>
		/// <param name="maxCount"></param>
		/// <returns></returns>
		[Authorize]
		[Route("meme/{id}")]
		public IHttpActionResult GetMemeTimeline(string id, int days, int maxCount)
		{
			TimelineGroupModel timelineGroupModel = new TimelineGroupModel
			{
				User = null,
				TimelineGroups = new List<TimelineGroup>()
			};

			// Get the specified meme
			IMeme meme = repository.GetMeme(id);
			if (meme == null)
			{
				// Meme has been removed or the ID was invlide. No need to error out, handle gracefully
				return Ok(timelineGroupModel);
			}

			// Determine the current user
			string currentUserId = User.Identity.UserId();
			var currentUser = repository.GetUser(currentUserId);

			// Restrict the activity returned to the current user and those he/she follows UNLESS THE CURRENT USER CREATED THE MEME
			bool includeActivityOfAllUsers= meme.CreatedByUserId == currentUserId;

			// Get the activity of the current user and those he/she follows (grouped by meme)
			List<string> userIds = new List<string> { currentUserId };
			userIds.AddRange(currentUser.FollowingIds.Select(x => x.Id));
			
			// Get all activity on this meme in the last X days, buy ANY user
			List<ITimeLine> memeActivities = repository.GetMemeTimeLine(id, days).OrderByDescending(x=>x.DateOfEntry).ToList();


			// Create the timeline group with all activity on this meme by the current user and those he/she follows
			foreach (ITimeLine memeActivity in memeActivities)
			{
				// Was this activity performed the current user or  those he/she follows?
				if (userIds.Contains(memeActivity.UserId) || includeActivityOfAllUsers)
				{
					// Add to model up to a max count
					AddActivitytoTimeLineGroups(ref timelineGroupModel, memeActivity, maxCount);
				}
				
			}
			Debug.Assert(timelineGroupModel.TimelineGroups.Count<=1, "Activity on single meme should only render one time line group!!!");

			// Ensure the timeline entries in each group are ordered descending by date of entry ... there should be only 1 group
			timelineGroupModel.TimelineGroups.ForEach(x =>
			{
				x.TimelineEntries = x.TimelineEntries.OrderByDescending(y => y.DateOfEntry).ToList();
			});

			// Order by date descending
			timelineGroupModel.TimelineGroups = timelineGroupModel.TimelineGroups.OrderByDescending(x => x.TimeStamp).ToList();

			return Ok(timelineGroupModel);
		}
		/// <summary>
		/// Add the supplied time line activity to the time lien groups
		/// </summary>
		/// <param name="timelineGroupModel">timline group model updated</param>
		/// <param name="activityOnMeme"></param>
		/// <param name="maxCount"></param>
		private void AddActivitytoTimeLineGroups(ref TimelineGroupModel timelineGroupModel, ITimeLine activityOnMeme, int maxCount)
		{
            
			// Find a time line group has has activity on thie meme already
			TimelineGroup existingTimelineGroup =
				timelineGroupModel.TimelineGroups.FirstOrDefault(x => x.Meme.Id == activityOnMeme.TimeLineRefId);
			if (existingTimelineGroup != null)
			{
				// Don't add more than 10
				if (existingTimelineGroup.TimelineEntries.Count < maxCount)
				{
					// Don't re-add an timeline entry that is already there 
					if (existingTimelineGroup.TimelineEntries.Any(x => x.TimelineId == activityOnMeme.Id) == false)
					{
                        existingTimelineGroup.TimelineEntries.Add(GetTimelineEntryModel(activityOnMeme));
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
				timeLineGroup.TimeStamp = activityOnMeme.DateOfEntry;
				timeLineGroup.Meme = new MemeLiteModel(repository, repository.GetMeme(activityOnMeme.TimeLineRefId));
                timeLineGroup.TimelineEntries = new List<TimelineEntryModel> { GetTimelineEntryModel(activityOnMeme) };

				timelineGroupModel.TimelineGroups.Add(timeLineGroup);
			}
		}
    }
}
