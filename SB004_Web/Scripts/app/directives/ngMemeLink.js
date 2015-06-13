
app.directive('ngMemelink', function() {
  return {
    restrict: 'A',
    require: '^ngModel',  
	scope: {
      ngModel: '=', onClick: '&', onLike: '&'
    },
    templateUrl: "Scripts/app/templates/ngMemelink.html"
  }
});