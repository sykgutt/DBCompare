using DifferenceEngine;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using Microsoft.SqlServer.Management.Common;
//using System.Runtime.InteropServices;
using DifferenceEngine;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.IO;
using System.Xml;

namespace DBCompare
{
    public partial class ObjectCompare : Form
    {
        DataTable dbOjects = null;
        string Database1 = null;
        string Database2 = null;

        Server server1 = null;
        Server server2 = null;
        bool showLoginScreen;

        string filename = null;
        string directory = null;

        public ObjectCompare()
        {
            showLoginScreen = true;
            InitializeComponent();
        }

        public ObjectCompare(bool _showLoginScreen)
        {
            showLoginScreen = _showLoginScreen;
            InitializeComponent();
        }

        public void SetServer1(Server srv1)
        {
            server1 = srv1;
        }

        public void SetServer2(Server srv2)
        {
            server2 = srv2;
        }

        public void SetDatabase1(string db1)
        {
            Database1 = db1;
        }

        public void SetDatabase2(string db2)
        {
            Database2 = db2;
        }

        private void ObjectCompare_Load(object sender, EventArgs e)
        {
            ObjectCompare_Resize(sender, e);
        }

        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public string Directory
        {
            get { return directory; }
            set { directory = value; }
        }

        private void ObjectCompare_Shown(object sender, EventArgs e)
        {
            
            if (showLoginScreen)
            {
                Login login = new Login();
                login.ShowDialog();
                if (login.DialogResult == DialogResult.OK)
                {
                    server1 = login.GetServer1();
                    server2 = login.GetServer2();
                    string database1 = login.GetDatabase1();
                    string database2 = login.GetDatabase2();
                    ObjectHelper.ScriptingOptions so = login.GetScriptiongOptions();

                    btnFromDB1ToDB2.Text = database1 + " -> " + database2;
                    btnFromDB2ToDB1.Text = database1 + " <- " + database2;

                    Database1 = database1;
                    Database2 = database2;

                    ObjectFetch objFetch = new ObjectFetch(server1,
                        database1, server1.ConnectionContext.Login,
                        server1.ConnectionContext.Password,
                        server2, database2, server2.ConnectionContext.Login, server2.ConnectionContext.Password, so);
                    //objFetch.Parent = this.Parent;
                    objFetch.ShowDialog();
                    if (objFetch.DialogResult == DialogResult.OK)
                    {
                        dbOjects = objFetch.GetDatabaseObjectsTable();

                        RefreshObjectsList();
                    }
                }
            }
        }

        public void LoadObjects(DataTable _dtObjects)
        {
            dbOjects = _dtObjects;
            RefreshObjectsList();
        }

        public void Save()
        {   
            if (filename == null)
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "XML document|*.xml";
                saveFileDialog1.Title = "Save an Project File";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    this.filename = saveFileDialog1.FileName;
                }
                System.Diagnostics.Debug.WriteLine(saveFileDialog1.FileName.LastIndexOf(@"\").ToString());
                System.Diagnostics.Debug.WriteLine(saveFileDialog1.FileName.Substring(0,saveFileDialog1.FileName.LastIndexOf(@"\")));
                directory = saveFileDialog1.FileName.Substring(0, saveFileDialog1.FileName.LastIndexOf(@"\"));
            }

            if (filename != null)
            {
                Save(filename, directory);
            }
        }

        public void Save(string filename, string directoryname)
        {

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;

            using (XmlWriter writer = XmlWriter.Create(filename, settings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("DBCompareProject");


                writer.WriteStartElement("Settings");

                writer.WriteStartElement("Server1");
                writer.WriteElementString("Server", server1.Name);
                writer.WriteElementString("Login", server1.ConnectionContext.Login);
                writer.WriteElementString("Password", server1.ConnectionContext.Password);
                writer.WriteElementString("Database", Database1);
                writer.WriteEndElement();

                writer.WriteStartElement("Server2");
                writer.WriteElementString("Server", server2.Name);
                writer.WriteElementString("Login", server2.ConnectionContext.Login);
                writer.WriteElementString("Password", server2.ConnectionContext.Password);
                writer.WriteElementString("Database", Database2);
                writer.WriteEndElement();


                writer.WriteEndElement();

                writer.WriteStartElement("Objects");
                foreach (DataRow dr in dbOjects.Rows)
                {
                    writer.WriteStartElement("Object");

                    foreach (DataColumn col in dbOjects.Columns)
                    {
                        writer.WriteAttributeString(col.ColumnName, dr[col.ColumnName].ToString());
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                writer.WriteEndElement();
                writer.WriteEndDocument();
                
                StringBuilder sb = new StringBuilder();
                string data = string.Empty;
                
                foreach (ListViewItem item in lwDatabaseObjects.CheckedItems)
	            {
                    data = string.Format("{1} --> [{2}].[{0}]\t#", item.SubItems[2].Text, item.SubItems[0].Text, item.SubItems[1].Text);
                    data = FormatQuery(data, item.SubItems[0].Text);

                    switch (item.Group.Name)
                    {
                        case "1":
                            sb.AppendLine(data.Replace("#", "(NUEVO DESA)"));
                            System.Diagnostics.Debug.WriteLine(data.Replace("#", "(NUEVO DESA)"));
                            break;
                        case "2":
                            sb.AppendLine(data.Replace("#", "(NUEVO PROD)"));
                            System.Diagnostics.Debug.WriteLine(data.Replace("#", "(NUEVO PROD)"));
                            break;
                        case "3":
                            sb.AppendLine(data.Replace("#", "(IDENTICO)"));
                            System.Diagnostics.Debug.WriteLine(data.Replace("#", "(IDENTICO)"));
                            break;
                        case "4":
                            sb.AppendLine(data.Replace("#", "(MODIFICADO)"));
                            System.Diagnostics.Debug.WriteLine(data.Replace("#", "(MODIFICADO)"));
                            break;
                        default:
                            break;
                    }
	            }
                System.IO.File.WriteAllText(directoryname + @"\Resumen.txt", sb.ToString());
            }
        }

        private void ObjectCompare_Resize(object sender, EventArgs e)
        {
            int w = splitContainer2.Panel2.ClientRectangle.Width / 2;
            lvSource.Location = new Point(0, 0);
            lvSource.Width = w;
            lvSource.Height = this.splitContainer2.Panel2.Height;

            lvDestination.Location = new Point(w + 1, 0);

            lvDestination.Width = this.splitContainer2.Panel2.Width - (w + 1);
            lvDestination.Height = this.splitContainer2.Panel2.Height;
        }

        private void lwDatabaseObjects_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                CodeShow();
            }
        }

        private void lwDatabaseObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CodeShow();
        }

        private void lwDatabaseObjects_Click(object sender, EventArgs e)
        {
            CodeShow();
        }

        private void lwDatabaseObjects_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            if (e.Button == MouseButtons.Right)
            {
                foreach(ListViewItem item in lwDatabaseObjects.SelectedItems)
                {
                    sb.AppendLine(string.Format("{2}\t[{0}].[{1}]", item.SubItems[1].Text, item.SubItems[2].Text, item.SubItems[0].Text));
                }
                Clipboard.SetText(sb.ToString());
                ((MDIMain)this.MdiParent).StatusText = string.Format("Clipboard {0} items", lwDatabaseObjects.SelectedItems.Count);
            }
            else
            {
                Utilidades extras = new Utilidades();

                string pathBeyond = extras.GetIni("Setup", "PathBeyon");
                string pathWinMerge = extras.GetIni("Setup", "PathWinMerge");
                string ProgramCompare = extras.GetIni("Setup", "ProgramCompare");
                string pathData = extras.GetIni("Setup", "PathCache");
                string objDef1 = string.Empty;
                string objDef2 = string.Empty;
                string obj = string.Empty;
                string objSource = string.Empty;
                string objDestination = string.Empty;
                
                DataView dwObjectDefinition = new DataView(this.dbOjects, "Schema='" + lwDatabaseObjects.SelectedItems[0].SubItems[1].Text + "' AND " + "Type='" + lwDatabaseObjects.SelectedItems[0].SubItems[0].Text + "' AND " + "Name='" + lwDatabaseObjects.SelectedItems[0].SubItems[2].Text + "'", "Name", DataViewRowState.CurrentRows);

                foreach (DataRowView dr in dwObjectDefinition)
                {
                    objDef1 = dr["ObjectDefinition1"].ToString();
                    objDef2 = dr["ObjectDefinition2"].ToString();
                }

                obj = string.Format("[{0}].[{1}] - (#).txt", lwDatabaseObjects.SelectedItems[0].SubItems[1].Text, lwDatabaseObjects.SelectedItems[0].SubItems[2].Text);

                objSource = pathData + obj.Replace("#", Database1);
                objDestination = pathData + obj.Replace("#", Database2);

                System.IO.File.WriteAllText(objSource, objDef1);
                System.IO.File.WriteAllText(objDestination, objDef2);

                Process.Start(((ProgramCompare == "1") ? pathBeyond : pathWinMerge), string.Format("\"{0}\" \"{1}\"", objSource, objDestination));
            }
        }

        private void lvDestination_Resize(object sender, EventArgs e)
        {
            if (lvDestination.Width > 100)
            {
                lvDestination.Columns[1].Width = -2;
            }
        }

        private void lvSource_Resize(object sender, EventArgs e)
        {
            if (lvSource.Width > 100)
            {
                lvSource.Columns[1].Width = -2;
            }
        }

        private void lvDestination_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvDestination.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvSource.Items[lvDestination.SelectedItems[0].Index];
                lvi.Selected = true;
                lvi.EnsureVisible();                
                //lvi.Position = new Point(lvi.Position.X, 50);
                //lvi.StateImageIndex = lvDestination.SelectedItems[0].StateImageIndex;
                //lvi.Position = lvSource.SelectedItems[0].Position;
            }
        }

        private void lvSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvSource.SelectedItems.Count > 0)
            {
                ListViewItem lvi = lvDestination.Items[lvSource.SelectedItems[0].Index];
                lvi.Selected = true;
                lvi.EnsureVisible();                
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshObjectsList();
        }

        private void RefreshObjectsList()
        {
        bool isFirstConditionAdded = false;
        StringBuilder condition =new StringBuilder();
        
            // create the list to use as the custom source.
        var source = new List<string>();


        for (int i = 0; i < tableLayoutPanel1.Controls.Count;i++ )
        {
            Control ctrl = tableLayoutPanel1.Controls[i];
            if (ctrl.GetType() == typeof(CheckBox))
            {
                if (((CheckBox)ctrl).Checked)
                {
                    if (!isFirstConditionAdded)
                    {
                        condition.Append("Type='" + ctrl.Tag.ToString() + "'");
                        isFirstConditionAdded = true;
                    }
                    else
                    {
                        condition.Append(" OR Type='" + ctrl.Tag.ToString() + "'");
                    }
                }
            }
        }

    DataView dwObjects = new DataView(dbOjects, condition.ToString(), "Type,Name", DataViewRowState.CurrentRows);
    lwDatabaseObjects.Items.Clear();
    lwDatabaseObjects.Groups.Clear();
            
    lwDatabaseObjects.Groups.Add("1", "Objects that exist only in " + Database1 + " ");
    lwDatabaseObjects.Groups.Add("2", "Objects that exist only in " + Database2 + " ");
    lwDatabaseObjects.Groups.Add("3", "Objects that exist in both databases and are identical");
    lwDatabaseObjects.Groups.Add("4", "Objects that exist in both databases and are different");
            
    foreach (DataRowView dr in dwObjects)
    {
        int imgIndex = 0;
        switch (dr["Type"].ToString().ToUpper())
        {
                //fullTextIndex
            case "STOREDPROCEDURE":
                imgIndex = 0;
                break;
            case "TABLE":
                imgIndex = 1;
                break;
            case "SQLUSERDEFINEDFUNCTION":
                imgIndex = 31;
                break;

            case "SQLUSERDEFINEDTABLEFUNCTION":
                imgIndex = 30;
                break;

            case "VIEW":
                imgIndex = 3;
                break;
            case "APPLICATIONROLE":
                imgIndex = 4;
                break;
            case "DATABASEROLE":
                imgIndex = 5;
                break;
            case "DEFAULT":
                imgIndex = 6;
                break;
            case "FULLTEXTCATALOG":
                imgIndex = 7;
                break;
            case "FULLTEXTSTOPLIST":
                imgIndex = 7;
                break;
            case "USERDEFINEDTABLETYPE":
                imgIndex = 1;
                break;
            case "MESSAGETYPE":
                imgIndex = 8;
                break;
            case "PARTITIONFUNCTION":
                imgIndex = 9;
                break;
            case "PARTITIONSCHEME":
                imgIndex = 10;
                break;
            case "PLANGUIDE":
                imgIndex = 12;
                break;
            case "REMOTESERVICEBINDING":
                imgIndex = 15;
                break;
            case "RULE":
                imgIndex = 13;
                break;
            case "SCHEMA":
                imgIndex = 14;
                break;
            case "SERVICECONTRACT":
                imgIndex = 16;
                break;
            case "SERVICEQUEUE":
                imgIndex = 17;
                break;
            case "SERVICEROUTE":
                imgIndex = 18;
                break;
            case "SQLASSEMBLY":
                imgIndex = 19;
                break;
            case "SQLDDLTRIGGER":
                imgIndex = 20;
                break;
            case "CLRDDLTRIGGER":
                imgIndex = 20;
                break;
            case "SYNONYM":
                imgIndex = 21;
                break;
            case "USER":
                imgIndex = 22;
                break;
            case "AGGREGATE":
                imgIndex = 23;
                break;
            case "USERDEFINEDTYPE":
                imgIndex = 24;
                break;
            case "USERDEFINEDDATATYPE":
                imgIndex = 25;
                break;
            case "XMLSCHEMACOLLECTION":
                imgIndex = 26;
                break;
            case "SQLDMLTRIGGER":
                imgIndex = 27;
                break;
            case "CLRDMLTRIGGER":
                imgIndex = 27;
                break;
            case "INDEX":
                imgIndex = 28;
                break;
            case "SERVICE":
                imgIndex = 29;
                break;
            case "CLRUSERDEFINEDFUNCTION":
                imgIndex = 31;
                break;
            case "CLRUSERDEFINEDTABLEFUNCTION":
                imgIndex = 30;
                break;

            case "CLRTrigger":
                imgIndex = 20;
                break;
            /*
            case "USERDEFINEDTABLETYPE":
                imgIndex = 1;
                break;*/
        }

        switch (dr["ResultSet"].ToString())
        {
            case "1":
                this.lwDatabaseObjects.Items.Add(new ListViewItem(new string[] { 
                            dr["Type"].ToString(),dr["Schema"].ToString(),dr["Name"].ToString() }, imgIndex, lwDatabaseObjects.Groups[0]));
                break;
            case "2":
                this.lwDatabaseObjects.Items.Add(new ListViewItem(new string[] { 
                            dr["Type"].ToString(),dr["Schema"].ToString(),dr["Name"].ToString() }, imgIndex, lwDatabaseObjects.Groups[1]));
                break;
            case "3":
                this.lwDatabaseObjects.Items.Add(new ListViewItem(new string[] { 
                            dr["Type"].ToString(),dr["Schema"].ToString(),dr["Name"].ToString() }, imgIndex, lwDatabaseObjects.Groups[2]));
                break;
            case "4":
                this.lwDatabaseObjects.Items.Add(new ListViewItem(new string[] { 
                            dr["Type"].ToString(),dr["Schema"].ToString(),dr["Name"].ToString() }, imgIndex, lwDatabaseObjects.Groups[3]));
                break;
        }        

        source.Add(string.Format("[{0}].[{1}]", dr["Schema"].ToString(), dr["Name"].ToString()));                
    }
        // assign the source to AutoCompleteList
        this.autoCompleteTextbox1.AutoCompleteList = new List<string>();
        this.autoCompleteTextbox1.AutoCompleteList = source;
}

        private void chkAll_Click(object sender, EventArgs e)
        {

        }

        private void chkAll_CheckStateChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < tableLayoutPanel1.Controls.Count; i++)
            {
                Control ctrl = tableLayoutPanel1.Controls[i];
                if (ctrl.GetType() == typeof(CheckBox))
                {
                    ((CheckBox)ctrl).Checked = chkAll.Checked;
                }
            }
        }

        private void btnFromDB1ToDB2_Click(object sender, EventArgs e)
        {
            StringBuilder sbScript = new StringBuilder();
            sbScript.AppendLine("USE [" + Database2 + "]");
            sbScript.AppendLine("GO");
            foreach (ListViewItem lvi in lwDatabaseObjects.Groups[0].Items)
            {
                if (lvi.Checked == true)
                {
                    DataView dwObjectDefinition = new DataView(this.dbOjects, "Schema='" + lvi.SubItems[1].Text + "' AND " + "Type='" + lvi.SubItems[0].Text + "' AND " + "Name='" + lvi.SubItems[2].Text + "'", "Name", DataViewRowState.CurrentRows);
                    foreach (DataRowView dr in dwObjectDefinition)
                    {
                        string objDef1 = dr["ObjectDefinition1"].ToString();
                        //sbScript.AppendLine("/**********Type: " + lvi.SubItems[0].Text + " Name: " + lvi.SubItems[2].Text + "**********/");
                        sbScript.AppendLine(string.Format("/**********Type: {0} Name: [{1}].[{2}]**********/", lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text));

                        sbScript.AppendLine(string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'U'))", lvi.SubItems[1].Text, lvi.SubItems[2].Text));
                        sbScript.AppendLine(string.Format("DROP {0} [{1}].[{2}]", lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text));
                        sbScript.AppendLine("GO");

                        sbScript.AppendLine(objDef1);
                        sbScript.AppendLine("");

                        sbScript = FormatQuery(sbScript, lvi.SubItems[0].Text);
                    }
                }
            }

            foreach (ListViewItem lvi in lwDatabaseObjects.Groups[3].Items)
            {
                if (lvi.Checked == true)
                {
                    DataView dwObjectDefinition = new DataView(this.dbOjects, "Schema='" + lvi.SubItems[1].Text + "' AND " + "Type='" + lvi.SubItems[0].Text + "' AND " + "Name='" + lvi.SubItems[2].Text + "'", "Name", DataViewRowState.CurrentRows);
                    foreach (DataRowView dr in dwObjectDefinition)
                    {
                        //DB1
                        string objDef1 = dr["ObjectDefinition1"].ToString();
                        //DB2
                        string objDef2 = dr["ObjectDefinition2"].ToString();
                        //sbScript.AppendLine("/**********Type: " + lvi.SubItems[0].Text + " Name: " + lvi.SubItems[2].Text + "**********/");
                        if (dr[2].ToString() == "Table" && !string.IsNullOrEmpty(objDef2 = dr["ObjectDefinition2"].ToString()))
                        {
                            sbScript.AppendLine(string.Format("ALTER TABLE [{0}].[{1}] (ADD/ALTER COLUMN/DROP COLUMN) ", dr[3].ToString(), dr[1].ToString()));
                            sbScript.AppendLine(CheckColumnsByTable(lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text));
                        }
                        else
                        {
                            sbScript.AppendLine(string.Format("/**********Type: {0} Name: [{1}].[{2}]**********/", lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text));

                            sbScript.AppendLine(string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'U'))", lvi.SubItems[1].Text, lvi.SubItems[2].Text));
                            sbScript.AppendLine(string.Format("DROP {0} [{1}].[{2}]", lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text));
                            sbScript.AppendLine("GO");

                            sbScript.AppendLine(objDef1);
                            sbScript.AppendLine("");

                            sbScript = FormatQuery(sbScript, lvi.SubItems[0].Text);
                        }
                    }
                }
            }

            ScriptView sw = new ScriptView(sbScript.ToString());
            sw.ShowDialog();

            /*
            using (StreamWriter outfile = new StreamWriter("sql.txt"))
            {
                outfile.Write(sbScript.ToString());
            }*/
        }

        private string CheckColumnsByTable(string Type, string Schema, string Name)
        {
            try
            {
                return string.Format("{0} - [{1}].[{2}]", Type, Schema, Name);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            //throw new NotImplementedException();
        }

        private StringBuilder FormatQuery(StringBuilder query, string tipo)
        {
            switch (tipo)
            {
                case "Schema":
                case "Index":
                    query= query.Replace("[].", string.Empty);                    
                    break;
                default:
                    break;
            }
            return query;
        }

        private string FormatQuery(string query, string tipo)
        {
            switch (tipo)
            {
                case "Schema":
                case "Index":
                    query = query.Replace("[].", string.Empty);
                    break;
                default:
                    break;
            }
            return query;
        }

        private void btnRefresh_Click_1(object sender, EventArgs e)
        {
            RefreshObjectsList();
        }

        private void btnFromDB2ToDB1_Click(object sender, EventArgs e)
        {
            StringBuilder sbScript = new StringBuilder();
            sbScript.AppendLine("USE [" + Database1 + "]");
            sbScript.AppendLine("GO");
            foreach (ListViewItem lvi in lwDatabaseObjects.Groups[1].Items)
            {
                if (lvi.Checked == true)
                {
                    DataView dwObjectDefinition = new DataView(this.dbOjects, "Schema='" + lvi.SubItems[1].Text + "' AND " + "Type='" + lvi.SubItems[0].Text + "' AND " + "Name='" + lvi.SubItems[2].Text + "'", "Name", DataViewRowState.CurrentRows);
                    foreach (DataRowView dr in dwObjectDefinition)
                    {
                        string objDef1 = dr["ObjectDefinition1"].ToString();
                        sbScript.AppendLine(string.Format("/**********Type: {0} Name: [{1}].[{2}]**********/", lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text));

                        sbScript.AppendLine(string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'U'))", lvi.SubItems[1].Text, lvi.SubItems[2].Text));
                        sbScript.AppendLine(string.Format("DROP {0} [{1}].[{2}]", lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text));
                        sbScript.AppendLine("GO");
                        
                        sbScript.AppendLine(objDef1);
                        sbScript.AppendLine("");

                        sbScript = FormatQuery(sbScript, lvi.SubItems[0].Text);
                    }
                }
            }

            foreach (ListViewItem lvi in lwDatabaseObjects.Groups[3].Items)
            {
                if (lvi.Checked == true)
                {
                    DataView dwObjectDefinition = new DataView(this.dbOjects, "Schema='" + lvi.SubItems[1].Text + "' AND " + "Type='" + lvi.SubItems[0].Text + "' AND " + "Name='" + lvi.SubItems[2].Text + "'", "Name", DataViewRowState.CurrentRows);
                    foreach (DataRowView dr in dwObjectDefinition)
                    {
                        string objDef1 = dr["ObjectDefinition2"].ToString();
                        sbScript.AppendLine(string.Format("/**********Type: {0} Name: [{1}].[{2}]**********/", lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text));

                        sbScript.AppendLine(string.Format("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'U'))", lvi.SubItems[1].Text, lvi.SubItems[2].Text));
                        sbScript.AppendLine(string.Format("DROP {0} [{1}].[{2}]", lvi.SubItems[0].Text, lvi.SubItems[1].Text, lvi.SubItems[2].Text));
                        sbScript.AppendLine("GO");

                        sbScript.AppendLine(objDef1);
                        sbScript.AppendLine("");

                        sbScript = FormatQuery(sbScript, lvi.SubItems[0].Text);
                    }
                }
            }
            ScriptView sw = new ScriptView(sbScript.ToString());
            sw.ShowDialog();
            /*
            using (StreamWriter outfile = new StreamWriter("sql2.txt"))
            {
                outfile.Write(sbScript.ToString());
            }*/
        }

        private void splitContainer2_Panel2_Resize(object sender, EventArgs e)
        {
            ObjectCompare_Resize(sender, e);
        }

        private void CodeShow()
        {
            try
            {
                if (lwDatabaseObjects.SelectedItems[0].Group.Name == "2")
                {
                    lvSource.Columns[1].Text = "Source  -->" + Database2;
                    lvDestination.Columns[1].Text = "Destination  -->" + Database1;
                }
                else
                {
                    lvSource.Columns[1].Text = "Source  -->" + Database1;
                    lvDestination.Columns[1].Text = "Destination  -->" + Database2;
                }

                //si el objeto existe solo en DB2, se muestra al reves para que el codigo de DB2 quede en el panel de la derecha
                //lvSource.Columns[1].Text = "Source  -->" + Database1;
                //lvDestination.Columns[1].Text = "Destination  -->" + Database2;

                //limpiar listviews
                lvDestination.Items.Clear();
                lvSource.Items.Clear();

                //obtener definicion de objeto
                DataView dwObjectDefinition = new DataView(this.dbOjects, "Schema='" + lwDatabaseObjects.SelectedItems[0].SubItems[1].Text + "' AND " + "Type='" + lwDatabaseObjects.SelectedItems[0].SubItems[0].Text + "' AND " + "Name='" + lwDatabaseObjects.SelectedItems[0].SubItems[2].Text + "'", "Name", DataViewRowState.CurrentRows);
                
                switch (lwDatabaseObjects.SelectedItems[0].Group.Name)
                {
                    case "1":
                        showCode(dwObjectDefinition, "ObjectDefinition1", "ObjectDefinition2");
                        break;
                    case "2":
                        showCode(dwObjectDefinition, "ObjectDefinition1", "ObjectDefinition2");
                        break;
                    case "3":
                        showCode(dwObjectDefinition, "ObjectDefinition1", "ObjectDefinition2");
                        break;
                    case "4":
                        showCode(dwObjectDefinition, "ObjectDefinition1", "ObjectDefinition2");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception err)
            {
                string error = err.Message;
            }
        }

        private void showCode(DataView dwObjectDefinition, string ObjectDefinition1, string ObjectDefinition2) {
            //comparar definiciones y mostrar diferencias
            foreach (DataRowView dr in dwObjectDefinition)
            {
                string objDef1 = dr[ObjectDefinition1].ToString();
                string objDef2 = dr[ObjectDefinition2].ToString();

                var diff = InlineDiffBuilder.Diff(objDef2, objDef1);

                ListViewItem lvisSource;
                ListViewItem lvisTarget;
                int Linecount = 1;

                foreach (var line in diff.Lines)
                {
                    switch (line.Type)
                    {
                        case ChangeType.Inserted:
                            lvisSource = new ListViewItem(Linecount.ToString("00000"));
                            lvisTarget = new ListViewItem(Linecount.ToString("00000"));

                            lvisSource.BackColor = Color.LightGreen;
                            lvisSource.SubItems.Add(line.Text);

                            lvisTarget.BackColor = Color.LightGray;
                            lvisTarget.SubItems.Add("");

                            lvSource.Items.Add(lvisSource);
                            lvDestination.Items.Add(lvisTarget);
                            Linecount++;
                            break;
                        case ChangeType.Deleted:
                            lvisSource = new ListViewItem(Linecount.ToString("00000"));
                            lvisTarget = new ListViewItem(Linecount.ToString("00000"));

                            lvisSource.BackColor = Color.LightPink;
                            lvisSource.SubItems.Add(line.Text);

                            lvisTarget.BackColor = Color.LightGray;
                            lvisTarget.SubItems.Add("");

                            lvSource.Items.Add(lvisSource);
                            lvDestination.Items.Add(lvisTarget);
                            Linecount++;
                            break;
                        case ChangeType.Modified:
                            lvisSource = new ListViewItem(Linecount.ToString("00000"));
                            lvisTarget = new ListViewItem(Linecount.ToString("00000"));

                            lvisSource.BackColor = Color.LightPink;
                            lvisSource.SubItems.Add(line.Text);

                            lvisTarget.BackColor = Color.LightGreen;
                            lvisTarget.SubItems.Add(line.Text);

                            lvSource.Items.Add(lvisSource);
                            lvDestination.Items.Add(lvisTarget);
                            Linecount++;
                            break;
                        default:

                            lvisSource = new ListViewItem(Linecount.ToString("00000"));
                            lvisTarget = new ListViewItem(Linecount.ToString("00000"));

                            lvisSource.BackColor = Color.LightGray;
                            lvisSource.SubItems.Add(line.Text);

                            lvisTarget.BackColor = Color.LightGray;
                            lvisTarget.SubItems.Add(line.Text);

                            lvSource.Items.Add(lvisSource);
                            lvDestination.Items.Add(lvisTarget);
                            Linecount++;
                            break;
                    }

                }

                /*
                //DiffList_TextFile sLF = null;
                //DiffList_TextFile dLF = null;
                DiffList_CharData sLF2 = null;
                DiffList_CharData dLF2 = null;

                //sLF = new DiffList_TextFile(objDef1);
                //dLF = new DiffList_TextFile(objDef2);

                sLF2 = new DiffList_CharData(objDef1);
                dLF2 = new DiffList_CharData(objDef2);

                double time = 0;
                DiffEngine de = new DiffEngine();
                //time = de.ProcessDiff(sLF, dLF, DiffEngineLevel.SlowPerfect);
                time = de.ProcessDiff(sLF2, dLF2, DiffEngineLevel.SlowPerfect);
                ArrayList rep = de.DiffReport();

                ListViewItem lviS;
                ListViewItem lviD;
                int cnt = 1;
                int i;

                foreach (DiffResultSpan drs in rep)
                {
                    switch (drs.Status)
                    {
                        case DiffResultSpanStatus.DeleteSource:
                            for (i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(cnt.ToString("00000"));
                                lviD = new ListViewItem(cnt.ToString("00000"));
                                lviS.BackColor = Color.IndianRed;
                                //lviS.SubItems.Add(((TextLine)sLF.GetByIndex(drs.SourceIndex + i)).Line);
                                lviS.SubItems.Add(((TextLine)sLF2.GetByIndex(drs.SourceIndex + i)).Line);
                                lviD.BackColor = Color.LightGray;
                                lviD.SubItems.Add("");
                                lvSource.Items.Add(lviS);
                                lvDestination.Items.Add(lviD);
                                cnt++;
                            }

                            break;
                        case DiffResultSpanStatus.NoChange:
                            for (i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(cnt.ToString("00000"));
                                lviD = new ListViewItem(cnt.ToString("00000"));
                                lviS.BackColor = Color.White;
                                //lviS.SubItems.Add(((TextLine)sLF.GetByIndex(drs.SourceIndex + i)).Line);
                                lviS.SubItems.Add(((TextLine)sLF2.GetByIndex(drs.SourceIndex + i)).Line);
                                lviD.BackColor = Color.White;
                                //lviD.SubItems.Add(((TextLine)dLF.GetByIndex(drs.DestIndex + i)).Line);
                                lviD.SubItems.Add(((TextLine)dLF2.GetByIndex(drs.DestIndex + i)).Line);

                                lvSource.Items.Add(lviS);
                                lvDestination.Items.Add(lviD);
                                cnt++;
                            }

                            break;
                        case DiffResultSpanStatus.AddDestination:
                            for (i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(cnt.ToString("00000"));
                                lviD = new ListViewItem(cnt.ToString("00000"));
                                lviS.BackColor = Color.LightGray;
                                lviS.SubItems.Add("");
                                lviD.BackColor = Color.LightGreen;
                                //lviD.SubItems.Add(((TextLine)dLF.GetByIndex(drs.DestIndex + i)).Line);
                                lviD.SubItems.Add(((TextLine)dLF2.GetByIndex(drs.DestIndex + i)).Line);

                                lvSource.Items.Add(lviS);
                                lvDestination.Items.Add(lviD);
                                cnt++;
                            }

                            break;
                        case DiffResultSpanStatus.Replace:
                            for (i = 0; i < drs.Length; i++)
                            {
                                lviS = new ListViewItem(cnt.ToString("00000"));
                                lviD = new ListViewItem(cnt.ToString("00000"));
                                lviS.BackColor = Color.IndianRed;
                                //lviS.SubItems.Add(((TextLine)sLF.GetByIndex(drs.SourceIndex + i)).Line);
                                lviS.SubItems.Add(((TextLine)sLF2.GetByIndex(drs.SourceIndex + i)).Line);
                                lviD.BackColor = Color.LightGreen;
                                //lviD.SubItems.Add(((TextLine)dLF.GetByIndex(drs.DestIndex + i)).Line);
                                lviD.SubItems.Add(((TextLine)dLF2.GetByIndex(drs.DestIndex + i)).Line);

                                lvSource.Items.Add(lviS);
                                lvDestination.Items.Add(lviD);
                                cnt++;
                            }

                            break;
                    }

                }

                */
            }
        }

        private void lwDatabaseObjects_ColumnClick(object sender, ColumnClickEventArgs e)
        {
        }

        private void lwDatabaseObjects_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void lvSource_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ClipboardList(lvSource, string.Format("Clipboard Detail {1} - {0} items", lvSource.SelectedItems.Count, Database1));
            }
        }

        private void lvDestination_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ClipboardList(lvDestination, string.Format("Clipboard Detail {1} - {0} items", lvDestination.SelectedItems.Count, Database2));
            }
        }

        private void ClipboardList(ListView listData, string copyMessage)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ListViewItem item in listData.SelectedItems)
                {
                    sb.AppendLine(string.Format("{0}", item.SubItems[1].Text));
                }
                Clipboard.SetText(sb.ToString());
                ((MDIMain)this.MdiParent).StatusText = copyMessage;
        }

        private void autoCompleteTextbox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void autoCompleteTextbox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                GetObject(autoCompleteTextbox1.Text);
            }
        }

        private void GetObject(string value)
        {
            lwDatabaseObjects.SelectedItems.Clear();
            foreach (ListViewItem item in lwDatabaseObjects.Items)
	        {
                if (string.Format("[{0}].[{1}]", item.SubItems[1].Text, item.SubItems[2].Text) == value)
                {                    
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();
                    lwDatabaseObjects.Focus();
                }
            }
        }

        private void autoCompleteTextbox1_MouseClick(object sender, MouseEventArgs e)
        {
            GetObject(autoCompleteTextbox1.Text);
        }
    }
}
