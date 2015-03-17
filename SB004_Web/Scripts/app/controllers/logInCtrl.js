'use strict';
(function () {

    var logInCtrl = function ($scope, dialog, hello, securityService) {
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
                
                securityService.connect(auth.network, auth.authResponse.access_token)
                    .then(function() {
                        dialog.close("Success");
                    })
                    .catch(function(e) {
                        dialog.close("Fail");
                    });

            });
            
        });

    }
  

    // Register the controller
    app.controller('logInCtrl', ["$scope", "dialog", "hello", "securityService", logInCtrl]);

})();