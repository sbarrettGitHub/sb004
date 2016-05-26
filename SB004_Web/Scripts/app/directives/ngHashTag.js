app.directive('ngHashtag', function ($timeout,hashTagService) {
  return {
    restrict: 'E',
    replace: true,
    scope: {
      ngTag: '=', ngLarge: '=', onHashtagselected: '&'
    },
    link: function (scope, elem, attrs) {
      elem.bind('click', function () {
        $timeout(function () {
          scope.openHashtag();
        }, 1);
      });

      scope.openHashtag = function () {
        // If a method is supplied then call that, otherwise open the hash tag
        if (angular.isDefined(attrs.onHashtagselected)) {
          scope.$apply(scope.onHashtagselected());
        } else {
          hashTagService.hashTagSearch(scope.ngTag);
        }
      }
    },
    templateUrl: "Scripts/app/templates/ngHashTag.html?t=" + new Date().getTime()
  };
});