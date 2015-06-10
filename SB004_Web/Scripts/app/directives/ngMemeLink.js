
app.directive('ngMemelink', function() {
  return {
    restrict: 'A',
    require: '^ngModel',  
	scope: {
      ngModel: '='
    },
    templateUrl: "Scripts/app/templates/ngMemelink.html"
  }
});