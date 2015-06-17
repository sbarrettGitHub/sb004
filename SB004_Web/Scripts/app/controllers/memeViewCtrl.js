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
				
				$scope.meme = data;
				if(data.replyIds){
					$scope.replies = [];
					for(var i=0;i<data.replyIds.length && i<viewingCount;i++){
						getMemeLite(data.replyIds[i])
						.then(function(replyData){
							$scope.replies.push(replyData);
						});
					}
					endWaiting();					
				}
				
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