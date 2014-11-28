'use strict';
(function () {
    var sharedDataService = function () {
        var data = {
          quoteSearch:{searchTerm: "", results:[]}
        };
        
        return {
            data:data
        }
    };

    // Register the service
    app.factory('sharedDataService', [sharedDataService]);

})();