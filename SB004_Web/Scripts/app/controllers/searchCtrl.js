'use strict'; 
(function() {

  var searchCtrl = function($scope,$rootScope, $location, $http, $q, sharedDataService, securityService ) {
	$scope.userName = "";
	$scope.isAuthenticated = false;
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

	function testAuthentication(){
		// Test if the user is signed in
		securityService.testIsAuthenticated()
		.then(function(isAuthenticated){
			$scope.isAuthenticated = isAuthenticated;
			$scope.userName = securityService.getCurrentUser().userName
		});	
	}
	// Set up the context when the user logs in
	$rootScope.$on('account.signIn', function (event, data) {
		testAuthentication();
	});
	// Set up the context when the user logs out
	$rootScope.$on('account.signOut', function (event, data) {
		testAuthentication();
	});
	// Test if the user is signed in
	testAuthentication();
  }
  // Register the controller
  app.controller('searchCtrl', ["$scope","$rootScope","$location", "$http", "$q","sharedDataService", "securityService", searchCtrl]);

})();