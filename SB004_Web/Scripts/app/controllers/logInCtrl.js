'use strict';
(function () {

    var logInCtrl = function ($scope, dialog) {
        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close("Fail");
        };
    }

    // Register the controller
    app.controller('logInCtrl', ["$scope", "dialog", logInCtrl]);

})();