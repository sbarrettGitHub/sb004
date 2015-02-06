'use strict';
(function () {
    var sharedDataService = function () {
        var data = {
			quoteSearch:{searchTerm: "", results:[]},
			seedImage:{
				id:null,
				image:null,
				width:null,
				height:null
			},
            meme: {
                
            },
            rawImage:{
			
            }
        };
        var resetMeme = function() {
            data.meme = {};
            data.seedImage = {};
        }
        return {
            resetMeme:resetMeme,
            data:data
        }
    };

    // Register the service
    app.factory('sharedDataService', [sharedDataService]);

})();