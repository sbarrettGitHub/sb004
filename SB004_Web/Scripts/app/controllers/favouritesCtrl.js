'use strict';
(function () {

    var favouritesCtrl = function ($scope, dialog, securityService) {
        $scope.favourites = [];
        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";		
		/*Control buttons*/
        $scope.closeMe = function () {
            dialog.close();
        };
		
		$scope.deleteFromFavourites = function(memeId){
			startWaiting();
			$http({ method: 'DELETE', url: '/api/meme/' + memeId + "/favourite/", data: {}})
				.success(function (data) {  
					$scope.meme.favourites++;	
					if(securityService.currentUser.profile.favouriteMemeIds){
						for(var i=$scope.favourites.length-1;i>0;i--){
							if($scope.favourites[i] === memeId){
								$scope.favourites[i].slice(i,1);								
							}
						}
					}
					endWaiting();
                }).error(function (e) {		
					endWaiting();				
					return;
                });
		};
		$scope.selectFavourites = function(memeId){
			dialog.close(memeId);
		};
		var refresh = function(){
			if(securityService.currentUser.profile)
			{
				if(securityService.currentUser.profile.favouriteMemeIds){
					for(var i=securityService.currentUser.profile.favouriteMemeIds.length-1;i>0;i--){
						$scope.favourites.push(securityService.currentUser.profile.favouriteMemeIds[i]);
					}
				}
			}
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
		refresh();
    }
  
    // Register the controller
    app.controller('favouritesCtrl', ["$scope", "dialog", "securityService", favouritesCtrl]);

})();