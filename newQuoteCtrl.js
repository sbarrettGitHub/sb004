'use strict';
(function() {

  var newQuoteCtrl = function($scope, $timeout, $location, fileReader, dialog) {
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
			  $timeout(function(){
				centerPreviewImage();
				}, 100);
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
	$scope.closeMe = function(){
		dialog.close(true);
	};
	$scope.centerImage = function(){
	$timeout(function(){
			centerPreviewImage()
		},100);
	};
	function centerPreviewImage(){
		var imageContainer = angular.element("#imageContainer");
		var imagePreview = angular.element("#imagePreview");
		var marginLeft = (imageContainer.width() - imagePreview.width())/2;
		var marginTop = (imageContainer.height() - imagePreview.height())/2;
		imagePreview.css({ 'margin-left': marginLeft });
		imagePreview.css({ 'margin-top': marginTop });
	}
  }

  // Register the controller
  app.controller('newQuoteCtrl', ["$scope", "$timeout", "$location", "fileReader", "dialog", newQuoteCtrl]);

})();