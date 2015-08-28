'use strict';
(function () {

    var memeViewCtrl = function ($scope,$location, $http,$q, $routeParams,$dialog, memeWizardService, securityService) {

        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";
		$scope.title = "Untitled";
		$scope.meme = {};
		$scope.replies = [];
		$scope.allowGetMoreReplies = true;
		var repliesIndex = 0;		
		$scope.myReplies = [];
		$scope.userComments = [];
		$scope.userComment = "";
		$scope.allowGetMoreComments = true;
		$scope.isUserFavourite = false;
		var userCommentsIndex = 0;
		$scope.userName = securityService.currentUser.isAuthenticated ? securityService.currentUser.userName:"Anonymous";
		var memeId = $routeParams.id;
		var constants = {
			commentViewingBlockCount:10,
			replyViewingBlockCount:10
		};
		var favouritesDialog = $dialog.dialog({
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "/Scripts/app/views/favourites.html",
            controller: "favouritesCtrl" 
        });
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
					$scope.refreshReplies();				
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
					// Record that you like the selected meme
					securityService.currentUser.myMemeLikes.push(data.id);					
					// Increment the number of likes for the selected meme
					var meme = ($scope.meme.id == memeId)?$scope.meme:findReply(memeId);
					if(meme){
						if(!meme.likes){
							meme.likes = 0;
						}
						meme.likes++;
					}
                }).error(function (e) {
					alert(e);
					return;
                });
		}
		function findReply(memeId){
			// Find the reply with te given id
			for(var i=0;i<$scope.replies.length;i++){
				if($scope.replies[i].id == memeId){
					return $scope.replies[i];
				}
			}	
			return null;
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
					securityService.currentUser.myMemeDislikes.push(data.id);
					// Decrement the number of likes for the selected meme
					var meme = ($scope.meme.id == memeId)?$scope.meme:findReply(memeId);
					if(meme){
						if(!meme.dislikes){
							meme.dislikes = 0;
						}
						meme.dislikes++;	
					}
                }).error(function (e) {
					alert(e);
					return;
                });
		}
		// -------------------------------------------------------------------
		// Favourites		
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
			if(isUserFavourite(memeId)){
				openFavouritesList();
				return;
			}
			$http({ method: 'PATCH', url: '/api/meme/' + memeId + "/favourite/", data: {}})
				.success(function (data) {  
					$scope.meme.favourites++;	
					if(securityService.currentUser.profile.favouriteMemeIds){
						securityService.currentUser.profile.favouriteMemeIds.push($scope.meme.id);
						$scope.isUserFavourite = true;
					}
					if(confirm("This has been added to your favourites! Would you like to open your favourite list now?")){
						openFavouritesList();
					};
					return;
                }).error(function (e) {					
					return;
                });
		}
		var openFavouritesList = function(){
			favouritesDialog.open().then(function (selectedMemeId) {
				if(selectedMemeId){
					$location.path('/meme/' + selectedMemeId);
				}else{
					// Is this meme still a favourite?
					$scope.isUserFavourite = isUserFavourite($scope.meme.id);
				}
				
			});			
		};
		var isUserFavourite= function(memeId){
			if(securityService.currentUser.profile)
			{
				if(securityService.currentUser.profile.favouriteMemeIds){
					return (securityService.currentUser.profile.favouriteMemeIds.indexOf(memeId) > -1);
				}
			}
			return false;
		}
		// -------------------------------------------------------------------
		// Comments
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
		$scope.refreshComments = function(){
			$scope.userComments = [];
			var currentCount = userCommentsIndex;
			userCommentsIndex = 0;
			
			// Retrieve again all the replies that are currently visible again
			$scope.getMoreComments(0, currentCount);
		}
		$scope.getMoreComments = function(skipComments, takeComments){
			// Skip the explicitly specified number of comments (used during a refresh as 0 so all previously retrieved comments are refreshed) 
			// or skip the current number of comments to get the next page worth
			var skip = skipComments ? skipComments : userCommentsIndex;
			
			// Take the explicitly specified number of replies (used during a refresh as the number of previously retrieved replies)
			// or a standard page worth
			var take = takeComments ? takeComments : constants.commentViewingBlockCount;
			$http.get('/api/Comment/' + memeId + "?skip=" + skip + "&take=" + take).
                success(function (data) {
					// Add the comments returned to the list of comments
					for(var i=0;i<data.userComments.length;i++){   
						$scope.userComments.push(data.userComments[i]);
					}
					// The Get More Comments button should be available if there are more comments out there than are displayed
					$scope.allowGetMoreComments = (data.fullCommentCount > $scope.userComments.length) ? true : false;
					
					// Maintain a cursor of comments. Push out by the number of comments retieved (less 1 because zero based index :))
					userCommentsIndex += $scope.userComments.length;	

					// Refresh the comment count on the meme 
					$scope.meme.userCommentCount = data.fullCommentCount;					
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
		$scope.refreshReplies = function(){
			$scope.replies = [];
			var currentCount = repliesIndex;
			repliesIndex = 0;
			
			// Retrieve again all the replies that are currently visible again
			$scope.getMoreReplies(0, currentCount);
		}
        $scope.getMoreReplies = function(skipReplies, takeReplies){
			
			// Skip the explicitly specified number of replies (used during a refresh as 0 so all previously retrieved replies are refreshed) 
			// or skip the current number of replies to get the next page worth
			var skip = skipReplies ? skipReplies : repliesIndex;
			
			// Take the explicitly specified number of replies (used during a refresh as the number of previously retrieved replies)
			// or a standard page worth
			var take = takeReplies ? takeReplies : constants.replyViewingBlockCount;
			
			$http.get('/api/Meme/' + memeId + "/Replies?skip=" + skip + "&take=" + take).
                success(function (data) {
					// Add the replies returned to the list of replies
					for(var i=0;i<data.replies.length;i++){   
						$scope.replies.push(data.replies[i]);
					}	
					// The Get More Replies button should be available if there are more replies out there than are displayed
					$scope.allowGetMoreReplies = (data.fullReplyCount > $scope.replies.length) ? true : false;
					
					// Maintain a cursor of replies. 
					repliesIndex = $scope.replies.length;	
					
					// Refresh the reply count on the meme 
					$scope.meme.replyCount = data.fullReplyCount;
                }).
                error(function (e) {
					alert(e);
                    
                });
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
				$scope.isUserFavourite = isUserFavourite($scope.meme.id);
				$scope.getMoreReplies();
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
    app.controller('memeViewCtrl', ["$scope", "$location", "$http", "$q", "$routeParams","$dialog","memeWizardService", "securityService", memeViewCtrl]);

})();