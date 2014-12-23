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
			fontSize: "10pt",
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
		
		$scope.over=function(el, target){
			target.addClass("hoverOver");
		};
		$scope.out=function(el, target){
			target.removeClass("hoverOver");
		};
		$scope.fontSize = function(size){
			$scope.comment.fontSize = size + "pt";
		};
		$scope.backColor = function(color){
			$scope.comment.backgroundColor = color;
		};
		$scope.color = function(color){
			$scope.comment.color = color;
		};		
		$scope.fontFamily = function(fontFamily){
			$scope.comment.fontFamily = fontFamily;
		};
	}
  
  // Register the controller
  app.controller('spawnCtrl', ["$scope","$location","$rootScope","$timeout", "dialog", "sharedDataService", spawnCtrl]);

})();