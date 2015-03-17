'use strict';
(function () {

    var memePublishAndShareCtrl = function ($scope, $timeout, $http, $q, dialog, sharedDataService, renderingService, securityService) {

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
            $http.post('/api/Meme', {
                SeedId: sharedDataService.data.seedImage.id,
                Comments: sharedDataService.data.meme.comments,
                ImageData: memeImageData
            }).
                success(function (data) {
                    sharedDataService.data.meme = data;
                    deferred.resolve();
                }).
                error(function (e) {
                    deferred.reject(e);
                });
            return deferred.promise;
        };
    }

    // Register the controller
    app.controller('memePublishAndShareCtrl', ["$scope", "$timeout", "$http", "$q", "dialog", "sharedDataService", "renderingService", "securityService", memePublishAndShareCtrl]);

})();