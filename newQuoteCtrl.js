'use strict';
(function() {

  var newQuoteCtrl = function($scope, $timeout, $location, fileReader) {
	$scope.image = null;
	$scope.imageFileName = null;
	$scope.file = null;
	$scope.loading = false;
	$scope.$on("Drop.Url",function(event, url){
		$timeout(function(){
			$scope.image = url;
			$scope.imageFileName = url;
		}, 100);		
	}); 
	$scope.getFile = function () {
		$scope.loading = true;
		$timeout(function(){
			fileReader.readAsDataUrl($scope.file, $scope)
			.then(function(result) {
			  $scope.image = result;
			  $scope.loading = false;
		  });
		}, 100);	
        
    };	
	$scope.$on("fileProgress", function(e, progress) {
        $scope.loading = true;
    });
	
	$scope.clear = function () {
		$scope.image = null;
		$scope.imageFileName = null;
		$scope.file = null;
		$scope.loading = false;
	};
	
	$scope.proceed = function () {
		$location.path("quote/yY7Y75G7U67");
	};
  }

  // Register the controller
  app.controller('newQuoteCtrl', ["$scope", "$timeout", "$location", "fileReader", newQuoteCtrl]);

})();