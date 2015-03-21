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
            clear: function () {
                isAuthenticated = false;
                userName = "";
                userId = "";
                accessToken = "";
                provider = "";
                thumbnail = "";
            }
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
                currentUser.userId = data.userId;
                currentUser.userName = data.userName;
                currentUser.accessToken = data.access_token;

                // Add bearer token to local storage for inclusion with future requests to the server
                localStorageService.set('authorizationData',
                    {
                        token: data.access_token,
                        userName: data.userName,
                        userId: data.userId,
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
                    currentUser = authData.userData;
                    $http.get('/api/Account/' + currentUser.userId).
                    success(function (data) {
                        deferred.resolve(data);
                    }).
                    error(function () {
                        currentUser.clear();
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