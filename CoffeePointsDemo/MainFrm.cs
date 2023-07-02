using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonFunctions;

namespace CoffeePointsDemo
{
    public partial class MainAppFrm : Form
    {
        public MainAppFrm()
        {
            InitializeComponent();
        }

        private void кофейныеТочкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CoffeePointsFrm frm = new CoffeePointsFrm();
            frm.WindowState = FormWindowState.Maximized;
            frm.MdiParent = this;
            frm.Show();
        }
    }
}
