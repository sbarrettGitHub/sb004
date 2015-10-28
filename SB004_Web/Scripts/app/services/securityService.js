'use strict';
(function () {
    var securityService = function ($http, $q, localStorageService, $dialog) {
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
        var loginDialog = $dialog.dialog({
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "/Scripts/app/views/logIn.html",
            controller: "logInCtrl"
        });

        var connect = function (provider, acessToken) {
            var deferred = $q.defer();
            $http.get('/api/Account/ObtainLocalAccessToken?provider=' + provider + '&externalAccessToken=' + acessToken).
            success(function (data) {

                currentUser.isAuthenticated = true;
                currentUser.userId = data.token.userId;
                currentUser.userName = data.token.userName;
                currentUser.accessToken = data.token.access_token;
                currentUser.profile = data.profile;
                // Add bearer token to local storage for inclusion with future requests to the server
                localStorageService.set('authorizationData',
                    {
                        token: data.token.access_token,
                        userName: data.token.userName,
                        userId: data.token.userId,
                        refreshToken: "",
                        useRefreshTokens: false,
                        userData: currentUser
                    });
                

                deferred.resolve(data);
            }).
            error(function () {
                deferred.reject();
            });

            return deferred.promise;
        }
        var testStillLoggedIn = function () {
            var deferred = $q.defer();
            var authData = localStorageService.get('authorizationData');
            if (authData) {
                if (authData.userData) {
					currentUser.isAuthenticated = authData.userData.isAuthenticated;
                    currentUser.userName = authData.userData.userName;
					currentUser.userId = authData.userData.userId
					currentUser.accessToken = authData.userData.accessToken;
					currentUser.provider = authData.userData.provider;
					currentUser.thumbnail = authData.userData.thumbnail;
                    $http.get('/api/Account/' + currentUser.userId).
                    success(function (data) {
                        currentUser.profile = data;
                        deferred.resolve(data);
                    }).
                    error(function () {
                        currentUser.isAuthenticated = false;
                        currentUser.userName = "";
                        currentUser.userId = "";
                        currentUser.accessToken = "";
                        currentUser.provider = "";
                        currentUser.thumbnail = "";
                        currentUser.profile = {};
                        deferred.reject();
                    });
                }
            } else {
                deferred.reject();
            }            

            return deferred.promise;
        }
        var logIn = function () {
            var deferred = $q.defer();
            loginDialog.open()
                .then(function (action) {
                    if (action == "Success") {
                        deferred.resolve();
                        return;
                    }
                    if (action == "Fail") {
                        deferred.reject();
                        return;
                    }
                });
            return deferred.promise;
        }
		var follow = function(followId){
			var deferred = $q.defer();
			if(currentUser.isAuthenticated==false){
				// Log in
				logIn()
					.then(function(){
						// Follow
						followUnfollowUser(followId, true)
						.then(function(data){
							deferred.resolve(data);
						});
					});
			}else{			
				// Follow
				followUnfollowUser(followId, true)
				.then(function(data){
					deferred.resolve(data);
				});
			}
			return deferred.promise;
		}
		var unfollow = function(followId){
			var deferred = $q.defer();
			if(currentUser.isAuthenticated==false){
				// Log in
				logIn()
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
				$http({ method: 'PATCH', url:'/api/Account/' + currentUser.userId + '/follow/' + followId, data: {}}).
				success(function (data) {       
					currentUser.profile = data;           
					deferred.resolve(data);
				}).
				error(function () {
					deferred.reject();
				});
			}else{
				$http({ method: 'PATCH', url:'/api/Account/' + currentUser.userId + '/unfollow/' + followId, data: {}}).
				success(function (data) {       
					currentUser.profile = data;           
					deferred.resolve(data);
				}).
				error(function () {
					deferred.reject();
				});			
			}
			return deferred.promise; 
		}
		
		var isFollowing = function(followId){
			var deferred = $q.defer();
			testStillLoggedIn()
			.then(function(){
				if(currentUser.isAuthenticated==false || !currentUser.profile || !currentUser.profile.followingIds){
					deferred.resolve(false);
				}
				for(var i=0;i<currentUser.profile.followingIds.length;i++){
					if(currentUser.profile.followingIds[i] == followId){
						deferred.resolve(true);
					}
				}
				deferred.resolve(false);
			});
			return deferred.promise;
		}
        return {
            logIn:logIn,
            connect: connect,
            testStillLoggedIn: testStillLoggedIn,
            currentUser: currentUser,
			follow:follow,
			unfollow: unfollow,
			isFollowing: isFollowing
        }
    }


    // Register the service
    app.factory('securityService', ['$http', '$q', 'localStorageService','$dialog', securityService]);

})();