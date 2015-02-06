'use strict';
(function () {
    var renderingService = function () {

        function isCanvasSupported() {
            var elem = document.createElement('canvas');
            return !!(elem.getContext && elem.getContext('2d'));
        }
        function supportsSvg() {
            return document.implementation.hasFeature("http://www.w3.org/TR/SVG11/feature#Image", "1.1");
        }

        var render = function (el, callback) {

            // Is SVG supported?
            if (supportsSvg()) {

                // Use domvas
                domvas();
                domvas.toImage(el, function(img) {
                    callback(img);
                });

            } else {
                // SVG not supported. If  the canvas supported?
                if (isCanvasSupported()) {
                    html2canvas(el, {
                        letterRendering: true,
                        onrendered: function(canvas) {

                            var dataURL = canvas.toDataURL("image/jpg");
                        }
                    });

                } else {
                    // Fall back on server generated image
                }
            }
        }
        return {
            render: render
        }
    };

    // Register the service
    app.factory('renderingService', [renderingService]);

})();