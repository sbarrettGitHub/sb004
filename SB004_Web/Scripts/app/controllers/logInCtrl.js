'use strict';
(function () {

    var logInCtrl = function ($scope, dialog, hello) {
        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close("Fail");
        };

        // Sign in using social media
        $scope.login = function (provider) {
            hello(provider).login();
        }

        hello.on("auth.login", function (auth) {
            // call user information, for the given network
            hello(auth.network).api('/me').success(function (profile) {

                var x = profile;
                alert("Hello " + profile.name);

            });
            
        });

    }
  

    // Register the controller
    app.controller('logInCtrl', ["$scope", "dialog", "hello", logInCtrl]);

})();