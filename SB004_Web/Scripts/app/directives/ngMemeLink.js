
app.directive('ngMemelink', function() {
  return {
    restrict: 'A',
    require: '^ngModel',  
	scope: {
      ngModel: '=', onClick: '&', onLike: '&', onDislike: '&', onAddtofavourites: '&', onClickrepost: '&', ngLargeuserimage:'='
    },
    templateUrl: "Scripts/app/templates/ngMemelink.html?t=" + new Date().getTime()
  }
});