const {
	contextBridge,
	ipcRenderer
} = require("electron");

// Expose protected methods that allow the renderer process to use
// the ipcRenderer without exposing the entire object
contextBridge.exposeInMainWorld(
	"api", {
		send: (channel, data) => {
			// whitelist channels
			const validChannels = [ "minimize", "close", "moveWindow", "findVRChat" ];
			if (validChannels.includes(channel))
				ipcRenderer.send(channel, data);
		},
		receive: (channel, func) => {
			const validChannels = ["findVRChat"];
			if (validChannels.includes(channel))
			// Deliberately strip event as it includes `sender`
				ipcRenderer.on(channel, (event, ...args) => func(...args));
		},
		receiveOnce: (channel, func) => {
			const validChannels = [];
			if (validChannels.includes(channel))
			// Deliberately strip event as it includes `sender`
				ipcRenderer.once(channel, (event, ...args) => func(...args));
		}
	}
);