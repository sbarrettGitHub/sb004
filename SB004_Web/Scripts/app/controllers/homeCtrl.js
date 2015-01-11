'use strict';
(function() {
  
  var homeCtrl = function($scope, $location,$rootScope,$dialog,$timeout, sharedDataService) {
    $scope.quotes = sharedDataService.data.quoteSearch.results;
    $scope.searchTerm = "";
    $scope.searchCategory = "";
	
	var spawnDialog = $dialog.dialog({
		  backdrop: true,
		  keyboard: true,
		  backdropClick: false,
		  templateUrl: "/Scripts/app/views/spawn.html",
		  controller: "spawnCtrl"
		});
	var publishDialog = $dialog.dialog({
		  backdrop: true,
		  keyboard: true,
		  backdropClick: false,
		  templateUrl: "/Scripts/app/views/publish.html",
		  controller: "publishCtrl"
		});	
    $scope.addNewQuote = function(){
		//$location.path("new");
		angular.element("#view").addClass("blurry");
		var newQuoteDialog = $dialog.dialog({
		  backdrop: true,
		  keyboard: true,
		  backdropClick: false,
		  templateUrl: "/Scripts/app/views/newQuote.html",
		  controller: "newQuoteCtrl"
		});

		newQuoteDialog.open().then(function(proceed) {
			if(!proceed){
				angular.element("#view").removeClass("blurry");
				return;
			}
			$scope.spawn();			
		});
		
		$timeout(function(){
			angular.element('#pasteInput>input').focus();
		},400);
    }
	$scope.spawn = function(){
		spawnDialog.open().then(function(action) {
			if(action){
				if(action=="StartAgain"){
					$scope.addNewQuote();
					return;
				}
				if(action=="Reset"){
					$scope.spawn();
					return;
				}
				if(action=="Proceed"){
					$scope.publish();
					return;
				}					
			}	
			angular.element("#view").removeClass("blurry");		
		});
	}
	$scope.publish = function(){
		publishDialog.open().then(function(action) {
			if(action){
				if(action=="StartAgain"){
					$scope.addNewQuote();
					return;
				}								
			}	
			angular.element("#view").removeClass("blurry");		
		});
	}
    $rootScope.$on('quoteSearch.complete', function (event, data) {
      
       $scope.quotes = sharedDataService.data.quoteSearch.results;
       
    });
	$scope.resized = function(width, height){
		console.log(width + " X " + height);
	};
   // $scope.search();
    $scope.init = function() {
		$scope.handle = $dialog.dialog({});
	  };
  
	$scope.init();
  }
  
  // Register the controller
  app.controller('homeCtrl', ["$scope","$location","$rootScope","$dialog", "$timeout", "sharedDataService", homeCtrl]);

})();