'use strict';
(function() {

  var searchCtrl = function($scope,$rootScope, $location, $http, $q, sharedDataService ) {
    $scope.search = function() {
      $rootScope.$broadcast('quoteSearch.begin', null);
	  search().then(function (quotes) {
				   // Hold the quotes in shared data 
				  sharedDataService.data.quoteSearch.results = quotes;
				  
				  // Search is complete
				  $rootScope.$broadcast('quoteSearch.complete',quotes);
				  
				  // redirect to home
				  $location.path("home");
			})
			.catch(function (e) {
				alert(e);
			});
		}
	var search = function () {
		var deferred = $q.defer();
		startWaiting();
		$http.get('api/Meme').
			success(function (data) {
				endWaiting();
				deferred.resolve(data);
			}).
			error(function (e) {
				endWaiting();
				deferred.reject(e);
			});
		return deferred.promise;
	};
	function startWaiting(){

	}
	function endWaiting(){

	}
  }
  // Register the controller
  app.controller('searchCtrl', ["$scope","$rootScope","$location", "$http", "$q","sharedDataService", searchCtrl]);

})();