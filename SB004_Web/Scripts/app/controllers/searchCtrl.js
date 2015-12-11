'use strict'; 
(function() {

  var searchCtrl = function($scope,$rootScope, $location, $http, $q, sharedDataService, securityService, memeWizardService, blurry ) {
	$scope.userName = "";
	$scope.isAuthenticated = false;
	
	$scope.home = function() {
		$location.path("home");
	}
	/*---------------------------------------------------------*/
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
	/*---------------------------------------------------------*/
	$scope.addNew = function () {
			
			blurry("view", true);
			memeWizardService.begin()
			.then(
				function(newMemeId){
					blurry("view", false);
					
					// Move to new meme
					$location.path('/meme/' + newMemeId);
				},
				function(){
					// Cancelled
					blurry("view", false);
				}				
			);
        }
	/*---------------------------------------------------------*/
	$scope.signOut = function(){
		securityService.signOut();
	}
	$scope.signIn = function(){
		securityService.logIn();
	}
	/*---------------------------------------------------------*/
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
	/*-----------------------------------------------------------------*/
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
	/*-----------------------------------------------------------------*/
  }
  // Register the controller
  app.controller('searchCtrl', ["$scope","$rootScope","$location", "$http", "$q","sharedDataService", "securityService","memeWizardService", "blurry", searchCtrl]);

})();