'use strict';
(function () {

    var profileCtrl = function ($scope, $http, $q, dialog, securityService, $window) {
        
        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";
        
        var currentUser = securityService.getCurrentUser();
        $scope.id = currentUser.userId;	
        $scope.nativeAuthentication = (currentUser.provider.length==0);
        $scope.authenticationProvider = (currentUser.provider.length==0) ? "SB004" : currentUser.provider;
         
        $scope.profileImageLink = "api/image/user/" + currentUser.userId;
        $scope.changeProfileImageLink = "";
        $scope.showChangeImage = false;
        
        $scope.profileMessage = currentUser.statusMessage;
        $scope.changeProfileMessage =currentUser.statusMessage;
        $scope.showChangeMessage = false;
        $scope.changeProfileMessageValid = true;
                
        $scope.profileName = currentUser.userName;
        $scope.changeProfileName =currentUser.userName;
        $scope.showChangeImage = false;
        $scope.changeProfileNameValid = true;
        
        $scope.profileEmail = currentUser.profile.email;
        $scope.changeProfileEmail = ""
        $scope.confirmChangeProfileEmail= "";
        $scope.changeProfileEmailValid = true;
        $scope.changeProfileEmailError = "";
        $scope.showChangeEmail = false;
        
        $scope.existingProfilePassword = "";
        $scope.changeProfilePassword = "";
        $scope.confirmChangeProfilePassword = "";
        $scope.changePasswordValid = true;
        $scope.changePasswordError = "";
        $scope.showChangePassword = false;
        
        $scope.closeAccountPasswordVerification = "";
        $scope.closeAccountValid = true;
        $scope.closeAccountError = "";
        $scope.showCloseAccount = false;
        
		/*Control buttons*/
        $scope.closeMe = function () {
            dialog.close();
        };        
        /*-----------------------------------------------------------------*/
        $scope.previewProfileImage = function(){
            $scope.profileImageLink = $scope.changeProfileImageLink;            
        }	
        $scope.clearProfileImage = function(){
            $scope.profileImageLink = "content/images/avatar.gif";
            $scope.saveProfileImage().then(function(){
                //$scope.resetProfileImage();
            });
                        
        }
		$scope.saveProfileImage = function(){
            var deferred = $q.defer();
            $http({ method: 'POST', url: 'api/image/user/' + $scope.id , data: {
                id:$scope.id,
                imageUrl: $scope.changeProfileImageLink
            }})
				.success(function (data) { 
                    $window.alert("Your profile image has been changed!");       
                    $scope.showChangeImage = false;
                    deferred.resolve();
                }).error(function (e) {
					$window.alert(e);
                    deferred.reject();
					return;
                });              
            return deferred.promise;
        }
        $scope.resetProfileImage = function(){
            $scope.showChangeImage=false;
            $scope.profileImageLink = "api/image/user/" + currentUser.userId;
            $scope.changeProfileImageLink = "";
        }
        /*-----------------------------------------------------------------*/
        
        $scope.saveProfileName = function(){
            $scope.changeProfileNameValid = $scope.changeProfileName.length>0;
            if($scope.changeProfileName.length == 0 ){
                $scope.changeProfileNameValid = false;
                $scope.changeProfileNameError = "Please supply a new profile name!";
                return;
            }
            if($scope.changeProfileName == $scope.profileName ){
                $scope.changeProfileNameValid = false;
                $scope.changeProfileNameError = "Profile name hasn't changed!";
                return;
            }            
            if($scope.changeProfileNameValid){
                var deferred = $q.defer();
                $http({ method: 'PATCH', url: 'api/account/' + $scope.id + '/name', data: {
                    userName: $scope.changeProfileName
                }})
                .success(function (data) { 
                    $window.alert("Your profile name has been changed!");       
                    $scope.showChangeName = false;
                    $scope.profileName = $scope.changeProfileName; 
                    securityService.updateUserName($scope.profileName); 
                    deferred.resolve();
                }).error(function (e) {
                    $window.alert(e);
                    return;
                });
                
            }
           
        }
        $scope.resetProfileName = function(){
           $scope.showChangeName = false;
           $scope.changeProfileName = currentUser.userName;
           $scope.changeProfileNameValid = true;
        }
        /*-----------------------------------------------------------------*/
        
        $scope.saveProfileMessage = function(){
            $scope.changeProfileMessageValid = $scope.changeProfileMessage.length>0;
            if($scope.changeProfileMessage.length == 0 ){
                $scope.changeProfileMessageValid = false;
                $scope.changeProfileMessageError = "Please supply a new profile Message!";
                return;
            }
            if($scope.changeProfileMessage == $scope.profileMessage ){
                $scope.changeProfileMessageValid = false;
                $scope.changeProfileMessageError = "Profile Message hasn't changed!";
                return;
            }            
            if($scope.changeProfileMessageValid){
                var deferred = $q.defer();
                $http({ method: 'PATCH', url: 'api/account/' + $scope.id + '/status', data: {
                    statusMessage: $scope.changeProfileMessage
                }})
                .success(function (data) { 
                    $window.alert("Your profile Message has been changed!");       
                    $scope.showChangeMessage = false;
                    $scope.profileMessage = $scope.changeProfileMessage; 
                    deferred.resolve();
                }).error(function (e) {
                    $window.alert(e);
                    return;
                });
                
            }
           
        }
        $scope.resetProfileMessage = function(){
           $scope.showChangeMessage = false;
           $scope.changeProfileMessage = currentUser.statusMessage;
           $scope.changeProfileNameValid = true;
        }        
        /*-----------------------------------------------------------------*/        
        $scope.saveProfileEmail = function(){
            $scope.changeProfileEmailValid = true;
            if($scope.changeProfileEmail.length == 0 ){
                $scope.changeProfileEmailValid = false;
                $scope.changeProfileEmailError = "Please supply a new email address";
                return;
            }
            if($scope.confirmChangeProfileEmail.length == 0 ){
                $scope.changeProfileEmailValid = false;
                $scope.changeProfileEmailError = "Please confirm new email address";
                return;
            }
            if($scope.changeProfileEmailPasswordVerification.length == 0 ){
                $scope.changeProfileEmailValid = false;
                $scope.changeProfileEmailError = "Please enter your password to confirm new email address";
                return;
            }
            if($scope.changeProfileEmail != $scope.confirmChangeProfileEmail){
                $scope.changeProfileEmailValid = false;
                $scope.changeProfileEmailError = "Email addresses do not match";
                return;
            }
            if($scope.changeProfileEmail == $scope.profileEmail){
                $scope.changeProfileEmailValid = false;
                $scope.changeProfileEmailError = "Email address hasn't changed!";
                return;
            }
            $http({ method: 'PATCH', url: 'api/account/' + $scope.id + '/email', data: {
                    email: $scope.changeProfileEmail,
                    password: $scope.changeProfileEmailPasswordVerification
                }})
                .success(function (data) { 
                    $window.alert("Your email address has been changed!");       
                    $scope.showChangeEmail = false;
                    $scope.profileEmail = $scope.changeProfileEmail;
                    return;
                }).error(function (e) {
                    $scope.changeProfileEmailValid = false;
                    $scope.changeProfileEmailError = "Could not change email. Invalid credentials or unknown user!";
                    return;
                });
            
        }
        $scope.resetProfileEmail = function(){
            $scope.changeProfileEmailValid = true;
            $scope.showChangeEmail = false;
            $scope.changeProfileEmail = currentUser.profile.email;
            $scope.confirmChangeProfileEmail= currentUser.profile.email;
        }
        /*-----------------------------------------------------------------*/       
        $scope.saveProfilePassword = function(){
            $scope.changePasswordValid = true;
            if($scope.existingProfilePassword.length == 0 ){
                $scope.changePasswordValid = false;
                $scope.changePasswordError = "Please supply your existing password!";
                return;
            }
            if($scope.changeProfilePassword.length == 0 ){
                $scope.changePasswordValid = false;
                $scope.changePasswordError = "Please supply a new password!";
                return;
            }
            if($scope.confirmChangeProfilePassword.length == 0 ){
                $scope.changePasswordValid = false;
                $scope.changePasswordError = "Please confirm your new password!";
                return;
            }    
            if($scope.changeProfilePassword != $scope.confirmChangeProfilePassword){
                $scope.changePasswordValid = false;
                $scope.changePasswordError = "New password and confirm password do not match!";
                return;
            }  
            if($scope.changeProfilePassword.length <6 ){
                $scope.changePasswordValid = false;
                $scope.changePasswordError = "New password is too short (min 6)!";
                return;
            }   
            if($scope.changeProfilePassword.match(/\d+/g) == null   ){
                $scope.changePasswordValid = false;
                $scope.changePasswordError = "New password needs at lease 1 digit!";
                return;
            }             
            if($scope.changeProfilePassword){
                $http({ method: 'PATCH', url: 'api/account/' + $scope.id + '/password', data: {
                    email: currentUser.profile.email,
                    password:$scope.existingProfilePassword,
                    newPassword: $scope.changeProfilePassword
                }})
                .success(function (data) {  
                    $window.alert("Your password has been changed!");                  
                    $scope.resetProfilePasswordChange();
                }).error(function (e) {
                    $scope.changePasswordValid = false;
                    $scope.changePasswordError = "Cannot update password. Invalid user credentials or user unknown. ";
                    return;
                });
                
            }
        }
        $scope.resetProfilePasswordChange = function(){
            $scope.changePasswordValid = true;
            $scope.showChangePassword = false;
            $scope.existingProfilePassword = "";
            $scope.changeProfilePassword= "";
            $scope.confirmChangeProfilePassword= "";
        }
        /*-----------------------------------------------------------------*/       
        $scope.closeAccount= function(){
            if($window.confirm("If you continue with this option, you will no longer be able to sign into SB004.\n\n *** This action cannot be undone. **** \n\n Are you sure you wish to close this account? ")){
                $scope.closeAccountValid = true;
                if($scope.closeAccountPasswordVerification.length == 0 ){
                    $scope.closeAccountValid = false;
                    $scope.closeAccountError = "Please enter your password to confirm that you wish to close your account";
                    return;
                }
                $http({ method: 'POST', url: 'api/account/' + $scope.id + '/close', data: {
                        email: currentUser.profile.email,
                        password: $scope.closeAccountPasswordVerification
                    }})
                    .success(function (data) { 
                        $window.alert("Your account has been closed!");       
                        $scope.showCloseAccount = false;
                        securityService.signOut();
                        $scope.closeMe();
                        return;
                    }).error(function (e) {
                        $window.alert("Your account has not been closed! Please check your password."); 
                        return;
                    });
                
            }
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
    }
  
    // Register the controller
    app.controller('profileCtrl', ["$scope", "$http","$q",  "dialog", "securityService", "$window", profileCtrl]);

})();