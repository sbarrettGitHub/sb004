'use strict';
(function () {

    var memeViewCtrl = function ($scope,$location, $http,$q, $routeParams, memeWizardService, securityService) {

        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";
		$scope.title = "Untitled";
		$scope.meme = {};
		$scope.replies = [];
		$scope.myReplies = [];
		$scope.userComments = [];
		$scope.userComment = "";
		$scope.allowGetMoreComments = true;
		var userCommentsIndex = 0;
		$scope.userName = securityService.currentUser.isAuthenticated ? securityService.currentUser.userName:"Anonymous";
		var memeId = $routeParams.id;
		var viewingCount = 10;
        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close(false);
        };
		$scope.respond = function () {
			memeWizardService.beginWithMeme($scope.meme,memeId)
			.then(function(newMemeId){
				alert("Resolved: " + newMemeId);
				if(!$scope.meme.replyIds){
					$scope.meme.replyIds = [];
				}
				$scope.meme.replyIds.unshift(newMemeId);				
				$http({ method: 'PATCH', url: '/api/Meme/' + memeId + "/reply/" + newMemeId, data: {}})
				.success(function (data) {  
					refresh();
					//$scope.myReplies.unshift(newMemeId);					
                }).error(function (e) {
					alert(e);
					return;
                });
					
			},
			function(){
				alert("Rejected");
			});
         
        };
		$scope.viewMeme = function(memeId)
		{
			if(memeId){
				$location.path('/meme/' + memeId);
			}else{
				$location.path("home");
			}
		}
		$scope.likeMeme = function(memeId)
		{
			// Don't allow multiple likes by the same user on the same meme
			for(var i=0;i<securityService.currentUser.myMemeLikes.length;i++){
				if(securityService.currentUser.myMemeLikes[i] == memeId){
					return;
				}
			}
			$http({ method: 'PATCH', url: '/api/meme/' + memeId + "/like/", data: {}})
				.success(function (data) { 
					if(!$scope.meme.likes){
						$scope.meme.likes = 0;
					}
					$scope.meme.likes++;
					securityService.currentUser.myMemeLikes.push(data.id);
                }).error(function (e) {
					alert(e);
					return;
                });
		}		
		$scope.dislikeMeme = function(memeId)
		{
			// Don't allow multiple dislikes by the same user on the same meme
			for(var i=0;i<securityService.currentUser.myMemeDislikes.length;i++){
				if(securityService.currentUser.myMemeDislikes[i] == memeId){
					return;
				}
			}
			$http({ method: 'PATCH', url: '/api/meme/' + memeId + "/dislike/", data: {}})
				.success(function (data) {  
					if(!$scope.meme.dislikes){
						$scope.meme.dislikes = 0;
					}
					$scope.meme.dislikes++;	
					securityService.currentUser.myMemeDislikes.push(data.id);
                }).error(function (e) {
					alert(e);
					return;
                });
		}		
		$scope.addMemeToFavourites = function(memeId)
		{
			if(securityService.currentUser.isAuthenticated==false){
				securityService.logIn()
					.then(function(){
						addToFavourites(memeId);
					});
			}else{
				addToFavourites(memeId);
			}
			 
		}
		var addToFavourites = function(memeId){
			$http({ method: 'PATCH', url: '/api/meme/' + memeId + "/favourite/", data: {}})
				.success(function (data) {  
					$scope.meme.favourites++;	
					alert("This meme has been added to your list of favourite memes!");
                }).error(function (e) {					
					return;
                });
		}
		$scope.addComment = function(){
			$http.post('/api/Comment', {
                MemeId: memeId,
                Comment: $scope.userComment
            }).
			success(function (data) {
				$scope.userComments.push(data);
				$scope.userComment = "";
				$scope.meme.userCommentCount++;
			}).
			error(function (e) {
				alert(e);
			});
		}
		$scope.getMoreComments = function(){
			$http.get('/api/Comment/' + memeId + "?skip=" + userCommentsIndex + "&take=11").
                success(function (data) {
					var commentCount = data.length;
					// Deliberately tried to retrieve 11. If 11 came back the show the "Get More Comments" button but only display 10
					if(commentCount > 10){
						$scope.allowGetMoreComments = true;
						commentCount = 10; // only show 10
					}else{
						$scope.allowGetMoreComments = false;
					}
					for(var i=0;i<commentCount;i++){   
						$scope.userComments.push(data[i]);
					}		
					// Maintain a cursor of comments. Push out by the number of comments retieved (less 1 because zero based index :))
					userCommentsIndex += data.length-1;		
                }).
                error(function (e) {
					alert(e);
                    
                });
		}	
		$scope.likeComment = function(commentId)
		{
			// Don't allow multiple likes by the same user on the same comment
			for(var i=0;i<securityService.currentUser.myCommentLikes.length;i++){
				if(securityService.currentUser.myCommentLikes[i] == commentId){
					return;
				}
			}
			$http({ method: 'PATCH', url: '/api/Comment/' + commentId + "/like/", data: {}})
				.success(function (data) {  
					updateComment(data);
					securityService.currentUser.myCommentLikes.push(data.id);// Remember that you like this comment (so you can keep clicking like)
                }).error(function (e) {
					alert(e);
					return;
                });
		}		
		$scope.dislikeComment = function(commentId)
		{
			// Don't allow multiple dislikes by the same user on the same comment
			for(var i=0;i<securityService.currentUser.myCommentDislikes.length;i++){
				if(securityService.currentUser.myCommentDislikes[i] == commentId){
					return;
				}
			}
			 $http({ method: 'PATCH', url: '/api/Comment/' + commentId + "/dislike/" , data: {}})
				.success(function (data) {  
					updateComment(data);
					securityService.currentUser.myCommentDislikes.push(data.id); // Remember that you dislike this comment (so you can keep clicking dislike)			
                }).error(function (e) {
					alert(e);
					return;
                });
		}	
		var updateComment = function(commentData){
			for(var i=0;i<$scope.userComments.length;i++){
				if($scope.userComments[i].id == commentData.id){
					$scope.userComments[i] = commentData;
				}
			}
		}
		function getMeme(id){
			var deferred = $q.defer();
			startWaiting();
            $http.get('/api/Meme/' + id).
                success(function (data) {
					endWaiting();
                    deferred.resolve(data);
                }).
                error(function (e) {
					endWaiting();
                    deferred.reject(e);
                });
            return deferred.promise;
		}
		function getMemeLite(id){
			var deferred = $q.defer();
			startWaiting();
            $http.get('/api/Meme/Lite/' + id).
                success(function (data) {
					endWaiting();
                    deferred.resolve(data);
                }).
                error(function (e) {
					endWaiting();
                    deferred.reject(e);
                });
            return deferred.promise;
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
		var refresh = function(){
			startWaiting();
			getMemeLite(memeId)
			.then(function (data) {
				var deferred = $q.defer();
				$scope.meme = data;
				if(data.replyIds){
					$scope.replies = [];
					for(var i=0;i<data.replyIds.length && i<viewingCount;i++){
						getMemeLite(data.replyIds[i])
						.then(function(replyData){
							$scope.replies.push(replyData);
						});
					}									
				}
				$scope.getMoreComments();
				endWaiting();	
            })
			.catch(function (e) {
                endWaiting();
				alert(e);
            });
		}
		
		// Load up the meme
		refresh();
		
		// Record that this meme was viewed (fire and forget)
		$http({ method: 'PATCH', url: '/api/meme/' + memeId + "/viewed/", data: {}});
	}

    // Register the controller
    app.controller('memeViewCtrl', ["$scope", "$location", "$http", "$q", "$routeParams","memeWizardService", "securityService", memeViewCtrl]);

})();