namespace RIFDCComponents.ExcelTreeViewBasedObjectReaderComponent
{
    partial class ExcelTreeViewBasedObjectReader
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
            this.tvrObjects = new System.Windows.Forms.TreeView();
            this.btnSelectNone = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.btnToggleMultiSelect = new System.Windows.Forms.Button();
            this.btnPassItOn = new System.Windows.Forms.Button();
            this.tbRangeEnd123 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.WorkSheetName = new System.Windows.Forms.Label();
            this.dgObjects = new System.Windows.Forms.DataGridView();
            this.tbInfo = new System.Windows.Forms.TextBox();
            this.btnLoadTargetFile = new System.Windows.Forms.Button();
            this.lbObjTypeText = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnOpenFile = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbTargetFileName = new System.Windows.Forms.TextBox();
            this.tbWorkSheetName = new System.Windows.Forms.TextBox();
            this.tbRangeBegin = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbRangeEnd = new System.Windows.Forms.TextBox();
            this.btnResetSearch = new System.Windows.Forms.Button();
            this.btnGoSearch = new System.Windows.Forms.Button();
            this.tbSearchField = new System.Windows.Forms.TextBox();
            this.btnExpandAll = new System.Windows.Forms.Button();
            this.btnCollapseAll = new System.Windows.Forms.Button();
            this.cbxGridIsChild = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgObjects)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvrObjects
            // 
            this.tvrObjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tvrObjects.Location = new System.Drawing.Point(12, 52);
            this.tvrObjects.Name = "tvrObjects";
            this.tvrObjects.Size = new System.Drawing.Size(260, 649);
            this.tvrObjects.TabIndex = 0;
            // 
            // btnSelectNone
            // 
            this.btnSelectNone.Location = new System.Drawing.Point(815, 23);
            this.btnSelectNone.Name = "btnSelectNone";
            this.btnSelectNone.Size = new System.Drawing.Size(42, 23);
            this.btnSelectNone.TabIndex = 55;
            this.btnSelectNone.Text = "--*-";
            this.btnSelectNone.UseVisualStyleBackColor = true;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Location = new System.Drawing.Point(764, 23);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(45, 23);
            this.btnSelectAll.TabIndex = 56;
            this.btnSelectAll.Text = "--*+";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            // 
            // btnToggleMultiSelect
            // 
            this.btnToggleMultiSelect.Location = new System.Drawing.Point(863, 23);
            this.btnToggleMultiSelect.Name = "btnToggleMultiSelect";
            this.btnToggleMultiSelect.Size = new System.Drawing.Size(42, 23);
            this.btnToggleMultiSelect.TabIndex = 57;
            this.btnToggleMultiSelect.Text = "--*";
            this.btnToggleMultiSelect.UseVisualStyleBackColor = true;
            // 
            // btnPassItOn
            // 
            this.btnPassItOn.Location = new System.Drawing.Point(936, 52);
            this.btnPassItOn.Name = "btnPassItOn";
            this.btnPassItOn.Size = new System.Drawing.Size(292, 23);
            this.btnPassItOn.TabIndex = 54;
            this.btnPassItOn.Text = "Include>>";
            this.btnPassItOn.UseVisualStyleBackColor = true;
            this.btnPassItOn.Click += new System.EventHandler(this.btnPassItOn_Click);
            // 
            // tbRangeEnd123
            // 
            this.tbRangeEnd123.AutoSize = true;
            this.tbRangeEnd123.Location = new System.Drawing.Point(-165, 165);
            this.tbRangeEnd123.Name = "tbRangeEnd123";
            this.tbRangeEnd123.Size = new System.Drawing.Size(58, 13);
            this.tbRangeEnd123.TabIndex = 50;
            this.tbRangeEnd123.Text = "RangeEnd";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-165, 139);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 51;
            this.label1.Text = "RangeBegin";
            // 
            // WorkSheetName
            // 
            this.WorkSheetName.AutoSize = true;
            this.WorkSheetName.Location = new System.Drawing.Point(-165, 113);
            this.WorkSheetName.Name = "WorkSheetName";
            this.WorkSheetName.Size = new System.Drawing.Size(89, 13);
            this.WorkSheetName.TabIndex = 52;
            this.WorkSheetName.Text = "WorkSheetName";
            // 
            // dgObjects
            // 
            this.dgObjects.AllowUserToAddRows = false;
            this.dgObjects.AllowUserToDeleteRows = false;
            this.dgObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgObjects.Location = new System.Drawing.Point(279, 80);
            this.dgObjects.MultiSelect = false;
            this.dgObjects.Name = "dgObjects";
            this.dgObjects.RowHeadersVisible = false;
            this.dgObjects.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgObjects.Size = new System.Drawing.Size(635, 613);
            this.dgObjects.TabIndex = 43;
            // 
            // tbInfo
            // 
            this.tbInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbInfo.Location = new System.Drawing.Point(921, 353);
            this.tbInfo.Multiline = true;
            this.tbInfo.Name = "tbInfo";
            this.tbInfo.ReadOnly = true;
            this.tbInfo.Size = new System.Drawing.Size(335, 279);
            this.tbInfo.TabIndex = 45;
            // 
            // btnLoadTargetFile
            // 
            this.btnLoadTargetFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadTargetFile.Location = new System.Drawing.Point(927, 324);
            this.btnLoadTargetFile.Name = "btnLoadTargetFile";
            this.btnLoadTargetFile.Size = new System.Drawing.Size(75, 23);
            this.btnLoadTargetFile.TabIndex = 53;
            this.btnLoadTargetFile.Text = "Load";
            this.btnLoadTargetFile.UseVisualStyleBackColor = true;
            this.btnLoadTargetFile.Click += new System.EventHandler(this.btnLoadTargetFile_Click);
            // 
            // lbObjTypeText
            // 
            this.lbObjTypeText.AutoSize = true;
            this.lbObjTypeText.Location = new System.Drawing.Point(-159, 39);
            this.lbObjTypeText.Name = "lbObjTypeText";
            this.lbObjTypeText.Size = new System.Drawing.Size(35, 13);
            this.lbObjTypeText.TabIndex = 46;
            this.lbObjTypeText.Text = "label1";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnOpenFile);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbTargetFileName);
            this.groupBox1.Controls.Add(this.tbWorkSheetName);
            this.groupBox1.Controls.Add(this.tbRangeBegin);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.tbRangeEnd);
            this.groupBox1.Location = new System.Drawing.Point(921, 88);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(328, 190);
            this.groupBox1.TabIndex = 58;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "OpenFileDialog";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 120);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "WorkSheetName";
            // 
            // btnOpenFile
            // 
            this.btnOpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenFile.Location = new System.Drawing.Point(12, 69);
            this.btnOpenFile.Name = "btnOpenFile";
            this.btnOpenFile.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFile.TabIndex = 0;
            this.btnOpenFile.Text = "Open";
            this.btnOpenFile.UseVisualStyleBackColor = true;
            this.btnOpenFile.Click += new System.EventHandler(this.btnOpenFile_Click);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 174);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "RangeEnd";
            // 
            // tbTargetFileName
            // 
            this.tbTargetFileName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTargetFileName.Location = new System.Drawing.Point(12, 34);
            this.tbTargetFileName.Name = "tbTargetFileName";
            this.tbTargetFileName.Size = new System.Drawing.Size(295, 20);
            this.tbTargetFileName.TabIndex = 1;
            // 
            // tbWorkSheetName
            // 
            this.tbWorkSheetName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbWorkSheetName.Location = new System.Drawing.Point(107, 117);
            this.tbWorkSheetName.Name = "tbWorkSheetName";
            this.tbWorkSheetName.Size = new System.Drawing.Size(200, 20);
            this.tbWorkSheetName.TabIndex = 2;
            // 
            // tbRangeBegin
            // 
            this.tbRangeBegin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRangeBegin.Location = new System.Drawing.Point(107, 143);
            this.tbRangeBegin.Name = "tbRangeBegin";
            this.tbRangeBegin.Size = new System.Drawing.Size(200, 20);
            this.tbRangeBegin.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "RangeBegin";
            // 
            // tbRangeEnd
            // 
            this.tbRangeEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRangeEnd.Location = new System.Drawing.Point(107, 169);
            this.tbRangeEnd.Name = "tbRangeEnd";
            this.tbRangeEnd.Size = new System.Drawing.Size(200, 20);
            this.tbRangeEnd.TabIndex = 2;
            // 
            // btnResetSearch
            // 
            this.btnResetSearch.Location = new System.Drawing.Point(847, 52);
            this.btnResetSearch.Name = "btnResetSearch";
            this.btnResetSearch.Size = new System.Drawing.Size(64, 23);
            this.btnResetSearch.TabIndex = 60;
            this.btnResetSearch.Text = "Reset";
            this.btnResetSearch.UseVisualStyleBackColor = true;
            // 
            // btnGoSearch
            // 
            this.btnGoSearch.Location = new System.Drawing.Point(776, 52);
            this.btnGoSearch.Name = "btnGoSearch";
            this.btnGoSearch.Size = new System.Drawing.Size(65, 23);
            this.btnGoSearch.TabIndex = 61;
            this.btnGoSearch.Text = "Search";
            this.btnGoSearch.UseVisualStyleBackColor = true;
            // 
            // tbSearchField
            // 
            this.tbSearchField.Location = new System.Drawing.Point(548, 54);
            this.tbSearchField.Name = "tbSearchField";
            this.tbSearchField.Size = new System.Drawing.Size(222, 20);
            this.tbSearchField.TabIndex = 59;
            // 
            // btnExpandAll
            // 
            this.btnExpandAll.Location = new System.Drawing.Point(279, 12);
            this.btnExpandAll.Name = "btnExpandAll";
            this.btnExpandAll.Size = new System.Drawing.Size(69, 25);
            this.btnExpandAll.TabIndex = 62;
            this.btnExpandAll.Text = "Expand";
            this.btnExpandAll.UseVisualStyleBackColor = true;
            // 
            // btnCollapseAll
            // 
            this.btnCollapseAll.Location = new System.Drawing.Point(354, 12);
            this.btnCollapseAll.Name = "btnCollapseAll";
            this.btnCollapseAll.Size = new System.Drawing.Size(69, 25);
            this.btnCollapseAll.TabIndex = 62;
            this.btnCollapseAll.Text = "Collapse";
            this.btnCollapseAll.UseVisualStyleBackColor = true;
            // 
            // cbxGridIsChild
            // 
            this.cbxGridIsChild.AutoSize = true;
            this.cbxGridIsChild.Location = new System.Drawing.Point(279, 56);
            this.cbxGridIsChild.Name = "cbxGridIsChild";
            this.cbxGridIsChild.Size = new System.Drawing.Size(76, 17);
            this.cbxGridIsChild.TabIndex = 63;
            this.cbxGridIsChild.Text = "GridIsChild";
            this.cbxGridIsChild.UseVisualStyleBackColor = true;
            // 
            // ExcelTreeViewBasedObjectReader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1261, 713);
            this.Controls.Add(this.cbxGridIsChild);
            this.Controls.Add(this.btnCollapseAll);
            this.Controls.Add(this.btnExpandAll);
            this.Controls.Add(this.btnResetSearch);
            this.Controls.Add(this.btnGoSearch);
            this.Controls.Add(this.tbSearchField);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSelectNone);
            this.Controls.Add(this.btnSelectAll);
            this.Controls.Add(this.btnToggleMultiSelect);
            this.Controls.Add(this.btnPassItOn);
            this.Controls.Add(this.tbRangeEnd123);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.WorkSheetName);
            this.Controls.Add(this.dgObjects);
            this.Controls.Add(this.tbInfo);
            this.Controls.Add(this.btnLoadTargetFile);
            this.Controls.Add(this.lbObjTypeText);
            this.Controls.Add(this.tvrObjects);
            this.Name = "ExcelTreeViewBasedObjectReader";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ExcelTreeViewBasedObjectReader_FormClosing);
            this.Load += new System.EventHandler(this.ExcelTreeViewBasedObjectReader_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgObjects)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvrObjects;
        private System.Windows.Forms.Button btnSelectNone;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnToggleMultiSelect;
        private System.Windows.Forms.Button btnPassItOn;
        private System.Windows.Forms.Label tbRangeEnd123;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label WorkSheetName;
        private System.Windows.Forms.DataGridView dgObjects;
        private System.Windows.Forms.TextBox tbInfo;
        private System.Windows.Forms.Button btnLoadTargetFile;
        private System.Windows.Forms.Label lbObjTypeText;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbTargetFileName;
        private System.Windows.Forms.TextBox tbWorkSheetName;
        private System.Windows.Forms.TextBox tbRangeBegin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbRangeEnd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnResetSearch;
        private System.Windows.Forms.Button btnGoSearch;
        private System.Windows.Forms.TextBox tbSearchField;
        private System.Windows.Forms.Button btnExpandAll;
        private System.Windows.Forms.Button btnCollapseAll;
        private System.Windows.Forms.CheckBox cbxGridIsChild;
    }
}