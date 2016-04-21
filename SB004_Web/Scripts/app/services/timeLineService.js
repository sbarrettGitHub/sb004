'use strict';
(function () {
    var timeLineService = function ($http, $q, $window) {
        var userTimeline = function(userId, entryType, skip, take){
            var deferred = $q.defer();
            
            $http.get('api/timeline/' + userId + '?skip=' + skip + '&take=' + take + '&type=' + entryType).
                success(function (data) {									
					deferred.resolve(data);
                }).
                error(function (e) {
					$window.alert(e);
					deferred.reject(e);
                });
            
            return deferred.promise;
        }	
        
        var userAndFollowingTimeline = function(userId, entryType, skip, take){
			var deferred = $q.defer();
            
            $http.get('api/timeline/full/' + userId + '?skip=' + skip + '&take=' + take + '&type=' + entryType).
                 success(function (data) {									
					deferred.resolve(data);
                }).
                error(function (e) {
					$window.alert(e);
					deferred.reject(e);
                });
			
			return deferred.promise;
		}	
        var userComprehensiveTimeline = function(userId, days, max){
			var deferred = $q.defer();
            
            $http.get('api/timeline/comprehensive/' + userId + '?days=' + days + '&maxCount='+ max + '&rnd='+new Date().getTime()).
                 success(function (data) {									
					deferred.resolve(data);
                }).
                error(function (e) {
					$window.alert(e);
					deferred.reject(e);
                });
			
			return deferred.promise;
		}	      
        var memeTimeline = function(memeId, days, max){
			var deferred = $q.defer();
            
            $http.get('api/timeline/meme/' + memeId + '?days=' + days+ '&maxCount='+ max).
                 success(function (data) {									
					deferred.resolve(data);
                }).
                error(function (e) {
					$window.alert(e);
					deferred.reject(e);
                });
			
			return deferred.promise;
		}          
        //-----------------------------------------
        // Group time line entries by meme
        //-----------------------------------------
        var organize = function(timelineEntries){
            var items = [];
            var memeTimelineGroups={};
            for(var i=0;i<timelineEntries.length;i++){  
                // Does this meme have a group already?
                if(!memeTimelineGroups[timelineEntries[i].meme.id]){
                    // Create new group
                    var newGroup ={
                        id: "",
                        entries: []
                    };
                    newGroup.id = timelineEntries[i].meme.id;
                    newGroup.entries.unshift(timelineEntries[i]);
                    // Create a new group for time line entried for this meme
                    memeTimelineGroups[timelineEntries[i].meme.id] = newGroup;
                }else{
                    // Get the exisitng group
                    var existingGroup = memeTimelineGroups[timelineEntries[i].meme.id];
                    // Add to existing group
                    existingGroup.entries.unshift(timelineEntries[i]);
                }
            }

            for (var memeTimelineGroup in memeTimelineGroups) {
                if(memeTimelineGroups[memeTimelineGroup].entries){
                    for(var ii=0;ii<memeTimelineGroups[memeTimelineGroup].entries.length;ii++){  
                        items.push(memeTimelineGroups[memeTimelineGroup].entries[ii]);
                    }
                }
            } 
            return items;   
        }
        
        var resolveTimelineEntryType = function(type){
            var timelineEntryType = 0;
            switch (type) {
                case "All":
                    timelineEntryType = 0;
                    break;				
                case "posts":
                    timelineEntryType = 1;
                    break;
                case "reposts":
                    timelineEntryType = 2;
                    break;					
                case "likes":
                    timelineEntryType = 3;
                    break;
                case "replies":
                    timelineEntryType = 5;
                    break;
                case "comments":
                    timelineEntryType = 6;
                    break;
                default:
                    timelineEntryType = 0;
                    break;
            }
            return timelineEntryType;            
        }
        var mergeTimelines = function(sourceTimeline, destinationTimeline){
            // Take each new group and insert into the current list of time line groups based on timestamp.
            // If the group is already in scope, update its timeline entries (most rescent first)
            for (var i = 0; i < sourceTimeline.timelineGroups.length; i++) {
                
                var group = sourceTimeline.timelineGroups[i];
                
                if(group.meme){
                    // Locate this group in scope
                    var index = findMemeGroupIndex(destinationTimeline, group.meme.id );
                    if(index >= 0){
                        
                        // Is in the current list of time line groups, add the newer timeline entries
                        
                        var newTimelineEntries = group.timelineEntries;
                        var currentTimelineEntries = destinationTimeline[index].timelineEntries;
                        var numberOfCurrentEntries = currentTimelineEntries.length;
                        
                        // Traverse all timeline entries in the retrieved group  
                        for (var ti = 0; ti < newTimelineEntries.length; ti++) {
                            
                            // Determine if this entry is already in the current time line entry list
                            var timelineIndex = findTimelineEntryIndex(currentTimelineEntries, newTimelineEntries[ti].timelineId)
                            
                            // If it is there already, delete it and add again based on date of entry
                            if(timelineIndex >= 0){
                                // Delete it
                                currentTimelineEntries.splice(timelineIndex, 1);
                            }
                            // Insert into the current list of time line entries of the current group (based on date of entry descending)
                            insertTimelineEntryBasedOnDatreOfEntry(newTimelineEntries[ti], currentTimelineEntries);										
                        }
                        
                    }else{
                        // Timeline Group is NOT is current list of time line groups. Insert based on time stamp (most recent first) 
                        insertTimelineGroupBasedOnTimeStamp(group, destinationTimeline);
                    }
                }
            }
        }
        // -----------------------------------------------------------------
		// Take the supplied group and insert into supplied items based on 
		// time stamp 
		function insertTimelineGroupBasedOnTimeStamp(group, items){
			for (var i = 0; i < items.length; i++) {
				if(group.timeStamp > items[i].timeStamp){
					// Insert above current position in items
					items.splice(i, 0, group);
					return;
				}				
			}
			// Add to the end
			items.push(group);
		}
		// -----------------------------------------------------------------
		// Take the timeline Entry and and insert into supplied timelineEntry 
		// array  based on date of entry
		function insertTimelineEntryBasedOnDatreOfEntry(entry, timelineEntries){
			for (var i = 0; i < timelineEntries.length; i++) {
				if(entry.dateOfEntry > timelineEntries[i].dateOfEntry){
					// Insert above current position in timelineEntries
					timelineEntries.splice(i, 0, entry);
					return;
				}				
			}
			// Add to the end
			timelineEntries.push(entry);
		}		
		// ----------------------------------------------------------------------------------
		// Find the first index in the supplied timeline Group array with the meme id supplied 
		function findMemeGroupIndex(items, memeId){
			for (var i = 0; i < items.length; i++) {
				if(items[i].meme.id == memeId){
					return i;
				}				
			}
			return -1;
		}
		// ----------------------------------------------------------------------------------
		// Find the first index in the supplied timeline entry array with the timeline id supplied 
		function findTimelineEntryIndex(entries, timelineId){
			for (var i = 0; i < entries.length; i++) {
				if(entries[i].timelineId == timelineId){
					return i;
				}				
			}
			return -1;
		}	        
        return {
            userTimeline:userTimeline,
            userAndFollowingTimeline: userAndFollowingTimeline,
            userComprehensiveTimeline: userComprehensiveTimeline,
            resolveTimelineEntryType: resolveTimelineEntryType,
            memeTimeline:memeTimeline,
            mergeTimelines: mergeTimelines,
            organize: organize
        }
    }
    // Register the service
    app.factory('timeLineService', ['$http', '$q', '$window', timeLineService]);

})();