'use strict';
(function () {

    var memeSelectConfirmImageCtrl = function ($scope, $timeout, $location, $http, fileReader, dialog, sharedDataService, focus) {
        $scope.image = null;
        $scope.imageFileName = null;
        $scope.file = null;
        $scope.waiting = false;
		$scope.waitHeading = "Please wait...";
		$scope.waitingMessage = "";
		$scope.showClose = false;
        $scope.$on("Drop.Url", function (event, url) {
            $timeout(function () {
                $scope.image = url;
                $scope.imageFileName = url;
            }, 100);
        });
        $scope.getFile = function () {
			startWaiting("Please wait...","");
            $timeout(function () {
                fileReader.readAsDataUrl($scope.file, $scope)
                .then(function (result) {
                    $scope.image = result;
                    endWaiting();
                    $timeout(function () {
                        centerPreviewImage();
                    }, 100);
                });
            }, 100);

        };
        $scope.$on("fileProgress", function (e, progress) {
            startWaiting("Please wait...","");
        });

        $scope.clear = function () {
            $scope.image = null;
            $scope.imageFileName = null;
            $scope.file = null;
			endWaiting();
        };

        $scope.proceed = function () {
            var imagePreview = angular.element("#imagePreview");
			var width = imagePreview.width();
			var height = imagePreview.height();
			startWaiting("Please wait...","");
            $http.post('api/Seed', {
                id: null,
                image: $scope.image,
                width: width,
                height: height
            }).
            success(function (data, status, headers, config) {
                sharedDataService.data.seedImage.id = data.id;
                sharedDataService.data.seedImage.image = data.image;
				$scope.waiting = false;
                dialog.close(
					{
						action:"proceed", 
						data:
						{
							seedImage: 
							{
								id:data.id,
								image:data.image,
								width: width,
								height: height
							}
						}
					});
            }).
            error(function (data, status, headers, config) {
                // called asynchronously if an error occurs
                // or server returns response with an error status.
				endWaiting();
            });

        };
        $scope.closeMe = function () {
            dialog.close({action:"cancel"});
        };
        $scope.imageSet = function () {
            $timeout(function () {
                centerPreviewImage()
            }, 100);
        };
        function centerPreviewImage() {
            var imageContainer = angular.element("#imageContainer");
            var imagePreview = angular.element("#imagePreview");
            var marginLeft = (imageContainer.width() - imagePreview.width()) / 2;
            var marginTop = (imageContainer.height() - imagePreview.height()) / 2;
            imagePreview.css({ 'margin-left': marginLeft });
            imagePreview.css({ 'margin-top': marginTop });
            sharedDataService.data.seedImage = {
                id: null,
                image: $scope.image,
                width: imagePreview.width(),
                height: imagePreview.height(),
            };
        }
		function startWaiting(heading, message){
			$scope.waiting = true;
			$scope.waitHeading = heading;
			$scope.waitingMessage = message;
		}
		function endWaiting(){
			$scope.waiting = false;
			$scope.waitHeading = "";
			$scope.waitingMessage = "";
		}
        focus("selectImage");
    }

    // Register the controller
    app.controller('memeSelectConfirmImageCtrl', ["$scope", "$timeout", "$location", "$http", "fileReader", "dialog", "sharedDataService", "focus", memeSelectConfirmImageCtrl]);

})();