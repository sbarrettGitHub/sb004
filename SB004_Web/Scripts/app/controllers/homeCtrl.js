'use strict';
(function () {

    var homeCtrl = function ($scope, $location, $rootScope, $dialog, $timeout, sharedDataService) {
        $scope.quotes = sharedDataService.data.quoteSearch.results;
        $scope.searchTerm = "";
        $scope.searchCategory = "";

        var memeApplyTextDialog = $dialog.dialog({
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "/Scripts/app/views/spawn.html",
            controller: "memeApplyTextCtrl"
        });
        var memeApplyTextDialogOpts = {
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "/Scripts/app/views/spawn.html",
            controller: "memeApplyTextCtrl"
        };		
        var memePublishAndShareDialog = $dialog.dialog({
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "/Scripts/app/views/publish.html",
            controller: "memePublishAndShareCtrl"
        });
		var memePublishAndShareDialogOpts ={
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "/Scripts/app/views/publish.html",
            controller: "memePublishAndShareCtrl"
        };
        var loginDialog = $dialog.dialog({
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "/Scripts/app/views/logIn.html",
            controller: "logInCtrl"
        });
		var memeViewDialogOpts = {
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "/Scripts/app/views/memeView.html",
            controller: "memeViewCtrl",
			dialogClass: 'modal dialog-wide'
        };
		
        $scope.memeSelectConfirmImage = function () {
            //$location.path("new");
            angular.element("#view").addClass("blurry");
            var memeSelectConfirmImageDialog = $dialog.dialog({
                backdrop: true,
                keyboard: true,
                backdropClick: false,
                templateUrl: "/Scripts/app/views/newQuote.html",
                controller: "memeSelectConfirmImageCtrl"
            });

            memeSelectConfirmImageDialog.open().then(function (dialogResult) {
                if (!dialogResult.action == "proceed") {
                    angular.element("#view").removeClass("blurry");
                    return;
                }
                $scope.spawn(dialogResult.data);
            });

            $timeout(function () {
                angular.element('#pasteInput>input').focus();
            }, 400);
        }
        $scope.spawn = function (seedData) {
			memeApplyTextDialogOpts.resolve = {memeData : function() {return angular.copy(seedData);}};
			$dialog.dialog(memeApplyTextDialogOpts).open().then(function (dialogResult) {
                if (dialogResult) {
                    if (dialogResult.action == "startAgain") {
                        sharedDataService.resetMeme();
                        $scope.memeSelectConfirmImage();
                        return;
                    }
                    if (dialogResult.action == "reset") {
                        $scope.spawn(dialogResult.data);
                        return;
                    }
                    if (dialogResult.action == "proceed") {
                        $scope.publish(dialogResult.data);
                        return;
                    }
                }
                angular.element("#view").removeClass("blurry");
            });
        }
        $scope.publish = function (memeData) {
			memePublishAndShareDialogOpts.resolve = {memeData : function() {return angular.copy(memeData);}};
            $dialog.dialog(memePublishAndShareDialogOpts).open().then(function (dialogResult) {
                if (dialogResult) {
                    if (dialogResult.action == "startAgain") {
                        sharedDataService.resetMeme();
                        $scope.memeSelectConfirmImage();
                        return;memeData
                    }
                    if (dialogResult.action == "changeMeme") {
                        $scope.spawn(memeData);
                        return;
                    } 
					if (dialogResult.action == "close") {
                        return;
                    } 					
                }
				// Saved
				alert("Meme saved: " + dialogResult.data.id);                
				$scope.viewMeme(dialogResult.data.id);
            });
        }
        $scope.logIn = function (callBackSuccess, callBackFail) {
            loginDialog.open().then(function (action) {
                if (action == "Success") {
                    callBackSuccess();
                    return;
                }
                if (action == "Fail") {
                    callBackFail();
                    return;
                }
            });
        }
		$scope.viewMeme = function(memeId)
		{
			memeViewDialogOpts.resolve = {memeId : function() {return angular.copy(memeId);}};
			$dialog.dialog(memeViewDialogOpts).open();
		}
		
        $rootScope.$on('quoteSearch.complete', function (event, data) {

            $scope.quotes = sharedDataService.data.quoteSearch.results;

        });
        $scope.resized = function (width, height) {
            console.log(width + " X " + height);
        };
        // $scope.search();
        $scope.init = function () {
            $scope.handle = $dialog.dialog({});
        };

        $scope.init();
    }

    // Register the controller
    app.controller('homeCtrl', ["$scope", "$location", "$rootScope", "$dialog", "$timeout", "sharedDataService", homeCtrl]);

})();