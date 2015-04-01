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
            profile: {}
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
        return {
            logIn:logIn,
            connect: connect,
            testStillLoggedIn: testStillLoggedIn,
            currentUser: currentUser
        }
    }


    // Register the service
    app.factory('securityService', ['$http', '$q', 'localStorageService','$dialog', securityService]);

})();