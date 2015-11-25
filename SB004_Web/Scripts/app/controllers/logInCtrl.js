'use strict';
(function () {

    var logInCtrl = function ($scope, dialog, hello, securityService, focus,$window) {
		
		$scope.submitted = false;
		$scope.email="";
		$scope.password="";
		
		$scope.nameSignUp="";
		$scope.emailSignUp="";
		$scope.passwordSignUp="";
		$scope.conformPasswordSignUp="";
		$scope.termsAcceptedSignUp=false;
		
		$scope.submitError = "";
		
        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close("Fail");
        };
		$scope.view="SignIn";
		$scope.switchView = function(newView){
			$scope.submitted = false;
			$scope.view=newView;
			switch(newView){
				case "SignIn":
					focus('SignIn');
				break;
				case "SignUp":
					focus('SignUp');
				break;				
				case "ForgotPassword":
					focus('ForgotPassword');
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
			if(!$scope.emailSignUp || !$scope.passwordSignUp || !$scope.nameSignUp){
				return;
			}
			securityService.signUp($scope.nameSignUp, $scope.emailSignUp, $scope.passwordSignUp)
                    .then(function() {
                        dialog.close("Success");
                    })
                    .catch(function(e) {
                        dialog.close("Fail");
                    });
		}
		// --------------------------------------------------------------
		// Validate password strength
		$scope.passwordTooWeak = function(){
			return $scope.passwordTooShort() || $scope.passwordNeedsOneDigit();
		}
		$scope.passwordTooShort = function(){
			if($scope.submitted && $scope.passwordSignUp){
				if($scope.passwordSignUp.length < 6){
					return true;
				}
			}
		}
		$scope.passwordNeedsOneDigit = function(){
			return $scope.submitted && $scope.passwordSignUp.length >= 6 && $scope.passwordSignUp.match(/\d+/g) == null;
		}

		// --------------------------------------------------------------
		$scope.signIn = function(){
			$scope.submitted = true;
			if(!$scope.email || !$scope.password){
				return;
			}
			securityService.signIn($scope.email, $scope.password)
                    .then(function() {
                        dialog.close("Success");
                    })
                    .catch(function(e) {
                        $scope.submitError = "Invalid email address or password";
                    });			
			
		}
		$scope.resetPassword = function(){
			$scope.submitted = true;
		}
		$scope.signUpError = function(){
			return (!$scope.nameSignUp || !!$scope.emailSignUp || !$scope.passwordSignUp 
			|| $scope.passwordSignUp!= $scope.confirmPasswordSignUp);
		}
		$scope.showTerms = function(){
					
		}
		// Set focus to sign in email address
		focus('SignIn');
    }
  

    // Register the controller
    app.controller('logInCtrl', ["$scope", "dialog", "hello", "securityService", "focus", "$window", logInCtrl]);

})();