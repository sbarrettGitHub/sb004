app.directive('ngUserhover', function($compile, $timeout, $location) {
  return {
    restrict: 'A',
    replace: false,
	scope: {
      ngUser: '='
    },
	link: function(scope, elem, attrs) {
     var inElement = false;
      elem.bind('mouseenter', function(e) {      
        inElement = true;  
        $timeout(function () {
            // Ensure that the mouse is still over the element
            if(inElement){
              
              var $target = angular.element(e.target);
              var pos = $target.position();  
              var width =$target.outerWidth();
              var left = pos.left;
              var top = pos.top;
              var height =$target.outerHeight();              
              var $hover = angular.element(elem).children(".ngUserHover");
              
              // Correct or right margin (show to the left of target)
              if(e.pageX + $hover.outerWidth() > window.innerWidth){
                  left -=  $hover.outerWidth();
              }
              
              // Correct for bottom margin (show above the target)
              if(e.pageY + $hover.outerHeight() > window.innerHeight){
                  top -=  $hover.outerHeight() + $target.outerHeight();
              }
              $hover.css("left",(left + (width/2)) + "px");
              $hover.css("top",(top + height) + "px");
              // Fade in
              $hover.fadeIn();
            }              
          }, 500);
      });
      elem.bind('mouseleave', function() {
        // Store that the mouse left the element (for use in timeout when fading in on mouse enter)
        inElement = false; 
        // fade out
        angular.element(elem).children(".ngUserHover").fadeOut();
      });
      scope.openUserMemes = function(){
        $location.path("/usermemes/" + scope.ngUser.id);
      }
      // elem.bind('click',function(){
      //   $location.path("/usermemes/" + scope.ngUser.id);
      // });
      
        var template = "";
        template += "	<div id='hoverUserDetail' class='ngUserHover rounded' style='display:none;' ng-model='ngUser'> ";
        template += "		<div class='header'>";
        template += "       	  <img class='userImage rounded'";
        template += "           	src='api/image/user/{{ngUser.id}}'>";
        template += "           </img>";
        template += "           <div class='userName hot' ng-click='openUserMemes()'>";
        template += "           	{{ngUser.userName}}";
        template += "           </div>	";
        template += "        </div>";
        template += "        <div class='userStats'>";
        template += "        	<div class='userStat'>";
        template += "           	<i class='fa fa-eye'></i> Views";
        template += "               <div>{{ngUser.views | nearestK}}</div>";
        template += "           </div>";
        template += "           <div class='userStat'>";
        template += "           	<i class='fa fa-thumbs-o-up'></i> Likes";
        template += "               <div>{{ngUser.likes | nearestK}}</div>";
        template += "           </div>";	
        template += "           <div class='userStat'>";
        template += "           	<i class='fa fa-thumbs-o-down'></i> Dislikes";
        template += "               <div>{{ngUser.dislikes | nearestK}}</div>";
        template += "           </div>";	
        template += "           <div class='userStat'>";
        template += "           	<i class='fa fa-retweet'></i> Reposts";
        template += "            	<div>{{ngUser.reposted | nearestK}}</div>";
        template += "           </div>";						
        template += "        </div>";	
        template += "	</div>";
        var new$ = angular.element(template);
        elem.append(new$); // append hover div as a child
        $compile(new$)(scope);
    }
  };
});