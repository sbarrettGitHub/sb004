'use strict';
(function () {
    var initialText = "Your text goes here ...";
    function Comment(id) {
        this.id = id;
        this.text = initialText;
        this.position = {
            align: "bottom",
            x: 0,
            y: 0,
            width: 0,
            height: 0,
            padding: 0
        };
        this.color = "black";
        this.backgroundColor = "none";
        this.fontFamily = "Arial";
        this.fontSize = "10pt";
        this.fontWeight = "bold";
        this.textDecoration = "none";
        this.fontStyle = "normal";
        this.textAlign = "center";
        this.dropped = false;
        this.selected = false;
        this.textShadow = "none";
    }
    var memeApplyTextCtrl = function ($scope, $location, $rootScope, $timeout, $window, $http, dialog, sharedDataService) {

        var intialComment = new Comment(0);
        $scope.comments = [intialComment];
        $scope.editingComment = false;
        $scope.comment = $scope.comments[0];
        $scope.selectedCommentId = 0;

        if (sharedDataService.data.seedImage) {
            if (sharedDataService.data.seedImage.id) {
                // Get the seed image
                $scope.seedImage = sharedDataService.data.seedImage;
                $scope.comment.position.width = $scope.seedImage.width;
                $scope.comment.position.height = $scope.seedImage.height;
            } else {
                $scope.seedImage = {
                    id: null,
                    image: 'unknown.jpg',
                    width: 'auto',
                    height: 'auto'
                }
            }
        }

        // Reapply the comments from the saved meme
        if (sharedDataService.data.meme && sharedDataService.data.meme.id) {
            $scope.comments = sharedDataService.data.meme.comments;
            $timeout(function () {
                for (var i = 0; i < $scope.comments.length; i++) {
                   // angular.element("#comment_" + i).position($scope.comments[i].position.x, $scope.comments[i].position.y);
                    //angular.element("#comment_" + i)[0].style.position = "relative";
                    //angular.element("#comment_" + i)[0].style.left = $scope.comments[i].position.x + "px";
                    //angular.element("#comment_" + i)[0].style.top = $scope.comments[i].position.y + "px";
                    //angular.element("#comment_" + i).parent().css({ position: 'relative' });
                    angular.element("#comment_" + i).css({ top: $scope.comments[i].position.y, left: $scope.comments[i].position.x, position: 'absolute' });
                    angular.element("#comment_" + i).css({ width: $scope.comments[i].position.width});
                }
            }, 3000);
            
            $scope.comment = $scope.comments[0];
        }

        /*Control buttons*/
        $scope.closeMe = function () {
            dialog.close(false);
        };
        $scope.startAgain = function () {
            dialog.close("StartAgain");
        };
        $scope.reset = function () {
            dialog.close("Reset");
        };
        $scope.addComment = function () {
            // Create new comment 
            var c = new Comment($scope.comments.length);

            // Position new comment
            c.position.width = $scope.seedImage.width;

            // Add new comment
            $scope.comments.push(c);
        };
        $scope.proceed = function () {
            html2canvas(document.getElementById("meme"), {
                letterRendering: true,
                onrendered: function (canvas) {
                    
                    var dataURL = canvas.toDataURL("image/jpg");

                    var memeImageData = dataURL.replace(/^data:image\/(png|jpg);base64,/, "");
                    // Save the meme
                    $http.post('/api/Meme', {
                        SeedId: sharedDataService.data.seedImage.id,
                        Comments: $scope.comments,
                        ImageData: memeImageData
                    }).
                    success(function (data, status, headers, config) {
                        sharedDataService.data.meme = data;
                        dialog.close("Proceed");
                    }).
                    error(function (data, status, headers, config) {
                        // called asynchronously if an error occurs
                        // or server returns response with an error status.
                    });
                }
            });


        };
        /*Drag, drop, resize, Edit & delete*/
        $scope.selectComment = function (id) {
            $scope.comment = $scope.comments[id];
            $scope.selectedCommentId = id;
            angular.element('.comment').tooltip('destroy');
        };
        $scope.startEdit = function (id) {
            $scope.editingComment = true;
            $scope.comment = $scope.comments[id];
            $scope.selectedCommentId = id;
            angular.element('.comment').tooltip('destroy');
        };
        $scope.endEdit = function () {
            $scope.editingComment = false;
            angular.element('.comment').tooltip('destroy');
        };
        $scope.startDrag = function () {
            angular.element('.comment').tooltip('destroy');
        };

        $scope.over = function (el, target) {
            target.addClass("hoverOver");
        };
        $scope.out = function (el, target) {
            target.removeClass("hoverOver");
        };
        $scope.dropped = function (left, top, relLeft, relTop, el) {
            var x = relLeft;
            $scope.comment.position.align = "none";
            $scope.comment.position.x = relLeft;
            $scope.comment.position.y = relTop;
            
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
    }

    // Register the controller
    app.controller('memeApplyTextCtrl', ["$scope", "$location", "$rootScope", "$timeout", "$window", "$http", "dialog", "sharedDataService", memeApplyTextCtrl]);

})();