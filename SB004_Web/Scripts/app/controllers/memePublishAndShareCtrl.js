'use strict';
(function() {

	var memePublishAndShareCtrl = function($scope, $location,$rootScope,$timeout,$window, dialog, sharedDataService) {
		
		$scope.meme = sharedDataService.data.meme;
		
		/*Control buttons*/
		$scope.closeMe = function(){
			dialog.close(false);
		};
		$scope.startAgain = function(){
			dialog.close("StartAgain");
		};
		$scope.changeMeme = function(){
			dialog.close("ChangeMeme");
		};
		
		$scope.proceed = function(){
			dialog.close("Proceed");
		};
				
	}
  
  // Register the controller
  app.controller('memePublishAndShareCtrl', ["$scope","$location","$rootScope","$timeout","$window", "dialog", "sharedDataService", memePublishAndShareCtrl]);

})();