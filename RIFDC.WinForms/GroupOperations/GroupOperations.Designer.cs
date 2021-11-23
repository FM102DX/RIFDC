namespace RIFDC
{
    partial class GroupOperationsFrm
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
            this.label1 = new System.Windows.Forms.Label();
            this.grpOperationSetValue = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cbxSetValueParameter = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnDoAction = new System.Windows.Forms.Button();
            this.ibTargetObjects = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.grpOperationSetValue.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(212, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Исходное моножество бизнес-объектов";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // grpOperationSetValue
            // 
            this.grpOperationSetValue.Controls.Add(this.label3);
            this.grpOperationSetValue.Controls.Add(this.label2);
            this.grpOperationSetValue.Controls.Add(this.cbxSetValueParameter);
            this.grpOperationSetValue.Location = new System.Drawing.Point(4, 22);
            this.grpOperationSetValue.Name = "grpOperationSetValue";
            this.grpOperationSetValue.Padding = new System.Windows.Forms.Padding(3);
            this.grpOperationSetValue.Size = new System.Drawing.Size(566, 239);
            this.grpOperationSetValue.TabIndex = 0;
            this.grpOperationSetValue.Text = "Присвоение значения";
            this.grpOperationSetValue.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Присвоить значение";
            this.label3.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Параметр";
            this.label2.Click += new System.EventHandler(this.label1_Click);
            // 
            // cbxSetValueParameter
            // 
            this.cbxSetValueParameter.FormattingEnabled = true;
            this.cbxSetValueParameter.Location = new System.Drawing.Point(19, 30);
            this.cbxSetValueParameter.Name = "cbxSetValueParameter";
            this.cbxSetValueParameter.Size = new System.Drawing.Size(341, 21);
            this.cbxSetValueParameter.TabIndex = 2;
            this.cbxSetValueParameter.SelectedIndexChanged += new System.EventHandler(this.cbxSetValueParameter_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.grpOperationSetValue);
            this.tabControl1.Location = new System.Drawing.Point(12, 76);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(574, 265);
            this.tabControl1.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(507, 347);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnDoAction
            // 
            this.btnDoAction.Location = new System.Drawing.Point(426, 347);
            this.btnDoAction.Name = "btnDoAction";
            this.btnDoAction.Size = new System.Drawing.Size(75, 23);
            this.btnDoAction.TabIndex = 2;
            this.btnDoAction.Text = "Выполнить";
            this.btnDoAction.UseVisualStyleBackColor = true;
            this.btnDoAction.Click += new System.EventHandler(this.btnDoAction_Click);
            // 
            // ibTargetObjects
            // 
            this.ibTargetObjects.AutoSize = true;
            this.ibTargetObjects.Location = new System.Drawing.Point(12, 40);
            this.ibTargetObjects.Name = "ibTargetObjects";
            this.ibTargetObjects.Size = new System.Drawing.Size(61, 13);
            this.ibTargetObjects.TabIndex = 1;
            this.ibTargetObjects.Text = "sampleText";
            this.ibTargetObjects.Click += new System.EventHandler(this.label1_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(507, 30);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // GroupOperationsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1140, 663);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnDoAction);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.ibTargetObjects);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.Name = "GroupOperationsFrm";
            this.Text = "Групповые операции";
            this.Load += new System.EventHandler(this.frmMakeGroupOperation_Load);
            this.grpOperationSetValue.ResumeLayout(false);
            this.grpOperationSetValue.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage grpOperationSetValue;
        private System.Windows.Forms.ComboBox cbxSetValueParameter;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnDoAction;
        private System.Windows.Forms.Label ibTargetObjects;
        private System.Windows.Forms.Button button1;
    }
}