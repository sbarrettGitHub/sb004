'use strict';
(function () {

    var homeCtrl = function ($scope, $location, $rootScope, $dialog, $timeout, $q, $window, $http, sharedDataService, memeWizardService, securityService, blurry, timeLineService, likeDislikeMemeService) {
        $scope.memes = sharedDataService.data.quoteSearch.results;
        $scope.searchTerm = "";
        $scope.searchCategory = "";
		$scope.view = "trending";
        $scope.following = [];
		$scope.viewing = [];
        $scope.userId = "";
		$scope.newUserComment = "";
        $scope.isAuthenticated=false;
		$scope.items = [];
        var itemsIndex=0;
		var daysIndex=1;
        var constants = {
			viewingBlockCount:100,
			dayblock : 5,
			maxEntryCount:10
		};
        $scope.maxCount = constants.maxEntryCount;
       
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
			if($scope.isAuthenticated){
				if(securityService.getCurrentUser().profile.following){
					$scope.following = securityService.getCurrentUser().profile.following;
				}
                startWaiting();
                $scope.getTimeline(daysIndex).
                then(function(){
                    endWaiting();
                },
                function(e){
					$window.alert(e);
                    endWaiting();
                });            	                
			}	
           
		}
        $scope.getTimeline = function(days){
			
			var deferred = $q.defer();
            
            // Get user and follower time lines
            timeLineService.userComprehensiveTimeline($scope.userId, days * constants.dayblock, constants.maxEntryCount)
            .then(function (data) {
					if(data.user){
						$scope.user = data.user;
					}
					
					// Merge with scope timeline group items, if there are any
					if($scope.items.length > 0){
						
						// Take each new group and insert into the current list of time line groups based on timestamp.
						// If the group is already in scope, update its timeline entries (most rescent first)
						timeLineService.mergeTimelines(data, $scope.items);
						
					}else{
						// No existing scope items. 
						$scope.items = data.timelineGroups;
					}

					deferred.resolve(data);
                },
                function (e) {
					$window.alert(e);
					deferred.reject(e);
                });

			return deferred.promise;
		}
			
        $scope.refreshMemeTimeline = function(memeId, days, max, maxPlus){
			
			var deferred = $q.defer();
			var maxCount = max;
            var currentMemeGroup;
			var currentMemeGroupIndex;
			for (var i = 0; i < $scope.items.length; i++) {
				if($scope.items[i].meme.id == memeId){
					currentMemeGroup = $scope.items[i];
					currentMemeGroupIndex = i;
				}
			}
			if(currentMemeGroup){
				maxCount = constants.maxEntryCount;
				if(max){
					maxCount = max;
				}
				if(!max){
					// Unless specified refresh teh same number of items as the meme is currently dislaying
					maxCount = currentMemeGroup.timelineEntries.length;
				}
				if(maxPlus){
					// If maxPlus is specified then get back the same number as the meme is currently dislaying
					// PLUS the maxPlus
					 maxCount = currentMemeGroup.timelineEntries.length + maxPlus;
				}
				// Get meme time line
				timeLineService.memeTimeline(memeId, days * constants.dayblock, maxCount)
				.then(function (data) {
						if(data && data.timelineGroups && data.timelineGroups.length>0){
							$scope.items[currentMemeGroupIndex] = data.timelineGroups[0];
						}		
						deferred.resolve(data);
					},
					function (e) {
						$window.alert(e);
						deferred.reject(e);
					});
			}
            
			return deferred.promise;
		}		
		$scope.addComment = function(memeId, comment){
			$http.post('api/Comment', {
                MemeId: memeId,
                Comment: comment
            }).
			success(function (data) {
				// Refresh the meme timline retrieving 1 extra entry
				$scope.refreshMemeTimeline(memeId, daysIndex, null, 1);
			}).
			error(function (e) {
				$window.alert(e);
			});
		}
		/*---------------------------------------------------------*/
		$scope.likeMeme = function(meme)
		{
			likeDislikeMemeService.like(meme).then(function(){
				// Refresh the meme timline retrieving 1 extra entry
				$scope.refreshMemeTimeline(meme.id, daysIndex, null, 1);
			});			
		}
		$scope.dislikeMeme = function(meme)
		{
			likeDislikeMemeService.dislike(meme).then(function(){
				// Refresh the meme timline retrieving 1 extra entry
				$scope.refreshMemeTimeline(meme.id, daysIndex, null, 1);
			});						
		}
		/*---------------------------------------------------------*/
        $scope.showMore = function(){
			daysIndex += constants.dayblock;
			$scope.getTimeline(daysIndex);
		}
		$scope.showMoreEntries = function(memeId){
			var currentMemeGroupIndex = -1;
			for (var i = 0; i < $scope.items.length; i++) {
				if($scope.items[i].meme.id == memeId){
					currentMemeGroupIndex = i;
				}
			}
			if(currentMemeGroupIndex>=0){
				// Get more items
				$scope.refreshMemeTimeline(memeId, daysIndex, null, $scope.items[currentMemeGroupIndex].timelineEntries.length + constants.maxEntryCount);
			}
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
    app.controller('homeCtrl', ["$scope", "$location", "$rootScope", "$dialog", "$timeout", "$q", "$window","$http", "sharedDataService", "memeWizardService", "securityService", "blurry", "timeLineService", "likeDislikeMemeService", homeCtrl]);

})();