namespace RIFDC
{
    partial class HistoryManagerFrm
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
            this.dgObjectHistory = new System.Windows.Forms.DataGridView();
            this.btnCancelHistoryItem = new System.Windows.Forms.Button();
            this.btnDeleteHistory = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgObjectHistory)).BeginInit();
            this.SuspendLayout();
            // 
            // dgObjectHistory
            // 
            this.dgObjectHistory.AllowUserToAddRows = false;
            this.dgObjectHistory.AllowUserToDeleteRows = false;
            this.dgObjectHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgObjectHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgObjectHistory.Location = new System.Drawing.Point(2, 53);
            this.dgObjectHistory.MultiSelect = false;
            this.dgObjectHistory.Name = "dgObjectHistory";
            this.dgObjectHistory.RowHeadersVisible = false;
            this.dgObjectHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgObjectHistory.Size = new System.Drawing.Size(812, 398);
            this.dgObjectHistory.TabIndex = 0;
            this.dgObjectHistory.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgObjectHistory_CellContentClick);
            // 
            // btnCancelHistoryItem
            // 
            this.btnCancelHistoryItem.Location = new System.Drawing.Point(12, 12);
            this.btnCancelHistoryItem.Name = "btnCancelHistoryItem";
            this.btnCancelHistoryItem.Size = new System.Drawing.Size(218, 23);
            this.btnCancelHistoryItem.TabIndex = 1;
            this.btnCancelHistoryItem.Text = "Отменить последнее действие";
            this.btnCancelHistoryItem.UseVisualStyleBackColor = true;
            this.btnCancelHistoryItem.Click += new System.EventHandler(this.btnCancelHistoryItem_Click);
            // 
            // btnDeleteHistory
            // 
            this.btnDeleteHistory.Location = new System.Drawing.Point(236, 12);
            this.btnDeleteHistory.Name = "btnDeleteHistory";
            this.btnDeleteHistory.Size = new System.Drawing.Size(136, 23);
            this.btnDeleteHistory.TabIndex = 1;
            this.btnDeleteHistory.Text = "Очистить историю";
            this.btnDeleteHistory.UseVisualStyleBackColor = true;
            this.btnDeleteHistory.Click += new System.EventHandler(this.btnDeleteHistory_Click);
            // 
            // HistoryManagerFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 454);
            this.Controls.Add(this.btnDeleteHistory);
            this.Controls.Add(this.btnCancelHistoryItem);
            this.Controls.Add(this.dgObjectHistory);
            this.Name = "HistoryManagerFrm";
            this.Text = "HistoryManagerFrm";
            this.Load += new System.EventHandler(this.HistoryManagerFrm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgObjectHistory)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgObjectHistory;
        private System.Windows.Forms.Button btnCancelHistoryItem;
        private System.Windows.Forms.Button btnDeleteHistory;
    }
}