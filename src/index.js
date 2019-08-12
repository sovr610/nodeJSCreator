const electron = require('electron')
const path = require('path')
var fs = require("fs");
const BrowserWindow = electron.remote.BrowserWindow
const notifyBtn = document.getElementById('notifyBtn')

var child_process = require('child_process');
var holder = document.getElementById('drag-file');
var genbutton = document.getElementById('generateNewJson');
var textarea_json = document.getElementById('json_data');
var execNode = document.getElementById("exec");
var os = require('os');


var executeTranspiler = function(resolve, rejected) {
    if (os.platform == "win32" || os.platform == "win64") {
        try {
            /*child_process.exec(__dirname + "\\..\\nodeWriter/nodeWriter/bin/Debug/netcoreapp3.0/nodeWriter.exe", function(error, stdout, stderr) {
                if (error) {
                    rejected(error);
                } else {
                    resolve(stdout);
                }
            });*/
            const modalPath = path.join('file://', __dirname, 'add.html')
            let win = new BrowserWindow({
                width: 400,
                height: 200,
                webPreferences: {
                    nodeIntegration: true
                }
            })




            win.on('close', function() { win = null })
            win.webContents.openDevTools()
            win.loadURL(modalPath)
            win.show();

            /*exec(path.join(__dirname, '/platform/windows/nodeWriter.bat'), function(err, data) {

                console.log(err)
                console.log(data.toString());
            });*/
        } catch (e) {
            console.error(e);
        }
    }
}

/**
 * 
 * @param {string} dir the path of the file & filename
 * @param {string} data the data to be written
 * @param {function} resolve success callback
 * @param {function} rejected failure callback
 */
var writeTemp = function(dir, data, resolve, rejected) {

    fs.exists(dir, check => {
        if (check == true) {

            fs.unlink(dir, err => {
                if (err) {
                    console.error(err);
                    rejected(err);
                } else {

                    fs.writeFile(dir, data, err => {
                        if (err) {
                            console.error(err);
                            rejected(err);
                        } else {
                            resolve();
                        }
                    });
                }
            });
        } else {

            fs.writeFile(dir, data, err => {
                if (err) {
                    console.error(err);
                    rejected(err);
                } else {
                    resolve();
                }
            });
        }
    });
}


genbutton.onclick = () => {
    var template = `
{
    "name": "string",
    "hasSwaggerJDSDocs": true,
    "swagger":{
        "host":"string,
        "basePath": "string",
        "info":{
            "title":"string",
            "version":"string",
            "description":"string"
        }
    },
    "DB":{
        "DBhost": "string",
        "DBport": 0,
        "DBusername": "string",
        "DBpassword": "string",
        "DB_name": "string",
        "dataBase": {
            "mysql":true
        }
    },
    "dataBaseLayout":{
        "tables": [
            {
                "id": 0,
                "tableName": "string",
                "columns": [
                    {
                        "columnName": "string",
                        "columnDataType": "string",
                        "id": 0
                    }
                ]
            }
        ]
    },
    "api":{
        "GET":[
            {
                "id":0,
                "callName": "string",
                "relatedDBTableID": 0,
                "condition":{
                    "checkQueryColID": [0]
                }
            }
        ],
        "POST": [
            {
                "id": 0,
                "callName": "string",
                "relatedDBTableID": 0,
                "dataStructure":{
                    "body":[
                        {
                            "attributeName": "string",
                            "DBcolumnID": 0,
                            "attributeType": "string"
                        }
                    ]
                }
            }
        ],
        "DELETE":[
            {
                "id": 0,
                "callName": "string",
                "relatedDBTableID": 0,
                "condition":{
                    "checkQueryColID":[0]
                }
            }
        ],
        "PUT":[
            {
                "id": 0,
                "callName": "string",
                "relatedDBTableID": 0,
                "condition":{
                    "checkQueryColID":[0]
                },
                "dataStructure":{
                    "body":[
                        {
                            "attributeName": "string",
                            "DBcolumnID": 0,
                            "attributeType": "string"
                        }
                    ]
                }
            }
        ]
    },
    "reports":[
        {
            "id": 0,
            "reportName": "string",
            "data":[
                {
                    "relatedDBTableID": 0,
                    "DBcolumnID": 0,
                    "DataName": "string",
                    "DataType": "string"
                }
            ]
        }
    ]
}`;
    textarea_json.textContent = template;
}


holder.ondrop = (e) => {
    e.preventDefault();

    for (let f of e.dataTransfer.files) {
        var dir = __dirname + "\\..\\nodeWriter/nodeWriter/bin/Debug/netcoreapp3.0/temp.json";
        fs.readFile(f.path, (err, data) => {
            var json_str = data.toString();
            writeTemp(dir, json_str, function() {
                executeTranspiler(data => {
                    console.log(data);
                }, err => {
                    console.error(err);
                    alert("ERROR: had issue starting transpiler");
                });
            }, err => {
                console.error(err);
                alert("ERROR: creating json.temp file");

            });
        });
    }
}

execNode.onclick = (e) => {
    e.preventDefault();

    var dir = __dirname + "\\..\\nodeWriter/nodeWriter/bin/Debug/netcoreapp3.0/temp.json";
    var json_str = textarea_json.value;

    writeTemp(dir, json_str, function() {
        executeTranspiler(data => {
            console.log(data);
        }, err => {
            console.error(err);
            alert("ERROR: had issue starting transpiler");
        });
    }, err => {
        console.error(err);
        alert("ERROR: creating json.temp file");

    });
};





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