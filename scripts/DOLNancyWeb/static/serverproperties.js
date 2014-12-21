$(document).ready(function () {
	$("#dlw_serverproperties input:not([readonly])").each(function (index, element) {
		var $element = $(element);
		
		$element.on("change", function () {
			$apply = $(this).parent().siblings().last().children().last();

			if ($(this).val() != $(this).prop("defaultValue"))
			{
				$(this).toggleClass("dlw_inputchanged", true);
			}
			else
			{
				$(this).toggleClass("dlw_inputchanged", false);
			}
		});		
	});
	
	$("#dlw_serverproperties textarea:not([readonly])").each(function (index, element) {
		var $element = $(element);
		
		$element.on("change", function () {
			if ($(this).val() != $(this).prop("defaultValue"))
			{
				$(this).toggleClass("dlw_inputchanged", true);
			}
			else
			{
				$(this).toggleClass("dlw_inputchanged", false);
			}
		});
	});
});