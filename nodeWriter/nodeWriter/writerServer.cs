using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Linq;

namespace nodeWriter
{
    public class writerServer
    {
        public serverJsonObj readJsonConfigFile()
        {
            try
            {
                string txt = File.ReadAllText(Environment.CurrentDirectory + "\\temp.json");
                serverJsonObj json = JsonConvert.DeserializeObject<serverJsonObj>(txt);
                return json;
            }
            catch(Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }

        public string writeServerCode(serverJsonObj server)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("'use strict'");
                sb.AppendLine("var express = require('express');");
                sb.AppendLine("const path = require('path');");
                sb.AppendLine("const fs = require('fs');");
                if (server.hasSwaggerJDcos)
                {
                    sb.AppendLine("var swaggerJSDoc = require('swagger-jsdoc');");
                }
                if (server.DB.dataBase.mysql)
                {
                    sb.AppendLine("const sql = require('./mysqlDB');");
                }

                sb.AppendLine("var cors = require('cors');");
                sb.AppendLine("const bodyParser = require('body-parser')");
                sb.AppendLine("const logger = pino({");
                sb.AppendLine("\tprettyPrint: {");
                sb.AppendLine("\t\tcolorize: true");
                sb.AppendLine("\t}");
                sb.AppendLine("});");
                sb.AppendLine("const app = express();");
                sb.AppendLine("const port = 3000;");
                sb.AppendLine("app.use(cors());");
                sb.AppendLine("app.use(logger);");
                sb.AppendLine("app.use(bodyParser.urlencoded({");
                sb.AppendLine("\textended: false");
                sb.AppendLine("}));");
                sb.AppendLine("app.use(bodyParser.json());");
                sb.AppendLine("app.use(function(req,res,next) {");
                sb.AppendLine("\tres.header('Access-Control-Allow-Origin','*');");
                sb.AppendLine("\tres.header('Access-Control-Allow-Headers','Origin, X-Requested-with, Content-Type, Accept');");
                sb.AppendLine("\tnext();");
                sb.AppendLine("});");
                if (server.hasSwaggerJDcos)
                {
                    sb.AppendLine("var swaggerDefinition = {");
                    sb.AppendLine("\tinfo: {");
                    sb.AppendLine("\t\ttitle: '" + server.swagger.info.title + "',");
                    sb.AppendLine("\t\tversion: '" + server.swagger.info.version + "',");
                    sb.AppendLine("\t\tdescription: '" + server.swagger.info.description + "'");
                    sb.AppendLine("\t},");
                    sb.AppendLine("\thost: '" + server.swagger.host + "',");
                    sb.AppendLine("\tbasePath: '" + server.swagger.basePath + "'");
                    sb.AppendLine("};");
                    sb.AppendLine("var options = {");
                    sb.AppendLine("\tswaggerDefinition: swaggerDefinition,");
                    sb.AppendLine("\tapis: [path.resolve(__dirname, 'server.js')]");
                    sb.AppendLine("};");
                    sb.AppendLine("var swaggerSpec = swaggerJSDoc(options);");
                    sb.AppendLine("");
                    sb.AppendLine("app.get('/docs',async(req,res,next) => {");
                    sb.AppendLine("\ttry{");
                    sb.AppendLine("\t\tres.sendFile(path.join(__dirname, 'redoc.html'));");
                    sb.AppendLine("\t}catch (e) {");
                    sb.AppendLine("\t\treq.log.error(e);");
                    sb.AppendLine("\t\tnext(e);");
                    sb.AppendLine("\t}");
                    sb.AppendLine("});");
                    sb.AppendLine(" ");
                    sb.AppendLine("app.get('/swagger.json',async(req,res,next) => {");
                    sb.AppendLine("\ttry{");
                    sb.AppendLine("\t\tres.sendHeader('Content-Type', 'application/json');");
                    sb.AppendLine("\t\tres.send(swaggerSpec);");
                    sb.AppendLine("\t} catch (e) {");
                    sb.AppendLine("\t\treq.log.error(e);");
                    sb.AppendLine("\t\tnext(e);");
                    sb.AppendLine("\t}");
                    sb.AppendLine("});");
                    sb.AppendLine(" ");
                }

                foreach (var getAPI in server.api.GET)
                {
                    string callName = getAPI.callName;
                    int DBtable = getAPI.relatedDBTableID;
                    DBtables dt = (from a in server.dataBaseLayout.tables
                                   where a.id == DBtable
                                   select a).ToList()[0];

                    sb.AppendLine("app.get('/" + callName + "', async(req,res,next) => {");
                    sb.AppendLine("\ttry{");
                    if (getAPI.condition.checkQueryColID.Length > 0)
                    {
                        string where_str = null;
                        foreach(int colID in getAPI.condition.checkQueryColID)
                        {

                            DBtableColumns col_name = (from a in dt.columns
                                                       where a.id == colID
                                                       select a).ToList()[0];
                            where_str = where_str + "'" + col_name.columnName + " = ' + req.query." + col_name.columnName + "+" ;
                        }
                        where_str = where_str.Substring(0, where_str.Length - 1);
                        sb.AppendLine("\t\tsql.getRowsInTableWithWhere({ tableName: '" + dt.tableName + "', where: " + where_str + "}, function(data) {");

                    }
                    else
                    {
                        sb.AppendLine("\t\tsql.getAllRowsInTable({ tableName: '" + dt.tableName + "'}, function(data) {");
                    }

                    sb.AppendLine("\t\t\treq.log.info(data);");
                    sb.AppendLine("\t\t\tres.json(data);");
                    sb.AppendLine("\t\t}, function(err) {");
                    sb.AppendLine("\t\t\treq.log.error(err);");
                    sb.AppendLine("\t\t\tnext({error:err});");
                    sb.AppendLine("\t\t});");
                    sb.AppendLine("\t}");
                    sb.AppendLine("\tcatch (e) {");
                    sb.AppendLine("\t\treq.log.error(e);");
                    sb.AppendLine("\t\tnext(e);");
                    sb.AppendLine("\t}");
                    sb.AppendLine("});");
                    sb.AppendLine(" ");


                }

                foreach(var putAPI in server.api.PUT)
                {
                    var callanme = putAPI.callName;
                    bodyObj[] body = putAPI.dataStructure.body;
                    sb.AppendLine("app.put('/" + callanme + "', async(req, res, next) => {");
                    sb.AppendLine("\ttry{");
                    int DBtable = putAPI.relatedDBTableID;
                    DBtables dt = (from a in server.dataBaseLayout.tables
                                   where a.id == DBtable
                                   select a).ToList()[0];
                    foreach(var singBody in body)
                    {
                        sb.AppendLine("\t\tvar " + singBody.attributeName + " = req.body.body." + singBody.attributeName);
                    }
                    sb.AppendLine("\t\tvar values = [];");
                    foreach (var singBody in body)
                    {
                        DBtableColumns col = (from a in dt.columns
                                              where a.id == singBody.DBcolumnID
                                              select a).ToList()[0];
                        sb.AppendLine("\t\tvalues.push({ columnName: '" + col.columnName + "', value: " + singBody.attributeName + "});");

                    }

                    string where_str = null;
                    foreach (int colID in putAPI.condition.checkQueryColID)
                    {

                        DBtableColumns col_name = (from a in dt.columns
                                                   where a.id == colID
                                                   select a).ToList()[0];
                        where_str = where_str + "'" + col_name.columnName + " = ' + req.query." + col_name.columnName + "+";
                    }
                    where_str = where_str.Substring(0, where_str.Length - 1);

                    sb.AppendLine("\t\tsql.updateTable({ tableName: '" + dt.tableName + "', where: " + where_str + "}, function(data) {");
                    sb.AppendLine("\t\t\treq.log.info(data);");
                    sb.AppendLine("\t\t\tres.json(data);");
                    sb.AppendLine("\t\t}, function(err) {");
                    sb.AppendLine("\t\t\treq.log.error(err);");
                    sb.AppendLine("\t\t\tnext({error:err});");
                    sb.AppendLine("\t\t});");
                    sb.AppendLine("\t}");
                    sb.AppendLine("\tcatch (e) {");
                    sb.AppendLine("\t\treq.log.error(e);");
                    sb.AppendLine("\t\tnext(e);");
                    sb.AppendLine("\t}");
                    sb.AppendLine("});");
                    sb.AppendLine(" ");

                }

                foreach(var postAPI in server.api.POST)
                {
                    var callanme = postAPI.callName;
                    bodyObj[] body = postAPI.dataStructure.body;
                    sb.AppendLine("app.post('/" + callanme + "', async(req, res, next) => {");
                    sb.AppendLine("\ttry{");
                    int DBtable = postAPI.relatedDBTableID;
                    DBtables dt = (from a in server.dataBaseLayout.tables
                                   where a.id == DBtable
                                   select a).ToList()[0];
                    foreach (var singBody in body)
                    {
                        sb.AppendLine("\t\tvar " + singBody.attributeName + " = req.body.body." + singBody.attributeName);
                    }
                    sb.AppendLine("\t\tvar values = [];");
                    foreach (var singBody in body)
                    {
                        DBtableColumns col = (from a in dt.columns
                                              where a.id == singBody.DBcolumnID
                                              select a).ToList()[0];
                        sb.AppendLine("\t\tvalues.push({ columnName: '" + col.columnName + "', value: " + singBody.attributeName + "});");

                    }



                    sb.AppendLine("\t\tsql.insertIntoTable({ tableName: '" + dt.tableName + "', values: values }, function(data) {");
                    sb.AppendLine("\t\t\tconsole.log(data);");
                    sb.AppendLine("\t\t}, function(err) {");
                    sb.AppendLine("\t\t\tconsole.log(err);");
                    sb.AppendLine("\t\t});");
                    sb.AppendLine("\t} catch (e) {");
                    sb.AppendLine("});");
                    sb.AppendLine(" ");

                }

                foreach(var delAPI in server.api.DELETE)
                {
                    var callanme = delAPI.callName;
                    //bodyObj[] body = delAPI.dataStructure.body;
                    sb.AppendLine("app.delete('/" + callanme + "', async(req, res, next) => {");
                    sb.AppendLine("\ttry{");
                    int DBtable = delAPI.relatedDBTableID;
                    DBtables dt = (from a in server.dataBaseLayout.tables
                                   where a.id == DBtable
                                   select a).ToList()[0];
                    string where_str = null;
                    foreach (int colID in delAPI.condition.checkQueryColID)
                    {

                        DBtableColumns col_name = (from a in dt.columns
                                                   where a.id == colID
                                                   select a).ToList()[0];
                        where_str = where_str + "'" + col_name.columnName + " = ' + req.query." + col_name.columnName + "+";
                    }
                    where_str = where_str.Substring(0, where_str.Length - 1);
                    sb.AppendLine("\t\tsql.deleteRowsInTable({ tableName: '" + dt.tableName + "', where: " + where_str + "}, function(data) {");
                    sb.AppendLine("\t\t\tconsole.log(data);");
                    sb.AppendLine("\t\t}, function(err) {");
                    sb.AppendLine("\t\t\tconsole.log(err);");
                    sb.AppendLine("\t\t});");
                    sb.AppendLine("\t} catch (e) {");
                    sb.AppendLine("\t\treq.log.error(e);");
                    sb.AppendLine("\t\tnext(e);");
                    sb.AppendLine("\t}");
                    sb.AppendLine("});");
                    sb.AppendLine(" ");
                }
                

                foreach(var report in server.reports)
                {
                    try
                    {
                        sb.AppendLine("app.get('/" + report.reportName + "', async(req, res, next) => {");
                        sb.AppendLine("\ttry{");
                        sb.AppendLine("\t\tsql.customQuery(\"" + )
                    }
                    catch(Exception i)
                    {
                        Console.WriteLine(i);
                    }
                }



                sb.AppendLine("app.listen(port, () => {");
                sb.AppendLine("\tconsole.log('server is running on port ' + port);");
                sb.AppendLine("\ttry{");
                sb.AppendLine("\t\tfs.readFile(path.join(__dirname, 'config.json'), 'utf8', function(err, contents) {");
                sb.AppendLine("\t\t\tif (err) throw err;");
                sb.AppendLine("\t\t\tvar obj = JSON.parse(contents);");
                sb.AppendLine("\t\t\tsql.connect(obj.host, obj.user, obj.pass, obj.db);");
                sb.AppendLine("\t\t});");
                sb.AppendLine("\t} catch (e) {");
                sb.AppendLine("\t\tlog.error(e);");
                sb.AppendLine("\t}");
                sb.AppendLine("});");
                sb.AppendLine("module.exports = app;");
                return sb.ToString();
            }
            catch (Exception i)
            {
                Console.WriteLine(i);
                return null;
            }
        }
    }
}
