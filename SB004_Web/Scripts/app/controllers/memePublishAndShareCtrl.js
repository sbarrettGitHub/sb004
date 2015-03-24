'use strict';
(function () {

    var memePublishAndShareCtrl = function ($scope, $timeout, $http, $q, dialog, sharedDataService, renderingService, securityService) {

		$scope.waiting = false;
		$scope.waitHeading = "Please wait...";
		$scope.waitingMessage = "";
		
        // Wait for the view load the render the meme
        $timeout(function () {
            renderingService.render("canvas", "image", sharedDataService.data.seedImage.width, sharedDataService.data.seedImage.height, sharedDataService.data.rawImage);
        }, 500);

        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close(false);
        };
        $scope.startAgain = function () {
            dialog.close("StartAgain");
        };
        $scope.changeMeme = function () {
            dialog.close("ChangeMeme");
        };

        $scope.proceed = function () {
            var dataUrl = canvas.toDataURL("image/jpg");

            var memeImageData = dataUrl.replace(/^data:image\/(png|jpg);base64,/, "");

            // Save the meme
            if (securityService.currentUser.isAuthenticated) {
                saveMeme(memeImageData)
                    .then(function () {
                        dialog.close("Saved");
                    })
                    .catch(function (e) {
                        alert(e);
                    });
            } else {
				
                securityService.logIn()
                .then(function () {
                    saveMeme(memeImageData)
                    .then(function () {
                        dialog.close("Saved");
                    })
                    .catch(function (e) {
                        alert(e);
                    });
                });                
            }
        };
        var saveMeme = function (memeImageData) {
            var deferred = $q.defer();
			startWaiting();
            $http.post('/api/Meme', {
                SeedId: sharedDataService.data.seedImage.id,
                Comments: sharedDataService.data.meme.comments,
                ImageData: memeImageData
            }).
                success(function (data) {
                    sharedDataService.data.meme = data;
					endWaiting();
                    deferred.resolve();
                }).
                error(function (e) {
					endWaiting();
                    deferred.reject(e);
                });
            return deferred.promise;
        };
		function startWaiting(heading, message){
			$scope.waiting = true;
			$scope.waitHeading = !heading ? "Please wait..." : heading;
			$scope.waitingMessage = !message ? "" : message;
		}
		function endWaiting(){
			$scope.waiting = false;
			$scope.waitHeading = "";
			$scope.waitingMessage = "";
		}
    }

    // Register the controller
    app.controller('memePublishAndShareCtrl', ["$scope", "$timeout", "$http", "$q", "dialog", "sharedDataService", "renderingService", "securityService", memePublishAndShareCtrl]);

})();