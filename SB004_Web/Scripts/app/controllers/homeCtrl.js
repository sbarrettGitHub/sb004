'use strict';
(function () {

    var homeCtrl = function ($scope, $location, $rootScope, $dialog, $timeout, $q, $window, $http, sharedDataService, memeWizardService, securityService, blurry, timeLineService, likeDislikeMemeService, hashTagService) {
        $scope.memes = sharedDataService.data.quoteSearch.results;		
        $scope.searchTerm = "";
        $scope.searchCategory = "";
		$scope.view = "trending";
        $scope.following = [];
		$scope.viewing = [];
		$scope.trendingHashTagMemes= [];
		$scope.trendingHashTags = [];
        $scope.userId = "";
		$scope.newUserComment = "";
        $scope.isAuthenticated=false;
		$scope.items = [];
        $scope.words = [];
		$scope.hashTags = [];
		
		var itemsIndex=0;		
        var constants = {
			viewingBlockCount:100,
			dayblock : 5,
			maxEntryCount:10,
			defaultTakeTrendingHashTags:20,
			trendingMemeBlockSize:20,
			trendingHashTagBlockSize:5,
			wordCloudMaxWeight:10
		};
		var takeTrendingHashTags = constants.defaultTakeTrendingHashTags;		
		var maxTrendingMemes = 100;	
		
        $scope.maxCount = constants.maxEntryCount;
       	$scope.daysIndex=constants.dayblock;
		var repostDialogOpts ={
            backdrop: true,
            keyboard: true,
            backdropClick: false,
            templateUrl: "Scripts/app/views/repost.html",
            controller: "repostCtrl" 
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
			switch (view) {
				case "timeline":
					if (securityService.getCurrentUser().isAuthenticated) {
						$scope.view = "timeline";
						showTimeline();
					} else {					
						securityService.logInDialog()
						.then(function () {
							$scope.view = "timeline";
							showTimeline();
						});                
					}					
					break;
				case "trending":
					$scope.view = view;
					refreshTrendingView();
					break;
					
				case "search":
					$scope.view = view;
					refreshHashTagView();
					break;					
				default:
					$scope.view = view;
					break;
			}
		}
		//-------------------------------------------------------------------------------------------
		// Trending
		var refreshTrendingView = function(){
			
			startWaiting();		
			
			var hashTagFilter = [];
			// Build the hash tag filter from selected hash tags ( if there are any as there won't be on the initial call)
			for (var ihashTagIndex = 0; ihashTagIndex < $scope.trendingHashTags.length; ihashTagIndex++) {
				if($scope.trendingHashTags[ihashTagIndex].include){
					hashTagFilter.push($scope.trendingHashTags[ihashTagIndex].hashTag);
				}
			}
			var includedHashTags = hashTagFilter.length == 0? takeTrendingHashTags:hashTagFilter.length ;
						
			// Calculate how many mmes per hash tag to be retrieved
			// The less hash tag s selected the more memes per hash tag
			var takeMemes = Math.floor(maxTrendingMemes/includedHashTags);
			
			// Retrieve the trending hash tag memes
			hashTagService.trendingHashTagMemes(takeTrendingHashTags,takeMemes, hashTagFilter)
			.then(function(data){
				if(data){
					// If there are no existing trending hash tags (as per inital state) build from the returned data 
					if($scope.trendingHashTags.length == 0){
						for (var i = 0; i < data.length; i++) {
							var hashTagMeme = data[i];
							$scope.trendingHashTags.push({
								hashTag:hashTagMeme.hashTag,
								include:true
							});						

						}
					} 
					
					// Add the trending memes
					$scope.trendingHashTagMemes= [];
					for (var i = 0; i < data.length; i++) {
						var hashTagMeme = data[i];
						// Is the hash tag marked as included?
						for (var ihashTagIndex = 0; ihashTagIndex < $scope.trendingHashTags.length; ihashTagIndex++) {
							if(hashTagMeme.hashTag == $scope.trendingHashTags[ihashTagIndex].hashTag && $scope.trendingHashTags[ihashTagIndex].include){
								if(hashTagMeme.memeLiteModels){
									for (var ii = 0; ii < hashTagMeme.memeLiteModels.length; ii++) {
										$scope.trendingHashTagMemes.push(hashTagMeme.memeLiteModels[ii]);								
									}							
								}
							}
						}
					}
				}
				endWaiting();
			},
			function(e){
				$window.alert(e);
				endWaiting();
			});
		}
		// Switch on/off the supplied hash tag and refresh the trend view
		$scope.toggleTrendingHashTag = function(hashTag){
			for (var i = 0; i < $scope.trendingHashTags.length; i++) {
				if($scope.trendingHashTags[i].hashTag == hashTag){
					$scope.trendingHashTags[i].include = !$scope.trendingHashTags[i].include;
					break;
				}							
			}
			// Refresh (with or without the toggled hash tag)
			refreshTrendingView();
		}	
		$scope.increaseTrendingMemesPerHashTag = function(){
			maxTrendingMemes+=constants.trendingMemeBlockSize;
			refreshTrendingView();
		}
		$scope.increaseTrendingHashTags = function(){
			takeTrendingHashTags+=constants.trendingHashTagBlockSize;
			hashTagService.trendingHashTags(takeTrendingHashTags)
			.then(function(data){
				if(data){
					for (var i = 0; i < data.length; i++) {
						var alreadyIncluded=false;
						for (var ihashTagIndex = 0; ihashTagIndex < $scope.trendingHashTags.length; ihashTagIndex++) {
							if(data[i] == $scope.trendingHashTags[ihashTagIndex].hashTag){
								alreadyIncluded = true;
							}
						}
						if(!alreadyIncluded){
							$scope.trendingHashTags.push({
								hashTag:data[i],
								include:true
							});
						}
					}
					refreshTrendingView();
				}
			},
			function(e){
				$window.alert(e);
				endWaiting();
			})
			
		}	

		//-------------------------------------------------------------------------------------------	
		// #HashTags
		var initiateHashTagSearch=function(criteria){
			$scope.switchView("search");
			$scope.hashTags=[];
			var hashTags = criteria.q.replace(/#/g, '').split(' ');
			if( hashTags.length > 0){
				for (var index = 0; index < hashTags.length; index++) {
					var hashTag = hashTags[index];
					if(!$scope.hashTags.contains(hashTag)){
						$scope.hashTags.push(hashTag);					
					}
				}
				hashTagSearch();	
			}
		}
		$scope.wordCloudClick = function(id){
			
			var hashTag = id.replace(/#/g, '');
			if(!$scope.hashTags.contains(hashTag)){
				$scope.hashTags.push(hashTag);
				hashTagSearch();
			}

			
		}
		$scope.removeHashTag = function(hashTag){
			var i = $scope.hashTags.indexOf(hashTag);
			if (i > -1) {
				$scope.hashTags.splice(i, 1);
				if($scope.hashTags.length>0){
					hashTagSearch();
				}else{
					refreshHashTagView();
				}
				
			}
		}
		var openHashTagView = function(){
			$scope.view = "search";
		}
		var refreshHashTagView = function(){	
			$scope.words = [];		
			hashTagService.trendingHashTags(50)
			.then(function(data){
				if(data){
					populateWordCloud(data);							
				}
			},
			function(e){
				$window.alert(e);
				endWaiting();
			})
		}	

		var hashTagSearch = function(){
			startWaiting();
			$scope.memes=[];
			hashTagService.hashTagMemes($scope.hashTags, 50)
			.then(function(data){
				if(data){
					if( data.memes){						
						// Populate the memes returned by the search
						populateMemes(data.memes);						
					}
					$scope.words = [];
					if(data.similarHashTags){
						// Populate the word cloud with similar hash tags to the one(s) searched for
						populateWordCloud(data.similarHashTags);
					}							
				}				
				endWaiting();
			},
			function(e){
				$window.alert(e);
				endWaiting();
			})
		}
		$scope.appendToHashtagsAndSearch = function(hashTag){
			if(hashTag && hashTag.ngTag){
				$scope.wordCloudClick(hashTag.ngTag);
			}
		}  
		function populateWordCloud(data){
			if(data){
				$scope.words = [];
				for (var i = 0; i < data.length; i++) {		
					if(!$scope.words.contains(data[i]))	{
						$scope.words.push({text: "#" + data[i], weight: ( data.length) - i + 1,handlers: {
							click: function(e) {		
								console.log(e);	
								$scope.wordCloudClick(this.innerHTML);
							}
						}});
					}						
				}							
			}
		}
		function populateMemes(data){
			if(data){
				$scope.memes=[];
				for (var i = 0; i < data.length; i++) {
					for (var ii = 0; ii < data[i].memeLiteModels.length; ii++) {
						$scope.memes.push( data[i].memeLiteModels[ii]);
						
					} 											
				}
			}
		}
        //-------------------------------------------------------------------------------------------
		// Homepage Timeline
		function showTimeline(){
			if($scope.isAuthenticated){
				if(securityService.getCurrentUser().profile.following){
					$scope.following = securityService.getCurrentUser().profile.following;
				}
                startWaiting();
                $scope.getTimeline($scope.daysIndex).
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
				$scope.refreshMemeTimeline(memeId, $scope.daysIndex, null, 1);
			}).
			error(function (e) {
				$window.alert(e);
			});
		}
		$scope.likeComment = function(commentId, memeId)
		{
			// Don't allow multiple likes by the same user on the same comment
			for(var i=0;i<securityService.getCurrentUser().myCommentLikes.length;i++){
				if(securityService.getCurrentUser().myCommentLikes[i] == commentId){
					return;
				}
			}
			$http({ method: 'PATCH', url: 'api/Comment/' + commentId + "/like/", data: {}})
				.success(function (data) {  
					securityService.getCurrentUser().myCommentLikes.push(data.id);// Remember that you like this comment (so you can't keep clicking like)
					$scope.refreshMemeTimeline(memeId, $scope.daysIndex, null, 1);
                }).error(function (e) {
					$window.alert(e);
					return;
                });
		}		
		$scope.dislikeComment = function(commentId, memeId)
		{
			// Don't allow multiple dislikes by the same user on the same comment
			for(var i=0;i<securityService.getCurrentUser().myCommentDislikes.length;i++){
				if(securityService.getCurrentUser().myCommentDislikes[i] == commentId){
					return;
				}
			}
			 $http({ method: 'PATCH', url: 'api/Comment/' + commentId + "/dislike/" , data: {}})
				.success(function (data) {  
					securityService.getCurrentUser().myCommentDislikes.push(data.id); // Remember that you dislike this comment (so you can't keep clicking dislike)
					$scope.refreshMemeTimeline(memeId, $scope.daysIndex, null, 1);	
							
                }).error(function (e) {
					$window.alert(e);
					return;
                });
		}		
		/*---------------------------------------------------------*/
		$scope.likeMeme = function(meme)
		{
			likeDislikeMemeService.like(meme).then(function(){
				// Refresh the meme timline retrieving 1 extra entry
				$scope.refreshMemeTimeline(meme.id, $scope.daysIndex, null, 1);
			});			
		}
		$scope.dislikeMeme = function(meme)
		{
			likeDislikeMemeService.dislike(meme).then(function(){
				// Refresh the meme timline retrieving 1 extra entry
				$scope.refreshMemeTimeline(meme.id, $scope.daysIndex, null, 1);
			});						
		}
		// -------------------------------------------------------------------
		// Respond
		// -------------------------------------------------------------------
		$scope.respond = function (meme) {
			memeWizardService.beginWithMemeId(meme.id, meme.id)
			.then(function(newMemeId){				
				$http({ method: 'PATCH', url: 'api/Meme/' + meme.id + "/reply/" + newMemeId, data: {}})
				.success(function (data) {  
					// Refresh the meme timline
					$scope.refreshMemeTimeline(meme.id, $scope.daysIndex, null, 1);				
                }).error(function (e) {
					$window.alert(e);
					return;
                });					
			},
			function(){
				$window.alert("Rejected");
			});
         
        };		
		// -------------------------------------------------------------------
		// Repost
		// -------------------------------------------------------------------
		// Repost
		$scope.repostMeme = function(memeId){
			if(securityService.getCurrentUser().isAuthenticated==false){
				securityService.logInDialog()
					.then(function(){
						repost(memeId);
						
					});
			}else{
				repost(memeId);
			}
		}
		var repost = function(memeId){
			var deferred = $q.defer();
			repostDialogOpts.resolve = {memeId : function() {return memeId;}};
			$dialog.dialog(repostDialogOpts).open().then(function (dialogResult) {	
				if(dialogResult.action == "repost"){
					$scope.meme.reposts++;
					// Refresh the meme timline
					$scope.refreshMemeTimeline(meme.id, $scope.daysIndex, null, 1);	
				}
				deferred.resolve();
			});	
			return deferred.promise;
		}
		/*---------------------------------------------------------*/
        $scope.showMore = function(){
			$scope.daysIndex += constants.dayblock;
			$scope.getTimeline($scope.daysIndex);
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
				$scope.refreshMemeTimeline(memeId, $scope.daysIndex, null, $scope.items[currentMemeGroupIndex].timelineEntries.length + constants.maxEntryCount);
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
					$scope.switchView("timeline");                    
				}else{
					$scope.switchView("trending");
				}
			});
		}
		
		/*-----------------------------------------------------------------*/
		// Listeners
		// Set up the context when the user logs in
		$rootScope.$on('account.signIn', function (event, data) {
			testAuthentication();
		});
		// Set up the context when the user logs out
		$rootScope.$on('account.signOut', function (event, data) {
			testAuthentication();
		});
		
		// Performed by search for hash tag to force the hompage to show the search. Neccessary of the home page is already visible
		$rootScope.$on('home.viewSearch', function (event, data) {			
			openHashTagView();
		});
		/*-----------------------------------------------------------------*/
		
		// Check if the user is authenticated, if so swap between trending and following
		var criteria = $location.search();
		if(criteria && criteria.q){
			initiateHashTagSearch(criteria);
			  
		}else{
			testAuthentication();
		}
		
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
    app.controller('homeCtrl', ["$scope", "$location", "$rootScope", "$dialog", "$timeout", "$q", "$window","$http", "sharedDataService", "memeWizardService", "securityService", "blurry", "timeLineService", "likeDislikeMemeService", "hashTagService", homeCtrl]);

})();