﻿'use strict';
(function () {
    var securityService = function ($http, $q, $dialog, $window, $timeout, $rootScope, blurry) {
		var signInOptions={
			signIn:"SignIn",
			signUp:"SignUp",
			forgotPassword:"ForgotPassword",
			socialMedia:"SocialMedia"
			
		}
        var currentUser = {
            isAuthenticated: false,
            userName: "",
            userId: "",
            accessToken: "",
            provider: "",
            thumbnail: "",
            profile: {},
			myCommentLikes : [],
			myCommentDislikes : [],
			myMemeLikes : [],
			myMemeDislikes : []
        }
        var loginDialogOpts = {
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "Scripts/app/views/logIn.html?t="  + new Date().getTime(),
            controller: "logInCtrl",
            dialogClass: 'loginModal modal'
        };		
        var loginDialog = $dialog.dialog({
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "Scripts/app/views/logIn.html",
            controller: "logInCtrl"
        });

        var connect = function (provider, acessToken) {
            var deferred = $q.defer();
            $http.get('api/Account/ObtainLocalAccessToken?provider=' + provider + '&externalAccessToken=' + acessToken).
            success(function (data) {

                addUserProfileToStorage(data);               

                deferred.resolve(data);
            }).
            error(function () {
                deferred.reject();
            });

            return deferred.promise;
        }
		var testIsAuthenticated = function () {
			var deferred = $q.defer();
			var authData;
			if( $window.Storage ){
				if(getCurrentUser().isAuthenticated == true){
					$timeout(function() {
							deferred.resolve(true);
					}, 1);
				}else{
					var data = $window.sessionStorage.getItem( 'SB004.authorizationData' );
					if(data){
						authData = $window.JSON.parse(data);
						currentUser.isAuthenticated = true;
						currentUser.userId = authData.token.userId;
						currentUser.userName = authData.token.userName;
						currentUser.accessToken = authData.token.access_token;
						currentUser.profile = authData.userData.profile;	
						$timeout(function() {
							deferred.resolve(true);
						}, 100);
												
					}else{
						$timeout(function() {
							deferred.resolve(false);
						}, 100);
					}
				}	
			}else{
				$timeout(function() {
					deferred.resolve(false);
				}, 100);
			}
			return deferred.promise;
		}
		
        var showLogInDialog = function (view, withBlurryBackground) {
			if(withBlurryBackground === true){
				blurry("view", true);
			}
            var deferred = $q.defer();
			loginDialogOpts.resolve = {view : function() {return view?view:"SignIn";}};
			
            $dialog.dialog(loginDialogOpts).open()
                .then(function (action) {
                    if (action == "Success") {                        
						if(withBlurryBackground === true){
							blurry("view", false);
						}
						deferred.resolve();
                        return;
                    }
                    if (action == "Fail") {
						if(withBlurryBackground === true){
							blurry("view", false);
						}						
                        deferred.reject();
                        return;
                    }
                });

            return deferred.promise;
        }
		
		function addUserProfileToStorage(data){
			currentUser.isAuthenticated = true;
			currentUser.userId = data.token.userId;
			currentUser.userName = data.token.userName;
			currentUser.accessToken = data.token.access_token;
			currentUser.profile = data.profile;
			
			// Add bearer token to session storage for inclusion with future requests to the server
			try{
				if( $window.Storage ){
					$window.sessionStorage.setItem( 'SB004.authorizationData', $window.JSON.stringify({
						token: data.token,
						refreshToken: "",
						useRefreshTokens: false,
						userData: currentUser
					}));
					
					// Broadcast that the user has signed in
					$rootScope.$broadcast('account.signIn', null);
				} 
			}catch( error ){
			  $window.alert(error.message );
			}			
		}
		function updateUserProfileInStorage(updatedProfileData){	
			try{
				if( $window.Storage ){
					var storageData = $window.sessionStorage.getItem( 'SB004.authorizationData' );
					if(storageData){
						var authData = $window.JSON.parse(storageData);
						// Update the profile data
						authData.userData.profile = updatedProfileData;	
						
						$window.sessionStorage.setItem( 'SB004.authorizationData', $window.JSON.stringify(authData));												
					}
					
					
					// Broadcast that the user has updated profile
					$rootScope.$broadcast('account.profileUpdate', updatedProfileData);
				} 
			}catch( error ){
			  $window.alert(error.message );
			}			
		}		
		var signUp = function(userName, email, password){
			var deferred = $q.defer();
			$http( { 
					method: 'POST', 
					url: 'api/account/RegisterNewUser', 
					data: {
							userName:userName,
							email:email,
							password:password
					}
				})
			.success(function (data) {
                addUserProfileToStorage(data)
                deferred.resolve();
            }).
            error(function (data, status, headers, config) {
                deferred.reject(data, status, headers, config);
            });

            return deferred.promise;
		}
		var signIn = function(email, password){
			var deferred = $q.defer();
			$http( { 
					method: 'POST', 
					url: 'api/account/SignIn', 
					data: {
							email:email,
							password:password
					}
				})
			.success(function (data) {
                addUserProfileToStorage(data)
                deferred.resolve();
            }).
            error(function (e) {
                deferred.reject(e);
            });

            return deferred.promise;
		}		
		var signOut = function(){
			currentUser.isAuthenticated = false;
			currentUser.userName = "";
			currentUser.userId = "";
			currentUser.accessToken = "";
			currentUser.provider = "";
			currentUser.thumbnail = "";
			currentUser.profile = {};

			// Remove bearer token from session storage 
			try{
				if( $window.Storage ){
					$window.sessionStorage.removeItem( 'SB004.authorizationData');
					
					// Broadcast that the user has signed out
					$rootScope.$broadcast('account.signOut', null);
				} 
			}catch( error ){
			  $window.alert(error.message );
			}	
		}
		var follow = function(followId){
			var deferred = $q.defer();
			if(currentUser.isAuthenticated==false){
				// Log in
				showLogInDialog()
					.then(function(){
						// Follow
						followUnfollowUser(followId, true)
						.then(function(data){
							// Broadcast that the user has updated his following list
							$rootScope.$broadcast('account.followingUpdate', null);
							deferred.resolve(data);
						});
					});
			}else{			
				// Follow
				followUnfollowUser(followId, true)
				.then(function(data){
					// Broadcast that the user has updated his following list
					$rootScope.$broadcast('account.followingUpdate', null);
					deferred.resolve(data);
				});
			}
			return deferred.promise;
		}
		var unfollow = function(followId){
			var deferred = $q.defer();
			if(currentUser.isAuthenticated==false){
				// Log in
				showLogInDialog()
					.then(function(){
						// Unfollow
						followUnfollowUser(followId, false)
						.then(function(data){						
							deferred.resolve(data);
						});
					});
			} else{			
				// Unfollow
				followUnfollowUser(followId, false)
				.then(function(data){
					deferred.resolve(data);
				});
			}
			return deferred.promise;
		}
		function followUnfollowUser(followId, follow)
		{
			var deferred = $q.defer();
			if(follow == true){
				$http({ method: 'PATCH', url:'api/Account/' + currentUser.userId + '/follow/' + followId, data: {}}).
				success(function (data) {       
					// Refesh the current user profile				
					currentUser.profile = data; 
					updateUserProfileInStorage(data);  
					deferred.resolve(data);
				}).
				error(function () {
					deferred.reject();
				});
			}else{
				$http({ method: 'PATCH', url:'api/Account/' + currentUser.userId + '/unfollow/' + followId, data: {}}).
				success(function (data) {       
					// Refesh the current user profile				
					currentUser.profile = data;   
					updateUserProfileInStorage(data);    
					deferred.resolve(data);
				}).
				error(function () {
					deferred.reject();
				});			
			}
			return deferred.promise; 
		}
		
		var isFollowing = function(followId){

			if(currentUser.isAuthenticated==false || !currentUser.profile || !currentUser.profile.followingIds){
				return false;
			}
			for(var i=0;i<currentUser.profile.followingIds.length;i++){
				if(currentUser.profile.followingIds[i].id == followId){
					return true;
				}
			}
			return false;
			
		}
		var getCurrentUser = function(){
			return currentUser;
		}
		var updateUserName = function(newUserName){
			currentUser.userName = newUserName;
			if( $window.Storage && getCurrentUser().isAuthenticated == true){
				// Force a sign in next visit
				$window.sessionStorage.removeItem( 'SB004.authorizationData');
			}
			// Broadcast that the user has updated his user name
			$rootScope.$broadcast('account.newUserName', newUserName);
		}
		var updateUserMessage = function(newUserMessage){
			currentUser.statusMessage = newUserMessage;

			// Broadcast that the user has updated his message
			$rootScope.$broadcast('account.newUserMessage', newUserMessage);
		}	
		var updateUserImage = function (){
			// Broadcast that the user has updated his image
			$rootScope.$broadcast('account.newUserImage', null);
		}	
		var forgotPassword = function(emailAddress){
			var deferred = $q.defer();
			$http( { 
					method: 'POST', 
					url: 'api/account/forgotpassword', 
					data: {
							email:emailAddress
					}
				})
			.success(function (data) {
                deferred.resolve();
            }).
            error(function (e, status) {
                deferred.reject(status);
            });

            return deferred.promise;
		}
		var resetPassword = function(newPassword, resetToken){
			var deferred = $q.defer();
			$http( { 
					method: 'POST', 
					url: 'api/account/resetpassword', 
					data: {
							newPassword: newPassword,
							resetToken: resetToken
					}
				})
			.success(function (data) {
                deferred.resolve(data);
            }).
            error(function (e, status) {
                deferred.reject(status);
            });

            return deferred.promise;
		}
		var isPasswordStrongEnough = function(password){
			if(password && password.length>=6 && password.match(/.*[0-9]+.*/)){
				return true;
			}

			return false;
		}
        return {
            logInDialog:showLogInDialog,			
			signIn:signIn,
			signUp: signUp,
			signOut: signOut,
            connect: connect,
            currentUser: currentUser,
			getCurrentUser: getCurrentUser,
			follow:follow,
			unfollow: unfollow,
			isFollowing: isFollowing,
			testIsAuthenticated:testIsAuthenticated,
			signInOptions:signInOptions,
			updateUserName:updateUserName,
			updateUserMessage:updateUserMessage,
			updateUserImage:updateUserImage,
			forgotPassword: forgotPassword,
			resetPassword: resetPassword,
			isPasswordStrongEnough: isPasswordStrongEnough
        }
    }


    // Register the service
    app.factory('securityService', ['$http', '$q', '$dialog', '$window','$timeout',"$rootScope", "blurry", securityService]);

})();