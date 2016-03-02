'use strict';
(function () {

    var homeCtrl = function ($scope, $location, $rootScope, $dialog, $timeout, sharedDataService, memeWizardService, securityService, blurry) {
        $scope.memes = sharedDataService.data.quoteSearch.results;
        $scope.searchTerm = "";
        $scope.searchCategory = "";
		$scope.view = "trending";
        $scope.following = [];
		$scope.viewing = [];
        $scope.addNew = function () {
			
			blurry("view", true);
			memeWizardService.begin()
			.then(
				function(newMemeId){
					// New meme added
					blurry("view", false);
					
					// Move to new meme
					$scope.viewMeme(newMemeId);
				},
				function(){
					// Cancelled
					blurry("view", false);
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
					securityService.logInDialog()
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
			if(securityService.getCurrentUser()){
				if(securityService.getCurrentUser().profile.following){
					$scope.following = securityService.getCurrentUser().profile.following;
				}
			}		
		}

		function testAuthentication(){
			// Swap between trending or following depending on if the user is authenticated and previous choices
			securityService.testIsAuthenticated()
			.then(function(isAuthenticated){
				if(isAuthenticated===true){
					$scope.switchView("following");
					showFollowing();
				}else{
					$scope.switchView("trending");
				}
			});
		}
		
		/*-----------------------------------------------------------------*/
		// Set up the context when the user logs in
		$rootScope.$on('account.signIn', function (event, data) {
			testAuthentication();
		});
		// Set up the context when the user logs out
		$rootScope.$on('account.signOut', function (event, data) {
			testAuthentication();
		});

		// Check if the user is authenticated, if so swap between trending and following
		testAuthentication();
		/*-----------------------------------------------------------------*/
			
    }

    // Register the controller
    app.controller('homeCtrl', ["$scope", "$location", "$rootScope", "$dialog", "$timeout", "sharedDataService", "memeWizardService", "securityService", "blurry", homeCtrl]);

})();