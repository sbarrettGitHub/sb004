'use strict';
(function () {

    var profileCtrl = function ($scope, $http, dialog, securityService) {
        
        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";
		
        var currentUser = securityService.getCurrentUser();
        $scope.id = currentUser.userId;	
        $scope.profileName = currentUser.userName;
        $scope.profileEmail = currentUser.profile.email;
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
    }
  
    // Register the controller
    app.controller('profileCtrl', ["$scope", "$http", "dialog", "securityService", profileCtrl]);

})();