app.directive('scrollTo', function() {
   return function(scope, elem, attr) {
      scope.$on('scrollTo', function(e, name) {
        if(name === attr.scrollTo) {
          //elem[0].focus();
          angular.element("body").animate({scrollTop: elem.offset().top},0);
        }
      });
   };
});

app.factory('scrollTo', function ($rootScope, $timeout) {
  return function(name) {
    $timeout(function (){
      $rootScope.$broadcast('scrollTo', name);
    });
  }
});