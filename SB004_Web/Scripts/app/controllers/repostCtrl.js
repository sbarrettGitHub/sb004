'use strict';
(function () {

    var repostCtrl = function ($scope, $http, dialog,securityService, memeId) {
        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";	
		$scope.meme = {};
		/*Control buttons*/
        $scope.closeMe = function () {
            dialog.close();
        };
		$scope.likeMeme = function(memeId)
		{
			// Don't allow multiple likes by the same user on the same meme
			for(var i=0;i<securityService.currentUser.myMemeLikes.length;i++){
				if(securityService.currentUser.myMemeLikes[i] == memeId){
					return;
				}
			}
			$http({ method: 'PATCH', url: 'api/meme/' + memeId + "/like/", data: {}})
				.success(function (data) { 
					// Record that you like the selected meme
					securityService.currentUser.myMemeLikes.push(data.id);					
					// Increment the number of likes for the selected meme					
					if($scope.meme){
						if(!$scope.meme.likes){
							$scope.meme.likes = 0;
						}
						$scope.meme.likes++;
					}
                }).error(function (e) {
					alert(e);
					return;
                });
		}

		$scope.dislikeMeme = function(memeId)
		{
			// Don't allow multiple dislikes by the same user on the same meme
			for(var i=0;i<securityService.currentUser.myMemeDislikes.length;i++){
				if(securityService.currentUser.myMemeDislikes[i] == memeId){
					return;
				}
			}
			$http({ method: 'PATCH', url: 'api/meme/' + memeId + "/dislike/", data: {}})
				.success(function (data) {  
					securityService.currentUser.myMemeDislikes.push(data.id);
					// Decrement the number of likes for the selected meme					
					if($scope.meme){
						if(!$scope.meme.dislikes){
							$scope.meme.dislikes = 0;
						}
						$scope.meme.dislikes++;	
					}
                }).error(function (e) {
					alert(e);
					return;
                });
		}
		$scope.repost=function(){
			$http({ method: 'PATCH', url: '/api/meme/' + memeId + "/repost/", data: {}})
				.success(function (data) {  
					dialog.close({action:"repost"});
                }).error(function (e) {
					alert(e);
					return;
                });
		};
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
		
		$http.get('/api/Meme/Lite/' + memeId).
			success(function (data) {
				endWaiting();
				if(data){
					$scope.meme = data;
				}
			}).
			error(function (e) {
				endWaiting();
			});
    }
  
    // Register the controller
    app.controller('repostCtrl', ["$scope", "$http", "dialog","securityService", "memeId", repostCtrl]);

})();