namespace DBCompare
{
    partial class DataCompare
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
            this.tcBaseDatos = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.cbAll = new System.Windows.Forms.CheckBox();
            this.btCancelar = new System.Windows.Forms.Button();
            this.btAceptar = new System.Windows.Forms.Button();
            this.lwDataTables = new System.Windows.Forms.ListView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.cbAllData = new System.Windows.Forms.CheckBox();
            this.btnCompare = new System.Windows.Forms.Button();
            this.lwCompare = new System.Windows.Forms.ListView();
            this.tcBaseDatos.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcBaseDatos
            // 
            this.tcBaseDatos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcBaseDatos.Controls.Add(this.tabPage1);
            this.tcBaseDatos.Controls.Add(this.tabPage2);
            this.tcBaseDatos.Location = new System.Drawing.Point(12, 12);
            this.tcBaseDatos.Name = "tcBaseDatos";
            this.tcBaseDatos.SelectedIndex = 0;
            this.tcBaseDatos.Size = new System.Drawing.Size(860, 538);
            this.tcBaseDatos.TabIndex = 4;
            this.tcBaseDatos.Selected += new System.Windows.Forms.TabControlEventHandler(this.tcBaseDatos_Selected);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.cbAll);
            this.tabPage1.Controls.Add(this.btCancelar);
            this.tabPage1.Controls.Add(this.btAceptar);
            this.tabPage1.Controls.Add(this.lwDataTables);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(852, 512);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Obtener Tablas";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // cbAll
            // 
            this.cbAll.AutoSize = true;
            this.cbAll.Location = new System.Drawing.Point(6, 6);
            this.cbAll.Name = "cbAll";
            this.cbAll.Size = new System.Drawing.Size(71, 17);
            this.cbAll.TabIndex = 3;
            this.cbAll.Text = "Cheak All";
            this.cbAll.UseVisualStyleBackColor = true;
            this.cbAll.CheckedChanged += new System.EventHandler(this.cbAll_CheckedChanged);
            // 
            // btCancelar
            // 
            this.btCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancelar.Location = new System.Drawing.Point(723, 483);
            this.btCancelar.Name = "btCancelar";
            this.btCancelar.Size = new System.Drawing.Size(123, 23);
            this.btCancelar.TabIndex = 2;
            this.btCancelar.Text = "Cancelar";
            this.btCancelar.UseVisualStyleBackColor = true;
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // btAceptar
            // 
            this.btAceptar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btAceptar.Location = new System.Drawing.Point(594, 483);
            this.btAceptar.Name = "btAceptar";
            this.btAceptar.Size = new System.Drawing.Size(123, 23);
            this.btAceptar.TabIndex = 3;
            this.btAceptar.Text = "Aceptar";
            this.btAceptar.UseVisualStyleBackColor = true;
            this.btAceptar.Click += new System.EventHandler(this.btAceptar_Click);
            // 
            // lwDataTables
            // 
            this.lwDataTables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lwDataTables.CheckBoxes = true;
            this.lwDataTables.Location = new System.Drawing.Point(6, 29);
            this.lwDataTables.MultiSelect = false;
            this.lwDataTables.Name = "lwDataTables";
            this.lwDataTables.Size = new System.Drawing.Size(840, 448);
            this.lwDataTables.TabIndex = 2;
            this.lwDataTables.UseCompatibleStateImageBehavior = false;
            this.lwDataTables.View = System.Windows.Forms.View.List;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.cbAllData);
            this.tabPage2.Controls.Add(this.btnCompare);
            this.tabPage2.Controls.Add(this.lwCompare);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(852, 512);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Comparar Datos";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // cbAllData
            // 
            this.cbAllData.AutoSize = true;
            this.cbAllData.Location = new System.Drawing.Point(6, 6);
            this.cbAllData.Name = "cbAllData";
            this.cbAllData.Size = new System.Drawing.Size(71, 17);
            this.cbAllData.TabIndex = 4;
            this.cbAllData.Text = "Cheak All";
            this.cbAllData.UseVisualStyleBackColor = true;
            this.cbAllData.CheckedChanged += new System.EventHandler(this.cbAllData_CheckedChanged);
            // 
            // btnCompare
            // 
            this.btnCompare.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompare.Location = new System.Drawing.Point(723, 483);
            this.btnCompare.Name = "btnCompare";
            this.btnCompare.Size = new System.Drawing.Size(123, 23);
            this.btnCompare.TabIndex = 1;
            this.btnCompare.Text = "Compare";
            this.btnCompare.UseVisualStyleBackColor = true;
            this.btnCompare.Click += new System.EventHandler(this.btnCompare_Click);
            // 
            // lwCompare
            // 
            this.lwCompare.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lwCompare.CheckBoxes = true;
            this.lwCompare.Location = new System.Drawing.Point(6, 29);
            this.lwCompare.Name = "lwCompare";
            this.lwCompare.Size = new System.Drawing.Size(840, 448);
            this.lwCompare.TabIndex = 0;
            this.lwCompare.UseCompatibleStateImageBehavior = false;
            this.lwCompare.View = System.Windows.Forms.View.List;
            this.lwCompare.SelectedIndexChanged += new System.EventHandler(this.lwCompare_SelectedIndexChanged);
            this.lwCompare.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lwCompare_MouseDoubleClick);
            // 
            // DataCompare
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 562);
            this.Controls.Add(this.tcBaseDatos);
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "DataCompare";
            this.Text = "DataCompare";
            this.Load += new System.EventHandler(this.DataCompare_Load);
            this.tcBaseDatos.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcBaseDatos;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.CheckBox cbAll;
        private System.Windows.Forms.Button btCancelar;
        private System.Windows.Forms.Button btAceptar;
        private System.Windows.Forms.ListView lwDataTables;
        private System.Windows.Forms.ListView lwCompare;
        private System.Windows.Forms.CheckBox cbAllData;
        private System.Windows.Forms.Button btnCompare;

    }
}