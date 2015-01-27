'use strict';
(function() {

	var memePublishAndShareCtrl = function($scope, $location,$rootScope,$timeout,$window, dialog, sharedDataService) {
		
		/*Control buttons*/
		$scope.closeMe = function(){
			dialog.close(false);
		};
		$scope.startAgain = function(){
			dialog.close("StartAgain");
		};
		
		$scope.proceed = function(){
			dialog.close("Proceed");
		};
				
	}
  
  // Register the controller
  app.controller('memePublishAndShareCtrl', ["$scope","$location","$rootScope","$timeout","$window", "dialog", "sharedDataService", memePublishAndShareCtrl]);

})();