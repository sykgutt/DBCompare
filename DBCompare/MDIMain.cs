using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using Ini;
using System.IO;

namespace DBCompare
{
    public partial class MDIMain : Form
    {
        //private int childFormNumber = 0;

        public MDIMain()
        {
            InitializeComponent();
        }

        public string StatusText
        {
            get
            {
                return toolStripStatusLabel.Text;
            }
            set
            {
                toolStripStatusLabel.Text = value;
            }
        }

        private void ShowNewForm(object sender, EventArgs e)
        {
            ObjectCompare objCompare = new ObjectCompare();
            objCompare.MdiParent = this;
            objCompare.WindowState = FormWindowState.Maximized;
            objCompare.Show();
        }

        private void OpenFile(object sender, EventArgs e)
        {
            LoadProject();
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ObjectCompare objCompare = this.ActiveMdiChild as ObjectCompare;
            if (objCompare != null)
            {
                objCompare.Filename = null;
                objCompare.Save();
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form objCompare = null;

            // Looking for MyForm among all opened forms 
            foreach (Form form in Application.OpenForms)
                if (form is ObjectCompare)
                {
                    objCompare = form;
                    break;
                }

            if (Object.ReferenceEquals(null, objCompare))
            {
                // No opened form, lets create it and show up:
                objCompare = new ObjectCompare();
                objCompare.MdiParent = this;
                objCompare.WindowState = FormWindowState.Normal;
                objCompare.Show();
            }
            else
            {
                // MyForm has been already opened
                objCompare.Close();
                objCompare = new ObjectCompare();
                objCompare.MdiParent = this;
                objCompare.WindowState = FormWindowState.Normal;
                objCompare.Show();

            }


        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveActiveObjectCompare();
        }

        private void helpMenu_Click(object sender, EventArgs e)
        {

        }

        private void LoadProject()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Text Files (*.xml)|*.xml|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;

                DataTable dt1 = new DataTable();
                dt1.Columns.Add("ResultSet");
                dt1.Columns.Add("Name");
                dt1.Columns.Add("Type");
                dt1.Columns.Add("Schema");
                dt1.Columns.Add("ObjectDefinition1");
                dt1.Columns.Add("ObjectDefinition2");

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(FileName);
                
                string db1 = "";
                string db2 = "";

                foreach(XmlNode xmlNode in xmlDocument.ChildNodes)
                {
                    string aa = xmlNode.Name + " " + xmlNode.Value;
                }

                string srv1 = "";
                string login1 = "";
                string pwd1 = "";

                foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("Server1"))
                {
                    foreach (XmlNode xmlServerNode in xmlNode.ChildNodes)
                    {
                        switch (xmlServerNode.Name)
                        { 
                            case "Server":
                                srv1 = xmlServerNode.InnerText;
                                break;
                            case "Database":
                                db1 = xmlServerNode.InnerText;
                                break;
                            case "Login":
                                login1 = xmlServerNode.InnerText;
                                break;
                            case "Password":
                                pwd1 = xmlServerNode.InnerText;
                                break;
                        }
                    }
                }

                ServerConnection conn = new ServerConnection();
                conn.ServerInstance = srv1;
                if (login1 != "")
                {
                    conn.LoginSecure = false;
                    conn.Login = login1;
                    conn.Password = CredentialProtector.Unprotect(pwd1);
                }
                Server server1 = new Server(conn);

                string srv2 = "";
                string login2 = "";
                string pwd2 = "";

                foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("Server2"))
                {
                    foreach (XmlNode xmlServerNode in xmlNode.ChildNodes)
                    {
                        switch (xmlServerNode.Name)
                        {
                            case "Server":
                                srv2 = xmlServerNode.InnerText;
                                break;
                            case "Database":
                                db2 = xmlServerNode.InnerText;
                                break;
                            case "Login":
                                login2 = xmlServerNode.InnerText;
                                break;
                            case "Password":
                                pwd2 = xmlServerNode.InnerText;
                                break;
                        }
                    }
                }

                conn = new ServerConnection();
                conn.ServerInstance = srv2;
                if (login2 != "")
                {
                    conn.LoginSecure = false;
                    conn.Login = login2;
                    conn.Password = CredentialProtector.Unprotect(pwd2);
                }
                Server server2 = new Server(conn);

                foreach (XmlNode xmlNode in xmlDocument.GetElementsByTagName("Object"))
                {
                    DataRow dr = dt1.NewRow();
                    dr["ResultSet"] = xmlNode.Attributes["ResultSet"].Value;
                    dr["Name"] = xmlNode.Attributes["Name"].Value;
                    dr["Type"] = xmlNode.Attributes["Type"].Value;
                    dr["Schema"] = xmlNode.Attributes["Schema"].Value;
                    dr["ObjectDefinition1"] = xmlNode.Attributes["ObjectDefinition1"].Value;
                    dr["ObjectDefinition2"] = xmlNode.Attributes["ObjectDefinition2"].Value;
                    dt1.Rows.Add(dr);
                }

                ObjectCompare objCompare = new ObjectCompare(false);
                objCompare.SetDatabase1(db1);
                objCompare.SetDatabase2(db2);

                objCompare.SetServer1(server1);
                objCompare.SetServer2(server2);

                objCompare.LoadObjects(dt1);
                objCompare.MdiParent = this;
                objCompare.Show();
                
            }
        }

        private void MDIMain_Load(object sender, EventArgs e)
        {
            Utilidades extras = new Utilidades();
            extras.FileIni();
        }

        private void beyondToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form objCompareProgram = null;

            // Looking for MyForm among all opened forms 
            foreach (Form form in Application.OpenForms)
                if (form is CompareProgram)
                {
                    objCompareProgram = form;
                    break;
                }

            if (Object.ReferenceEquals(null, objCompareProgram))
            {
                // No opened form, lets create it and show up:
                objCompareProgram = new CompareProgram();
                objCompareProgram.MdiParent = this;
                objCompareProgram.Show();
            }
            else
            {
                // MyForm has been already opened

                // Lets bring it to front, focus, restore it sizes (if minimized)
                if (objCompareProgram.WindowState == FormWindowState.Minimized)
                    objCompareProgram.WindowState = FormWindowState.Normal;

                objCompareProgram.BringToFront();

                if (objCompareProgram.CanFocus)
                    objCompareProgram.Focus();
            }

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveActiveObjectCompare();
        }

        private void SaveActiveObjectCompare()
        {
            ObjectCompare objCompare = this.ActiveMdiChild as ObjectCompare;
            if (objCompare != null)
            {
                objCompare.Save();
            }
        }

        private void dataCompareToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form objDataCompare = null;

            // Looking for MyForm among all opened forms 
            foreach (Form form in Application.OpenForms) 
                if (form is DataCompare) 
                    {
                        objDataCompare = form;
                        break; 
                    }

            if (Object.ReferenceEquals(null, objDataCompare))
            {
                // No opened form, lets create it and show up:
               objDataCompare = new DataCompare();
                objDataCompare.MdiParent = this;
                objDataCompare.WindowState = FormWindowState.Maximized;
                objDataCompare.Show();
              }
              else {
                // MyForm has been already opened

                // Lets bring it to front, focus, restore it sizes (if minimized)
                  if (objDataCompare.WindowState == FormWindowState.Minimized)
                      objDataCompare.WindowState = FormWindowState.Normal;

                  objDataCompare.BringToFront();

                  if (objDataCompare.CanFocus)
                      objDataCompare.Focus();
              }
        }
    }
}
