using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CellularEvolution
{
    public partial class InfoView : Form
    {
        public InfoView()
        {
            InitializeComponent();
        }

        Form1 HostParent = null;

        Form1.Grid CellGrid = null;

        Form1.IAgent LastAgent = null;

        public InfoView(Form1 parent)
        {
            InitializeComponent();
            HostParent = parent;
        }

        public void Update(int x, int y, Form1.Grid grid, Form1.CellType painter, int painterSize)
        {
            CellGrid = grid;

            lblPopulation.Text = "Population: " + Form1.CurrentPopulationSize;

            switch (painter)
            {
                case Form1.CellType.Agent:
                    comboPainterType.Text = "Agent";
                    break;
                case Form1.CellType.Food:
                    comboPainterType.Text = "Food";
                    break;
                case Form1.CellType.Grass:
                    comboPainterType.Text = "Grass";
                    break;
                case Form1.CellType.Mud:
                    comboPainterType.Text = "Mud";
                    break;
                case Form1.CellType.Rock:
                    comboPainterType.Text = "Rock";
                    break;
                case Form1.CellType.Water:
                    comboPainterType.Text = "Water";
                    break;
            }

            lblCellLocation.Text = "Location: (" + x + ", " + y + ")";
            lblCellType.Text = "Type: " + grid[x, y].Type.ToString();
            lblResourceCount.Text = "Resource Count: " + grid[x, y].ResourceCount.ToString();
            lblReplenishRate.Text = "Replenish Rate: " + grid[x, y].ReplenishRate.ToString();
            lblResourceMax.Text = "Local resource max: " + grid[x, y].LocalMaximumSustainability.ToString();

            if (grid[x,y].Type == Form1.CellType.Agent)
            {
                lblAgentInfo.Text = grid[x, y].HoldingAgent.GetStats();

                if (LastAgent != grid[x, y].HoldingAgent)
                {
                    LastAgent = null;
                    chkTrackThisAgent.Checked = grid[x, y].HoldingAgent.GetTracking();
                }
                LastAgent = grid[x, y].HoldingAgent;
            }

            lblBrushSize.Text = "Brush size: " + painterSize;
            lblIteration.Text = "Iteration: " + HostParent.IterationIndex;
        }

        private void btnSaveMap_Click(object sender, EventArgs e)
        {
            if (CellGrid == null)
            {
                MessageBox.Show("No map to save");
            }

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                BinaryWriter bw = new BinaryWriter(sw.BaseStream);
                bw.Write(CellGrid.Width);
                bw.Write(CellGrid.Height);
                for (int i = 0; i < CellGrid.Width; ++i)
                {
                    for (int j = 0; j < CellGrid.Height; ++j)
                    {
                        bw.Write((int)(CellGrid[i, j].Type == Form1.CellType.Agent ? CellGrid[i, j].OldType : CellGrid[i, j].Type));
                    }
                }
                sw.Close();
                sw.Dispose();
            }
        }

        private void btnLoadMap_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                BinaryReader br = new BinaryReader(new StreamReader(openFileDialog1.FileName).BaseStream);

                int width = br.ReadInt32();
                int height = br.ReadInt32();

                Form1.Grid loadedCells = new Form1.Grid(width, height);
                for (int i = 0; i < width; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        loadedCells[i, j].Type = (Form1.CellType)br.ReadInt32();
                    }
                }

                HostParent.CellGrid = loadedCells;

                br.Close();
                br.Dispose();
            }
        }

        public int GetIterationsPerVisualRefresh()
        {
            return (int)numericUpDown1.Value;
        }

        public bool InsertRandomAgents()
        {
            return (bool)chkRandomAgents.Checked;
        }

        public bool TrackAgents()
        {
            return (bool)chkTracking.Checked;
        }

        public void Log(String s)
        {
            txtLog.Text += s + "\n";
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }

        private void chkTrackThisAgent_CheckedChanged(object sender, EventArgs e)
        {
            if (LastAgent != null)
            {
                LastAgent.SetTracking(chkTrackThisAgent.Checked);
            }
        }
    }
}
