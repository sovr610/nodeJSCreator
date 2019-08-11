require('electron-reload')(__dirname)
    //path.join(__dirname, 'src/index.html')
const path = require('path')
const { app, BrowserWindow } = require('electron')

// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
let win

function createWindow() {
    // Create the browser window.
    win = new BrowserWindow({
        width: 800,
        height: 600,
        webPreferences: {
            nodeIntegration: true
        }
    })

    win.on('ondragstart', (event, filePath) => {
            event.sender.startDrag({
                file: filePath,
                //icon: '/path/to/icon.png'
            })
        })
        // and load the index.html of the app.
    win.loadFile(path.join(__dirname, 'src/index.html'))

    // Open the DevTools.
    win.webContents.openDevTools()

    // Emitted when the window is closed.
    win.on('closed', () => {
        // Dereference the window object, usually you would store windows
        // in an array if your app supports multi windows, this is the time
        // when you should delete the corresponding element.
        win = null
    })
}

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on('ready', createWindow)

// Quit when all windows are closed.
app.on('window-all-closed', () => {
    // On macOS it is common for applications and their menu bar
    // to stay active until the user quits explicitly with Cmd + Q
    if (process.platform !== 'darwin') {
        app.quit()
    }
})



function onProgress(progess) {
    // Use values 0 to 1, or -1 to hide the progress bar
    win.setProgressBar(progress || -1) // Progress bar works on all platforms
}

// When work completes while the app is in the background, show a badge
var numDoneInBackground = 0

function onDone() {
    var dock = electron.app.dock // Badge works only on Mac
    if (!dock || win.isFocused()) return
    numDoneInBackground++
    dock.setBadge('' + numDoneInBackground)
}

// Subscribe to the window focus event. When that happens, hide the badge
function onFocus() {
    numDoneInBackground = 0
    dock.setBadge('')
}

app.on('activate', () => {
    // On macOS it's common to re-create a window in the app when the
    // dock icon is clicked and there are no other windows open.
    if (win === null) {
        createWindow()
    }
})