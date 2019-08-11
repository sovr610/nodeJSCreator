const electron = require('electron')
const path = require('path')
var fs = require("fs");
const BrowserWindow = electron.remote.BrowserWindow
const notifyBtn = document.getElementById('notifyBtn')

notifyBtn.addEventListener('click', function(event) {

    var data = "New File Contents";

    fs.writeFile("temp.txt", data, (err) => {
        if (err) console.log(err);
        console.log("Successfully Written to File.");
    });

    var html = [
        "<html>",
        "<body>",
        "<h1>It works</h1>",
        "</body>",
        "</html>"
    ].join("");

    const modalPath = path.join('file://', __dirname, 'add.html')
    let win = new BrowserWindow({ width: 400, height: 200 })
    win.on('close', function() { win = null })
    win.loadURL(modalPath)
        //win.loadUrl("data:text/html;charset=utf-8," + encodeURI(html));

    win.show()
})