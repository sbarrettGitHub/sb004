app.directive('ngUserhover', function($compile, $timeout) {
  return {
    restrict: 'A',
    replace: false,
	scope: {
      ngUser: '=',
    },
	link: function(scope, elem, attrs) {
     var inElement = false;
      elem.bind('mouseenter', function() {      
        inElement = true;  
        $timeout(function () {
            // Ensure that the mouse is still over the element
            if(inElement){
              // Fade in
              angular.element(elem).children(".ngUserHover").fadeIn();
            }              
          }, 500);
      });
      elem.bind('mouseleave', function() {
        // Store that the mouse left the element (for use in timeout when fading in on mouse enter)
        inElement = false; 
        // fade out
        angular.element(elem).children(".ngUserHover").fadeOut();
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