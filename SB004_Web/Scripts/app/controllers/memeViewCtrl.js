'use strict';
(function () {

    var memeViewCtrl = function ($scope,$http,$q, $routeParams, sharedDataService, memeWizardService) {

        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";
		$scope.title = "Untitled";
		$scope.meme = {};
		$scope.replies = [];
		$scope.myReplies = [];
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
				//refresh();
				$scope.myReplies.unshift(newMemeId);
			},
			function(){
				alert("Rejected");
			});
         
        };
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
						getMeme(data.replyIds[i])
						.then(function(replyData){
							$scope.replies.push(replyData);
						});
					}					
				}

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
    app.controller('memeViewCtrl', ["$scope", "$http", "$q", "$routeParams", "sharedDataService","memeWizardService", memeViewCtrl]);

})();