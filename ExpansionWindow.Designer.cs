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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbLocationFile = new System.Windows.Forms.TextBox();
            this.btnLocNamesPath = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbExpansionMemory = new System.Windows.Forms.TextBox();
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
            this.ExpFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBanks)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbLocationFile);
            this.groupBox3.Controls.Add(this.btnLocNamesPath);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.btnOk);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.tbExpansionMemory);
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
            this.groupBox3.Size = new System.Drawing.Size(337, 271);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Map Expansion";
            // 
            // tbLocationFile
            // 
            this.tbLocationFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLocationFile.Location = new System.Drawing.Point(13, 198);
            this.tbLocationFile.Name = "tbLocationFile";
            this.tbLocationFile.Size = new System.Drawing.Size(310, 20);
            this.tbLocationFile.TabIndex = 18;
            // 
            // btnLocNamesPath
            // 
            this.btnLocNamesPath.Location = new System.Drawing.Point(219, 167);
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
            this.label8.Location = new System.Drawing.Point(10, 171);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(194, 14);
            this.label8.TabIndex = 16;
            this.label8.Text = "Expanded location names file path";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(243, 242);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(80, 23);
            this.btnOk.TabIndex = 15;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(246, 111);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 16);
            this.label6.TabIndex = 14;
            this.label6.Text = "$";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 112);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(174, 14);
            this.label7.TabIndex = 13;
            this.label7.Text = "Expansion memory byte offset";
            // 
            // tbExpansionMemory
            // 
            this.tbExpansionMemory.Location = new System.Drawing.Point(262, 108);
            this.tbExpansionMemory.MaxLength = 6;
            this.tbExpansionMemory.Name = "tbExpansionMemory";
            this.tbExpansionMemory.Size = new System.Drawing.Size(61, 22);
            this.tbExpansionMemory.TabIndex = 12;
            this.tbExpansionMemory.Text = "EDC47F";
            this.tbExpansionMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ckZdPlus
            // 
            this.ckZdPlus.AutoSize = true;
            this.ckZdPlus.Enabled = false;
            this.ckZdPlus.Location = new System.Drawing.Point(13, 136);
            this.ckZdPlus.Name = "ckZdPlus";
            this.ckZdPlus.Size = new System.Drawing.Size(267, 18);
            this.ckZdPlus.TabIndex = 11;
            this.ckZdPlus.Text = "I was using FF6LE+ or Zone Doctor+ before";
            this.ckZdPlus.UseVisualStyleBackColor = true;
            // 
            // btnExpand
            // 
            this.btnExpand.Enabled = false;
            this.btnExpand.Location = new System.Drawing.Point(157, 242);
            this.btnExpand.Name = "btnExpand";
            this.btnExpand.Size = new System.Drawing.Size(80, 23);
            this.btnExpand.TabIndex = 10;
            this.btnExpand.Text = "Expand";
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
            // ExpFolderBrowserDialog
            // 
            this.ExpFolderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            // 
            // ExpansionWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 295);
            this.Controls.Add(this.groupBox3);
            this.Name = "ExpansionWindow";
            this.Text = "ExpansionWindow";
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudBanks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbExpansionMemory;
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
    }
}