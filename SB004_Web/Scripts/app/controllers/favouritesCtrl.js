'use strict';
(function () {

    var favouritesCtrl = function ($scope, $http, dialog, securityService) {
        $scope.favourites = [];
        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";
		$scope.currentFavourite = null;		
		/*Control buttons*/
        $scope.closeMe = function () {
            dialog.close();
        };
		
		$scope.deleteFromFavourites = function(memeId){
			if(!confirm("Are you sure you wish to remove this from your list of favoutites?"))
			{
				return;
			}
			startWaiting();
			$http({ method: 'DELETE', url: 'api/meme/' + memeId + "/favourite/", data: {}})
				.success(function (data) { 
					// Remove from the current user profile favourites (saves reloading)
					if(securityService.getCurrentUser().profile.favouriteMemeIds){
						for(var i=securityService.getCurrentUser().profile.favouriteMemeIds.length-1;i>0;i--){
							if(securityService.getCurrentUser().profile.favouriteMemeIds[i] === memeId){ 
								securityService.getCurrentUser().profile.favouriteMemeIds.splice(i,1);								
							}
						}
						refresh();
					}
					endWaiting();
                }).error(function (e) {		
					endWaiting();				
					return;
                });
		};
		$scope.selectFavourite = function(memeId){
			dialog.close(memeId);
		};
		var refresh = function(){
			if(securityService.getCurrentUser().profile)
			{
				if(securityService.getCurrentUser().profile.favouriteMemeIds){
					$scope.favourites = [];
					for(var i=securityService.getCurrentUser().profile.favouriteMemeIds.length-1;i>0;i--){
						$scope.favourites.push(securityService.getCurrentUser().profile.favouriteMemeIds[i]);
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
    app.controller('favouritesCtrl', ["$scope", "$http", "dialog", "securityService", favouritesCtrl]);

})();