'use strict';
(function() {

    var newQuoteCtrl = function($scope, $timeout, $location,$http, fileReader, dialog, sharedDataService) {
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
	    var imagePreview = angular.element("#imagePreview");
	    $http.post('/api/Seed', {
	        id: null,
	        image: $scope.image,
	        width: imagePreview.width(),
	        height: imagePreview.height()
	    }).
        success(function (data, status, headers, config) {
            dialog.close(true);
        }).
        error(function (data, status, headers, config) {
            // called asynchronously if an error occurs
            // or server returns response with an error status.
        });
		
	};
	$scope.closeMe = function(){
		dialog.close(false);
	};
	$scope.imageSet = function(){
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
		sharedDataService.data.seedImage= {
			id:null,
			image:$scope.image,
			width: imagePreview.width(),
			height:imagePreview.height(),
		};
	}
  }

  // Register the controller
    app.controller('newQuoteCtrl', ["$scope", "$timeout", "$location", "$http", "fileReader", "dialog", "sharedDataService", newQuoteCtrl]);

})();