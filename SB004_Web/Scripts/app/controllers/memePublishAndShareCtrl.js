'use strict';
(function () {

    var memePublishAndShareCtrl = function ($scope, $timeout, $http, $q, dialog, sharedDataService, renderingService, securityService, memeData, respondingToMemeId) {

		$scope.waiting = false;
		$scope.waitHeading = "Please wait...";
		$scope.waitingMessage = "";
		$scope.memeData = memeData;
        $scope.hashTags = [];
        $scope.hashTagInput="";
        // Wait for the view load the render the meme
        $timeout(function () {
            renderingService.render("canvas", "image", memeData.seedImage.width, memeData.seedImage.height, sharedDataService.data.rawImage);
        }, 500);

        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close({action:"close"});
        };
        $scope.startAgain = function () {
            dialog.close({action:"startAgain"});
        };
        $scope.changeMeme = function () {
            dialog.close({
						action:"changeMeme", 
						data:memeData
					});
        };

        $scope.proceed = function () {
            var dataUrl = canvas.toDataURL("image/jpg");

            var memeImageData = dataUrl.replace(/^data:image\/(png|jpg);base64,/, "");
			$scope.memeData.imageData = memeImageData;
            // Save the meme
            if (securityService.getCurrentUser().isAuthenticated) {
                saveMeme(memeImageData)
                    .then(function (id) {
						$scope.memeData.id = id;
                        dialog.close({
							action:"saved", 
							data:memeData
						});
                    })
                    .catch(function (e) {
                        alert(e);
                    });
            } else {
				
                securityService.logInDialog()
                .then(function () {
                    saveMeme(memeImageData)
                    .then(function (id) {
						$scope.memeData.id = id;
                        dialog.close({
							action:"saved", 
							data:memeData
						});
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
            $http.post('api/Meme', {
                SeedId: $scope.memeData.seedImage.id,
                Comments: $scope.memeData.comments,
                ImageData: $scope.memeData.imageData,
				ResponseToId: respondingToMemeId
            }).
                success(function (data) {
                    sharedDataService.data.currentMeme = data;
					endWaiting();
                    deferred.resolve(data.id);
                }).
                error(function (e) {
					endWaiting();
                    deferred.reject(e);
                });
            return deferred.promise;
        };
        $scope.setHashTags = function(){
            var htags = $scope.hashTagInput.split(" ");
            
            // Add all new hash tags
            for (var i = 0; i < htags.length; i++) {
                var htag = htags[i].trim();
                var alreadyAdded = false;
                for (var ii = $scope.hashTags.length -1; ii >=0 ; ii--) {
                    if(htag == $scope.hashTags[ii].trim()){
                        alreadyAdded=true;
                        break;
                    }                    
                }
                if(alreadyAdded != true){
                    $scope.hashTags.push(htag);
                }
            }
            $scope.hashTagInput = "";
        }
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
    app.controller('memePublishAndShareCtrl', ["$scope", "$timeout", "$http", "$q", "dialog", "sharedDataService", "renderingService", "securityService", "memeData", "respondingToMemeId", memePublishAndShareCtrl]);

})();