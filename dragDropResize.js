app.directive('ngdraggableresizable', function($parse) {
  return {
    restrict: 'AE',
	scope: false,
    link: function(scope, elem, attrs) {
		$(elem).draggable({
		start: function(e, ui) {
			var pos = ui.helper.offset();
            $("#position").html(pos.left + ' x ' + pos.top);
			//console.log('ngdragstart: ' + pos.left + ' x ' + pos.top);	
			var invoker = $parse(attrs.ngdragstart);
			invoker(scope, {left:pos.left, top:pos.top});
        },
		 revert: "invalid",
		 scope: "ngdroppable"
		}).resizable({
			start: function(e, ui) {
				//console.log('ngresizestart:' + ui.size.width + ' x ' + ui.size.height);
				scope.$apply(attrs.ngresizestart);
			},
			resize: function(e, ui) {
				//console.log('ngresizing:' + ui.size.width + ' x ' + ui.size.height);				
				var invoker = $parse(attrs.ngresizing);
				invoker(scope, {width: ui.size.width, height: ui.size.height});
			},
			stop: function(e, ui) {
				//console.log('ngresizestop:' + ui.size.width + ' x ' + ui.size.height);				
				var invoker = $parse(attrs.ngresizestop);
				invoker(scope, {width: ui.size.width, height: ui.size.height});
			}
		});      
    }
  };
});

app.directive('ngdroppable', function($parse) {
  return {
    restrict: 'AE',
    link: function(scope, elem, attrs) {	
		$(elem).droppable({
			scope: "ngdroppable",            
			drop: function (event, ui) {				
				var pos = ui.draggable.offset(), dPos = $(this).offset();
				posTop = pos.top - dPos.top;
				posLeft = pos.left - dPos.left			
				//console.log('ngdrop: Relative: ' + posLeft + ' x ' + posTop + ' => Absolute: ' + pos.left + ' x ' + pos.top + ' => element: ' + ui.draggable[0].id);						
				var invoker = $parse(attrs.ngdrop);
				invoker(scope, {left: pos.left, top:pos.top, relLeft: posLeft, relTop: posTop, el:ui.draggable[0]});			
			},
			over: function (event, ui) {				
				//console.log('ngdragover:Element: ' + ui.draggable[0].id);						
				var invoker = $parse(attrs.ngdragover);
				invoker(scope, {el:ui.draggable[0]});
			},
			out: function (event, ui) {
				//console.log('ngdragout:Element: ' + ui.draggable[0].id);						
				var invoker = $parse(attrs.ngdragout);
				invoker(scope, {el:ui.draggable[0]});
			}
		});
    }
  };
});