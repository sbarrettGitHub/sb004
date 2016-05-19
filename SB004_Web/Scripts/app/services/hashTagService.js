'use strict';
(function () {
    var hashTagService = function ($http, $q, $window) {
        
        var trendingHashTagMemes = function(takeHashTags, takeMemes, filterList){
            var deferred = $q.defer();
            var request = {
               takeHashTags: takeHashTags,
               takeMemes:takeMemes,
               filterList: filterList
            };
            $http.post('api/meme/trending?rnd='+new Date().getTime(),request).
                 success(function (data) {									
					deferred.resolve(data);
                }).
                error(function (e) {
					$window.alert(e);
					deferred.reject(e);
                });
			
			return deferred.promise;
        } 
        var trendingHashTags = function(take){
            var deferred = $q.defer();
            
            $http.get('api/meme/trending/tags?take=' + take + '&rnd='+new Date().getTime()).
                 success(function (data) {									
					deferred.resolve(data);
                }).
                error(function (e) {
					$window.alert(e);
					deferred.reject(e);
                });
			
			return deferred.promise;         
        }   
        var trendingUserHashTags = function(){
            
        }   
        return {
            trendingHashTagMemes: trendingHashTagMemes,
            trendingHashTags: trendingHashTags,
            trendingUserHashTags: trendingUserHashTags
        }
    }
    // Register the service
    app.factory('hashTagService', ['$http', '$q', '$window', hashTagService]);

})();