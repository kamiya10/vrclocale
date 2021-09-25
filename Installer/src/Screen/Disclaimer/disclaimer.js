$("#accept-disclaimer-checkbox").change(() => {
	if($("#accept-disclaimer-checkbox").is(":checked"))
		$("#next-button").removeClass("disabled");
	else
		$("#next-button").addClass("disabled");
});