var app = angular.module('sb004', ['ngRoute', 'ui.bootstrap']);

app.config(['$routeProvider',
  function($routeProvider) {
    $routeProvider.
      when('/home', {
        templateUrl: '/Scripts/app/views/home.html',
        controller: 'homeCtrl'
      }).when('/spawn/:id', {
        templateUrl: 'views/spawn.html',
        controller: 'memeApplyTextCtrl'
      }).when('/publish/:id', {
        templateUrl: 'views/publish.html',
        controller: 'memePublishAndShareCtrl'
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
