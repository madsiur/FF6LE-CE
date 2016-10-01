namespace FF3LE
{
    partial class ImportWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportWindow));
            this.tbSettingFile = new System.Windows.Forms.TextBox();
            this.btnSettingFile = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tbLocNames = new System.Windows.Forms.TextBox();
            this.btnLocNames = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbMemory = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbSettingFile
            // 
            this.tbSettingFile.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbSettingFile.Location = new System.Drawing.Point(15, 105);
            this.tbSettingFile.Name = "tbSettingFile";
            this.tbSettingFile.Size = new System.Drawing.Size(357, 22);
            this.tbSettingFile.TabIndex = 21;
            // 
            // btnSettingFile
            // 
            this.btnSettingFile.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSettingFile.Location = new System.Drawing.Point(268, 74);
            this.btnSettingFile.Name = "btnSettingFile";
            this.btnSettingFile.Size = new System.Drawing.Size(104, 23);
            this.btnSettingFile.TabIndex = 20;
            this.btnSettingFile.Text = "Choose Folder";
            this.btnSettingFile.UseVisualStyleBackColor = true;
            this.btnSettingFile.Click += new System.EventHandler(this.btnSettingFile_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(12, 78);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(100, 14);
            this.label8.TabIndex = 19;
            this.label8.Text = "Settings file path";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tbLocNames
            // 
            this.tbLocNames.Enabled = false;
            this.tbLocNames.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbLocNames.Location = new System.Drawing.Point(15, 46);
            this.tbLocNames.Name = "tbLocNames";
            this.tbLocNames.Size = new System.Drawing.Size(357, 22);
            this.tbLocNames.TabIndex = 24;
            // 
            // btnLocNames
            // 
            this.btnLocNames.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLocNames.Location = new System.Drawing.Point(268, 15);
            this.btnLocNames.Name = "btnLocNames";
            this.btnLocNames.Size = new System.Drawing.Size(104, 23);
            this.btnLocNames.TabIndex = 23;
            this.btnLocNames.Text = "Choose File";
            this.btnLocNames.UseVisualStyleBackColor = true;
            this.btnLocNames.Click += new System.EventHandler(this.btnLocNames_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 14);
            this.label1.TabIndex = 22;
            this.label1.Text = "Location names file";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(92, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(15, 16);
            this.label3.TabIndex = 27;
            this.label3.Text = "$";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 142);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 14);
            this.label4.TabIndex = 26;
            this.label4.Text = "Memory Byte";
            // 
            // tbMemory
            // 
            this.tbMemory.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbMemory.Location = new System.Drawing.Point(107, 138);
            this.tbMemory.MaxLength = 6;
            this.tbMemory.Name = "tbMemory";
            this.tbMemory.Size = new System.Drawing.Size(51, 22);
            this.tbMemory.TabIndex = 25;
            this.tbMemory.Text = "EDC47F";
            this.tbMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(268, 140);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(104, 23);
            this.btnOk.TabIndex = 28;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // ImportWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 173);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbMemory);
            this.Controls.Add(this.tbLocNames);
            this.Controls.Add(this.btnLocNames);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSettingFile);
            this.Controls.Add(this.btnSettingFile);
            this.Controls.Add(this.label8);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ImportWindow";
            this.Text = "Import Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbSettingFile;
        private System.Windows.Forms.Button btnSettingFile;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox tbLocNames;
        private System.Windows.Forms.Button btnLocNames;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbMemory;
        private System.Windows.Forms.Button btnOk;
    }
}