const electron = require('electron')
const path = require('path')
const remote = electron.remote
const closeBtn = document.getElementById('closeBtn')
const out = document.getElementById('console');

const spawn = require('child_process').spawn;
const bat = spawn('cmd.exe', ['/c', __dirname + "\\..\\nodeWriter/nodeWriter/bin/Debug/netcoreapp3.0/start.bat"]);

bat.stdout.on('data', (data) => {
    // As said before, convert the Uint8Array to a readable string.
    var str = String.fromCharCode.apply(null, data);
    console.log(str);
    out.textContent = out.value + str;
});

bat.stderr.on('data', (data) => {
    // As said before, convert the Uint8Array to a readable string.
    var str = String.fromCharCode.apply(null, data);
    console.error(str);
    out.textContent = out.value + str;
});

bat.on('exit', (code) => {
    var preText = `Child exited with code ${code} : `;

    switch (code) {
        case 0:
            console.error(preText + "Something unknown happened executing the batch.");
            break;
        case 1:
            console.error(preText + "The file already exists");
            break;
        case 2:
            console.error(preText + "The file doesn't exists and now is created");
            break;
        case 3:
            console.error(preText + "An error ocurred while creating the file");
            break;
    }

    var window = remote.getCurrentWindow();
    window.close();
});