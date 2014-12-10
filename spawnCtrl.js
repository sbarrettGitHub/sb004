'use strict';
(function() {
  
  var spawnCtrl = function($scope, $location,$rootScope,$timeout, dialog, sharedDataService) {
		$scope.comment = {
			text:"Add text here and drag it into position",
			position: {
				align:"bottom",
				x:0,
				y:0
			},
			color:"black",
			backgroundColor:"none"
		}
		if(sharedDataService.data.seedImage){
			if(!sharedDataService.data.seedImage.id){
				$scope.seedImage=sharedDataService.data.seedImage;								
			}else{
				$scope.seedImage={
					id:null,
					image:'f4d493260.jpg',
					width:'auto',
					height:'auto'
				}
			}
		}

		$scope.closeMe = function(){
			dialog.close(false);
		};
  }
  
  // Register the controller
  app.controller('spawnCtrl', ["$scope","$location","$rootScope","$timeout", "dialog", "sharedDataService", spawnCtrl]);

})();