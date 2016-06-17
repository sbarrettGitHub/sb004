'use strict';
(function () {

    var memeViewCtrl = function ($scope,$location, $http,$q, $routeParams,$dialog,$window, $sanitize, memeWizardService, securityService, likeDislikeMemeService, markdownService) {

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
		$scope.userName = securityService.getCurrentUser().isAuthenticated ? securityService.getCurrentUser().userName:"Anonymous";
		$scope.url = $location.absUrl();
		var memeId = $routeParams.id;
		var constants = {
			commentViewingBlockCount:10,
			replyViewingBlockCount:10
		};
		var favouritesDialog = $dialog.dialog({
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "Scripts/app/views/favourites.html",
            controller: "favouritesCtrl" 
        });
		var repostDialogOpts ={
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "Scripts/app/views/repost.html?t="  + new Date().getTime(),
            controller: "repostCtrl" 
        };
		var reportDialogOpts ={
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "Scripts/app/views/report.html?t="  + new Date().getTime(),
            controller: "reportCtrl" 
        };
        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close(false);
        };
		$scope.respond = function (memeData) {
			memeWizardService.beginWithMeme(memeData?memeData:$scope.meme,memeId)
			.then(function(newMemeId){
				$window.alert("Resolved: " + newMemeId);
				if(!$scope.meme.replyIds){
					$scope.meme.replyIds = [];
				}
				$scope.meme.replyIds.unshift(newMemeId);				
				$http({ method: 'PATCH', url: 'api/Meme/' + memeId + "/reply/" + newMemeId, data: {}})
				.success(function (data) {  
					$scope.refreshReplies();				
                }).error(function (e) {
					$window.alert(e);
					return;
                });					
			},
			function(){
				$window.alert("Rejected");
			});
         
        };
		$scope.respondFromFavourites = function () {
			if(securityService.getCurrentUser().isAuthenticated==false){
				// Log in
				securityService.logInDialog()
					.then(function(){
						// Select a favourite
						openFavouritesList().then(function (selectedMemeId) {
							if(selectedMemeId){
								// Get the favourtie data
								getMeme(selectedMemeId)
								.then(function(memeData){
									// Respons with favourite meme data
									$scope.respond(memeData);	
								});
								
							}
						});
					});
			}else{
				openFavouritesList().then(function (selectedMemeId) {
					if(selectedMemeId){
						getMeme(selectedMemeId)
						.then(function(memeData){
							$scope.respond(memeData);	
						});
					}
				});
			}		
         
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
			var meme = ($scope.meme.id == memeId)?$scope.meme:findReply(memeId);
			likeDislikeMemeService.like(meme);			
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
			var meme = ($scope.meme.id == memeId)?$scope.meme:findReply(memeId);
			likeDislikeMemeService.dislike(meme);				
		}
		// -------------------------------------------------------------------
		// Favourites		
		$scope.addMemeToFavourites = function(memeId)
		{			
			if(securityService.getCurrentUser().isAuthenticated==false){
				securityService.logInDialog()
					.then(function(){
						addToFavourites(memeId);
					});
			}else{
				addToFavourites(memeId);
			}			 
		}
		var addToFavourites = function(memeId){
			if(isUserFavourite(memeId)){
				openFavouritesList()
				.then(function (selectedMemeId) {
					if(selectedMemeId){
						$location.path('/meme/' + selectedMemeId);
					}else{
						// Is this meme still a favourite?
						$scope.isUserFavourite = isUserFavourite($scope.meme.id);
					}
				});
				return;
			}
			$http({ method: 'PATCH', url: 'api/meme/' + memeId + "/favourite/", data: {}})
				.success(function (data) {  
					$scope.meme.favourites++;	
					if(securityService.getCurrentUser().profile.favouriteMemeIds){
						securityService.getCurrentUser().profile.favouriteMemeIds.push($scope.meme.id);
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
			var deferred = $q.defer();
			favouritesDialog.open().then(function (selectedMemeId) {				
				deferred.resolve(selectedMemeId);
			});	
			return deferred.promise;			
		};
		var isUserFavourite= function(memeId){
			if(securityService.getCurrentUser().profile)
			{
				if(securityService.getCurrentUser().profile.favouriteMemeIds){
					return (securityService.getCurrentUser().profile.favouriteMemeIds.indexOf(memeId) > -1);
				}
			}
			return false;
		}
		// -------------------------------------------------------------------
		// Repost
		$scope.repostMeme = function(memeId){
			if(securityService.getCurrentUser().isAuthenticated==false){
				securityService.logInDialog()
					.then(function(){
						repost(memeId);
						
					});
			}else{
				repost(memeId);
			}
		}
		var repost = function(memeId){
			var deferred = $q.defer();
			repostDialogOpts.resolve = {memeId : function() {return memeId;}};
			$dialog.dialog(repostDialogOpts).open().then(function (dialogResult) {	
				if(dialogResult.action == "repost"){
					$scope.meme.reposts++;
				}
				deferred.resolve();
			});	
			return deferred.promise;
		}
		// -------------------------------------------------------------------
		// Comments
		$scope.addComment = function(){
			$http.post('api/Comment', {
                MemeId: memeId,
                Comment: $scope.userComment
            }).
			success(function (data) {
				data.comment = markdownService.makeHtml(data.comment);
				$scope.userComments.push(data);
				$scope.userComment = "";
				$scope.meme.userCommentCount++;
			}).
			error(function (e) {
				$window.alert(e);
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
			$http.get('api/Comment/' + memeId + "?skip=" + skip + "&take=" + take).
                success(function (data) {
					// Add the comments returned to the list of comments
					for(var i=0;i<data.userComments.length;i++){   
						var userComment = data.userComments[i];
						userComment.comment = markdownService.makeHtml(userComment.comment);
						$scope.userComments.push(userComment);
					}
					// The Get More Comments button should be available if there are more comments out there than are displayed
					$scope.allowGetMoreComments = (data.fullCommentCount > $scope.userComments.length) ? true : false;
					
					// Maintain a cursor of comments. Push out by the number of comments retieved (less 1 because zero based index :))
					userCommentsIndex += $scope.userComments.length;	

					// Refresh the comment count on the meme 
					$scope.meme.userCommentCount = data.fullCommentCount;					
                }).
                error(function (e) {
					$window.alert(e);
                    
                });
		}	
		$scope.likeComment = function(commentId)
		{
			// Don't allow multiple likes by the same user on the same comment
			for(var i=0;i<securityService.getCurrentUser().myCommentLikes.length;i++){
				if(securityService.getCurrentUser().myCommentLikes[i] == commentId){
					return;
				}
			}
			$http({ method: 'PATCH', url: 'api/Comment/' + commentId + "/like/", data: {}})
				.success(function (data) {  
					updateComment(data);
					securityService.getCurrentUser().myCommentLikes.push(data.id);// Remember that you like this comment (so you can keep clicking like)
                }).error(function (e) {
					$window.alert(e);
					return;
                });
		}		
		$scope.dislikeComment = function(commentId)
		{
			// Don't allow multiple dislikes by the same user on the same comment
			for(var i=0;i<securityService.getCurrentUser().myCommentDislikes.length;i++){
				if(securityService.getCurrentUser().myCommentDislikes[i] == commentId){
					return;
				}
			}
			 $http({ method: 'PATCH', url: 'api/Comment/' + commentId + "/dislike/" , data: {}})
				.success(function (data) {  
					updateComment(data);
					securityService.getCurrentUser().myCommentDislikes.push(data.id); // Remember that you dislike this comment (so you can keep clicking dislike)			
                }).error(function (e) {
					$window.alert(e);
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
		// -------------------------------------------------------------------
		// Shared
		$scope.shared = function(){
			$http({ method: 'PATCH', url: 'api/meme/' + memeId + "/shared/", data: {}})
				.success(function () {  
					$scope.meme.shares++;
                }).error(function (e) {
					$window.alert(e);
					return;
                });
		}
		// -------------------------------------------------------------------
		// Report
		$scope.reportMeme = function(memeId){
			if(securityService.getCurrentUser().isAuthenticated==false){
				securityService.logInDialog()
					.then(function(){
						report(memeId);
						
					});
			}else{
				report(memeId);
			}
		}
		var report = function(memeId){
			var deferred = $q.defer();
			reportDialogOpts.resolve = {memeId : function() {return memeId;}};
			$dialog.dialog(reportDialogOpts).open().then(function (dialogResult) {	
				if(dialogResult.action == "report"){
					$window.alert("Thank you for bringing this to our attention. This post has been reported and action will be taken.");
				}
				deferred.resolve();
			});	
			return deferred.promise;
		}
		$scope.openUser = function(userId){
			$location.path("/usermemes/" + userId);
		}
		function getMeme(id){
			var deferred = $q.defer();
			startWaiting();
            $http.get('api/Meme/' + id).
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
            $http.get('api/Meme/Lite/' + id).
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
			
			$http.get('api/Meme/' + memeId + "/Replies?skip=" + skip + "&take=" + take).
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
					$window.alert(e);
                    
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
				$window.alert(e);
            });
		}
		
		// Load up the meme
		refresh();
		
		// Record that this meme was viewed (fire and forget)
		$http({ method: 'PATCH', url: 'api/meme/' + memeId + "/viewed/", data: {}});
	}

    // Register the controller
    app.controller('memeViewCtrl', ["$scope", "$location", "$http", "$q", "$routeParams","$dialog","$window", "$sanitize", "memeWizardService", "securityService", "likeDislikeMemeService","markdownService", memeViewCtrl]);

})();