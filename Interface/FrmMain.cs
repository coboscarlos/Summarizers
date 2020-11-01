using System;
using System.Windows.Forms;

namespace Interface
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void algorithmsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var myFrmExecuteAlgorithm = new FrmExecuteAlgorithm { MdiParent = this };
            myFrmExecuteAlgorithm.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}