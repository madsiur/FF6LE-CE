namespace FF3LE
{
    partial class ExpansionWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExpansionWindow));
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbLocationFile = new System.Windows.Forms.TextBox();
            this.btnLocNamesPath = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.ckZdPlus = new System.Windows.Forms.CheckBox();
            this.btnExpand = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.nudBanks = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbExpansionTilemaps = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tbExpansionData = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.ExpFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnExpandChests = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBanks)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbLocationFile);
            this.groupBox3.Controls.Add(this.btnLocNamesPath);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.ckZdPlus);
            this.groupBox3.Controls.Add(this.btnExpand);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.nudBanks);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.tbExpansionTilemaps);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.tbExpansionData);
            this.groupBox3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(337, 239);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Map Expansion";
            // 
            // tbLocationFile
            // 
            this.tbLocationFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLocationFile.Location = new System.Drawing.Point(13, 166);
            this.tbLocationFile.Name = "tbLocationFile";
            this.tbLocationFile.Size = new System.Drawing.Size(310, 20);
            this.tbLocationFile.TabIndex = 18;
            // 
            // btnLocNamesPath
            // 
            this.btnLocNamesPath.Location = new System.Drawing.Point(219, 135);
            this.btnLocNamesPath.Name = "btnLocNamesPath";
            this.btnLocNamesPath.Size = new System.Drawing.Size(104, 23);
            this.btnLocNamesPath.TabIndex = 17;
            this.btnLocNamesPath.Text = "Choose Folder";
            this.btnLocNamesPath.UseVisualStyleBackColor = true;
            this.btnLocNamesPath.Click += new System.EventHandler(this.btnLocNamesPath_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 139);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(141, 14);
            this.label8.TabIndex = 16;
            this.label8.Text = "Project settings file path";
            // 
            // ckZdPlus
            // 
            this.ckZdPlus.AutoSize = true;
            this.ckZdPlus.Enabled = false;
            this.ckZdPlus.Location = new System.Drawing.Point(13, 107);
            this.ckZdPlus.Name = "ckZdPlus";
            this.ckZdPlus.Size = new System.Drawing.Size(267, 18);
            this.ckZdPlus.TabIndex = 11;
            this.ckZdPlus.Text = "I was using FF6LE+ or Zone Doctor+ before";
            this.ckZdPlus.UseVisualStyleBackColor = true;
            // 
            // btnExpand
            // 
            this.btnExpand.Enabled = false;
            this.btnExpand.Location = new System.Drawing.Point(219, 202);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(104, 23);
            this.btnExpand.TabIndex = 10;
            this.btnExpand.Text = "Expand Maps";
            this.btnExpand.UseVisualStyleBackColor = true;
            this.btnExpand.Click += new System.EventHandler(this.btnExpand_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(198, 14);
            this.label5.TabIndex = 9;
            this.label5.Text = "Number of banks for Tilemaps data";
            // 
            // nudBanks
            // 
            this.nudBanks.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudBanks.Location = new System.Drawing.Point(286, 77);
            this.nudBanks.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.nudBanks.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudBanks.Name = "nudBanks";
            this.nudBanks.Size = new System.Drawing.Size(37, 22);
            this.nudBanks.TabIndex = 8;
            this.nudBanks.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nudBanks.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(283, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "$";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(242, 14);
            this.label4.TabIndex = 6;
            this.label4.Text = "Locations Tilemaps expansion starting bank";
            // 
            // tbExpansionTilemaps
            // 
            this.tbExpansionTilemaps.Enabled = false;
            this.tbExpansionTilemaps.Location = new System.Drawing.Point(299, 49);
            this.tbExpansionTilemaps.MaxLength = 2;
            this.tbExpansionTilemaps.Name = "tbExpansionTilemaps";
            this.tbExpansionTilemaps.Size = new System.Drawing.Size(24, 22);
            this.tbExpansionTilemaps.TabIndex = 5;
            this.tbExpansionTilemaps.Text = "F4";
            this.tbExpansionTilemaps.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(283, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "$";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(264, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "NPCs, Events, Exits and Chests expansion bank";
            // 
            // tbExpansionData
            // 
            this.tbExpansionData.Enabled = false;
            this.tbExpansionData.Location = new System.Drawing.Point(299, 21);
            this.tbExpansionData.MaxLength = 2;
            this.tbExpansionData.Name = "tbExpansionData";
            this.tbExpansionData.Size = new System.Drawing.Size(24, 22);
            this.tbExpansionData.TabIndex = 2;
            this.tbExpansionData.Text = "F3";
            this.tbExpansionData.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(231, 316);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(104, 23);
            this.btnOk.TabIndex = 15;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // ExpFolderBrowserDialog
            // 
            this.ExpFolderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(121, 316);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(104, 23);
            this.btnApply.TabIndex = 16;
            this.btnApply.Text = "Apply Changes";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnExpandChests
            // 
            this.btnExpandChests.Enabled = false;
            this.btnExpandChests.Location = new System.Drawing.Point(219, 21);
            this.btnExpandChests.Name = "btnExpandChests";
            this.btnExpandChests.Size = new System.Drawing.Size(104, 23);
            this.btnExpandChests.TabIndex = 11;
            this.btnExpandChests.Text = "Expand Chests";
            this.btnExpandChests.UseVisualStyleBackColor = true;
            this.btnExpandChests.Click += new System.EventHandler(this.btnExpandChests_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 25);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(196, 14);
            this.label9.TabIndex = 19;
            this.label9.Text = "Expand chests bits ($1E20-$1E7F)";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.btnExpandChests);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 257);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 53);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Chest Expansion";
            // 
            // ExpansionWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 354);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ExpansionWindow";
            this.Text = "ExpansionWindow";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBanks)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.CheckBox ckZdPlus;
        private System.Windows.Forms.Button btnExpand;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudBanks;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbExpansionTilemaps;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbExpansionData;
        private System.Windows.Forms.TextBox tbLocationFile;
        private System.Windows.Forms.Button btnLocNamesPath;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.FolderBrowserDialog ExpFolderBrowserDialog;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnExpandChests;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}