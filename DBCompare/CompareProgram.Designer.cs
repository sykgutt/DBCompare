namespace DBCompare
{
    partial class CompareProgram
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Button2 = new System.Windows.Forms.Button();
            this.Button3 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.txtCache = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.btnBeyon = new System.Windows.Forms.Button();
            this.txtBeyon = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.FB1 = new System.Windows.Forms.FolderBrowserDialog();
            this.OF1 = new System.Windows.Forms.OpenFileDialog();
            this.btnWinMerge = new System.Windows.Forms.Button();
            this.txtWinMerge = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rbBeyond = new System.Windows.Forms.RadioButton();
            this.rbWinMerge = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Button2
            // 
            this.Button2.Location = new System.Drawing.Point(455, 217);
            this.Button2.Name = "Button2";
            this.Button2.Size = new System.Drawing.Size(75, 23);
            this.Button2.TabIndex = 15;
            this.Button2.Text = "Cancelar";
            this.Button2.UseVisualStyleBackColor = true;
            this.Button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // Button3
            // 
            this.Button3.Location = new System.Drawing.Point(347, 217);
            this.Button3.Name = "Button3";
            this.Button3.Size = new System.Drawing.Size(75, 23);
            this.Button3.TabIndex = 14;
            this.Button3.Text = "Acceptar";
            this.Button3.UseVisualStyleBackColor = true;
            this.Button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // Button1
            // 
            this.Button1.Location = new System.Drawing.Point(508, 179);
            this.Button1.Name = "Button1";
            this.Button1.Size = new System.Drawing.Size(22, 23);
            this.Button1.TabIndex = 13;
            this.Button1.Text = "...";
            this.Button1.UseVisualStyleBackColor = true;
            this.Button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // txtCache
            // 
            this.txtCache.Location = new System.Drawing.Point(12, 179);
            this.txtCache.Name = "txtCache";
            this.txtCache.ReadOnly = true;
            this.txtCache.Size = new System.Drawing.Size(489, 20);
            this.txtCache.TabIndex = 12;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(9, 162);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(64, 13);
            this.Label2.TabIndex = 11;
            this.Label2.Text = "Ruta Cache";
            // 
            // btnBeyon
            // 
            this.btnBeyon.Location = new System.Drawing.Point(508, 79);
            this.btnBeyon.Name = "btnBeyon";
            this.btnBeyon.Size = new System.Drawing.Size(22, 23);
            this.btnBeyon.TabIndex = 10;
            this.btnBeyon.Text = "...";
            this.btnBeyon.UseVisualStyleBackColor = true;
            this.btnBeyon.Click += new System.EventHandler(this.btnBeyon_Click);
            // 
            // txtBeyon
            // 
            this.txtBeyon.Location = new System.Drawing.Point(12, 79);
            this.txtBeyon.Name = "txtBeyon";
            this.txtBeyon.ReadOnly = true;
            this.txtBeyon.Size = new System.Drawing.Size(489, 20);
            this.txtBeyon.TabIndex = 9;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(9, 62);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(63, 13);
            this.Label1.TabIndex = 8;
            this.Label1.Text = "Ruta Beyon";
            // 
            // OF1
            // 
            this.OF1.FileName = "OpenFileDialog1";
            // 
            // btnWinMerge
            // 
            this.btnWinMerge.Location = new System.Drawing.Point(508, 128);
            this.btnWinMerge.Name = "btnWinMerge";
            this.btnWinMerge.Size = new System.Drawing.Size(22, 23);
            this.btnWinMerge.TabIndex = 18;
            this.btnWinMerge.Text = "...";
            this.btnWinMerge.UseVisualStyleBackColor = true;
            this.btnWinMerge.Click += new System.EventHandler(this.btnWinMerge_Click);
            // 
            // txtWinMerge
            // 
            this.txtWinMerge.Location = new System.Drawing.Point(12, 128);
            this.txtWinMerge.Name = "txtWinMerge";
            this.txtWinMerge.ReadOnly = true;
            this.txtWinMerge.Size = new System.Drawing.Size(489, 20);
            this.txtWinMerge.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 111);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Ruta WinMerge";
            // 
            // rbBeyond
            // 
            this.rbBeyond.AutoSize = true;
            this.rbBeyond.Location = new System.Drawing.Point(12, 30);
            this.rbBeyond.Name = "rbBeyond";
            this.rbBeyond.Size = new System.Drawing.Size(61, 17);
            this.rbBeyond.TabIndex = 19;
            this.rbBeyond.TabStop = true;
            this.rbBeyond.Text = "Beyond";
            this.rbBeyond.UseVisualStyleBackColor = true;
            // 
            // rbWinMerge
            // 
            this.rbWinMerge.AutoSize = true;
            this.rbWinMerge.Location = new System.Drawing.Point(79, 30);
            this.rbWinMerge.Name = "rbWinMerge";
            this.rbWinMerge.Size = new System.Drawing.Size(74, 17);
            this.rbWinMerge.TabIndex = 20;
            this.rbWinMerge.TabStop = true;
            this.rbWinMerge.Text = "WinMerge";
            this.rbWinMerge.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "Programa Principal";
            // 
            // CompareProgram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 252);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rbWinMerge);
            this.Controls.Add(this.rbBeyond);
            this.Controls.Add(this.btnWinMerge);
            this.Controls.Add(this.txtWinMerge);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Button2);
            this.Controls.Add(this.Button3);
            this.Controls.Add(this.Button1);
            this.Controls.Add(this.txtCache);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.btnBeyon);
            this.Controls.Add(this.txtBeyon);
            this.Controls.Add(this.Label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "CompareProgram";
            this.Text = " ";
            this.Load += new System.EventHandler(this.CompareProgram_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Button Button2;
        internal System.Windows.Forms.Button Button3;
        internal System.Windows.Forms.Button Button1;
        internal System.Windows.Forms.TextBox txtCache;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Button btnBeyon;
        internal System.Windows.Forms.TextBox txtBeyon;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.FolderBrowserDialog FB1;
        internal System.Windows.Forms.OpenFileDialog OF1;
        internal System.Windows.Forms.Button btnWinMerge;
        internal System.Windows.Forms.TextBox txtWinMerge;
        internal System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rbBeyond;
        private System.Windows.Forms.RadioButton rbWinMerge;
        internal System.Windows.Forms.Label label4;
    }
}