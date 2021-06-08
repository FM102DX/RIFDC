namespace CoffeePointsDemo
{
    partial class CoffeePointsFrm
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
            this.dgCoffeePoints = new System.Windows.Forms.DataGridView();
            this.btnReload = new System.Windows.Forms.Button();
            this.btnAddNewRecord = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.tbId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbCoffeePointName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tbComment = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbLastVisitDate = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbBigLattePrice = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.tbAlias = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbBigLattePrice2 = new System.Windows.Forms.TextBox();
            this.tbComment2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.tbLastVisitDate2 = new System.Windows.Forms.MaskedTextBox();
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnCancelSearch = new System.Windows.Forms.Button();
            this.btnGroupOperations = new System.Windows.Forms.Button();
            this.btnSpecLineSelectNone = new System.Windows.Forms.Button();
            this.btnToggleSpecLineMultiSelect = new System.Windows.Forms.Button();
            this.btnSpecLineSelectAll = new System.Windows.Forms.Button();
            this.btnHistory = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgCoffeePoints)).BeginInit();
            this.SuspendLayout();
            // 
            // dgCoffeePoints
            // 
            this.dgCoffeePoints.AllowUserToAddRows = false;
            this.dgCoffeePoints.AllowUserToDeleteRows = false;
            this.dgCoffeePoints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgCoffeePoints.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgCoffeePoints.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
            this.dgCoffeePoints.Location = new System.Drawing.Point(12, 42);
            this.dgCoffeePoints.MultiSelect = false;
            this.dgCoffeePoints.Name = "dgCoffeePoints";
            this.dgCoffeePoints.RowHeadersVisible = false;
            this.dgCoffeePoints.Size = new System.Drawing.Size(947, 717);
            this.dgCoffeePoints.TabIndex = 0;
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(211, 13);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(65, 23);
            this.btnReload.TabIndex = 44;
            this.btnReload.Text = "Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            // 
            // btnAddNewRecord
            // 
            this.btnAddNewRecord.Location = new System.Drawing.Point(12, 12);
            this.btnAddNewRecord.Name = "btnAddNewRecord";
            this.btnAddNewRecord.Size = new System.Drawing.Size(44, 23);
            this.btnAddNewRecord.TabIndex = 42;
            this.btnAddNewRecord.Text = "+";
            this.btnAddNewRecord.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(62, 12);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(44, 23);
            this.btnDelete.TabIndex = 43;
            this.btnDelete.Text = "-";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnPrevious
            // 
            this.btnPrevious.Location = new System.Drawing.Point(112, 13);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(42, 23);
            this.btnPrevious.TabIndex = 41;
            this.btnPrevious.Text = "<<";
            this.btnPrevious.UseVisualStyleBackColor = true;
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(160, 13);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(45, 23);
            this.btnNext.TabIndex = 40;
            this.btnNext.Text = ">>";
            this.btnNext.UseVisualStyleBackColor = true;
            // 
            // tbId
            // 
            this.tbId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbId.Location = new System.Drawing.Point(1005, 42);
            this.tbId.Name = "tbId";
            this.tbId.ReadOnly = true;
            this.tbId.Size = new System.Drawing.Size(245, 20);
            this.tbId.TabIndex = 45;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1004, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 13);
            this.label1.TabIndex = 46;
            this.label1.Text = "Id";
            // 
            // tbCoffeePointName
            // 
            this.tbCoffeePointName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbCoffeePointName.Location = new System.Drawing.Point(1005, 86);
            this.tbCoffeePointName.Name = "tbCoffeePointName";
            this.tbCoffeePointName.Size = new System.Drawing.Size(245, 20);
            this.tbCoffeePointName.TabIndex = 45;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1004, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 46;
            this.label2.Text = "Название";
            // 
            // tbComment
            // 
            this.tbComment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbComment.Location = new System.Drawing.Point(1005, 304);
            this.tbComment.Multiline = true;
            this.tbComment.Name = "tbComment";
            this.tbComment.Size = new System.Drawing.Size(245, 49);
            this.tbComment.TabIndex = 45;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1004, 287);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 46;
            this.label3.Text = "Комментарий";
            // 
            // tbLastVisitDate
            // 
            this.tbLastVisitDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLastVisitDate.Location = new System.Drawing.Point(1009, 189);
            this.tbLastVisitDate.Mask = "00/00/0000";
            this.tbLastVisitDate.Name = "tbLastVisitDate";
            this.tbLastVisitDate.Size = new System.Drawing.Size(115, 20);
            this.tbLastVisitDate.TabIndex = 48;
            this.tbLastVisitDate.ValidatingType = typeof(System.DateTime);
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1006, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(155, 13);
            this.label7.TabIndex = 47;
            this.label7.Text = "Дата последнего посещения";
            // 
            // tbBigLattePrice
            // 
            this.tbBigLattePrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBigLattePrice.Location = new System.Drawing.Point(1009, 241);
            this.tbBigLattePrice.Name = "tbBigLattePrice";
            this.tbBigLattePrice.Size = new System.Drawing.Size(115, 20);
            this.tbBigLattePrice.TabIndex = 45;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1008, 225);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 13);
            this.label4.TabIndex = 46;
            this.label4.Text = "Цена большого латте";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(282, 13);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(60, 23);
            this.btnSave.TabIndex = 49;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // tbAlias
            // 
            this.tbAlias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAlias.Location = new System.Drawing.Point(1005, 131);
            this.tbAlias.Name = "tbAlias";
            this.tbAlias.Size = new System.Drawing.Size(245, 20);
            this.tbAlias.TabIndex = 45;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(1004, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 46;
            this.label5.Text = "Алиас";
            // 
            // tbBigLattePrice2
            // 
            this.tbBigLattePrice2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbBigLattePrice2.Location = new System.Drawing.Point(1009, 514);
            this.tbBigLattePrice2.Name = "tbBigLattePrice2";
            this.tbBigLattePrice2.Size = new System.Drawing.Size(115, 20);
            this.tbBigLattePrice2.TabIndex = 45;
            // 
            // tbComment2
            // 
            this.tbComment2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbComment2.Location = new System.Drawing.Point(1008, 565);
            this.tbComment2.Multiline = true;
            this.tbComment2.Name = "tbComment2";
            this.tbComment2.Size = new System.Drawing.Size(245, 54);
            this.tbComment2.TabIndex = 45;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(1006, 498);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 13);
            this.label6.TabIndex = 46;
            this.label6.Text = "Цена большого латте-2";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(1007, 549);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(86, 13);
            this.label8.TabIndex = 46;
            this.label8.Text = "Комментарий-2";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(1006, 445);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(164, 13);
            this.label9.TabIndex = 47;
            this.label9.Text = "Дата последнего посещения-2";
            // 
            // tbLastVisitDate2
            // 
            this.tbLastVisitDate2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbLastVisitDate2.Location = new System.Drawing.Point(1009, 462);
            this.tbLastVisitDate2.Mask = "00/00/0000";
            this.tbLastVisitDate2.Name = "tbLastVisitDate2";
            this.tbLastVisitDate2.Size = new System.Drawing.Size(115, 20);
            this.tbLastVisitDate2.TabIndex = 48;
            this.tbLastVisitDate2.ValidatingType = typeof(System.DateTime);
            // 
            // tbSearch
            // 
            this.tbSearch.Location = new System.Drawing.Point(705, 13);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(136, 20);
            this.tbSearch.TabIndex = 50;
            this.tbSearch.Visible = false;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(847, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(57, 23);
            this.btnSearch.TabIndex = 51;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Visible = false;
            // 
            // btnCancelSearch
            // 
            this.btnCancelSearch.Location = new System.Drawing.Point(907, 12);
            this.btnCancelSearch.Name = "btnCancelSearch";
            this.btnCancelSearch.Size = new System.Drawing.Size(52, 23);
            this.btnCancelSearch.TabIndex = 51;
            this.btnCancelSearch.Text = "Cancel";
            this.btnCancelSearch.UseVisualStyleBackColor = true;
            this.btnCancelSearch.Visible = false;
            // 
            // btnGroupOperations
            // 
            this.btnGroupOperations.Location = new System.Drawing.Point(1208, 12);
            this.btnGroupOperations.Name = "btnGroupOperations";
            this.btnGroupOperations.Size = new System.Drawing.Size(45, 23);
            this.btnGroupOperations.TabIndex = 49;
            this.btnGroupOperations.Text = "[...]";
            this.btnGroupOperations.UseVisualStyleBackColor = true;
            this.btnGroupOperations.Visible = false;
            // 
            // btnSpecLineSelectNone
            // 
            this.btnSpecLineSelectNone.Location = new System.Drawing.Point(458, 13);
            this.btnSpecLineSelectNone.Name = "btnSpecLineSelectNone";
            this.btnSpecLineSelectNone.Size = new System.Drawing.Size(40, 23);
            this.btnSpecLineSelectNone.TabIndex = 52;
            this.btnSpecLineSelectNone.Text = "*--";
            this.btnSpecLineSelectNone.UseVisualStyleBackColor = true;
            // 
            // btnToggleSpecLineMultiSelect
            // 
            this.btnToggleSpecLineMultiSelect.Location = new System.Drawing.Point(373, 13);
            this.btnToggleSpecLineMultiSelect.Name = "btnToggleSpecLineMultiSelect";
            this.btnToggleSpecLineMultiSelect.Size = new System.Drawing.Size(37, 23);
            this.btnToggleSpecLineMultiSelect.TabIndex = 53;
            this.btnToggleSpecLineMultiSelect.Text = "***";
            this.btnToggleSpecLineMultiSelect.UseVisualStyleBackColor = true;
            // 
            // btnSpecLineSelectAll
            // 
            this.btnSpecLineSelectAll.Location = new System.Drawing.Point(416, 13);
            this.btnSpecLineSelectAll.Name = "btnSpecLineSelectAll";
            this.btnSpecLineSelectAll.Size = new System.Drawing.Size(37, 23);
            this.btnSpecLineSelectAll.TabIndex = 54;
            this.btnSpecLineSelectAll.Text = "*+";
            this.btnSpecLineSelectAll.UseVisualStyleBackColor = true;
            // 
            // btnHistory
            // 
            this.btnHistory.Location = new System.Drawing.Point(548, 13);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(65, 23);
            this.btnHistory.TabIndex = 52;
            this.btnHistory.Text = "[History]";
            this.btnHistory.UseVisualStyleBackColor = true;
            // 
            // CoffeePointsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1279, 771);
            this.Controls.Add(this.btnHistory);
            this.Controls.Add(this.btnSpecLineSelectNone);
            this.Controls.Add(this.btnToggleSpecLineMultiSelect);
            this.Controls.Add(this.btnSpecLineSelectAll);
            this.Controls.Add(this.btnCancelSearch);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.tbSearch);
            this.Controls.Add(this.btnGroupOperations);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.tbLastVisitDate2);
            this.Controls.Add(this.tbLastVisitDate);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbComment2);
            this.Controls.Add(this.tbBigLattePrice2);
            this.Controls.Add(this.tbComment);
            this.Controls.Add(this.tbBigLattePrice);
            this.Controls.Add(this.tbAlias);
            this.Controls.Add(this.tbCoffeePointName);
            this.Controls.Add(this.tbId);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnAddNewRecord);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.dgCoffeePoints);
            this.KeyPreview = true;
            this.Name = "CoffeePointsFrm";
            this.Text = "CoffeePoints";
            this.Load += new System.EventHandler(this.CoffeePointsFrm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgCoffeePoints)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgCoffeePoints;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Button btnAddNewRecord;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.TextBox tbId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbCoffeePointName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbComment;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.MaskedTextBox tbLastVisitDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbBigLattePrice;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TextBox tbAlias;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tbBigLattePrice2;
        private System.Windows.Forms.TextBox tbComment2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.MaskedTextBox tbLastVisitDate2;
        private System.Windows.Forms.TextBox tbSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancelSearch;
        private System.Windows.Forms.Button btnGroupOperations;
        private System.Windows.Forms.Button btnSpecLineSelectNone;
        private System.Windows.Forms.Button btnToggleSpecLineMultiSelect;
        private System.Windows.Forms.Button btnSpecLineSelectAll;
        private System.Windows.Forms.Button btnHistory;
    }
}