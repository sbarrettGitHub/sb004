'use strict';
(function () {

    var memePublishAndShareCtrl = function ($scope, $timeout, $http, dialog, sharedDataService, renderingService) {

        // Wait for the view load the render the meme
        $timeout(function () {
            renderingService.render("canvas","image", sharedDataService.data.seedImage.width, sharedDataService.data.seedImage.height, sharedDataService.data.rawImage);
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
            /*
            $http.post('/api/Meme', {
                SeedId: sharedDataService.data.seedImage.id,
                Comments: sharedDataService.data.meme.comments,
                ImageData: memeImageData
            }).
            success(function (data) {
                sharedDataService.data.meme = data;
            }).
            error(function () {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });*/
            dialog.close("LogInAndSave");
        };

    }

    // Register the controller
    app.controller('memePublishAndShareCtrl', ["$scope", "$timeout", "$http", "dialog", "sharedDataService", "renderingService", memePublishAndShareCtrl]);

})();