function findVRChat() {
	window.api.send("findVRChat");
}

window.api.receive("findVRChat", ( result) => {
	console.log(result);
	$("#vrchatpath-text").val(result);
	validatePath("vrchatpath-text", result);
});

$("#vrchatpath-text").on("input", () => {
	const text = $("#vrchatpath-text").val();
	validatePath("vrchatpath-text", text);
});

$("#vrchatpath-file").change(() => {
	console.log($("#vrchatpath-file").files[0].path);
	$("#vrchatpath-text").val();
});

function validatePath(id, text) {
	if (!text.match(/^[a-z]:((\\|\/)[a-z0-9\s_@\-^!#$%&+={}\[\]]+)+$/i)){
		$(`#${id}`).addClass("error");
		$("#next-button").addClass("disabled");
	} else{
		$(`#${id}`).removeClass("error");
		$("#next-button").removeClass("disabled");
	}
}