app.directive('ngHashtag', function($compile, $timeout, $location, $rootScope, hashTagService) {
  return {
    restrict: 'E',
    replace: true,
	scope: {
      ngTag: '=', ngLarge:'='
    },
	link: function(scope, elem, attrs) {
     
     elem.bind('click', function() {
        $timeout(function () {
          scope.openHashtag();
        },250);
        
      });
      scope.openHashtag = function(){        
			  hashTagService.hashTagSearch(scope.ngTag);
      }
    },
    templateUrl: "Scripts/app/templates/ngHashTag.html?t=" + new Date().getTime()
  };
});