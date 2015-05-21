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

        public InfoView(Form1 parent)
        {
            InitializeComponent();
            HostParent = parent;
        }

        public void Update(int x, int y, Form1.Grid grid, Form1.Cell.CellType painter, int painterSize)
        {
            CellGrid = grid;

            switch (painter)
            {
                case Form1.Cell.CellType.Agent:
                    comboPainterType.Text = "Agent";
                    break;
                case Form1.Cell.CellType.Food:
                    comboPainterType.Text = "Food";
                    break;
                case Form1.Cell.CellType.Grass:
                    comboPainterType.Text = "Grass";
                    break;
                case Form1.Cell.CellType.Mud:
                    comboPainterType.Text = "Mud";
                    break;
                case Form1.Cell.CellType.Rock:
                    comboPainterType.Text = "Rock";
                    break;
                case Form1.Cell.CellType.Water:
                    comboPainterType.Text = "Water";
                    break;
            }

            lblCellLocation.Text = "Location: (" + x + ", " + y + ")";
            lblCellType.Text = "Type: " + grid[x, y].Type.ToString();
            lblResourceCount.Text = "Resource Count: " + grid[x, y].ResourceCount.ToString();
            lblReplenishRate.Text = "Replenish Rate: " + grid[x, y].ReplenishRate.ToString();
            lblResourceMax.Text = "Local resource max: " + grid[x, y].LocalMaximumSustainability.ToString();

            if (grid[x,y].Type == Form1.Cell.CellType.Agent)
            {
                lblAgentFaction.Text = "Faction: " + grid[x, y].HoldingAgent.AgentColor.ToString();
                lblAgentFood.Text = "Food: " + grid[x, y].HoldingAgent.FOOD.ToString();
                lblAgentWater.Text = "Water: " + grid[x, y].HoldingAgent.WATER.ToString();
                lblAgentEnergy.Text = "Energy: " + grid[x, y].HoldingAgent.ENERGY.ToString();
                lblGeneration.Text = "Generation: " + grid[x, y].HoldingAgent.Generation.ToString();
                lblAge.Text = "Age: " + grid[x, y].HoldingAgent.Age.ToString();

                ulong instructionCount = 0;
                Form1.Agent CurrentAgent = grid[x, y].HoldingAgent;

                for (int i = 0; i < CurrentAgent.Brain.Count; ++i)
                {
                    instructionCount += (ulong)CurrentAgent.Brain[i].Instructions.Length;
                }

                lblInstructionCount.Text = "Instruction Count: " + instructionCount.ToString();
                lblInstructionGroupCount.Text = "Instruction Groups: " + CurrentAgent.Brain.Count.ToString();
            }

            lblBrushSize.Text = "Brush size: " + painterSize;
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
                        bw.Write((int)(CellGrid[i, j].Type == Form1.Cell.CellType.Agent ? CellGrid[i, j].OldType : CellGrid[i, j].Type));
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
                        loadedCells[i, j].Type = (Form1.Cell.CellType)br.ReadInt32();
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
    }
}
