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
        
        $scope.profileName = currentUser.userName;
        $scope.changeProfileName =currentUser.userName;
        $scope.showChangeImage = false;
        $scope.changeProfileNameValid = true;
        
        $scope.profileEmail = currentUser.profile.email;
        $scope.changeProfileEmail = currentUser.profile.email;
        $scope.confirmChangeProfileEmail= currentUser.profile.email;
        $scope.changeProfileEmailValid = true;
        $scope.changeProfileEmailError = "";
        $scope.showChangeEmail = false;
        
        $scope.existingProfilePassword = "";
        $scope.changeProfilePassword = "";
        $scope.confirmChangeProfilePassword = "";
        $scope.changePasswordValid = true;
        $scope.changePasswordError = "";
        
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
                    $scope.showChangeImage = false;
                    deferred.resolve();
                }).error(function (e) {
					alert(e);
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
                $scope.profileName = $scope.changeProfileName;    
            }
           
        }
        $scope.resetProfileName = function(){
           $scope.showChangeName = false;
           $scope.changeProfileName = currentUser.userName;
           $scope.changeProfileNameValid = true;
        }
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
            $scope.showChangeEmail = false;
            $scope.profileEmail = $scope.changeProfileEmail;
        }
        $scope.resetProfileEmail = function(){
            $scope.changeProfileEmailValid = true;
            $scope.showChangeEmail = false;
            $scope.changeProfileEmail = currentUser.profile.email;
            $scope.confirmChangeProfileEmail= currentUser.profile.email;
        }
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
        
            $scope.showChangePassword = false;
        }
        $scope.resetProfilePasswordChange = function(){
            $scope.changePasswordValid = true;
            $scope.showChangePassword = false;
            $scope.existingProfilePassword = "";
            $scope.changeProfilePassword= "";
            $scope.confirmChangeProfilePassword= "";
        }
        
        $scope.closeAccount= function(){
            if($window.confirm("If you continue with this option, you will no longer be able to sign into SB004.\n\n *** This action cannot be undone. **** \n\n Are you sure you wish to close this account? ")){
                
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