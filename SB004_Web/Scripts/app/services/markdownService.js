'use strict';
(function () {
    var markdownService = function ($window) {
        var makeHtml = function(markdown){
            var converter = new $window.Showdown.converter();
            return converter.makeHtml(markdown);
        }	        
        return {
            makeHtml:makeHtml
        }
    }
    // Register the service
    app.factory('markdownService', ['$window', markdownService]);

})();