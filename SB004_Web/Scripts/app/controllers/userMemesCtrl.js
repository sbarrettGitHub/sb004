'use strict';
(function () {

    var userMemesCtrl = function ($scope, $rootScope, $routeParams, $http, $window, $location, likeDislikeMemeService, securityService) {
        $scope.userId = $routeParams.id;
		var itemsIndex=0;
		$scope.entryType = "All";
		$scope.items = []
		$scope.user = {};
		$scope.isFollowing = false;
		$scope.statOptionSelected ="feed";
		var constants = {
			viewingBlockCount:10
		};
		
		$scope.refreshTimeline = function(type){
			$scope.items = [];
			var currentCount = itemsIndex;
			itemsIndex = 0;
			$scope.entryType = type;
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
			// Retrieve  all the items that are currently visible again
			$scope.getTimeline(0, constants.viewingBlockCount, timelineEntryType);
		}		
		$scope.getTimeline = function(skipitems, takeitems, type){
			startWaiting();
			// Skip the explicitly specified number of items (used during a refresh as 0 so all previously retrieved items are refreshed) 
			// or skip the current number of items to get the next page worth
			var skip = skipitems ? skipitems : itemsIndex;
			
			// Take the explicitly specified number of items (used during a refresh as the number of previously retrieved items)
			// or a standard page worth
			var take = takeitems ? takeitems : constants.viewingBlockCount;
			
			$http.get('api/timeline/' + $scope.userId + '?skip=' + skip + '&take=' + take + '&type=' + type).
                success(function (data) {
					if(data.user){
						$scope.user = data.user;
					}
					var memeGroup=[];
					if(data.timelineEntries){
						// Add the items returned to the list of items in descending order of date of entry. 
						// Group actions on the same meme together and list in ascending order with in the group
						for(var i=0;i<data.timelineEntries.length;i++){   
							
							
							var currentTimeLineEntry = data.timelineEntries[i];
							var nextTimeLineEntry = i < data.timelineEntries.length - 2 ? data.timelineEntries[i+1]: null;
							var currentScopeEntry = $scope.items.length > 0 ? $scope.items[$scope.items.length-1]: null;
							var currentMemeGroupInPlace = (memeGroup.length > 0);
							var currentMemeGroupHead = currentMemeGroupInPlace ? memeGroup[0] : null;
							// Is the time line entry is the same meme as the previous one (or the next one when there is no current group)
							if((currentScopeEntry && currentTimeLineEntry.meme.id == currentScopeEntry.meme.id) || 
								(currentMemeGroupInPlace == false && nextTimeLineEntry && currentTimeLineEntry.meme.id == nextTimeLineEntry.meme.id) ||
								currentMemeGroupInPlace == true && !currentScopeEntry && currentTimeLineEntry.meme.id == currentMemeGroupHead.meme.id){
								// Add to a group of ascending timeline entries for the meme group
								// ***Time line entries are displayed in descending time but when meme actions appear togehter 
								// they are grouped and displayed in ascending order within the group ***
								memeGroup.unshift(data.timelineEntries[i]);
							}else{
								
								// The meme group has ended push the group items to the scope
								for (var memeGroupIndex = 0; memeGroupIndex < memeGroup.length; memeGroupIndex++) {
									$scope.items.push(memeGroup[memeGroupIndex]);									
								}
								// Clear the meme group
								memeGroup=[];
								// Add the new entry 
								$scope.items.push(data.timelineEntries[i]);
							}
							
						}
					}						
										
					// Maintain a cursor of items. 
					itemsIndex = $scope.items.length;
					endWaiting();
                }).
                error(function (e) {
					$window.alert(e);
                    endWaiting();
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
    app.controller('userMemesCtrl', ["$scope", "$rootScope", "$routeParams", "$http", "$window", "$location", "likeDislikeMemeService", "securityService", userMemesCtrl]);

})();