'use strict';
(function () {
    var initialText = "Your text goes here ...";
    function Comment(id) {
        var self = this;
        this.id = id;
        this.text = initialText;
        this.position = {
            align: "bottom",
            x: 0,
            y: 0,
            width: 0,
            height: 32,
            padding: 0
        };
        this.color = "black";
        this.backgroundColor = "none";
        this.fontFamily = "Arial";
        this.fontSize = "15pt";
        this.fontWeight = "bold";
        this.textDecoration = "none";
        this.fontStyle = "normal";
        this.textAlign = "center";
        this.dropped = false;
        this.selected = false;
        this.textShadow = "white";
        this.style = {},
		this.innerStyle = function(){
			return {
			'font-family':this.fontFamily,
			 'font-size':this.fontSize,
			 'font-weight':this.fontWeight,
			 'font-style':this.fontStyle,
			 'text-decoration':this.textDecoration,
			 'color':this.color,
			 'background-color':this.backgroundcColor,
			 'text-align':this.textAlign,
			 'text-shadow': '-1px 0 ' + this.textShadow + ' , 0 1px ' + this.textShadow + ' , 1px 0 ' + this.textShadow + ' , 0 -1px ' + this.textShadow,
			 'z-index':1
			 }
		},
        this.location = {
            apply : function() {
                self.style.position = "absolute";
                self.style.left = self.position.x + "px";
                self.style.top = self.position.y + "px";
            },
            center : function(width, height) {
                self.position.x = 0;
                self.position.width = width;
                self.position.y = (height - self.position.height) / 2;
            }
        }
        this.dimensions = {
            apply: function () {
                self.style.width = self.position.width + "px";
                self.style.height = self.position.height + "px";
            }
        }
    }
    var memeApplyTextCtrl = function ($scope, $location, $rootScope, $timeout, $window, $http, dialog, sharedDataService, renderingService, memeData) {

        var intialComment = new Comment(0);        
        $scope.editingComment = false;        
        $scope.selectedCommentId = 0;		
		$scope.propertiesHinted = false;
		
		$scope.waiting = false;
		$scope.waitHeading = "Please wait...";
		$scope.waitingMessage = "";
		$scope.memeData = memeData;
        $scope.toolbarStyle = function() {
            if ($scope.comment) {
                var width = $scope.comment.position.width / 2;

                return {
                    position: "absolute",
                    left: ($scope.comment.position.x + width) + "px",
                    top: ($scope.comment.position.y) + "px",
					'z-index':0,
                };
            }
        }

        // Reapply the comments from the saved meme
        if ($scope.memeData.comments) {
            //$scope.memeData.comments = memeData.comments;
            $timeout(function() {
                for (var i = 0; i < $scope.memeData.comments.length; i++) {
                    $scope.memeData.comments[i].location.apply();
                    $scope.memeData.comments[i].dimensions.apply();
                }
            }, 1000);

            $scope.comment = $scope.memeData.comments[0];
        } else {

            // Default comment
            $scope.memeData.comments = [intialComment];
            $scope.comment = $scope.memeData.comments[0];
            $scope.comment.location.center($scope.memeData.seedImage.width, $scope.memeData.seedImage.height);
            $scope.comment.location.apply();
            $scope.comment.dimensions.apply();
        }

        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close(false);
        };
        $scope.startAgain = function () {
            dialog.close({action:"startAgain"});
        };
        $scope.reset = function () {
			if (!$window.confirm("This will reset the meme to it's original state, discarding your changes. Are you sure you wish to continue?")) {
				return;
            }
            dialog.close({
						action:"reset", 
						data:memeData
					});
        };
        $scope.addComment = function () {
            // Create new comment 
            var c = new Comment($scope.memeData.comments.length);

            // Position new comment
            c.position.width = $scope.memeData.seedImage.width;

            // Add new comment
            $scope.memeData.comments.push(c);
        };
        $scope.proceed = function () {
			startWaiting("Please wait...","");
            renderingService.capture("meme", $scope.width, $scope.height, function (img) {
                sharedDataService.data.rawImage = img; // cannot add to memeData to pass to next dialog because of encoding issues of base 64                							
				memeData.comments = $scope.memeData.comments;
				endWaiting();
                dialog.close({
						action:"proceed", 
						data:memeData
					});
            });
        };
        /*Drag, drop, resize, Edit & delete*/
        $scope.selectComment = function (id) {
            $scope.comment = $scope.memeData.comments[id];
            $scope.selectedCommentId = id;
			$scope.propertiesHinted = true;
            angular.element('.comment').tooltip('destroy');
        };
		$scope.unselectComment = function (id) {
			$timeout(function () {
				$scope.propertiesHinted = false;
			}, 500);
			
        };
        $scope.startEdit = function (id) {
            $scope.editingComment = true;
			$scope.propertiesHinted = false;
			if(id){
				$scope.comment = $scope.memeData.comments[id];
				$scope.selectedCommentId = id;
			}            
            angular.element('.comment').tooltip('destroy');
        };
        $scope.endEdit = function () {
            $scope.editingComment = false;
			$scope.selectedCommentId = -1;
            angular.element('.comment').tooltip('destroy');
        };
        $scope.startDrag = function (commentId) {
            angular.element('.comment').tooltip('destroy');
            $scope.endEdit();
            $scope.selectComment(commentId);
        };

        $scope.over = function (el, target) {
            target.addClass("hoverOver");
        };
        $scope.out = function (el, target) {
            target.removeClass("hoverOver");
        };
        $scope.dropped = function (left, top, relLeft, relTop, el) {
            var x = relLeft;
            $scope.memeData.comments[$scope.comment.id].position.align = "none";
            $scope.memeData.comments[$scope.comment.id].position.x = relLeft;
            $scope.memeData.comments[$scope.comment.id].position.y = relTop;
            console.log("dropped: " + $scope.comment.id + "-" + $scope.comment.position.x + " X " + $scope.comment.position.y);

        };
        $scope.alignBottom = function (left, top, relLeft, relTop, el) {
            $scope.comment.position.align = "bottom";
            $scope.comment.position.x = 0;
            $scope.comment.position.y = 0;
        };
        $scope.resized= function (width, height)
        {
            $scope.comment.position.width = width;
            $scope.comment.position.height = height;
        }
        $scope.deleteComment = function (el, target) {
            if ($scope.comment.text != initialText) {
                if (!$window.confirm("Removing this text will discard your changes. Are you sure you wish to continue?")) {
                    return;
                }
            }
            $scope.endEdit();
            $scope.comment.dropped = true;
        };

        /*Toolbar*/
        $scope.bold = function () {
            $scope.comment.fontWeight = $scope.comment.fontWeight == "bold" ? "normal" : "bold";
        };
        $scope.italic = function () {
            $scope.comment.fontStyle = $scope.comment.fontStyle == "italic" ? "normal" : "italic";
        };
        $scope.underline = function () {
            $scope.comment.textDecoration = $scope.comment.textDecoration == "underline" ? "none" : "underline";
        };
        $scope.fontFamily = function (fontFamily) {
            $scope.comment.fontFamily = fontFamily;
			console.log($scope.comment.innerStyle());
        };
        $scope.fontSize = function (size) {
            $scope.comment.fontSize = size + "pt";
        };
        $scope.backColor = function (color) {
            $scope.comment.backgroundColor = color;
        };
        $scope.color = function (color) {
            $scope.comment.color = color;
        };
        $scope.textShadow = function (textShadow) {
            $scope.comment.textShadow = textShadow;
        };
        $scope.align = function (alignment) {
            $scope.comment.textAlign = alignment;
        };

        $timeout(function () {
            angular.element('#comment').tooltip({ placement: 'top', trigger: 'manual' }).tooltip('show');
        }, 1000);
	
		function startWaiting(heading, message){
			$scope.waiting = true;
			$scope.waitHeading = heading;
			$scope.waitingMessage = message;
		}
		function endWaiting(){
			$scope.waiting = false;
			$scope.waitHeading = "";
			$scope.waitingMessage = "";
		}
    }

    // Register the controller
    app.controller('memeApplyTextCtrl', ["$scope", "$location", "$rootScope", "$timeout", "$window", "$http", "dialog", "sharedDataService", "renderingService", "memeData", memeApplyTextCtrl]);

})();