'use strict';
(function () {
    var likeDislikeMemeService = function ($http, $q, $window, securityService) {
        var like = function(meme){
			var deferred = $q.defer();
			var memeId = meme.id;
			// Don't allow multiple likes by the same user on the same meme
			for(var i=0;i<securityService.currentUser.myMemeLikes.length;i++){
				if(securityService.currentUser.myMemeLikes[i] == memeId){
					return;
				}
			}
			$http({ method: 'PATCH', url: 'api/meme/' + memeId + "/like/", data: {}})
				.success(function (data) { 
					// Record that you like the selected meme
					securityService.currentUser.myMemeLikes.push(data.id);					
					// Increment the number of likes for the selected meme
					if(meme){
						if(!meme.likes){
							meme.likes = 0;
						}
						meme.likes++;
					}
					deferred.resolve(meme);
                }).error(function (e) {
					$window.alert(e);
					deferred.reject();
                });
			return deferred.promise;
		}		
		var dislike = function(meme){
			var deferred = $q.defer();
			var memeId = meme.id;
			// Don't allow multiple dislikes by the same user on the same meme
			for(var i=0;i<securityService.currentUser.myMemeDislikes.length;i++){
				if(securityService.currentUser.myMemeDislikes[i] == memeId){
					return;
				}
			}
			$http({ method: 'PATCH', url: 'api/meme/' + memeId + "/dislike/", data: {}})
				.success(function (data) {  
					securityService.currentUser.myMemeDislikes.push(data.id);
					// Decrement the number of likes for the selected meme
					if(meme){
						if(!meme.dislikes){
							meme.dislikes = 0;
						}
						meme.dislikes++;	
					}
					deferred.resolve(meme);
                }).error(function (e) {
					$window.alert(e);
					deferred.reject();
                });
			return deferred.promise;
		}
        return {
            like:like,
            dislike: dislike
        }
    }


    // Register the service
    app.factory('likeDislikeMemeService', ['$http', '$q', '$window', 'securityService', likeDislikeMemeService]);

})();