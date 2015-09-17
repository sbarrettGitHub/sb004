'use strict';
(function () {

    var repostCtrl = function ($scope, $http, dialog, memeId) {
        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";	
		$scope.meme = {};
		/*Control buttons*/
        $scope.closeMe = function () {
            dialog.close();
        };
		
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
		
		$http.get('/api/Meme/Lite/' + memeId).
			success(function (data) {
				endWaiting();
				if(data){
					$scope.meme = data;
				}
			}).
			error(function (e) {
				endWaiting();
			});
    }
  
    // Register the controller
    app.controller('repostCtrl', ["$scope", "$http", "dialog", "memeId", repostCtrl]);

})();