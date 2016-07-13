var app = angular.module('sb004', ['ngRoute', 'ui.bootstrap', 'ngHello', 'angular-jqcloud', 'ngSanitize'])
    .config(function ($routeProvider, $httpProvider, helloProvider, $locationProvider) {
        helloProvider.init({
            facebook: 224433161069107,
            google: 'myGoogleToken',
            twitter: 'myTwitterToken'
        });
        $routeProvider.
          when('/home', {
              templateUrl: 'Scripts/app/views/home.html',
              controller: 'homeCtrl'
          }).when('/meme/:id', {
              templateUrl: 'Scripts/app/views/memeView.html',
              controller: 'memeViewCtrl'
          }).when('/usermemes/:id', {
              templateUrl: 'Scripts/app/views/userMemes.html',
              controller: 'userMemesCtrl'
          }).when('/spawn/:id', {
              templateUrl: 'views/spawn.html',
              controller: 'memeApplyTextCtrl'
          }).when('/publish/:id', {
              templateUrl: 'views/publish.html',
              controller: 'memePublishAndShareCtrl'
          }).when('/resetpassword/:id', {
              templateUrl: 'Scripts/app/views/resetPassword.html',
              controller: 'resetPasswordCtrl'
          }).
          otherwise({
              redirectTo: '/home'
          });
		  //$locationProvider.html5Mode(true);
        $httpProvider.interceptors.push('authInterceptorService');       

    })
	.filter('nearestK', function() {
		return function(input) {
		  if (typeof input=="undefined") {
			return;
		  }
		  else {
			input = input+'';    // make string
			if (input < 1000) {
			  return input;		 // return the same number
			}
			if (input < 10000) { // place a comma between
			  return input.charAt(0) + ',' + input.substring(1);
			}
			
			// divide and format
			return (input/1000).toFixed(input % 1000 != 0)+'k';
		  }
		}
	})
	.run(function ()
    {
        // Do start up items here...
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
Array.prototype.contains = function(obj) {
    var i = this.length;
    while (i--) {
        if (this[i] === obj) {
            return true;
        }
    }
    return false;
}
