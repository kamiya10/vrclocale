document.onreadystatechange = () => {
	if (document.readyState == "complete")
		handleWindowControls();
};

window.onbeforeunload = () => {
	$("#titlebar").children().off();
};

function handleWindowControls() {
	let wX, wY, dragging = false;
	$("#min-button").on("click", () => {
		window.api.send("minimize");
	});
	$("#close-button").on("click", () => {
		window.api.send("close");
	});
	$("#titlebar").mousedown((e) => {
		dragging = true;
		wX = e.pageX;
		wY = e.pageY;
	});
	$(window).mousemove((e) => {
		if (dragging) {
			e.stopPropagation();
			e.preventDefault();
			const xLoc = e.screenX - wX;
			const yLoc = e.screenY - wY;
			try {
				window.api.send("moveWindow", { x: xLoc, y: yLoc });
			} catch (err) {
				console.log(err);
			}
		}
	});
	$("#titlebar").mouseup(() => {
		dragging = false;
	});
}

function changeview(view) {
	$("#wrapper").fadeOut(200, () => {
		$("#wrapper").load(`../${view}/${view}.html`, () => {
			$("#wrapper").fadeIn(400);
		});
	});
}
changeview("Disclaimer");