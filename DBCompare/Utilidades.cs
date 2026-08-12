using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ini;
using System.IO;

namespace DBCompare
{
    class Utilidades
    {
        public void FileIni()
        {
            // to get the location the assembly is executing from
            //(not necessarily where the it normally resides on disk)
            // in the case of the using shadow copies, for instance in NUnit tests, 
            // this will be in a temp directory.
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

            //To get the location the assembly normally resides on disk or the install directory
            //string path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

            //once you have the path you get the directory with:
            var directory = System.IO.Path.GetDirectoryName(path);

            IniFile ini = new IniFile(directory + @"\Setting.ini");

            if (!File.Exists(directory + @"\Setting.ini"))
            {
                if (string.IsNullOrEmpty(ini.IniReadValue("SetupDB1", "Server"))) { ini.IniWriteValue("SetupDB1", "Server", ""); }
                if (string.IsNullOrEmpty(ini.IniReadValue("SetupDB1", "DataBase"))) { ini.IniWriteValue("SetupDB1", "DataBase", ""); }
                if (string.IsNullOrEmpty(ini.IniReadValue("SetupDB1", "Usuario"))) { ini.IniWriteValue("SetupDB1", "Usuario", ""); }
                if (string.IsNullOrEmpty(ini.IniReadValue("SetupDB1", "Password"))) { ini.IniWriteValue("SetupDB1", "Password", ""); }
                if (string.IsNullOrEmpty(ini.IniReadValue("SetupDB1", "UseIntegrated"))) { ini.IniWriteValue("SetupDB1", "UseIntegrated", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("SetupDB2", "Server"))) { ini.IniWriteValue("SetupDB2", "Server", ""); }
                if (string.IsNullOrEmpty(ini.IniReadValue("SetupDB2", "DataBase"))) { ini.IniWriteValue("SetupDB2", "DataBase", ""); }
                if (string.IsNullOrEmpty(ini.IniReadValue("SetupDB2", "Usuario"))) { ini.IniWriteValue("SetupDB2", "Usuario", ""); }
                if (string.IsNullOrEmpty(ini.IniReadValue("SetupDB2", "Password"))) { ini.IniWriteValue("SetupDB2", "Password", ""); }
                if (string.IsNullOrEmpty(ini.IniReadValue("SetupDB2", "UseIntegrated"))) { ini.IniWriteValue("SetupDB2", "UseIntegrated", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Setup", "PathBeyon"))) { ini.IniWriteValue("Setup", "PathBeyon", ""); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Setup", "PathWinMerge"))) { ini.IniWriteValue("Setup", "PathWinMerge", ""); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Setup", "PathCache"))) { ini.IniWriteValue("Setup", "PathCache", @"C:\Temp\"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Setup", "Upper"))) { ini.IniWriteValue("Setup", "Upper", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Setup", "FormatText"))) { ini.IniWriteValue("Setup", "FormatText", "1"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Setup", "ProgramCompare"))) { ini.IniWriteValue("Setup", "ProgramCompare", "1"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Aggregates"))) { ini.IniWriteValue("Preferences", "Aggregates", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "ApplicationRoles"))) { ini.IniWriteValue("Preferences", "ApplicationRoles", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Assemblies"))) { ini.IniWriteValue("Preferences", "Assemblies", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "BrokerPriorities"))) { ini.IniWriteValue("Preferences", "BrokerPriorities", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "ClusteredIndexes"))) { ini.IniWriteValue("Preferences", "ClusteredIndexes", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "CLRTriggers"))) { ini.IniWriteValue("Preferences", "CLRTriggers", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "CLRUserDefinedFunctions"))) { ini.IniWriteValue("Preferences", "CLRUserDefinedFunctions", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "CheckConstraints"))) { ini.IniWriteValue("Preferences", "CheckConstraints", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Collation"))) { ini.IniWriteValue("Preferences", "Collation", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Contracts"))) { ini.IniWriteValue("Preferences", "Contracts", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "DatabaseRoles"))) { ini.IniWriteValue("Preferences", "DatabaseRoles", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "DataCompression"))) { ini.IniWriteValue("Preferences", "DataCompression", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Defaults"))) { ini.IniWriteValue("Preferences", "Defaults", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "DDLTriggers"))) { ini.IniWriteValue("Preferences", "DDLTriggers", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "DefaultConstraints"))) { ini.IniWriteValue("Preferences", "DefaultConstraints", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "DMLTriggers"))) { ini.IniWriteValue("Preferences", "DMLTriggers", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "ForeignKeys"))) { ini.IniWriteValue("Preferences", "ForeignKeys", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "FullTextCatalogPath"))) { ini.IniWriteValue("Preferences", "FullTextCatalogPath", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "FullTextCatalogs"))) { ini.IniWriteValue("Preferences", "FullTextCatalogs", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "FullTextIndexes"))) { ini.IniWriteValue("Preferences", "FullTextIndexes", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "FullTextStopLists"))) { ini.IniWriteValue("Preferences", "FullTextStopLists", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "MessageTypes"))) { ini.IniWriteValue("Preferences", "MessageTypes", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "NoFileStream"))) { ini.IniWriteValue("Preferences", "NoFileStream", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "NoIdentities"))) { ini.IniWriteValue("Preferences", "NoIdentities", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "NonClusteredIndexes"))) { ini.IniWriteValue("Preferences", "NonClusteredIndexes", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "PartitionFunctions"))) { ini.IniWriteValue("Preferences", "PartitionFunctions", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "PartitionSchemes"))) { ini.IniWriteValue("Preferences", "PartitionSchemes", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "PrimaryKeys"))) { ini.IniWriteValue("Preferences", "PrimaryKeys", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "RemoteServiceBindings"))) { ini.IniWriteValue("Preferences", "RemoteServiceBindings", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Routes"))) { ini.IniWriteValue("Preferences", "Routes", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Rules"))) { ini.IniWriteValue("Preferences", "Rules", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Schemas"))) { ini.IniWriteValue("Preferences", "Schemas", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "ServiceQueues"))) { ini.IniWriteValue("Preferences", "ServiceQueues", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Services"))) { ini.IniWriteValue("Preferences", "Services", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "SQLUserDefinedFunctions"))) { ini.IniWriteValue("Preferences", "SQLUserDefinedFunctions", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "StoredProcedures"))) { ini.IniWriteValue("Preferences", "StoredProcedures", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Synonyms"))) { ini.IniWriteValue("Preferences", "Synonyms", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Tables"))) { ini.IniWriteValue("Preferences", "Tables", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "UniqueConstraints"))) { ini.IniWriteValue("Preferences", "UniqueConstraints", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "UserDefinedDataTypes"))) { ini.IniWriteValue("Preferences", "UserDefinedDataTypes", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "UserDefinedTableTypes"))) { ini.IniWriteValue("Preferences", "UserDefinedTableTypes", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "UserDefinedTypes"))) { ini.IniWriteValue("Preferences", "UserDefinedTypes", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Users"))) { ini.IniWriteValue("Preferences", "Users", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "Views"))) { ini.IniWriteValue("Preferences", "Views", "0"); }
                if (string.IsNullOrEmpty(ini.IniReadValue("Preferences", "XMLSchemaCollections"))) { ini.IniWriteValue("Preferences", "XMLSchemaCollections", "0"); }
            }
            //else
            //{
            //    MessageBox.Show(ini.IniReadValue("1", "2"));
            //}        
        }

        public string GetIni(string Section, string key)
        {
            try
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var directory = System.IO.Path.GetDirectoryName(path);

                IniFile ini = new IniFile(directory + @"\Setting.ini");
                return ini.IniReadValue(Section, key);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
        }

        public string SetIni(string Section, string key, string Value)
        {
            try
            {
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var directory = System.IO.Path.GetDirectoryName(path);

                IniFile ini = new IniFile(directory + @"\Setting.ini");
                ini.IniWriteValue(Section, key, Value);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
            
    }
}
