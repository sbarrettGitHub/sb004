var app = angular.module('sb004', ['ngRoute', 'ui.bootstrap']);

app.config(['$routeProvider',
  function($routeProvider) {
    $routeProvider.
      when('/home', {
        templateUrl: 'home.html',
        controller: 'homeCtrl'
      }).when('/quote/:id', {
        templateUrl: 'quote.html',
        controller: 'quoteCtrl'
      }).when('/spawn/:id', {
        templateUrl: 'spawn.html',
        controller: 'spawnCtrl'
      }).when('/publish/:id', {
        templateUrl: 'publish.html',
        controller: 'publishCtrl'
      }).
      otherwise({
        redirectTo: '/home'
      });
  }]);

app.directive("ngFileSelect",function(){

  return {
    link: function($scope,el){
      
      el.bind("change", function(e){
      
        $scope.file = (e.srcElement || e.target).files[0];
        $scope.getFile();
      })
      
    }
    
  }
})
