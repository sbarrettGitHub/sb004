'use strict';
(function () {
    var renderingService = function () {
        var renderingEnum = { svg: 1, canvas: 2, other: 0 };
        var currentRender;
        function isCanvasSupported() {
            var elem = document.createElement('canvas');
            return !!(elem.getContext && elem.getContext('2d'));
        }
        function supportsSvg() {
            return false;
            return document.implementation.hasFeature("http://www.w3.org/TR/SVG11/feature#Image", "1.1");
        }

        var capture = function (elName, width, height, callback) {
            var el = document.getElementById(elName);
            // Is SVG supported?
            if (supportsSvg()) {

                currentRender = renderingEnum.svg;

                // Use domvas
                domvs();
                domvas.toImage(el, function (img) {
                    callback(img);
                });

            } else {
                // SVG not supported. If  the canvas supported?
                if (isCanvasSupported()) {
                    currentRender = renderingEnum.canvas;
                    html2canvas(el, {
                        letterRendering: true,
                        onrendered: function(canvas) {

                            var dataURL = canvas.toDataURL("image/jpg");
                            callback(dataURL);
                        }
                    });

                } else {
                    // Fall back on server generated image
                    currentRender = renderingEnum.other;
                }
            }
        }
        var render = function (canvasName, width, height, rawImage) {
            if (currentRender == renderingEnum.svg)
            {
                var canvas = document.getElementById(canvasName),
                context = canvas.getContext('2d');
                canvas.width = width;
                canvas.height = height;

                // Otherwise, add the image to the canvas
                context.drawImage(rawImage, 0, 0);
            }
            
        }
        return {
            capture: capture,
            render: render
        }
    };

    // Register the service
    app.factory('renderingService', [renderingService]);

})();