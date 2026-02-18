using System;
//using System.Collections.Generic;
//using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
//using System.Text;
using System.Windows.Forms;

using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management;
using Microsoft.SqlServer.Management.Common;

using ObjectHelper;
using Ini;

namespace DBCompare
{
    public partial class Login : Form
    {

        Server server1 = null;
        Server server2 = null;

        public Login()
        {
            InitializeComponent();
        }

        private void Login_Resize(object sender, EventArgs e)
        {
            pnlSettings.Location = new Point(
                this.ClientSize.Width / 2 - pnlSettings.Size.Width / 2,
                this.ClientSize.Height / 2 - pnlSettings.Size.Height / 2);
            pnlSettings.Anchor = AnchorStyles.None;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            //Mostrar Lista de Servidores en la RED

            //DataTable dt = SmoApplication.EnumAvailableSqlServers(false);

            //if (dt.Rows.Count > 0)
            //{
            //    foreach (DataRow dr in dt.Rows)
            //    {

            //        cboServer1.Items.Add(dr["Name"].ToString());
            //        cboServer2.Items.Add(dr["Name"].ToString());

            //    }
            //}

            GetServer();
            Reload();
        }

        private void cmdRefreshServer1_Click(object sender, EventArgs e)
        {
            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;
            RefreshServerList(cboServer1);
            // Set cursor as default arrow
            Cursor.Current = Cursors.Default;
        }

        private void cmdRefreshServer2_Click(object sender, EventArgs e)
        {
            // Set cursor as hourglass
            Cursor.Current = Cursors.WaitCursor;
            RefreshServerList(cboServer2);
            // Set cursor as default arrow
            Cursor.Current = Cursors.Default;
        }

        private void rbWindowsAuthentication1_Click(object sender, EventArgs e)
        {
            txtUser1.Enabled = false;
            lblUserName1.Enabled = false;
            txtPassword1.Enabled = false;
            lblPassword1.Enabled = false;
        }

        private void rbSQLServerAuthentication1_CheckedChanged(object sender, EventArgs e)
        {
            txtUser1.Enabled = true;
            lblUserName1.Enabled = true;
            txtPassword1.Enabled = true;
            lblPassword1.Enabled = true;
        }

        private void rbWindowsAuthentication2_CheckedChanged(object sender, EventArgs e)
        {
            txtUser2.Enabled = false;
            lblUserName2.Enabled = false;
            txtPassword2.Enabled = false;
            lblPassword2.Enabled = false;
        }

        private void rbSQLServerAuthentication2_CheckedChanged(object sender, EventArgs e)
        {
            txtUser2.Enabled = true;
            lblUserName2.Enabled = true;
            txtPassword2.Enabled = true;
            lblPassword2.Enabled = true;
        }

        private void cboDatabase1_Click(object sender, EventArgs e)
        {
            if (cboDatabase1.Items.Count == 0)
            {
                if (rbWindowsAuthentication1.Checked)
                {
                    RefreshDatabaseList(cboDatabase1, cboServer1.Text);
                }
                else
                {
                    RefreshDatabaseList(cboDatabase1, cboServer1.Text, txtUser1.Text, txtPassword1.Text);
                }
            }
        }

        private void RefreshServerList(ComboBox cbo)
        {
            cbo.Items.Clear();
            DataTable dt = SmoApplication.EnumAvailableSqlServers(false);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    cbo.Items.Add(dr["Name"].ToString());
                }
            }
        }

        private void RefreshDatabaseList(ComboBox cbo, string server)
        {
            try
            {
                cbo.Items.Clear();
                ServerConnection conn = new ServerConnection();
                conn.ServerInstance = server;
                Server srv = new Server(conn);

                foreach (Database db in srv.Databases)
                {
                    cbo.Items.Add(db.Name);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshDatabaseList(ComboBox cbo, string server, string login, string password)
        {
            try
            {
                cbo.Items.Clear();
                ServerConnection conn = new ServerConnection();
                conn.ServerInstance = server;
                conn.LoginSecure = false;
                conn.Login = login;
                conn.Password = password;
                Server srv = new Server(conn);

                foreach (Database db in srv.Databases)
                {
                    cbo.Items.Add(db.Name);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Authentication Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboDatabase2_Click(object sender, EventArgs e)
        {
            if (cboDatabase2.Items.Count == 0)
            {
                if (rbWindowsAuthentication2.Checked)
                {
                    RefreshDatabaseList(cboDatabase2, cboServer2.Text);
                }
                else
                {
                    RefreshDatabaseList(cboDatabase2, cboServer2.Text, txtUser2.Text, txtPassword2.Text);
                }
            }
        }

        private void cboServer1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboDatabase1.Items.Clear();
        }

        private void cboServer2_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboDatabase2.Items.Clear();
        }

        private void cmdCompare_Click(object sender, EventArgs e)
        {
           
            if (rbSQLServerAuthentication1.Checked == true)
            {
                ServerConnection conn = new ServerConnection
                {
                    ServerInstance = cboServer1.Text,
                    LoginSecure = false,
                    Login = txtUser1.Text,
                    Password = txtPassword1.Text
                };
                server1 = new Server(conn);
            }
            else
            {
                ServerConnection conn = new ServerConnection
                {
                    ServerInstance = cboServer1.Text
                };
                server1 = new Server(conn);
            }


            if (rbSQLServerAuthentication2.Checked == true)
            {
                ServerConnection conn = new ServerConnection
                {
                    ServerInstance = cboServer2.Text,
                    LoginSecure = false,
                    Login = txtUser2.Text,
                    Password = txtPassword2.Text
                };
                server2 = new Server(conn);
            }
            else
            {
                ServerConnection conn = new ServerConnection
                {
                    ServerInstance = cboServer2.Text
                };
                server2 = new Server(conn);
            }
            try
            {
                string srv1 = server1.Information.Version.ToString();
                string srv2 = server2.Information.Version.ToString();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            /*
            ObjectFetch objFetch = new ObjectFetch(server1.Name, cboDatabase1.Text, txtUser1.Text, txtPassword1.Text,server2.Name, cboDatabase2.Text,txtUser2.Text,txtPassword2.Text);
            //objFetch.Parent = this.Parent;
            objFetch.Show();
            this.Hide();
             */
        }

        public ObjectHelper.ScriptingOptions GetScriptiongOptions()
        {
            ObjectHelper.ScriptingOptions so = new ObjectHelper.ScriptingOptions();
            so.Aggregates = chkAggregates.Checked;
            so.ApplicationRoles = chkApplicationRoles.Checked;
            so.Assemblies = chkAssemblies.Checked;
            so.BrokerPriorities = chkBrokerPriorities.Checked;
            so.ClusteredIndexes = chkClusteredIndexes.Checked;
            so.CLRTriggers = chkCLRTriggers.Checked;
            so.CLRUserDefinedFunctions = chkCLRUserDefinedFunctions.Checked;
            so.CheckConstraints = chkCheckConstraints.Checked;
            so.Collation = chkCollation.Checked;
            so.Contracts = chkContracts.Checked;
            so.DatabaseRoles = chkDatabaseRoles.Checked;
            so.DataCompression = chkDataCompression.Checked;
            so.Defaults = chkDefaults.Checked;
            so.DDLTriggers = chkDDLTriggers.Checked;
            so.DefaultConstraints = chkDefaultConstraints.Checked;
            so.DMLTriggers = chkDMLTriggers.Checked;
            so.ForeignKeys = chkForeignKeys.Checked;
            so.FullTextCatalogPath = chkFullTextCatalogPath.Checked;
            so.FullTextCatalogs = chkFullTextCatalogs.Checked;
            so.FullTextIndexes = chkFullTextIndexes.Checked;
            so.FullTextStopLists = chkFullTextStopLists.Checked;
            so.MessageTypes = chkMessageTypes.Checked;
            so.NoFileStream = chkNoFileStream.Checked;
            so.NoIdentities = chkNoIdentities.Checked;
            so.NonClusteredIndexes = chkNonClusteredIndexes.Checked;
            so.PartitionFunctions = chkPartitionFunctions.Checked;
            so.PartitionSchemes = chkPartitionSchemes.Checked;
            so.PrimaryKeys = chkPrimaryKeys.Checked;
            so.RemoteServiceBindings = chkRemoteServiceBindings.Checked;
            so.Routes = chkRoutes.Checked;
            so.Rules = chkRules.Checked;
            so.Schemas = chkSchemas.Checked;
            so.ServiceQueues = chkServiceQueues.Checked;
            so.Services = chkServices.Checked;
            so.SQLUserDefinedFunctions = chkSQLUserDefinedFunctions.Checked;
            so.StoredProcedures = chkStoredProcedures.Checked;
            so.Synonyms = chkSynonyms.Checked;
            so.Tables = chkTables.Checked;            
            so.UniqueConstraints = chkUniqueConstraints.Checked;
            so.UserDefinedDataTypes = chkUserDefinedDataTypes.Checked;
            so.UserDefinedTableTypes = chkUserDefinedTableTypes.Checked;
            so.UserDefinedTypes = chkUserDefinedTypes.Checked;
            so.Users = chkUsers.Checked;
            so.Views = chkViews.Checked;
            so.XMLSchemaCollections = chkXMLSchemaCollections.Checked;

            return so;
        }

        public Server GetServer1()
        {
            return server1;
        }

        public Server GetServer2()
        {
            return server2;
        }

        public string GetDatabase1()
        {
            return cboDatabase1.Text;
        }

        public string GetDatabase2()
        {
            return cboDatabase2.Text;
        }

        private void GetServer()
        {
            Utilidades extras = new Utilidades();

            if (!(rbWindowsAuthentication1.Checked || rbWindowsAuthentication2.Checked))
            {
                if (extras.GetIni("SetupDB1", "UseIntegrated") == "1")
                {
                    rbWindowsAuthentication1.Checked = true;
                }
                else
                {
                    rbSQLServerAuthentication1.Checked = true;
                }

                if (extras.GetIni("SetupDB2", "UseIntegrated") == "1")
                {
                    rbWindowsAuthentication2.Checked = true;
                }
                else
                {
                    rbSQLServerAuthentication2.Checked = true;
                }    
            }

            cboServer1.Text = (string.IsNullOrEmpty(cboServer1.Text)) ? extras.GetIni("SetupDB1", "Server") : cboServer1.Text;
            txtUser1.Text = (string.IsNullOrEmpty(txtUser1.Text)) ? extras.GetIni("SetupDB1", "Usuario") : txtUser1.Text;
            txtPassword1.Text = (string.IsNullOrEmpty(txtPassword1.Text)) ? extras.GetIni("SetupDB1", "Password") : txtPassword1.Text;
            cboDatabase1.Text = (string.IsNullOrEmpty(cboDatabase1.Text)) ? extras.GetIni("SetupDB1", "DataBase") : cboDatabase1.Text;

            cboServer2.Text = (string.IsNullOrEmpty(cboServer2.Text)) ? extras.GetIni("SetupDB2", "Server") : cboServer2.Text;
            txtUser2.Text = (string.IsNullOrEmpty(txtUser2.Text)) ? extras.GetIni("SetupDB2", "Usuario") : txtUser2.Text;
            txtPassword2.Text = (string.IsNullOrEmpty(txtPassword2.Text)) ? extras.GetIni("SetupDB2", "Password") : txtPassword2.Text;
            cboDatabase2.Text = (string.IsNullOrEmpty(cboDatabase2.Text)) ? extras.GetIni("SetupDB2", "DataBase") : cboDatabase2.Text;
        }

        private void cboOptions_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control x in tableLayoutPanel1.Controls)
            {
                ((CheckBox)x).Checked = cboOptions.Checked;
            }
        }

        private void txtReload_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void txtSave_Click(object sender, EventArgs e)
        {
            SavePreferences();
        }

        private void Reload()
        {
            try
            {
                Utilidades extras = new Utilidades();

                string Aggregates = extras.GetIni("Preferences", "Aggregates");
                string ApplicationRoles = extras.GetIni("Preferences", "ApplicationRoles");
                string Assemblies = extras.GetIni("Preferences", "Assemblies");
                string BrokerPriorities = extras.GetIni("Preferences", "BrokerPriorities");
                string ClusteredIndexes = extras.GetIni("Preferences", "ClusteredIndexes");
                string CLRTriggers = extras.GetIni("Preferences", "CLRTriggers");
                string CLRUserDefinedFunctions = extras.GetIni("Preferences", "CLRUserDefinedFunctions");
                string CheckConstraints = extras.GetIni("Preferences", "CheckConstraints");
                string Collation = extras.GetIni("Preferences", "Collation");
                string Contracts = extras.GetIni("Preferences", "Contracts");
                string DatabaseRoles = extras.GetIni("Preferences", "DatabaseRoles");
                string DataCompression = extras.GetIni("Preferences", "DataCompression");
                string Defaults = extras.GetIni("Preferences", "Defaults");
                string DDLTriggers = extras.GetIni("Preferences", "DDLTriggers");
                string DefaultConstraints = extras.GetIni("Preferences", "DefaultConstraints");
                string DMLTriggers = extras.GetIni("Preferences", "DMLTriggers");
                string ForeignKeys = extras.GetIni("Preferences", "ForeignKeys");
                string FullTextCatalogPath = extras.GetIni("Preferences", "FullTextCatalogPath");
                string FullTextCatalogs = extras.GetIni("Preferences", "FullTextCatalogs");
                string FullTextIndexes = extras.GetIni("Preferences", "FullTextIndexes");
                string FullTextStopLists = extras.GetIni("Preferences", "FullTextStopLists");
                string MessageTypes = extras.GetIni("Preferences", "MessageTypes");
                string NoFileStream = extras.GetIni("Preferences", "NoFileStream");
                string NoIdentities = extras.GetIni("Preferences", "NoIdentities");
                string NonClusteredIndexes = extras.GetIni("Preferences", "NonClusteredIndexes");
                string PartitionFunctions = extras.GetIni("Preferences", "PartitionFunctions");
                string PartitionSchemes = extras.GetIni("Preferences", "PartitionSchemes");
                string PrimaryKeys = extras.GetIni("Preferences", "PrimaryKeys");
                string RemoteServiceBindings = extras.GetIni("Preferences", "RemoteServiceBindings");
                string Routes = extras.GetIni("Preferences", "Routes");
                string Rules = extras.GetIni("Preferences", "Rules");
                string Schemas = extras.GetIni("Preferences", "Schemas");
                string ServiceQueues = extras.GetIni("Preferences", "ServiceQueues");
                string Services = extras.GetIni("Preferences", "Services");
                string SQLUserDefinedFunctions = extras.GetIni("Preferences", "SQLUserDefinedFunctions");
                string StoredProcedures = extras.GetIni("Preferences", "StoredProcedures");
                string Synonyms = extras.GetIni("Preferences", "Synonyms");
                string Tables = extras.GetIni("Preferences", "Tables");
                string UniqueConstraints = extras.GetIni("Preferences", "UniqueConstraints");
                string UserDefinedDataTypes = extras.GetIni("Preferences", "UserDefinedDataTypes");
                string UserDefinedTableTypes = extras.GetIni("Preferences", "UserDefinedTableTypes");
                string UserDefinedTypes = extras.GetIni("Preferences", "UserDefinedTypes");
                string Users = extras.GetIni("Preferences", "Users");
                string Views = extras.GetIni("Preferences", "Views");
                string XMLSchemaCollections = extras.GetIni("Preferences", "XMLSchemaCollections");

                chkAggregates.Checked = (Aggregates == "1") ? true : false;
                chkApplicationRoles.Checked = (ApplicationRoles == "1") ? true : false;
                chkAssemblies.Checked = (Assemblies == "1") ? true : false;
                chkBrokerPriorities.Checked = (BrokerPriorities == "1") ? true : false;
                chkClusteredIndexes.Checked = (ClusteredIndexes == "1") ? true : false;
                chkCLRTriggers.Checked = (CLRTriggers == "1") ? true : false;
                chkCLRUserDefinedFunctions.Checked = (CLRUserDefinedFunctions == "1") ? true : false;
                chkCheckConstraints.Checked = (CheckConstraints == "1") ? true : false;
                chkCollation.Checked = (Collation == "1") ? true : false;
                chkContracts.Checked = (Contracts == "1") ? true : false;
                chkDatabaseRoles.Checked = (DatabaseRoles == "1") ? true : false;
                chkDataCompression.Checked = (DataCompression == "1") ? true : false;
                chkDefaults.Checked = (Defaults == "1") ? true : false;
                chkDDLTriggers.Checked = (DDLTriggers == "1") ? true : false;
                chkDefaultConstraints.Checked = (DefaultConstraints == "1") ? true : false;
                chkDMLTriggers.Checked = (DMLTriggers == "1") ? true : false;
                chkForeignKeys.Checked = (ForeignKeys == "1") ? true : false;
                chkFullTextCatalogPath.Checked = (FullTextCatalogPath == "1") ? true : false;
                chkFullTextCatalogs.Checked = (FullTextCatalogs == "1") ? true : false;
                chkFullTextIndexes.Checked = (FullTextIndexes == "1") ? true : false;
                chkFullTextStopLists.Checked = (FullTextStopLists == "1") ? true : false;
                chkMessageTypes.Checked = (MessageTypes == "1") ? true : false;
                chkNoFileStream.Checked = (NoFileStream == "1") ? true : false;
                chkNoIdentities.Checked = (NoIdentities == "1") ? true : false;
                chkNonClusteredIndexes.Checked = (NonClusteredIndexes == "1") ? true : false;
                chkPartitionFunctions.Checked = (PartitionFunctions == "1") ? true : false;
                chkPartitionSchemes.Checked = (PartitionSchemes == "1") ? true : false;
                chkPrimaryKeys.Checked = (PrimaryKeys == "1") ? true : false;
                chkRemoteServiceBindings.Checked = (RemoteServiceBindings == "1") ? true : false;
                chkRoutes.Checked = (Routes == "1") ? true : false;
                chkRules.Checked = (Rules == "1") ? true : false;
                chkSchemas.Checked = (Schemas == "1") ? true : false;
                chkServiceQueues.Checked = (ServiceQueues == "1") ? true : false;
                chkServices.Checked = (Services == "1") ? true : false;
                chkSQLUserDefinedFunctions.Checked = (SQLUserDefinedFunctions == "1") ? true : false;
                chkStoredProcedures.Checked = (StoredProcedures == "1") ? true : false;
                chkSynonyms.Checked = (Synonyms == "1") ? true : false;
                chkTables.Checked = (Tables == "1") ? true : false;
                chkUniqueConstraints.Checked = (UniqueConstraints == "1") ? true : false;
                chkUserDefinedDataTypes.Checked = (UserDefinedDataTypes == "1") ? true : false;
                chkUserDefinedTableTypes.Checked = (UserDefinedTableTypes == "1") ? true : false;
                chkUserDefinedTypes.Checked = (UserDefinedTypes == "1") ? true : false;
                chkUsers.Checked = (Users == "1") ? true : false;
                chkViews.Checked = (Views == "1") ? true : false;
                chkXMLSchemaCollections.Checked = (XMLSchemaCollections == "1") ? true : false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }        
        }

        private void SavePreferences()
        {
            try
            {
                Utilidades extras = new Utilidades();

                extras.SetIni("Preferences", "Aggregates", (chkAggregates.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "ApplicationRoles", (chkApplicationRoles.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Assemblies", (chkAssemblies.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "BrokerPriorities", (chkBrokerPriorities.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "ClusteredIndexes", (chkClusteredIndexes.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "CLRTriggers", (chkCLRTriggers.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "CLRUserDefinedFunctions", (chkCLRUserDefinedFunctions.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "CheckConstraints", (chkCheckConstraints.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Collation", (chkCollation.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Contracts", (chkContracts.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "DatabaseRoles", (chkDatabaseRoles.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "DataCompression", (chkDataCompression.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Defaults", (chkDefaults.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "DDLTriggers", (chkDDLTriggers.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "DefaultConstraints", (chkDefaultConstraints.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "DMLTriggers", (chkDMLTriggers.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "ForeignKeys", (chkForeignKeys.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "FullTextCatalogPath", (chkFullTextCatalogPath.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "FullTextCatalogs", (chkFullTextCatalogs.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "FullTextIndexes", (chkFullTextIndexes.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "FullTextStopLists", (chkFullTextStopLists.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "MessageTypes", (chkMessageTypes.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "NoFileStream", (chkNoFileStream.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "NoIdentities", (chkNoIdentities.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "NonClusteredIndexes", (chkNonClusteredIndexes.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "PartitionFunctions", (chkPartitionFunctions.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "PartitionSchemes", (chkPartitionSchemes.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "PrimaryKeys", (chkPrimaryKeys.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "RemoteServiceBindings", (chkRemoteServiceBindings.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Routes", (chkRoutes.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Rules", (chkRules.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Schemas", (chkSchemas.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "ServiceQueues", (chkServiceQueues.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Services", (chkServices.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "SQLUserDefinedFunctions", (chkSQLUserDefinedFunctions.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "StoredProcedures", (chkStoredProcedures.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Synonyms", (chkSynonyms.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Tables", (chkTables.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "UniqueConstraints", (chkUniqueConstraints.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "UserDefinedDataTypes", (chkUserDefinedDataTypes.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "UserDefinedTableTypes", (chkUserDefinedTableTypes.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "UserDefinedTypes", (chkUserDefinedTypes.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Users", (chkUsers.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "Views", (chkViews.Checked) ? "1" : "0");
                extras.SetIni("Preferences", "XMLSchemaCollections", (chkXMLSchemaCollections.Checked) ? "1" : "0");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
