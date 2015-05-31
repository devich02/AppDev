using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameOfLife
{
    public partial class Props : Form
    {
        public Props()
        {
            InitializeComponent();
        }

        public int IterationDelay {  get { return (int)numIterationDelay.Value * 10000; } }

        private void Props_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }
    }
}
