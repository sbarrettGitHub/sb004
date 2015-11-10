'use strict';
(function () {

    var reportCtrl = function ($scope, $http, dialog,securityService, memeId) {
        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";	
		$scope.meme = {};
		$scope.objection = "";
		/*Control buttons*/
        $scope.closeMe = function () {
            dialog.close();
        };
		$scope.report=function(){
			$http({ method: 'PATCH', url: 'api/meme/' + memeId + "/report/", data: {objection:$scope.objection}})
				.success(function (data) {  
					dialog.close({action:"report"});
                }).error(function (e) {
					alert(e);
					return;
                });
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
		
		$http.get('api/Meme/Lite/' + memeId).
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
    app.controller('reportCtrl', ["$scope", "$http", "dialog","securityService", "memeId", reportCtrl]);

})();