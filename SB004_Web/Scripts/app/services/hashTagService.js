'use strict';
(function () {
    var hashTagService = function ($http, $q, $window, $location, $rootScope) {
        
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
        var hashTagMemes = function(filterList, takeMemes){   
            var deferred = $q.defer();
            var request = {
               takeHashTags: filterList.length,
               takeMemes:takeMemes,
               filterList: filterList
            };
            $http.post('api/meme/hashtag/memes?rnd='+new Date().getTime(),request).
                 success(function (data) {									
					deferred.resolve(data);
                }).
                error(function (e) {
					$window.alert(e);
					deferred.reject(e);
                });
			
			return deferred.promise;
        }   
        var hashTagSearch = function(criteria){
            if(criteria.length>0){
                // Open teh home page is search mode
                $location.path("home").search({q: criteria});
                // Notify the home page to show the search. REQUIRED IF THE HOMPAGE IS ALREADY OPEN
                $rootScope.$broadcast('home.viewSearch');  
            }

        }       
        return {
            trendingHashTagMemes: trendingHashTagMemes,
            trendingHashTags: trendingHashTags,
            trendingUserHashTags: trendingUserHashTags,
            hashTagMemes: hashTagMemes,
            hashTagSearch: hashTagSearch
        }
    }
    // Register the service
    app.factory('hashTagService', ['$http', '$q', '$window', '$location', '$rootScope', hashTagService]);

})();