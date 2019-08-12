using System;
using System.Collections.Generic;
using System.Text;

namespace nodeWriter
{
    public class serverJsonObj
    {
        public string name;
        public bool hasSwaggerJDcos;
        public DBObj DB;
        public dataBaseLayoutObj dataBaseLayout;
        public apiObj api;
        public reportsObj[] reports;
        public swaggerObj swagger;
    }

    public class swaggerObj
    {
        public string host;
        public string basePath;
        public swaggerInfo info;
    }

    public class swaggerInfo
    {
        public string title;
        public string version;
        public string description;
    }

    public class DBObj
    {
        public string DBhost;
        public string DBport;
        public string DBusername;
        public string DBpassword;
        public string DB_name;
        public dataBaseObj dataBase;
    }

    public class dataBaseObj
    {
        public bool mysql;
    }

    public class dataBaseLayoutObj
    {
        public DBtables[] tables;
    }

    public class DBtables
    {
        public int id;
        public string tableName;
        public DBtableColumns[] columns;
    }

    public class DBtableColumns
    {
        public string columnName;
        public string columnDataType;
        public int id;
    }

    public class apiObj
    {
        public GETobj[] GET;
        public POSTobj[] POST;
        public DELETEobj[] DELETE;
        public PUTobj[] PUT;
    }
    //------------------------GET-------------------
    public class GETobj
    {
        public int id;
        public string callName;
        public int relatedDBTableID;
        public apiCondition condition;
    }
    //----------------------------------------------
    public class apiCondition
    {
        public int[] checkQueryColID;
    }

    public class POSTobj
    {
        public int id;
        public string callName;
        public int relatedDBTableID;
        public dataStructureBodyObj dataStructure;
    }

    public class dataStructureBodyObj
    {
        public bodyObj[] body;
    }

    public class bodyObj
    {
        public string attributeName;
        public int DBcolumnID;
        public string attributeType;

    }

    public class DELETEobj
    {
        public int id;
        public string callName;
        public int relatedDBTableID;
        public apiCondition condition;
        public dataStructureBodyObj dataStructure;
    }

    public class PUTobj
    {
        public int id;
        public string callName;
        public int relatedDBTableID;
        public apiCondition condition;
        public dataStructureBodyObj dataStructure;
    }

    public class reportsObj
    {
        public int id;
        public string reportName;
        public reportData[] data;

    }

    public class reportData
    {
        public int relatedDBTableID;
        public int DBcolumnID;
        public string DataName;
        public string DataType;
    }
}
