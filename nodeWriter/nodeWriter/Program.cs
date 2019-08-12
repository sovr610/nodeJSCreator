using System;
using System.IO;

namespace nodeWriter
{
    public class Program
    {
        static void Main(string[] args)
        {
            writerServer server = new writerServer();
            serverJsonObj obj = server.readJsonConfigFile();
            string code = server.writeServerCode(obj);
            writeProjectData proj = new writeProjectData();
            proj.writeDependecies(obj, code);
        }
    }
}
