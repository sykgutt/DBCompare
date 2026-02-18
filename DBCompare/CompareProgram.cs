using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DBCompare
{
    public partial class CompareProgram : Form
    {
        public CompareProgram()
        {
            InitializeComponent();
        }

        private string PathBeyon = string.Empty;
        private string PathWinMerge = string.Empty;
        private string PathCache = string.Empty;
        private string ProgramCompare = string.Empty;

        private void Button2_Click(object sender, EventArgs e)
        {
            Utilidades extras = new Utilidades();
            extras.SetIni("Setup", "PathBeyon", PathBeyon);
            extras.SetIni("Setup", "PathCache", PathCache);
            extras.SetIni("Setup", "PathWinMerge", PathWinMerge);
            extras.SetIni("Setup", "ProgramCompare", ProgramCompare);
            this.Close();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Utilidades extras = new Utilidades();
            extras.SetIni("Setup", "PathBeyon", (string.IsNullOrEmpty(txtBeyon.Text)) ? PathBeyon : txtBeyon.Text);
            extras.SetIni("Setup", "PathCache", (string.IsNullOrEmpty(txtCache.Text)) ? PathCache : txtCache.Text);
            extras.SetIni("Setup", "PathWinMerge", (string.IsNullOrEmpty(txtWinMerge.Text)) ? PathWinMerge : txtWinMerge.Text);
            if (rbBeyond.Checked)
	        {
		        extras.SetIni("Setup", "ProgramCompare", "1");
	        }
            if (rbWinMerge.Checked)
    	    {
		        extras.SetIni("Setup", "ProgramCompare", "0");
	        }
        }

        private void btnBeyon_Click(object sender, EventArgs e)
        {
            OF1.CheckFileExists =  true;
            OF1.CheckPathExists = true;
            OF1.DefaultExt = "EXE";
            OF1.Filter = "*.exe|Archivos Ejecutables";
            OF1.Title = "Archivo de Beyon";
            OF1.FileName = "BCompare.exe";      
            if (OF1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
	        {
                txtBeyon.Text = OF1.FileName;
	        }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            FB1.SelectedPath = txtCache.Text;
            FB1.ShowNewFolderButton = true;

            if (FB1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
	        {
		    string Cad = FB1.SelectedPath;
                if (Cad.Substring(Cad.Length - 1, 1) == @"\")
	            {
		            txtCache.Text = Cad;
	            }
                else
                {
                    txtCache.Text = Cad + @"\";
                }
	        }
        }

        private void CompareProgram_Load(object sender, EventArgs e)
        {
            Utilidades extras = new Utilidades();
            PathBeyon = extras.GetIni("Setup", "PathBeyon");
            PathWinMerge = extras.GetIni("Setup", "PathWinMerge");
            PathCache = extras.GetIni("Setup", "PathCache");
            ProgramCompare = extras.GetIni("Setup", "ProgramCompare");

            txtWinMerge.Text = PathWinMerge;
            txtBeyon.Text = PathBeyon;
            txtCache.Text = PathCache;

            if (ProgramCompare == "1")
            {
                rbBeyond.Checked = true;
                rbWinMerge.Checked = false;
            }
            else
            {
                rbWinMerge.Checked = true;
                rbBeyond.Checked = false;
            }
        }

        private void btnWinMerge_Click(object sender, EventArgs e)
        {
            OF1.CheckFileExists = true;
            OF1.CheckPathExists = true;
            OF1.DefaultExt = "EXE";
            OF1.Filter = "*.exe|Archivos Ejecutables";
            OF1.Title = "Archivo de WinMerge";
            OF1.FileName = "WinMergeU.exe";
            if (OF1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtBeyon.Text = OF1.FileName;
            }
        }
    }
}
