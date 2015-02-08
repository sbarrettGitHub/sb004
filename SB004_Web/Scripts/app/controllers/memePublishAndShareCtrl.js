'use strict';
(function () {

    var memePublishAndShareCtrl = function ($scope, $timeout, $http, dialog, sharedDataService, renderingService) {

        // Wait for the view load the render the meme
        $timeout(function () {
            renderingService.render("canvas", sharedDataService.data.seedImage.width, sharedDataService.data.seedImage.height, sharedDataService.data.rawImage);
            //var canvas = document.getElementById("canvas"),
            //context = canvas.getContext('2d');
            //canvas.width = sharedDataService.data.seedImage.width;
            //canvas.height = sharedDataService.data.seedImage.height;

            //// Otherwise, add the image to the canvas
            //context.drawImage(sharedDataService.data.rawImage, 0, 0);
        }, 2000);

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
            $http.post('/api/Meme', {
                SeedId: sharedDataService.data.seedImage.id,
                Comments: sharedDataService.data.meme.comments,
                ImageData: memeImageData
            }).
            success(function (data, status, headers, config) {
                sharedDataService.data.meme = data;
            }).
            error(function (data, status, headers, config) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
            });
            dialog.close("Proceed");
        };

    }

    // Register the controller
    app.controller('memePublishAndShareCtrl', ["$scope", "$timeout", "$http", "dialog", "sharedDataService", "renderingService", memePublishAndShareCtrl]);

})();