'use strict';
(function() {
  
  var homeCtrl = function($scope, $location,$rootScope,$dialog, sharedDataService) {
    $scope.quotes = sharedDataService.data.quoteSearch.results;
    $scope.searchTerm = "";
    $scope.searchCategory = "";
  
    $scope.addNewQuote = function(){
		//$location.path("new");
		angular.element("#view").addClass("blurry");
		var newQuoteDialog = $dialog.dialog({
		  backdrop: true,
		  keyboard: true,
		  backdropClick: false,
		  templateUrl: "newQuote.html",
		  controller: "newQuoteCtrl"
		});
		
		newQuoteDialog.open().then(function(proceed) {
			if(!proceed){
				angular.element("#view").removeClass("blurry");
				return;
			}
			var spawnDialog = $dialog.dialog({
			  backdrop: true,
			  keyboard: true,
			  backdropClick: false,
			  templateUrl: "spawn.html",
			  controller: "spawnCtrl"
			});
			spawnDialog.open().then(function() {
				angular.element("#view").removeClass("blurry");
			});
		});
    }

    $rootScope.$on('quoteSearch.complete', function (event, data) {
      
       $scope.quotes = sharedDataService.data.quoteSearch.results;
       
    });
   // $scope.search();
    $scope.init = function() {
		$scope.handle = $dialog.dialog({});
	  };
  
	$scope.init();
  }
  
  // Register the controller
  app.controller('homeCtrl', ["$scope","$location","$rootScope","$dialog", "sharedDataService", homeCtrl]);

})();