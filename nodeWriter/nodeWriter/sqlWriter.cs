using System;
using System.Collections.Generic;
using System.Text;

namespace nodeWriter
{
    public class sqlWriter
    {
        public bool writeSQLFile(serverJsonObj server)
        {
            try
            {
                string dir = Environment.CurrentDirectory + "\\projects\\" + server.name;
                StringBuilder sb = new StringBuilder();

                DBtables[] dts = server.dataBaseLayout.tables;  
                foreach(var dt in dts)
                {
                    string col_str = null;
                    foreach(var col in dt.columns)
                    {
                        col_str = col_str + " " + col.columnName + " " + col.columnDataType + ",";
                    }
                    col_str = col_str.Substring(0, col_str.Length - 1);
                    sb.AppendLine("CREATE TABLE " + dt.tableName + "(" + col_str + ");");
                    sb.AppendLine("");
                }

                System.IO.File.WriteAllText(dir + "\\init.sql", sb.ToString());
                Console.WriteLine("writing SQL is done!");
                return true;

            }
            catch(Exception i)
            {
                Console.WriteLine(i);
                return false;
            }
        }
    }
}
