app.directive('ngUserhover', function($compile) {
  return {
    restrict: 'A',
    replace: false,
	scope: {
      ngUser: '=',
    },
	link: function(scope, elem, attrs) {
      elem.bind('mouseenter', function() {
        angular.element(elem).children(".ngUserHover").show();
      });
      elem.bind('mouseleave', function() {
        angular.element(elem).children(".ngUserHover").hide();
      });
      elem.bind('click', function() {
        alert("");
      });
        var template = '<div class="ngUserHover" style="display:none;" ng-model="ngUser">  \
                          Hovering {{ngUser.userName}}                                   \
                        </div>'; 
        var new$ = angular.element(template);
        elem.append(new$); // append hover div as a child
        $compile(new$)(scope);
    }
  };
});