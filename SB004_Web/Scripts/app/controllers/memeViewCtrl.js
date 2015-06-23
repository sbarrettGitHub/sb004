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
		var myCommentLikes = [];
		var myCommentDislikes = [];
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
			 alert("Like:" + memeId);
		}		
		$scope.dislikeMeme = function(memeId)
		{
			 alert("Dislike:" + memeId);
		}		
		$scope.addMemeToFavourites = function(memeId)
		{
			 alert("Add to Favourites:" + memeId);
		}
		$scope.addComment = function(){
			$http.post('/api/Comment', {
                MemeId: memeId,
                Comment: $scope.userComment
            }).
			success(function (data) {
				$scope.userComments.push(data);
				$scope.userComment = "";
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
			for(var i=0;i<myCommentLikes.length;i++){
				if(myCommentLikes[i] == commentId){
					return;
				}
			}
			$http({ method: 'PATCH', url: '/api/Comment/' + commentId + "/like/", data: {}})
				.success(function (data) {  
					updateComment(data);
					myCommentLikes.push(data.id);// Remember that you like this comment (so you can keep clicking like)
                }).error(function (e) {
					alert(e);
					return;
                });
		}		
		$scope.dislikeComment = function(commentId)
		{
			// Don't allow multiple dislikes by the same user on the same comment
			for(var i=0;i<myCommentDislikes.length;i++){
				if(myCommentDislikes[i] == commentId){
					return;
				}
			}
			 $http({ method: 'PATCH', url: '/api/Comment/' + commentId + "/dislike/" , data: {}})
				.success(function (data) {  
					updateComment(data);
					myCommentDislikes.push(data.id); // Remember that you dislike this comment (so you can keep clicking dislike)			
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
			getMeme(memeId)
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
		
		
		refresh();
	}

    // Register the controller
    app.controller('memeViewCtrl', ["$scope", "$location", "$http", "$q", "$routeParams","memeWizardService", "securityService", memeViewCtrl]);

})();