﻿'use strict';
(function () {

    var memeViewCtrl = function ($scope,$http,$q, dialog, sharedDataService) {

        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";
		$scope.title = "Untitled";
		$scope.meme = {};
		
        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close(false);
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
		getMeme(sharedDataService.data.currentMeme.id)
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
    app.controller('memeViewCtrl', ["$scope", "$http", "$q", "dialog", "sharedDataService", memeViewCtrl]);

})();