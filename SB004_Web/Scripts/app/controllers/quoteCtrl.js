'use strict';
(function() {

  var quoteCtrl = function($scope) {
	$scope.image = null;	
  }

  // Register the controller
  app.controller('quoteCtrl', ["$scope", quoteCtrl]);

})();