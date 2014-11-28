var app = angular.module('sb004', ['ngRoute']);

app.config(['$routeProvider',
  function($routeProvider) {
    $routeProvider.
      when('/home', {
        templateUrl: 'home.html',
        controller: 'homeCtrl'
      }).
      when('/new', {
        templateUrl: 'newQuote.html',
        controller: 'newQuoteCtrl'
      }).
      otherwise({
        redirectTo: '/home'
      });
  }]);


