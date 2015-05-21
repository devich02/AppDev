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
    public partial class Form1 : Form
    {

        int GridSquareSize = 20;

        class Cell
        {

            public enum CellState
            {
                Alive,
                Dead
            }

            public CellState m_State = CellState.Dead;
            public CellState m_NextState = CellState.Dead;

            public Cell()
            {

            }


        }

        Cell[][] CellGrid;

        public Form1()
        {
            InitializeComponent();

            this.ClientSize = new Size(800, 700);
            CellGrid = new Cell[800 / GridSquareSize][];
            for (int i = 0; i < CellGrid.Length; ++i)
            {
                CellGrid[i] = new Cell[600 / GridSquareSize];
            }

            for (int i = 0; i < CellGrid.Length; ++i)
            {
                for ( int j = 0; j < CellGrid[i].Length; ++j)
                {
                    CellGrid[i][j] = new Cell();
                }
            }
        }

        Cell GetCell(int x, int y)
        {
            if (x < 0)
                x = CellGrid.Length + x;
            if (x >= CellGrid.Length)
                x -= CellGrid.Length;
            if (y < 0)
                y = CellGrid[x].Length + y;
            if (y >= CellGrid[x].Length)
                y -= CellGrid[x].Length;

            return CellGrid[x][y];
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        int mx = 0, my = 0;

        bool mdown = false;
        MouseButtons mbutton = MouseButtons.Left;
        bool brunning = false;
        bool bstepping = false;
        private long llIterationDelay = 0;
        long llLastUpdate = 0;

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(0, 100);

            int rx = mx / GridSquareSize;
            int ry = my / GridSquareSize;

            e.Graphics.FillRectangle(Brushes.Orange, rx * GridSquareSize, ry * GridSquareSize, GridSquareSize, GridSquareSize);

            if (mdown)
            {
                if (rx >= 0 && rx < CellGrid.Length &&
                    ry >= 0 && ry < CellGrid[rx].Length)
                {
                    if (mbutton == MouseButtons.Left)
                        CellGrid[rx][ry].m_State = Cell.CellState.Alive;
                    else
                        CellGrid[rx][ry].m_State = Cell.CellState.Dead;
                }
            }

            for (int i = 0; i < CellGrid.Length; ++i)
            {
                for (int j = 0; j < CellGrid[i].Length; ++j)
                {
                    if (CellGrid[i][j].m_State == Cell.CellState.Alive)
                    {
                        e.Graphics.FillRectangle(Brushes.LightGreen, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                    }
                }
            }

            for (int i = 0; i < this.Width; i += GridSquareSize)
            {
                e.Graphics.DrawLine(Pens.Black, i, 0, i, this.Height);
            }

            for (int i = 0; i < this.Height; i += GridSquareSize)
            {
                e.Graphics.DrawLine(Pens.Black, 0, i, this.Width, i);
            }

            if (brunning  && (DateTime.Now.Ticks - llLastUpdate) > llIterationDelay)
            {

                llLastUpdate = DateTime.Now.Ticks;

                for (int i = 0; i < CellGrid.Length; ++i)
                {
                    for (int j = 0; j < CellGrid[i].Length; ++j)
                    {
                        Cell top_left = GetCell(i - 1, j - 1);
                        Cell top = GetCell(i, j - 1);
                        Cell top_right = GetCell(i + 1, j - 1);

                        Cell right = GetCell(i + 1, j);

                        Cell bottom_right = GetCell(i + 1, j + 1);
                        Cell bottom = GetCell(i, j + 1);
                        Cell bottom_left = GetCell(i - 1, j + 1);

                        Cell left = GetCell(i - 1, j);

                        int count = 0;

                        count += (top_left != null && top_left.m_State == Cell.CellState.Alive) ? 1 : 0;
                        count += (top != null && top.m_State == Cell.CellState.Alive) ? 1 : 0;
                        count += (top_right != null && top_right.m_State == Cell.CellState.Alive) ? 1 : 0;

                        count += (right != null && right.m_State == Cell.CellState.Alive) ? 1 : 0;

                        count += (bottom_right != null && bottom_right.m_State == Cell.CellState.Alive) ? 1 : 0;
                        count += (bottom != null && bottom.m_State == Cell.CellState.Alive) ? 1 : 0;
                        count += (bottom_left != null && bottom_left.m_State == Cell.CellState.Alive) ? 1 : 0;

                        count += (left != null && left.m_State == Cell.CellState.Alive) ? 1 : 0;
                        
                        if ((count == 2 || count == 3) && CellGrid[i][j].m_State == Cell.CellState.Alive)
                        {
                            CellGrid[i][j].m_NextState = Cell.CellState.Alive;
                        }
                        else if (count < 2)
                        {
                            CellGrid[i][j].m_NextState = Cell.CellState.Dead;
                        }
                        else if (count > 3)
                        {
                            CellGrid[i][j].m_NextState = Cell.CellState.Dead;
                        }
                        else if (count == 3 && CellGrid[i][j].m_State == Cell.CellState.Dead)
                        {
                            CellGrid[i][j].m_NextState = Cell.CellState.Alive;
                        }
                     
                    }
                }


                for (int i =0; i < CellGrid.Length; ++i)
                {
                    for (int j = 0; j < CellGrid[i].Length; ++j)
                    {
                        CellGrid[i][j].m_State = CellGrid[i][j].m_NextState;
                    }
                }
            }

            if (bstepping && brunning)
            {
                brunning = false;
            }

            /*
            for (int i = 0; i < CellGrid.Length; ++i)
            {
                for (int j = 0; j < CellGrid[i].Length; ++j)
                {
                    Cell top_left = GetCell(i - 1, j - 1);
                    Cell top = GetCell(i, j - 1);
                    Cell top_right = GetCell(i + 1, j - 1);

                    Cell right = GetCell(i + 1, j);

                    Cell bottom_right = GetCell(i + 1, j + 1);
                    Cell bottom = GetCell(i, j + 1);
                    Cell bottom_left = GetCell(i - 1, j + 1);

                    Cell left = GetCell(i - 1, j);

                    int count = 0;

                    count += (top_left != null && top_left.m_State == Cell.CellState.Alive) ? 1 : 0;
                    count += (top != null && top.m_State == Cell.CellState.Alive) ? 1 : 0;
                    count += (top_right != null && top_right.m_State == Cell.CellState.Alive) ? 1 : 0;

                    count += (right != null && right.m_State == Cell.CellState.Alive) ? 1 : 0;

                    count += (bottom_right != null && bottom_right.m_State == Cell.CellState.Alive) ? 1 : 0;
                    count += (bottom != null && bottom.m_State == Cell.CellState.Alive) ? 1 : 0;
                    count += (bottom_left != null && bottom_left.m_State == Cell.CellState.Alive) ? 1 : 0;

                    count += (left != null && left.m_State == Cell.CellState.Alive) ? 1 : 0;

                    e.Graphics.DrawString("" + count, new Font("Courier New", 8), Brushes.Black, i * GridSquareSize, j * GridSquareSize);


                }
            }*/

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            mdown = true;
            mbutton = e.Button;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            mdown = false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (brunning)
                {
                    brunning = false;
                }
                else
                {
                    brunning = true;
                }
            }

            if (e.KeyCode == Keys.S)
            {
                if (bstepping)
                {
                    bstepping = false;
                }
                else
                {
                    bstepping = true;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            llIterationDelay = (long)(numericUpDown1.Value * 10000);
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            mx = e.X;
            my = e.Y - 100;
        }
    }
}
