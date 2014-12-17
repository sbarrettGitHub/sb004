'use strict';
(function() {
  
	var spawnCtrl = function($scope, $location,$rootScope,$timeout, dialog, sharedDataService) {
		$scope.editingComment=false;
		$scope.comment = {
			text:"Click here to add your text ...",
			position: {
				align:"bottom",
				x:0,
				y:0
			},
			color:"black",
			backgroundColor:"none",
			fontFamily:"Arial",			
			fontSize: "15pt",
			fontWeight: "bold",
			textDecoration: "none",
			textAlign: "center"			
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
		$scope.startEdit = function(){
			$scope.editingComment = true;
		};
		$scope.endEdit = function(){
			$scope.editingComment = false;
		};
		
		$scope.toggleBold = function(){			
			$scope.comment.fontWeight = $scope.comment.fontWeight == "bold" ? "normal" : "bold";			
		};
	}
  
  // Register the controller
  app.controller('spawnCtrl', ["$scope","$location","$rootScope","$timeout", "dialog", "sharedDataService", spawnCtrl]);

})();