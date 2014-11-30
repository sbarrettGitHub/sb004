'use strict';
(function() {

  var newQuoteCtrl = function($scope, $timeout) {
	$scope.image = null;
	$scope.imageFileName = null;
	$scope.$on("Drop.Url",function(event, url){
		$timeout(function(){
			$scope.image = url;
			$scope.imageFileName = url;
		}, 100);		
	}); 	
  }

  // Register the controller
  app.controller('newQuoteCtrl', ["$scope", "$timeout", newQuoteCtrl]);

})();