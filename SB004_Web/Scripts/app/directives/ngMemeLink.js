
app.directive('ngMemelink', function() {
  return {
    restrict: 'A',
    require: '^ngModel',  
	scope: {
      ngModel: '=', onClick: '&', onLike: '&', onDislike: '&', onAddtofavourites: '&', onClickrepost: '&', ngLargeuserimage:'=', onHashtagselected:'&'
    },
    templateUrl: "Scripts/app/templates/ngMemelink.html?t=" + new Date().getTime(),
    link: function(scope,elem,attrs) {
      scope.hasHashtagselectedCallback = angular.isDefined(attrs.onHashtagselected); 
    }
  }
});