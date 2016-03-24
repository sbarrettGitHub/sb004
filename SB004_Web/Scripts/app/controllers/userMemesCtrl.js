'use strict';
(function () {

    var userMemesCtrl = function ($scope, $rootScope, $routeParams, $http, $window, $location, likeDislikeMemeService, securityService) {
        $scope.userId = $routeParams.id;
		var itemsIndex=0;
		var entryType = "All";
		$scope.items = []
		$scope.user = {};
		$scope.isFollowing = false;
		$scope.statOptionSelected ="feed";
		var constants = {
			viewingBlockCount:10
		};
		
		var refreshTimeline = function(){
			$scope.items = [];
			var currentCount = itemsIndex;
			itemsIndex = 0;
			
			// Retrieve  all the items that are currently visible again
			$scope.getTimeline(0, currentCount, entryType);
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
					if(data.timelineEntries){
						// Add the items returned to the list of items
						for(var i=0;i<data.timelineEntries.length;i++){   
							$scope.items.push(data.timelineEntries[i]);
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
			refreshTimeline();
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