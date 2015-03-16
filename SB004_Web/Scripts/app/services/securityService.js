'use strict';
(function () {
    var securityService = function ($http, $q) {
        var currentUser = {
            isAuthenticated: false,
            userName: "",
            userId: "",
            accessToken: "",
            provider: "",
            thumbnail: ""
        }

        var connect = function (provider, acessToken) {
            var deferred = $q.defer();
            $http.get('/api/Account/ObtainLocalAccessToken?provider=' + provider + '&externalAccessToken=' + acessToken).
            success(function (data) {
                deferred.resolve();
            }).
            error(function () {
                deferred.reject();
            });

            return deferred;
        }
        return {
            connect: connect,
            currentUser: currentUser
        }
    }


    // Register the service
    app.factory('securityService', ['$http', '$q', securityService]);

})();