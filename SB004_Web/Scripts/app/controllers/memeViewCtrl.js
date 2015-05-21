'use strict';
(function () {

    var memeViewCtrl = function ($scope,$http,$q, $routeParams, sharedDataService, memeWizardService) {

        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";
		$scope.title = "Untitled";
		$scope.meme = {};
		var memeId = $routeParams.id;
        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close(false);
        };
		$scope.respond = function () {
			memeWizardService.beginWithMeme($scope.meme,memeId)
			.then(function(){
				alert("Resolved");
				getMeme(memeId);
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
		
		startWaiting();
		getMeme(memeId)
			.then(function (data) {
				$scope.meme = data;
				endWaiting();
            })
			.catch(function (e) {
                endWaiting();
				alert(e);
            });	
	}

    // Register the controller
    app.controller('memeViewCtrl', ["$scope", "$http", "$q", "$routeParams", "sharedDataService","memeWizardService", memeViewCtrl]);

})();