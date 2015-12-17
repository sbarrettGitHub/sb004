app.directive('ngUserhover', function($compile, $timeout) {
  return {
    restrict: 'A',
    replace: false,
	scope: {
      ngUser: '=',
    },
	link: function(scope, elem, attrs) {
     var inElement = false;
      elem.bind('mouseenter', function(e) {      
        inElement = true;  
        $timeout(function () {
            // Ensure that the mouse is still over the element
            if(inElement){
              var $hover = angular.element(elem).children(".ngUserHover");
              var left = e.pageX;
              if(left + 460 > window.innerWidth){
                  left =  -460;
              }
              $hover.css("left",left);
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
        /*var template = '<div class="ngUserHover" style="display:none;" ng-model="ngUser">  \
                          Hovering {{ngUser.userName}}                                   \
                        </div>'; 
                        src='http://img-s-msn-com.akamaized.net/tenant/amp/entityid/BBnxJQ1.img?h=64&w=80&m=6&q=60&u=t&o=t&l=f&x=918&y=846'>";*/
        var template = "";
        template += "	<div id='hoverUserDetail' class='ngUserHover rounded' style='display:none;' ng-model='ngUser'> ";
        template += "		<div class='header'>";
        template += "       	<img class='userImage rounded'";
        template += "           	src='api/image/user/{{ngUser.id}}'>";
        template += "           </img>";
        template += "           <div class='userName'>";
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