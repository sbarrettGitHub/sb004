'use strict';
(function () {

    var memeViewCtrl = function ($scope, dialog) {

        $scope.waiting = false;
        $scope.waitHeading = "Please wait...";
        $scope.waitingMessage = "";

        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close(false);
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
    app.controller('memeViewCtrl', ["$scope", "dialog", memeViewCtrl]);

})();