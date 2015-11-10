'use strict';
(function () {

    var logInCtrl = function ($scope, dialog, hello, securityService, focus) {
		$scope.submitted = false;
		$scope.email="";
		$scope.password="";
		
		$scope.nameSignUp="";
		$scope.emailSignUp="";
		$scope.passwordSignUp="";
		$scope.conformPasswordSignUp="";
		
        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close("Fail");
        };
		$scope.view="SignIn";
		$scope.switchView = function(newView){
			$scope.view=newView;
			switch(newView){
				case "SignIn":
					focus('SignIn');
				break;
				case "SignUp":
					focus('SignUp');
				break;
				default:
					return;
			}
			
		}
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
		$scope.signUp = function(){
			$scope.submitted = true;
		}
		$scope.signUpError = function(){
			return (!$scope.nameSignUp || !!$scope.emailSignUp || !$scope.passwordSignUp 
			|| $scope.passwordSignUp!= $scope.confirmPasswordSignUp);
		}
		// Set focus to sign in email address
		focus('SignIn');
    }
  

    // Register the controller
    app.controller('logInCtrl', ["$scope", "dialog", "hello", "securityService", "focus", logInCtrl]);

})();