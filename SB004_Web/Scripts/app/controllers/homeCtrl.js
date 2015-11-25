'use strict';
(function () {

    var homeCtrl = function ($scope, $location, $rootScope, $dialog, $timeout, sharedDataService, memeWizardService, securityService) {
        $scope.memes = sharedDataService.data.quoteSearch.results;
        $scope.searchTerm = "";
        $scope.searchCategory = "";
		$scope.view = "trending";
        $scope.following = [];
		$scope.viewing = [];
        $scope.addNew = function () {
			
			dialogsViewBegin();
			memeWizardService.begin()
			.then(
				function(newMemeId){
					// New meme added
					allDialogsComplete();
					
					// Move to new meme
					$scope.viewMeme(newMemeId);
				},
				function(){
					// Cancelled
					allDialogsComplete();
				}				
			);
        }
        
		$scope.viewMeme = function(memeId)
		{
			 $location.path('/meme/' + memeId);
		}
        $rootScope.$on('quoteSearch.complete', function (event, data) {

            $scope.memes = sharedDataService.data.quoteSearch.results;

        });
        $scope.resized = function (width, height) {
            console.log(width + " X " + height);
        };
        // $scope.search();
        $scope.init = function () {
            $scope.handle = $dialog.dialog({});
        };

        $scope.init();
		$scope.switchView = function(view)
		{
			if(view=="following"){
				if (securityService.getCurrentUser().isAuthenticated) {
					$scope.view = "following";
					showFollowing();
				} else {					
					securityService.logIn()
					.then(function () {
						$scope.view = "following";
						showFollowing();
					});                
				}
			}else{
				$scope.view = view;
			}
			
		}
		function showFollowing(){
			var f = securityService.getCurrentUser().profile.followingIds;
			$scope.following = [];
			for(var i=0;i<f.length;i++){
				$scope.following.push({id:f[i].id, userName:f[i].userName,selected:true});
			}
		}
		function dialogsViewBegin(){
			angular.element("#view").addClass("blurry");
		}
		function allDialogsComplete(){
			angular.element("#view").removeClass("blurry");			
		}
		
		// Swap between trending or following depending on if the user is authenticated and previous choices
		securityService.testIsAuthenticated()
		.then(function(isAuthenticated){
			if(isAuthenticated===true){
				$scope.switchView("following");
			}else{
				$scope.switchView("trending");
			}
		});		
    }

    // Register the controller
    app.controller('homeCtrl', ["$scope", "$location", "$rootScope", "$dialog", "$timeout", "sharedDataService", "memeWizardService", "securityService", homeCtrl]);

})();