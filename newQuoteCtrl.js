'use strict';
(function() {

  var newQuoteCtrl = function($scope, $timeout, fileReader) {
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
  }

  // Register the controller
  app.controller('newQuoteCtrl', ["$scope", "$timeout", "fileReader", newQuoteCtrl]);

})();