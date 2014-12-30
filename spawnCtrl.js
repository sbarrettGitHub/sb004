'use strict';
(function() {
	  function Comment (id) {
		this.id = id;
		this.text = "Double click here to edit text ...";
		this.position =  {
			align:"bottom",
			x:0,
			y:0
		};
		this.color = "black";
		this.backgroundColor = "none";
		this.fontFamily = "Arial";			
		this.fontSize =  "10pt";
		this.fontWeight =  "bold";
		this.textDecoration =  "none";
		this.fontStyle = "normal";
		this.textAlign =  "center";	
		this.dropped = false;
		this.selected = false;
	}
	var spawnCtrl = function($scope, $location,$rootScope,$timeout, dialog, sharedDataService) {
		
		var intialComment = new Comment(0);
		$scope.comments = [intialComment];
		$scope.editingComment=false;
		$scope.comment = $scope.comments[0];
		$scope.selectedCommentId = 0;
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
		$scope.selectComment = function(id){			
			$scope.comment = $scope.comments[id];
			$scope.selectedCommentId = id;
			angular.element('#comment').tooltip('destroy');			
		};
		$scope.startEdit = function(id){
			$scope.editingComment = true;	
			$scope.comment = $scope.comments[id];
			$scope.selectedCommentId = id;
			angular.element('#comment').tooltip('destroy');			
		};
		$scope.endEdit = function(){
			$scope.editingComment = false;
			angular.element('#comment').tooltip('destroy');
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
		$scope.addComment = function(){			
			$scope.comments.push(new Comment($scope.comments.length));
		};
		$timeout(function(){
			angular.element('#comment').tooltip({placement: 'top',trigger: 'manual'}).tooltip('show');
		}, 1000);		
	}
  
  // Register the controller
  app.controller('spawnCtrl', ["$scope","$location","$rootScope","$timeout", "dialog", "sharedDataService", spawnCtrl]);

})();