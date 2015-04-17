'use strict';
(function() {

  var searchCtrl = function($scope,$rootScope, $location, $http, $q, sharedDataService ) {
    $scope.search = function() {
      $rootScope.$broadcast('quoteSearch.begin', null);
	  search().then(function (quotes) {
				   // Hold the quotes in shared data 
				  sharedDataService.data.quoteSearch.results = quotes;
				  
				  /// Search is complete
				  $rootScope.$broadcast('quoteSearch.complete',quotes);
				  
				  // redirect to home
				  $location.path("home");
			})
			.catch(function (e) {
				alert(e);
			});
		}
/*      var quotes = [];
      quotes = [
        dummyQuote("http://media-cache-ak0.pinimg.com/736x/2f/8d/bb/2f8dbba1cbbbd4a2f62a90b4c5371b37.jpg", "Woman", "Funny", "01/01/2014", 2000, '2 hours ago', 'Stevie Nicks', '2f8dbba1cbbbd4a2f62a90b4c5371b37'),
        dummyQuote("http://media-cache-ec0.pinimg.com/736x/32/7b/17/327b177290dc868f12bbed7385b29172.jpg", "Nice Kitty", "Funny", "01/01/2014", 1999, '4 hours ago', 'Donnie', '327b177290dc868f12bbed7385b29172'),
        dummyQuote("http://media-cache-ak0.pinimg.com/736x/d0/fa/2c/d0fa2ce3ab7fdf78ed5636ae4e3c8204.jpg", "", "Funny", "01/01/2014", 1870, 'Yesterday', 'Joan Smithie', 'd0fa2ce3ab7fdf78ed5636ae4e3c8204'),
        dummyQuote("http://media-cache-ak0.pinimg.com/736x/fc/9b/31/fc9b311f892a190b59750fc47d13e2a9.jpg", "On hold", "Funny", "01/01/2014", 1850, '2 days ago', 'Seamus Barrett', 'fc9b311f892a190b59750fc47d13e2a9'),
        dummyQuote("http://media-cache-ak0.pinimg.com/736x/7f/cc/4b/7fcc4bf75ac0245d1e282529f0f29179.jpg", "Deodorant", "Thoughtful", "01/01/2014", 1800, 'Last week', 'Paolo', '7fcc4bf75ac0245d1e282529f0f29179')
      ];
     */

    /*}
    var dummyQuote = function(url, desc, cat, dAdded, votes, when, by, id) {
      return {
        url: url,
        description: desc,
        category: cat,
        dateAdded: dAdded,
        votes: votes,
        when: when,
        by: by,
		id: id
      };
    }*/
	var search = function () {
		var deferred = $q.defer();
		startWaiting();
		$http.get('/api/Meme').
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