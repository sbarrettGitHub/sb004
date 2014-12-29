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
			fontStyle:"italic",
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
		$scope.startAgain = function(){
			dialog.close("StartAgain");
		};
		$scope.reset = function(){
			dialog.close("Reset");		
		};
		$scope.startEdit = function(){
			$scope.editingComment = true;	
			angular.element('#comment').tooltip('destroy');			
		};
		$scope.endEdit = function(){
			$scope.editingComment = false;
		};
		$scope.startDrag = function(){
			angular.element('#comment').tooltip('destroy');			
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
		$scope.bold = function(){
			$scope.comment.fontWeight = $scope.comment.fontWeight == "bold"?"normal":"bold";
		};
		$scope.italic = function(){			
			$scope.comment.fontStyle = $scope.comment.fontStyle == "italic"?"normal":"italic";
		};
		$scope.underline = function(){
			$scope.comment.textDecoration = $scope.comment.textDecoration == "underline"?"none":"underline";
		};
		$scope.align = function(alignment){
			$scope.comment.textAlign = alignment;
		};
		
		$timeout(function(){
			angular.element('#comment').tooltip({placement: 'top',trigger: 'manual'}).tooltip('show');
		}, 1000);		
	}
  
  // Register the controller
  app.controller('spawnCtrl', ["$scope","$location","$rootScope","$timeout", "dialog", "sharedDataService", spawnCtrl]);

})();