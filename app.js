var app = angular.module('sb004', ['ngRoute']);

app.config(['$routeProvider',
  function($routeProvider) {
    $routeProvider.
      when('/home', {
        templateUrl: 'home.html',
        controller: 'homeCtrl'
      }).when('/new', {
        templateUrl: 'newQuote.html',
        controller: 'newQuoteCtrl'
      }).when('/quote/:id', {
        templateUrl: 'quote.html',
        controller: 'quoteCtrl'
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
