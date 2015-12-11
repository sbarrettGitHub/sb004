app.directive('blurry', function() {
   return function(scope, elem, attr) {
      scope.$on('blurryOn', function(e, name) {
        if(name === attr.blurry) {
          elem[0].classList.add("blurry");
        }
      });
      scope.$on('blurryOff', function(e, name) {
        if(name === attr.blurry) {
          elem[0].classList.remove("blurry");
        }
      });	  
   };
});

app.factory('blurry', function ($rootScope, $timeout) {
  return function(name, on) {
    $timeout(function (){
      if(on){
        $rootScope.$broadcast('blurryOn', name);
      }else{
        $rootScope.$broadcast('blurryOff', name);
      }      
    });
  }
});

