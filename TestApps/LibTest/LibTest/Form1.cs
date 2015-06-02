using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LibTest
{
    public partial class Form1 : Form
    {
        List<ILibTest> listLibraryTests = new List<ILibTest>();
        int iCurrentTest = 0;

        public Form1()
        {
            InitializeComponent();

            listLibraryTests.Add(new VecTest());

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            bool resultOutput = false;
            String resultString = listLibraryTests[iCurrentTest].LibTest(out resultOutput);

            if (resultOutput)
            {
                e.Graphics.DrawString(listLibraryTests[iCurrentTest].GetType().FullName + "\nLib tests: Passed", new Font("Segoe UI", 10), Brushes.Green, 10, 10);
            }
            else
            {
                e.Graphics.DrawString(listLibraryTests[iCurrentTest].GetType().FullName + "\nLib tests: Failed [" + resultString + "]", new Font("Segoe UI", 10), Brushes.Red, 10, 10);
            }

            listLibraryTests[iCurrentTest].PaintTest(e.Graphics, this.ClientRectangle);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
