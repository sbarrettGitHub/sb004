
app.directive('ngMemelink', function() {
  return {
    restrict: 'A',
    require: '^ngModel',  
	scope: {
      ngModel: '=', onClick: '&', onLike: '&', onDislike: '&', onAddtofavourites: '&', onUserclick: '&'
    },
    templateUrl: "Scripts/app/templates/ngMemelink.html"
  }
});