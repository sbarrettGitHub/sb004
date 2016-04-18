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
            
            $http.get('api/timeline/comprehensive/' + userId + '?days=' + days + '&maxCount='+ max).
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
        return {
            userTimeline:userTimeline,
            userAndFollowingTimeline: userAndFollowingTimeline,
            userComprehensiveTimeline: userComprehensiveTimeline,
            resolveTimelineEntryType: resolveTimelineEntryType,
            memeTimeline:memeTimeline,
            organize: organize
        }
    }
    // Register the service
    app.factory('timeLineService', ['$http', '$q', '$window', timeLineService]);

})();