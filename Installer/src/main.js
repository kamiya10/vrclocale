/* eslint-disable no-empty */
const { BrowserWindow, app, ipcMain, shell } = require("electron");
const path = require("path");
const { exec } = require("child_process");

try {
	require("electron-reloader")(module);
} catch (_) { }

let win;

function createWindow() {
	win = new BrowserWindow({
		width           : 800,
		height          : 500,
		resizable       : false,
		darkTheme       : true,
		backgroundColor : "#222",
		center          : true,
		frame           : false,
		icon            : path.join(app.getAppPath(), "icon.ico"),
		webPreferences  : {
			enableRemoteModule : true,
			//			devTools           : false,
			preload            : path.join(app.getAppPath(), "preload.js")
		}
	});
	win.loadFile("./Screen/Mainview/mainview.html");
}

app.on("ready", () => {
	createWindow();
	win.webContents.on("new-window", (e, url) => {
		e.preventDefault();
		shell.openExternal(url);
	});
});

ipcMain.on("minimize", () => {
	BrowserWindow.getFocusedWindow()?.minimize();
});
ipcMain.on("close", () => {
	BrowserWindow.getFocusedWindow()?.close();
});
ipcMain.on("moveWindow", (e, { x, y }) => {
	BrowserWindow.getFocusedWindow().setBounds({
		width  : 800,
		height : 500,
		x      : x,
		y      : y
	});
});

ipcMain.on("findVRChat", () => {
	exec("@echo off && for /f \"tokens=3*\" %a in ('reg query HKCU\\SOFTWARE\\VRChat') do echo %a%b", function(error, stdout, stderr) {
		win.webContents.send("findVRChat", stdout);
	});
});