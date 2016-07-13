'use strict';
(function () {

    var resetPasswordCtrl = function ($scope, $routeParams, securityService, $window, $location) {
        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";	
        $scope.email = "";
        $scope.newPassword = "";
        $scope.confirmPassword = "";
        $scope.serverError = "";
        $scope.submitted = false;
        var resetToken = $routeParams.id;

		/*Control buttons*/
        $scope.resetPassword = function () {
            $scope.submitted = true;
             $scope.serverError = "";

             if(!$scope.newPassword || !$scope.confirmPassword){
                 $scope.serverError = "All fields are required!";
                 return;
             }
             if($scope.newPassword !=  $scope.confirmPassword){
                 $scope.serverError = "Passwords do not match!";
                 return;
             }
             if(securityService.isPasswordStrongEnough($scope.newPassword) == false){
                $scope.serverError = "New password must be at least 6 characters long and contain at least 1 number!";
                 return;
             }
             securityService.resetPassword($scope.newPassword, resetToken)
             .then(function(data){
                $window.alert("Password successfully changed!");
                
                securityService.logInDialog()
                .then(function(){
                    $location.path("home");
                },function(){
                    $location.path("home");
                });
             },
             function(status) {
                 switch (status) {
                     case 404:
                          $scope.serverError = "The password reset link you are using is invalid.";
                           return;                       
                    case 410:
                          $scope.serverError = "The ability to reset this password has expired. Please choose Forgot Password to receive a new password reset link.";
                           return; 
                     default:
                         $scope.serverError = "Unable to reset account password at this time.";
                           return; 
                 }
               
             })
        };
		
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
		
    }
  
    // Register the controller
    app.controller('resetPasswordCtrl', ["$scope", "$routeParams", "securityService", "$window", "$location", resetPasswordCtrl]);

})();