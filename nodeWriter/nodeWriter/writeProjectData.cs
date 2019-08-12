using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace nodeWriter
{
    public class writeProjectData
    {
        private string[] generic_deps = { "express", "pino", "pino-pretty", "swaggerjsdoc", "mysql" };

        public writeProjectData()
        {

        }

        public void writeDependecies(serverJsonObj server, string serverCode)
        {
            try
            {
                if (!Directory.Exists(Environment.CurrentDirectory + "\\projects"))
                {
                    System.IO.Directory.CreateDirectory(Environment.CurrentDirectory + "\\projects");
                }
                string dir = Environment.CurrentDirectory + "\\projects\\" + server.name;

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                System.IO.File.WriteAllText(dir + "\\server.js", serverCode);

                DBObj db_settings = server.DB;
                dbConfigFormat db_config = new dbConfigFormat();
                db_config.host = db_settings.DBhost;
                db_config.pass = db_settings.DBpassword;
                db_config.user = db_settings.DBusername;
                db_config.db = db_settings.DB_name;
                string db_string = JsonConvert.SerializeObject(db_config);
                File.WriteAllText(dir + "\\config.json", db_string);
                File.Copy(Environment.CurrentDirectory + "\\DB_temps\\mysql\\mysqlDB.js", dir + "\\mysqlDB.js");
                sqlWriter sql = new sqlWriter();
                sql.writeSQLFile(server);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    /*winShell("cd " + dir);
                    //winShell("npm init");
                    winShell("npm i pino");
                    winShell("npm i express");
                    winShell("npm i swagger-jsdoc");
                    winShell("npm i mocha -g");
                    winShell("npm i chai - g");
                    winShell("npm i pino-pretty");
                    winShell("npm i express-pino-logger");
                    winShell("npm i cors");
                    winShell("npm i body-parser");
                    winShell("npm i path");*/
                    string[] args_one =
                    {
                        "@echo off",
                        "cd \"" + dir + "\"",
                        "npm init"
                    };
                    string[] args =
                    {
                        "@echo off",
                        "cd \"" + dir + "\"",
                        "npm i pino && npm i express && npm i swagger-jsdoc && npm i mocha -g && npm i chai -g && npm i pino-pretty && npm i express-pino-logger && npm i cors && npm i body-parser && npm i path"

                    };

                    if(!File.Exists(Environment.CurrentDirectory + "\\init.bat"))
                    {
                        File.WriteAllLines(Environment.CurrentDirectory + "\\init.bat", args_one);
                    }

                    if(!File.Exists(Environment.CurrentDirectory + "\\dep_install.bat"))
                    {
                        File.WriteAllLines(Environment.CurrentDirectory + "\\dep_install.bat", args);
                    }

                    Process proc = Process.Start(Environment.CurrentDirectory + "\\init.bat");
                    proc.WaitForExit();
                    Process proc2 = Process.Start(Environment.CurrentDirectory + "\\dep_install.bat");
                    proc2.WaitForExit();
                    Console.WriteLine("all done!");
                    Console.ReadLine();
                }
                else
                {
                    ShellHelper.Bash("cd " + dir + " && npm init");
                    ShellHelper.Bash("cd " + dir + " && npm i pino");
                    ShellHelper.Bash("cd " + dir + " && npm i express");
                    ShellHelper.Bash("cd " + dir + " && npm i swagger-jsdoc");
                    ShellHelper.Bash("cd " + dir + " && npm i mocha -g");
                    ShellHelper.Bash("cd " + dir + " && npm i chai -g");
                    ShellHelper.Bash("cd " + dir + " && npm i pino-pretty");
                    ShellHelper.Bash("cd " + dir + " && npm i express-pino-logger");
                    ShellHelper.Bash("cd " + dir + " && npm i cors");
                    ShellHelper.Bash("cd " + dir + " && npm i body-parser");
                    ShellHelper.Bash("cd " + dir + " && npm i path");
                }

            }
            catch(Exception i)
            {
                Console.WriteLine(i);
            }
        }

        /*public void winShell(string cmd)
        {
            /*using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WorkingDirectory = @"C:\";
                process.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "cmd.exe");

                // Redirects the standard input so that commands can be sent to the shell.
                process.StartInfo.RedirectStandardInput = true;
                // Runs the specified command and exits the shell immediately.
                //process.StartInfo.Arguments = @"/c ""dir""";

                process.OutputDataReceived += ProcessOutputDataHandler;
                process.ErrorDataReceived += ProcessErrorDataHandler;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                // Send a directory command and an exit command to the shell
                process.StandardInput.WriteLine(cmd);
                //process.StandardInput.WriteLine("pause");
                //process.StandardInput.WriteLine("exit");


                //process.WaitForExit();
            }
        }

        public static void ProcessOutputDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }

        public static void ProcessErrorDataHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }*/
    }

    public class dbConfigFormat
    {
        public string host;
        public string user;
        public string pass;
        public string db;
    }
}
