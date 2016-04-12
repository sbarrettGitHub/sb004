'use strict';
(function () {

    var userMemesCtrl = function ($scope, $rootScope, $routeParams, $http, $window, $location, $q, likeDislikeMemeService, securityService, scrollTo) {
        $scope.userId = $routeParams.id;
		var itemsIndex=0;
		$scope.entryType = "All";
		$scope.items = []
		$scope.user = {};
		$scope.isFollowing = false;
		$scope.statOptionSelected ="feed";
		$scope.lastMeme = "";
		var constants = {
			viewingBlockCount:10
		};
		
		$scope.refreshTimeline = function(type){
			$scope.items = [];
			itemsIndex = 0;
			$scope.entryType = type;
			var timelineEntryType = getTimelineEntryType($scope.entryType);
			
			// Retrieve  all the items that are currently visible again
			$scope.getTimeline(0, constants.viewingBlockCount, timelineEntryType);
		}	
		var getTimelineEntryType = function(type){
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
						$scope.entryType = "All";
						break;
				}
				return timelineEntryType;
			}	
		$scope.getTimeline = function(skipitems, takeitems, type){
			startWaiting();
			// Skip the explicitly specified number of items (used during a refresh as 0 so all previously retrieved items are refreshed) 
			// or skip the current number of items to get the next page worth
			var skip = skipitems ? skipitems : itemsIndex;
			
			// Take the explicitly specified number of items (used during a refresh as the number of previously retrieved items)
			// or a standard page worth
			var take = takeitems ? takeitems : constants.viewingBlockCount;
			
			// Use the current entry type unless secified (convert scope entry type to time line entry type)
			var entryType = type ? type: getTimelineEntryType($scope.entryType);
			var deferred = $q.defer();

			$http.get('api/timeline/' + $scope.userId + '?skip=' + skip + '&take=' + take + '&type=' + entryType).
                success(function (data) {
					if(data.user){
						$scope.user = data.user;
					}
					// Group time lin eenties by meme
					var memeTimelineGroups={};					
					if(data.timelineEntries){
						for(var i=0;i<data.timelineEntries.length;i++){  
							// Does this meme have a group already?
							if(!memeTimelineGroups[data.timelineEntries[i].meme.id]){
								// Create new group
								var newGroup ={
									id: "",
									entries: []
								};
								newGroup.id = data.timelineEntries[i].meme.id;
								newGroup.entries.unshift(data.timelineEntries[i]);
								// Create a new group for time line entried for this meme
								memeTimelineGroups[data.timelineEntries[i].meme.id] = newGroup;
							}else{
								// Get the exisitng group
								var existingGroup = memeTimelineGroups[data.timelineEntries[i].meme.id];
								// Add to existing group
								existingGroup.entries.unshift(data.timelineEntries[i]);
							}
						}

						for (var memeTimelineGroup in memeTimelineGroups) {
							if(memeTimelineGroups[memeTimelineGroup].entries){
								for(var ii=0;ii<memeTimelineGroups[memeTimelineGroup].entries.length;ii++){  
									$scope.items.push(memeTimelineGroups[memeTimelineGroup].entries[ii]);
								}
							}
						}
					}					
										
					// Maintain a cursor of items. 
					itemsIndex = $scope.items.length;
					if($scope.items){
						$scope.lastMeme = $scope.items[itemsIndex-1].meme.id;
					}					
					endWaiting();
					deferred.resolve(data);
                }).
                error(function (e) {
					$window.alert(e);
                    endWaiting();
					deferred.reject(e);
                });

			return deferred.promise;	
		}
		$scope.showMore = function(){
			var lastMemeBefore = $scope.lastMeme;
			$scope.getTimeline().then(function(){
				scrollTo(lastMemeBefore);			
			});

		}
		$scope.viewMeme = function(memeId)
		{
			if(memeId){
				$location.path('/meme/' + memeId);
			}else{
				$location.path("home");
			}
		}
		$scope.likeMeme = function(memeId)
		{
			likeDislikeMemeService.like(findMeme(memeId));			
		}
		$scope.dislikeMeme = function(memeId)
		{
			likeDislikeMemeService.dislike(findMeme(memeId));			
		}
		$scope.openUser = function(userId)
		{
			$location.path("/usermemes/" + userId);		
		}
		function findMeme(memeId){
			// Find the meme in scope with the given id
			for(var i=0;i<$scope.memes.length;i++){
				if($scope.memes[i].id == memeId){
					return $scope.memes[i];
				}
			}	
			return null;
		}
		$scope.follow = function()
		{
			securityService.follow($scope.user.id)
			.then(function(){
				$window.alert("You are now following " + $scope.user.userName + '!');
				$scope.isFollowing = true;
			});
		}		
		$scope.unfollow = function()
		{
			securityService.unfollow($scope.user.id)
			.then(function(){
				$window.alert("You are no longer following " + $scope.user.userName + '!');
				$scope.isFollowing = false;
			});
		}			
		function startWaiting(heading, message) {
            $scope.waiting = true;
            $scope.waitHeading = !heading ? "Please wait..." : heading;
            $scope.waitingMessage = !message ? "" : message;
        }
        function endWaiting() {
            $scope.waiting = false;
            $scope.waitHeading = "";
            $scope.waitingMessage = "";
        }
		
		securityService.testIsAuthenticated()
		.then(function(isAuthenticated){
			if(isAuthenticated===true){
				// If signed in then check if the current user follows the selected user
				$scope.isFollowing = securityService.isFollowing($scope.userId);
				$scope.user = securityService.getCurrentUser();
			}
			// Refresh the users time line
			$scope.refreshTimeline("All");
		});
		
		// User message updated
		$rootScope.$on('account.newUserMessage', function (event, data) {
			$scope.user.statusMessage = data;
		});
		// User name updated
		$rootScope.$on('account.newUserName', function (event, data) {
			$scope.user.userName = data;
		});
		// User image updated
		$rootScope.$on('account.newUserImage', function (event, data) {
			$scope.userId =  $scope.userId + '?' + new Date().getTime();
		});
    }

    // Register the controller
    app.controller('userMemesCtrl', ["$scope", "$rootScope", "$routeParams", "$http", "$window", "$location","$q", "likeDislikeMemeService", "securityService", "scrollTo", userMemesCtrl]);

})();