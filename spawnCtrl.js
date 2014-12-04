'use strict';
(function() {
  
  var spawnCtrl = function($scope, $location,$rootScope, sharedDataService) {
       $scope.image='f4d493260.jpg';
  }
  
  // Register the controller
  app.controller('spawnCtrl', ["$scope","$location","$rootScope", "sharedDataService", spawnCtrl]);

})();