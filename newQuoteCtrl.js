'use strict';
(function() {

  var newQuoteCtrl = function($scope) {
   // $scope.imageUrl = "drag";
            $scope.image = null
        $scope.imageFileName = ''
    /*angular.element("#newQuoteBuilder").on(
      'drop',
      function(e) {

        e.preventDefault();
        e.stopPropagation();
       
        
       e.dataTransfer = e.originalEvent.dataTransfer;
        var url = e.dataTransfer.getData('URL');
        if (url.length == 0) {
          url = e.dataTransfer.getData('text');
        }
        //$scope.imageUrl = "dropped";
        $scope.updateUrl(url);

      }).on('dragover', function(e) {
      e.preventDefault();
    });
    //$scope.updateUrl = function(url){
      $scope.imageUrl = url;
    }*/
  }

  // Register the controller
  app.controller('newQuoteCtrl', ["$scope", newQuoteCtrl]);

})();