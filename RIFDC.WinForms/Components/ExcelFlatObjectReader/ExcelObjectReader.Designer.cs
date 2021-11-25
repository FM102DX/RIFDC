namespace IntraToolAutomation
{
    partial class ExcelObjectReaderFrm
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
            this.dgObjects = new System.Windows.Forms.DataGridView();
            this.lbObjTypeText = new System.Windows.Forms.Label();
            this.btnPassItOn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.tbRangeEnd123 = new System.Windows.Forms.Label();
            this.tbTargetFileName = new System.Windows.Forms.TextBox();
            this.tbWorkSheetName = new System.Windows.Forms.TextBox();
            this.tbRangeBegin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbRangeEnd = new System.Windows.Forms.TextBox();
            this.WorkSheetName = new System.Windows.Forms.Label();
            this.btnLoadTargetFile = new System.Windows.Forms.Button();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnToggleMultiSelect = new System.Windows.Forms.Button();
            this.tbInfo = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgObjects)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgObjects
            // 
            this.dgObjects.AllowUserToAddRows = false;
            this.dgObjects.AllowUserToDeleteRows = false;
            this.dgObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgObjects.Location = new System.Drawing.Point(12, 68);
            this.dgObjects.MultiSelect = false;
            this.dgObjects.Name = "dgObjects";
            this.dgObjects.RowHeadersVisible = false;
            this.dgObjects.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgObjects.Size = new System.Drawing.Size(799, 464);
            this.dgObjects.TabIndex = 0;
            // 
            // lbObjTypeText
            // 
            this.lbObjTypeText.AutoSize = true;
            this.lbObjTypeText.Location = new System.Drawing.Point(12, 30);
            this.lbObjTypeText.Name = "lbObjTypeText";
            this.lbObjTypeText.Size = new System.Drawing.Size(35, 13);
            this.lbObjTypeText.TabIndex = 1;
            this.lbObjTypeText.Text = "label1";
            // 
            // btnPassItOn
            // 
            this.btnPassItOn.Location = new System.Drawing.Point(817, 68);
            this.btnPassItOn.Name = "btnPassItOn";
            this.btnPassItOn.Size = new System.Drawing.Size(245, 23);
            this.btnPassItOn.TabIndex = 38;
            this.btnPassItOn.Text = "Include>>";
            this.btnPassItOn.UseVisualStyleBackColor = true;
            this.btnPassItOn.Click += new System.EventHandler(this.btnPassItOn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnOpenFile);
            this.groupBox1.Controls.Add(this.tbRangeEnd123);
            this.groupBox1.Controls.Add(this.tbTargetFileName);
            this.groupBox1.Controls.Add(this.tbWorkSheetName);
            this.groupBox1.Controls.Add(this.tbRangeBegin);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbRangeEnd);
            this.groupBox1.Controls.Add(this.WorkSheetName);
            this.groupBox1.Location = new System.Drawing.Point(817, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(323, 190);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "OpenFileDialog";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Location = new System.Drawing.Point(6, 55);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFile.TabIndex = 0;
            this.btnOpenFile.Text = "Open";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // tbRangeEnd123
            // 
            this.tbRangeEnd123.AutoSize = true;
            this.tbRangeEnd123.Location = new System.Drawing.Point(6, 156);
            this.tbRangeEnd123.Name = "tbRangeEnd123";
            this.tbRangeEnd123.Size = new System.Drawing.Size(58, 13);
            this.tbRangeEnd123.TabIndex = 3;
            this.tbRangeEnd123.Text = "RangeEnd";
            // 
            // tbTargetFileName
            // 
            this.tbTargetFileName.Location = new System.Drawing.Point(6, 23);
            this.tbTargetFileName.Name = "tbTargetFileName";
            this.tbTargetFileName.Size = new System.Drawing.Size(311, 20);
            this.tbTargetFileName.TabIndex = 1;
            // 
            // tbWorkSheetName
            // 
            this.tbWorkSheetName.Location = new System.Drawing.Point(145, 101);
            this.tbWorkSheetName.Name = "tbWorkSheetName";
            this.tbWorkSheetName.Size = new System.Drawing.Size(100, 20);
            this.tbWorkSheetName.TabIndex = 2;
            // 
            // tbRangeBegin
            // 
            this.tbRangeBegin.Location = new System.Drawing.Point(145, 127);
            this.tbRangeBegin.Name = "tbRangeBegin";
            this.tbRangeBegin.Size = new System.Drawing.Size(100, 20);
            this.tbRangeBegin.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 130);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "RangeBegin";
            // 
            // tbRangeEnd
            // 
            this.tbRangeEnd.Location = new System.Drawing.Point(145, 153);
            this.tbRangeEnd.Name = "tbRangeEnd";
            this.tbRangeEnd.Size = new System.Drawing.Size(100, 20);
            this.tbRangeEnd.TabIndex = 2;
            // 
            // WorkSheetName
            // 
            this.WorkSheetName.AutoSize = true;
            this.WorkSheetName.Location = new System.Drawing.Point(6, 104);
            this.WorkSheetName.Name = "WorkSheetName";
            this.WorkSheetName.Size = new System.Drawing.Size(89, 13);
            this.WorkSheetName.TabIndex = 3;
            this.WorkSheetName.Text = "WorkSheetName";
            // 
            // btnLoadTargetFile
            // 
            this.btnLoadTargetFile.Location = new System.Drawing.Point(823, 315);
            this.btnLoadTargetFile.Name = "btnLoadTargetFile";
            this.btnLoadTargetFile.Size = new System.Drawing.Size(75, 23);
            this.btnLoadTargetFile.TabIndex = 36;
            this.btnLoadTargetFile.Text = "Load";
            this.btnLoadTargetFile.UseVisualStyleBackColor = true;
            this.btnLoadTargetFile.Click += new System.EventHandler(this.btnLoadTargetFile_Click);
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Location = new System.Drawing.Point(721, 30);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(42, 23);
            this.btnSelectNone.TabIndex = 39;
            this.btnSelectNone.Text = "--*-";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(670, 30);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(45, 23);
            this.btnSelectAll.TabIndex = 40;
            this.btnSelectAll.Text = "--*+";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            // 
            // btnToggleMultiSelect
            // 
            this.btnToggleMultiSelect.Location = new System.Drawing.Point(769, 30);
            this.btnToggleMultiSelect.Name = "btnToggleMultiSelect";
            this.btnToggleMultiSelect.Size = new System.Drawing.Size(42, 23);
            this.btnToggleMultiSelect.TabIndex = 41;
            this.btnToggleMultiSelect.Text = "--*";
            this.btnToggleMultiSelect.UseVisualStyleBackColor = true;
            // 
            // tbInfo
            // 
            this.tbInfo.Location = new System.Drawing.Point(826, 369);
            this.tbInfo.Multiline = true;
            this.tbInfo.Name = "tbInfo";
            this.tbInfo.ReadOnly = true;
            this.tbInfo.Size = new System.Drawing.Size(311, 163);
            this.tbInfo.TabIndex = 1;
            // 
            // ExcelObjectReaderFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1148, 581);
            this.Controls.Add(this.btnSelectNone);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.tbInfo);
            this.Controls.Add(this.btnToggleMultiSelect);
            this.Controls.Add(this.btnPassItOn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnLoadTargetFile);
            this.Controls.Add(this.lbObjTypeText);
            this.Controls.Add(this.dgObjects);
            this.Name = "ExcelObjectReaderFrm";
            this.Text = "Чтение объекта из Excel таблицы";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExcelObjectReaderFrm_FormClosing);
            this.Load += new System.EventHandler(this.ExcelObjectReaderFrm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgObjects)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgObjects;
        private System.Windows.Forms.Label lbObjTypeText;
        private System.Windows.Forms.Button btnPassItOn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Label tbRangeEnd123;
        private System.Windows.Forms.TextBox tbTargetFileName;
        private System.Windows.Forms.TextBox tbWorkSheetName;
        private System.Windows.Forms.TextBox tbRangeBegin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbRangeEnd;
        private System.Windows.Forms.Label WorkSheetName;
        private System.Windows.Forms.Button btnLoadTargetFile;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnToggleMultiSelect;
        private System.Windows.Forms.TextBox tbInfo;
    }
}