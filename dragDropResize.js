app.directive('ngdraggableresizable', function($timeout) {
  return {
    restrict: 'AE',
	scope: false,
    link: function(scope, elem, attrs) {
		$(elem).draggable({
		start: function(e, ui) {
			var pos = ui.helper.offset();
            $("#position").html(pos.left + ' x ' + pos.top);
        },
		 revert: "invalid",
		 scope: "ngdroppable"
		}).resizable({
			start: function(e, ui) {
				$("#dimensions").html(ui.size.width + ' x ' + ui.size.height);
				scope.$apply(attrs.ngresizestart);
			},
			resize: function(e, ui) {
				$("#dimensions").html(ui.size.width + ' x ' + ui.size.height);
				scope.$apply(attrs.ngresizing);
			},
			stop: function(e, ui) {
				$("#dimensions").html(ui.size.width + ' x ' + ui.size.height);
				//scope.$apply(attrs.ngresizestop(ui.size.width, ui.size.height));
				var invoker = $parse(attrs.ngresizestop);
				invoker(scope, {width: ui.size.width, height: ui.size.height});
			}
		});      
    }
  };
});

app.directive('ngdroppable', function() {
  return {
    restrict: 'AE',
    link: function(scope, elem, attrs) {	
		$(elem).droppable({
			scope: "ngdroppable",            
			drop: function (event, ui) {
				$(this).css("background-color", "lightgreen")			
				var pos = ui.draggable.offset(), dPos = $(this).offset();
				posTop = pos.top - dPos.top;
				posLeft = pos.left - dPos.left
			
				$("#position").html('Relative: ' + posLeft + ' x ' + posTop + ' => Absolute: ' + pos.left + ' x ' + pos.top);
				$("#dragged").html(ui.draggable[0].id);
				$("#droppedOn").html($(this).attr("id"));
				scope.$apply(attrs.drop);
			},
			over: function (event, ui) {
				$(this).css("background-color", "green")
				scope.$apply(attrs.over);
			},
			out: function (event, ui) {
				$(this).css("background-color", "")
				scope.$apply(attrs.out);
			}
		});
    }
  };
});