'use strict';
(function () {

    var userMemesCtrl = function ($scope, $routeParams, $http, $window, $location, likeDislikeMemeService) {
        var userId = $routeParams.id;
		var memesIndex=0;
		$scope.memes = [];
		$scope.user = {};
		$scope.allowGetMoreMemes = false;
		var constants = {
			memeViewingBlockCount:10
		};
		var refreshMemes = function(){
			$scope.memes = [];
			var currentCount = memesIndex;
			memesIndex = 0;
			
			// Retrieve again all the memes that are currently visible again
			$scope.getMoreMemes(0, currentCount);
		}
        $scope.getMoreMemes = function(skipMemes, takeMemes){
			startWaiting();
			// Skip the explicitly specified number of memes (used during a refresh as 0 so all previously retrieved memes are refreshed) 
			// or skip the current number of memes to get the next page worth
			var skip = skipMemes ? skipMemes : memesIndex;
			
			// Take the explicitly specified number of memes (used during a refresh as the number of previously retrieved memes)
			// or a standard page worth
			var take = takeMemes ? takeMemes : constants.memeViewingBlockCount;
			
			$http.get('/api/Meme/byuser/' + userId + "?skip=" + skip + "&take=" + take).
                success(function (data) {
					if(data.user){
						$scope.user = data.user;
					}
					// Add the memes returned to the list of memes
					for(var i=0;i<data.memes.length;i++){   
						$scope.memes.push(data.memes[i]);
					}	
					// The Get More Memes button should be available if there are more memes out there than are displayed
					$scope.allowGetMoreMemes = (data.fullMemeCount > $scope.memes.length) ? true : false;
					
					// Maintain a cursor of memes. 
					memesIndex = $scope.memes.length;
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
		function findMeme(memeId){
			// Find the meme in scope with the given id
			for(var i=0;i<$scope.memes.length;i++){
				if($scope.memes[i].id == memeId){
					return $scope.memes[i];
				}
			}	
			return null;
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
		
		refreshMemes();
    }

    // Register the controller
    app.controller('userMemesCtrl', ["$scope", "$routeParams", "$http", "$window", "$location", "likeDislikeMemeService", userMemesCtrl]);

})();