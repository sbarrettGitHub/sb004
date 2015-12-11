'use strict';
(function () {
    var memeWizardService = function ($q, $dialog, $timeout) {
		var memeApplyTextDialog = $dialog.dialog({
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "Scripts/app/views/spawn.html",
            controller: "memeApplyTextCtrl"
        });
        var memeApplyTextDialogOpts = {
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "Scripts/app/views/spawn.html",
            controller: "memeApplyTextCtrl"
        };		
        var memePublishAndShareDialog = $dialog.dialog({
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "Scripts/app/views/publish.html",
            controller: "memePublishAndShareCtrl"
        });
		var memePublishAndShareDialogOpts ={
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "Scripts/app/views/publish.html",
            controller: "memePublishAndShareCtrl"
        };
        var loginDialog = $dialog.dialog({
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "Scripts/app/views/logIn.html",
            controller: "logInCtrl"
        });
	
		var memeSelectConfirmImage = function () {
			var deferred = $q.defer();
            
			
            var memeSelectConfirmImageDialog = $dialog.dialog({
                backdrop: true,
                keyboard: true,
                backdropClick: false,
                templateUrl: "Scripts/app/views/newQuote.html",
                controller: "memeSelectConfirmImageCtrl"
            });

            memeSelectConfirmImageDialog.open().then(function (dialogResult) {
                if (dialogResult.action == "proceed") {
					spawn(dialogResult.data) 
					.then(function(data){
							deferred.resolve(data);
						},
						function(){
							deferred.reject();
						});
                    return;
                }
              
				deferred.reject();
            });


			
			return deferred.promise;
        }
		var spawn = function (seedData, respondingToMemeId) {
			var deferred = $q.defer();
			memeApplyTextDialogOpts.resolve = {memeData : function() {return angular.copy(seedData);}};
			$dialog.dialog(memeApplyTextDialogOpts).open().then(function (dialogResult) {
                if (dialogResult) {
                    if (dialogResult.action == "startAgain") {
                        memeSelectConfirmImage()
						.then(function(data){
							deferred.resolve(data);
						},
						function(){
							deferred.reject();
						});
                        return;
                    }
                    if (dialogResult.action == "reset") {
                        spawn(dialogResult.data, respondingToMemeId);
                        return;
                    }
                    if (dialogResult.action == "proceed") {
                        publish(dialogResult.data, respondingToMemeId)
						.then(function(data){
							deferred.resolve(data);
						},
						function(){
							deferred.reject();
						});
                        return;
                    }
                }                
				deferred.reject();
            });
			
			return deferred.promise;
        }
		var publish = function (memeData, respondingToMemeId) {
			var deferred = $q.defer();
			memePublishAndShareDialogOpts.resolve = {memeData : function() {return angular.copy(memeData);}, respondingToMemeId : function(){ return respondingToMemeId;}};
            $dialog.dialog(memePublishAndShareDialogOpts).open().then(function (dialogResult) {
                if (dialogResult) {
                    if (dialogResult.action == "startAgain") {
                        sharedDataService.resetMeme();
                        memeSelectConfirmImage()
						.then(function(data){
							deferred.resolve(data);
						},
						function(){
							deferred.reject();
						});
                        return;
                    }
                    if (dialogResult.action == "changeMeme") {
                        spawn(memeData, respondingToMemeId)
						.then(function(data){
							deferred.resolve(data);
						},
						function(){
							deferred.reject();
						});
                        return;
                    } 
					if (dialogResult.action == "close") {
						deferred.reject();
                        return;
                    } 					
                }
				deferred.resolve(dialogResult.data.id);
            });
			return deferred.promise;
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

			
		var begin = function(){
			var deferred = $q.defer();
			memeSelectConfirmImage()
			.then(function(data){
				deferred.resolve(data);
			},function(){
				deferred.reject();
			});
			return deferred.promise;
		}
		var beginWithMeme = function(seedData, respondingToMemeId){
			var deferred = $q.defer();
			spawn(seedData, respondingToMemeId)
			.then(function(data){
				deferred.resolve(data);
			},function(){
				deferred.reject();
			});
			return deferred.promise;
		}
        return {			
			begin: begin,
			beginWithMeme: beginWithMeme
        }
    };

    // Register the service
    app.factory('memeWizardService', ["$q", "$dialog", "$timeout", memeWizardService]);

})();