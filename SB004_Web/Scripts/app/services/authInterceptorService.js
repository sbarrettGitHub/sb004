'use strict';
app.factory('authInterceptorService', ['$q', '$injector','$location', '$window', function ($q, $injector,$location, $window) {

    var authInterceptorServiceFactory = {};

    var _request = function (config) {

        config.headers = config.headers || {};
       
        var authData;
		if( $window.Storage ){
			var data = $window.sessionStorage.getItem( 'SB004.authorizationData' );
			if(data){
				authData = $window.JSON.parse(data);
			}
		}
        if (authData) {
            config.headers.Authorization = 'Bearer ' + authData.token.access_token;
        }

        return config;
    }

    //var _responseError = function (rejection) {
    //    if (rejection.status === 401) {
    //        var authService = $injector.get('authService');
    //        var authData = localStorageService.get('authorizationData');

    //        if (authData) {
    //            if (authData.useRefreshTokens) {
    //                $location.path('/refresh');
    //                return $q.reject(rejection);
    //            }
    //        }
    //        authService.logOut();
    //        $location.path('/login');
    //    }
    //    return $q.reject(rejection);
    //}

    authInterceptorServiceFactory.request = _request;
    //authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
}]);