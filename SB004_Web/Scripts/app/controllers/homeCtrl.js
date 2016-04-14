'use strict';
(function () {

    var homeCtrl = function ($scope, $location, $rootScope, $dialog, $timeout, $q, sharedDataService, memeWizardService, securityService, blurry, timeLineService) {
        $scope.memes = sharedDataService.data.quoteSearch.results;
        $scope.searchTerm = "";
        $scope.searchCategory = "";
		$scope.view = "trending";
        $scope.following = [];
		$scope.viewing = [];
        $scope.userId = "";
        $scope.isAuthenticated=false;
        var itemsIndex=0;
        var constants = {
			viewingBlockCount:100
		};
        
        $scope.addNew = function () {

			blurry("view", true);
			memeWizardService.begin()
			.then(
				function(newMemeId){
					// New meme added
					blurry("view", false);
					
					// Move to new meme
					$scope.viewMeme(newMemeId);
				},
				function(){
					// Cancelled
					blurry("view", false);
				}				
			);
        }
        
		$scope.viewMeme = function(memeId)
		{
			 $location.path('/meme/' + memeId);
		}
        $rootScope.$on('quoteSearch.complete', function (event, data) {

            $scope.memes = sharedDataService.data.quoteSearch.results;

        });

        $scope.init = function () {
            $scope.handle = $dialog.dialog({});
        };

        $scope.init();
		$scope.switchView = function(view)
		{
			if(view=="following"){
				if (securityService.getCurrentUser().isAuthenticated) {
					$scope.view = "following";
					showFollowing();
				} else {					
					securityService.logInDialog()
					.then(function () {
						$scope.view = "following";
						showFollowing();
					});                
				}
			}else{
				$scope.view = view;
			}
			
		}
        //-------------------------------------------------------------------------------------------
		function showFollowing(){
			if(securityService.getCurrentUser()){
				if(securityService.getCurrentUser().profile.following){
					$scope.following = securityService.getCurrentUser().profile.following;
				}
                $scope.items = [];
                itemsIndex = 0;
                $scope.entryType = "All";
                var timelineEntryType = timeLineService.resolveTimelineEntryType($scope.entryType);
                
                // Retrieve  all the items that are currently visible again
                startWaiting();
                $scope.getFullTimeline(0,  constants.viewingBlockCount / (1 + $scope.following.length), timelineEntryType).
                then(function(){
                    endWaiting();
                },
                function(){
                    endWaiting();
                });            	                
			}	
           
		}
        
        $scope.getFullTimeline = function(skipitems, takeitems, type){
			
			// Skip the explicitly specified number of items (used during a refresh as 0 so all previously retrieved items are refreshed) 
			// or skip the current number of items to get the next page worth
			var skip = skipitems ? skipitems : itemsIndex;
			
			// Take the explicitly specified number of items (used during a refresh as the number of previously retrieved items)
			// or a standard page worth
			var take = takeitems ? takeitems : constants.viewingBlockCount / (1 + $scope.following.length);
			
			// Use the current entry type unless specified (convert scope entry type to time line entry type)
			var entryType = type ? type: timeLineService.resolveTimelineEntryType($scope.entryType);
			var deferred = $q.defer();
            
            // Get user and follower time lines
            timeLineService.userAndFollowingTimeline($scope.userId, entryType, skip, take)
            .then(function (data) {
					if(data.user){
						$scope.user = data.user;
					}
                    
                    // Add new time line entries						
					if(data.timelineEntries && data.timelineEntries.length>0){
                        for (var index = 0; index < data.timelineEntries.length; index++) {
                            $scope.items.push(data.timelineEntries[index]);                            
                        }
                        // Group by meme
                        $scope.items = timeLineService.organize($scope.items);
                                    
                        // Maintain a cursor of items. 
                        itemsIndex = $scope.items.length;
					}

					deferred.resolve(data);
                },
                function (e) {
					$window.alert(e);
					deferred.reject(e);
                });

			return deferred.promise;	
		}
        $scope.showMore = function(){
			$scope.getFullTimeline();
		}
        /*---------------------------------------------------------*/
        $scope.viewMyPosts= function(){
            $location.path("/usermemes/" + $scope.userId);
        }
        /*---------------------------------------------------------*/
        $scope.editMyProfile = function(){
            var profileDialog = $dialog.dialog({
                backdrop: true,
                keyboard: true,
                backdropClick: false,
                templateUrl: "Scripts/app/views/profile.html",
                controller: "profileCtrl" 
            });
            blurry("view", true);
            // Open the user profile
            profileDialog.open()
            .then(function () {				
                blurry("view", false);
            },
            function(){
                blurry("view", false);								
            });	
        }    
         /*---------------------------------------------------------*/
        $scope.signOut = function(){
            securityService.signOut();
        }   
         /*---------------------------------------------------------*/ 
		function testAuthentication(){
			// Swap between trending or following depending on if the user is authenticated and previous choices
			securityService.testIsAuthenticated()
			.then(function(isAuthenticated){
                $scope.isAuthenticated = isAuthenticated;
				if(isAuthenticated===true){
                    $scope.userId = securityService.getCurrentUser().userId;
					$scope.switchView("following");                    
				}else{
					$scope.switchView("trending");
				}
			});
		}
		
		/*-----------------------------------------------------------------*/
		// Set up the context when the user logs in
		$rootScope.$on('account.signIn', function (event, data) {
			testAuthentication();
		});
		// Set up the context when the user logs out
		$rootScope.$on('account.signOut', function (event, data) {
			testAuthentication();
		});

		// Check if the user is authenticated, if so swap between trending and following
		testAuthentication();
		/*-----------------------------------------------------------------*/
		
		function startWaiting(heading, message) {
            $scope.waiting = true;
            $scope.waitHeading = !heading ? "Please wait..." : heading;
            $scope.waitingMessage = !message ? "" : message;
        }
        function endWaiting() {
            $scope.waiting = false;
            $scope.waitHeading = "";
            $scope.waitingMessage = "";
        }        	
    }

    // Register the controller
    app.controller('homeCtrl', ["$scope", "$location", "$rootScope", "$dialog", "$timeout", "$q", "sharedDataService", "memeWizardService", "securityService", "blurry", "timeLineService", homeCtrl]);

})();