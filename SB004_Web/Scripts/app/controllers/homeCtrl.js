'use strict';
(function () {

    var homeCtrl = function ($scope, $location, $rootScope, $dialog, $timeout, sharedDataService, memeWizardService) {
        $scope.quotes = sharedDataService.data.quoteSearch.results;
        $scope.searchTerm = "";
        $scope.searchCategory = "";

        
        $scope.memeSelectConfirmImage = function () {
			
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

            $scope.quotes = sharedDataService.data.quoteSearch.results;

        });
        $scope.resized = function (width, height) {
            console.log(width + " X " + height);
        };
        // $scope.search();
        $scope.init = function () {
            $scope.handle = $dialog.dialog({});
        };

        $scope.init();
		
		function dialogsViewBegin(){
			angular.element("#view").addClass("blurry");
		}
		function allDialogsComplete(){
			angular.element("#view").removeClass("blurry");			
		}
    }

    // Register the controller
    app.controller('homeCtrl', ["$scope", "$location", "$rootScope", "$dialog", "$timeout", "sharedDataService", "memeWizardService", homeCtrl]);

})();