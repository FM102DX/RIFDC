namespace CoffeePointsDemo
{
    partial class MainAppFrm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.объектыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.кофейныеТочкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.объектыToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // объектыToolStripMenuItem
            // 
            this.объектыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.кофейныеТочкиToolStripMenuItem});
            this.объектыToolStripMenuItem.Name = "объектыToolStripMenuItem";
            this.объектыToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.объектыToolStripMenuItem.Text = "Объекты";
            // 
            // кофейныеТочкиToolStripMenuItem
            // 
            this.кофейныеТочкиToolStripMenuItem.Name = "кофейныеТочкиToolStripMenuItem";
            this.кофейныеТочкиToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.кофейныеТочкиToolStripMenuItem.Text = "Кофейные точки";
            this.кофейныеТочкиToolStripMenuItem.Click += new System.EventHandler(this.кофейныеТочкиToolStripMenuItem_Click);
            // 
            // MainAppFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainAppFrm";
            this.Text = "CoffeePointsDemo";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem объектыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem кофейныеТочкиToolStripMenuItem;
    }
}

