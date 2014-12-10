'use strict';
(function() {
  
  var spawnCtrl = function($scope, $location,$rootScope,$timeout, sharedDataService) {
		if(sharedDataService.data.seedImage){
			if(!sharedDataService.data.seedImage.id){
				$scope.seedImage=sharedDataService.data.seedImage;
				
				return;
			}
		}
		$scope.seedImage={
				id:null,
				image:'f4d493260.jpg',
				width:'auto',
				height:'auto'
		}
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
  app.controller('spawnCtrl', ["$scope","$location","$rootScope","$timeout", "sharedDataService", spawnCtrl]);

})();