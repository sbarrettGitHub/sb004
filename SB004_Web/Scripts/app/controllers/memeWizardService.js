'use strict';
(function () {
    var memeWizardService = function ($dialog, $timeout) {
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
		var memeSelectConfirmImage = function () {

            dialogsViewBegin();
			
            var memeSelectConfirmImageDialog = $dialog.dialog({
                backdrop: true,
                keyboard: true,
                backdropClick: false,
                templateUrl: "/Scripts/app/views/newQuote.html",
                controller: "memeSelectConfirmImageCtrl"
            });

            memeSelectConfirmImageDialog.open().then(function (dialogResult) {
                if (dialogResult.action == "proceed") {
					$scope.spawn(dialogResult.data);                    
                    return;
                }
                allDialogsComplete();
            });

            $timeout(function () {
                angular.element('#pasteInput>input').focus();
            }, 400);
        }
		var spawn = function (seedData, respondingToMemeId) {
			memeApplyTextDialogOpts.resolve = {memeData : function() {return angular.copy(seedData);}};
			$dialog.dialog(memeApplyTextDialogOpts).open().then(function (dialogResult) {
                if (dialogResult) {
                    if (dialogResult.action == "startAgain") {
                        memeSelectConfirmImage();
                        return;
                    }
                    if (dialogResult.action == "reset") {
                        spawn(dialogResult.data, respondingToMemeId);
                        return;
                    }
                    if (dialogResult.action == "proceed") {
                        publish(dialogResult.data, respondingToMemeId);
                        return;
                    }
                }
                allDialogsComplete();
            });
        }
		var publish = function (memeData, respondingToMemeId) {
			memePublishAndShareDialogOpts.resolve = {memeData : function() {return angular.copy(memeData);}, respondingToMemeId : function(){ return respondingToMemeId;}};
            $dialog.dialog(memePublishAndShareDialogOpts).open().then(function (dialogResult) {
                if (dialogResult) {
                    if (dialogResult.action == "startAgain") {
                        sharedDataService.resetMeme();
                        $scope.memeSelectConfirmImage();
                        return;memeData
                    }
                    if (dialogResult.action == "changeMeme") {
                        $scope.spawn(memeData, respondingToMemeId);
                        return;
                    } 
					if (dialogResult.action == "close") {
                        return;
                    } 					
                }
				// Saved. Open the meme in a dialog              
				viewMeme(dialogResult.data.id);
            });
        }
		var logIn = function (callBackSuccess, callBackFail) {
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
		var viewMeme = function(memeId)
		{
			 $location.path('/meme/' + memeId);
		}
		function dialogsViewBegin(){
			angular.element("#view").addClass("blurry");
		}
		function allDialogsComplete(){
			angular.element("#view").removeClass("blurry");			
		}		
        return {
			memeSelectConfirmImage: memeSelectConfirmImage,
			spawn: spawn,
			publish: publish,
			logIn: logIn
        }
    };

    // Register the service
    app.factory('memeWizardService', ["$dialog", "$timeout", memeWizardService]);

})();