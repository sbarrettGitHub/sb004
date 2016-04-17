'use strict';
(function () {

    var userMemesCtrl = function ($scope, $rootScope, $routeParams, $http, $window, $location, $q, likeDislikeMemeService, securityService, scrollTo, timeLineService) {
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
			var timelineEntryType = timeLineService.resolveTimelineEntryType($scope.entryType);
			
			// Retrieve  all the items that are currently visible again
            startWaiting();
			$scope.getTimeline(0, constants.viewingBlockCount, timelineEntryType).
            then(function(){
                endWaiting();
            },
            function(){
                endWaiting();
            });
		}	
                    
		$scope.getTimeline = function(skipitems, takeitems, type){
			
			// Skip the explicitly specified number of items (used during a refresh as 0 so all previously retrieved items are refreshed) 
			// or skip the current number of items to get the next page worth
			var skip = skipitems ? skipitems : itemsIndex;
			
			// Take the explicitly specified number of items (used during a refresh as the number of previously retrieved items)
			// or a standard page worth
			var take = takeitems ? takeitems : constants.viewingBlockCount;
			
			// Use the current entry type unless specified (convert scope entry type to time line entry type)
			var entryType = type ? type: timeLineService.resolveTimelineEntryType($scope.entryType);
			var deferred = $q.defer();
            
            // Get user time line
            timeLineService.userTimeline($scope.userId, entryType, skip, take)
            .then(function (data) {
					if(data.user){
						$scope.user = data.user;
					}
                    
                    // Add new time line entries						
					if(data.timelineEntries){
                        for (var index = 0; index < data.timelineEntries.length; index++) {
                            $scope.items.push(data.timelineEntries[index]);                            
                        }
					}
                    					
					// Group by meme
                    $scope.items = timeLineService.organize($scope.items);
                    			
					// Maintain a cursor of items. 
					itemsIndex = $scope.items.length;

					deferred.resolve(data);
                },
                function (e) {
					$window.alert(e);
					deferred.reject(e);
                });

			return deferred.promise;	
		}
		$scope.showMore = function(){
			var lastMemeBefore = $scope.lastMeme;
			$scope.getTimeline();
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
    app.controller('userMemesCtrl', ["$scope", "$rootScope", "$routeParams", "$http", "$window", "$location","$q", "likeDislikeMemeService", "securityService", "scrollTo", "timeLineService", userMemesCtrl]);

})();