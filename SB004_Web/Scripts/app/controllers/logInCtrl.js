'use strict';
(function () {

    var logInCtrl = function ($scope, dialog, hello, securityService, focus,$window, view) {
		
		$scope.submitted = false;
		$scope.email="";
		$scope.password="";
		
		$scope.nameSignUp="";
		$scope.emailSignUp="";
		$scope.passwordSignUp="";
		$scope.conformPasswordSignUp="";
		$scope.termsAcceptedSignUp=false;
		
		$scope.resetSuccess = false;

		$scope.submitError = "";
		$scope.forgotPasswordError = "";
        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close("Fail");
        };
		$scope.view=view?view:securityService.signInOptions.signIn; //Defult to sign in if view not specified
		$scope.switchView = function(newView){
			$scope.submitted = false;
			$scope.view=newView;
			switch(newView){
				case securityService.signInOptions.signIn:
					focus('SignIn');
				break;
				case securityService.signInOptions.signUp:
					focus('SignUp');
				break;				
				case securityService.signInOptions.forgotPassword:
					$scope.resetSuccess = false;
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
			if(!$scope.emailSignUp || !$scope.passwordSignUp || !$scope.nameSignUp || $scope.passwordTooWeak()){
				return;
			}
			securityService.signUp($scope.nameSignUp, $scope.emailSignUp, $scope.passwordSignUp)
                    .then(function() {
                        dialog.close("Success");
                    },
					function(data, status, headers, config) {
						$window.alert("Failed to register user! \n" + data.message)
                    });
		}
		// --------------------------------------------------------------
		// Validate password strength
		$scope.passwordTooWeak = function(){
			if($scope.submitted){
				return !securityService.isPasswordStrongEnough($scope.passwordSignUp);
			}
			return false;
			
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
		$scope.forgotPassword = function(){
			$scope.forgotPasswordError = "";
			$scope.resetSuccess = false;
			if(!$scope.email){
				$scope.forgotPasswordError = "Invalid email address.";
				return;
			}
			startWaiting();
			securityService.forgotPassword($scope.email)
			.then(function(){
				endWaiting();
				$scope.submitted = true;
				$scope.resetSuccess = true;
			},function(status){
				 endWaiting();
				switch (status) {
					case 404:
						$scope.forgotPasswordError = "Email address entered is not registered. Please review and try again";
						break;
				
					default:
						$scope.forgotPasswordError = "Unable to issue reset password link at this time. Please try again later.";
						break;
				}
			});
			
		}
		$scope.signUpError = function(){
			return (!$scope.nameSignUp || !!$scope.emailSignUp || !$scope.passwordSignUp 
			|| $scope.passwordSignUp!= $scope.confirmPasswordSignUp);
		}
		$scope.showTerms = function(){
					
		}
		function startWaiting(heading, message) {
            $scope.waiting = true;
            $scope.waitHeading = !heading ? "Please wait..." : heading;
            $scope.waitingMessage = !message ? "" : message;
        }
        function endWaiting() {
            $scope.waiting = false;
            $scope.waitHeading = "";
            $scope.waitingMessage = "";
        }
		// Set focus based on the supplied intial view
		$scope.switchView($scope.view);

		
    }
  

    // Register the controller
    app.controller('logInCtrl', ["$scope", "dialog", "hello", "securityService", "focus", "$window", "view", logInCtrl]);

})();