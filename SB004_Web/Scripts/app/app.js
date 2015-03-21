var app = angular.module('sb004', ['ngRoute', 'ui.bootstrap', 'ngHello', 'LocalStorageModule'])
    .config(function ($routeProvider, $httpProvider, helloProvider) {
        helloProvider.init({
            facebook: 224433161069107,
            google: 'myGoogleToken',
            twitter: 'myTwitterToken'
        });
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
        $httpProvider.interceptors.push('authInterceptorService');

    })
    .run(function (securityService)
    {
        securityService.testStillLoggedIn();
    }); 

app.directive("ngFileSelect", function () {

    return {
        link: function ($scope, el) {

            el.bind("change", function (e) {

                $scope.file = (e.srcElement || e.target).files[0];
                $scope.getFile();
            });

        }

    }
});
