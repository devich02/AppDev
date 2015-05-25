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

        const int GridSquareSize = 20;

        class Cell
        {

            public enum CellState
            {
                Alive,
                Dead
            }

            public CellState m_State = CellState.Dead;
            public CellState m_NextState = CellState.Dead;

            public bool m_ShipHere = false; // if m_ShipHere, always count as alive and don't update (not used yet)

            public Cell()
            {

            }


        }

        class Rule
        {
            public Cell.CellState[][] RuleBehavior; // bool[2][9] with 0,1 -> aliveness, and 0-8 possible neighbor counts
            /* RuleBehavior[0][i] indicates aliveness corresponding to i neighbors around a dead cell.
             * RuleBehavior[1][i] indicates aliveness corresponding to i neighbors around a live cell.
             * in other words,
             * 0 <--> dead
             * 1 <--> alive
             */

            public Rule()
            {
                // default to Game of Life for now

                // better way to do this?
                this.RuleBehavior = new Cell.CellState[2][];
                this.RuleBehavior[0] = new Cell.CellState[9];
                this.RuleBehavior[1] = new Cell.CellState[9];

                SetRuleFromClassic(2, 3, 3, 3);
            }

            public Rule(int a,int b,int c,int d)
            {
                // better way to do this?
                this.RuleBehavior = new Cell.CellState[2][];
                this.RuleBehavior[0] = new Cell.CellState[9];
                this.RuleBehavior[1] = new Cell.CellState[9];

                SetRuleFromClassic(a, b, c, d);
            }

            public void SetRuleFromClassic(int a, int b, int c, int d)
            {
                /* for a <= N <= b, alive cell stays alive
                 * otherwise it dies.
                 * for c <= N <= d, dead cell comes alive
                 * otherwise it stays dead.
                 * "Classic" refers to wxyz Environment/Fertility rule model
                 */

                for (int i = 0; i < 9; i++)
                {
                    this.RuleBehavior[0][i] = (i >= c && i <= d) ? Cell.CellState.Alive : Cell.CellState.Dead; // currently dead updates
                    this.RuleBehavior[1][i] = (i >= a && i <= b) ? Cell.CellState.Alive : Cell.CellState.Dead; // currently alive updates
                }

            }

            /* should implement custom rule creation (i.e., anything other than classic) here */

            public Cell.CellState GetNewCellStateFromRule(int numNeighbors, Cell.CellState alive)
            {
                /* interface between cell and rule when updating cells */
                return (alive == Cell.CellState.Alive) ? RuleBehavior[1][numNeighbors] : RuleBehavior[0][numNeighbors];
            }
        }

        class Ship
        {

            public enum ShipState
            {
                Alive,
                Dead
            }

            public ShipState m_State = ShipState.Dead;
            public ShipState m_NextState = ShipState.Dead;
            public int[] m_Pos; // (x,y) position of center of ship

            public Ship()
            {
                /* define default initial ship position */
                this.m_Pos = new int[2] {10,10}; 
            }

            public Ship(int a, int b)
            {
                /* set initial ship position */
                this.m_Pos = new int[2] { a, b };
            }


        }

        Cell[][] CellGrid;
        Ship PlayerShip;
        Rule R;

        public Form1()
        {
            InitializeComponent();

            R = new Rule(); // this is the default constructor --> Game of Life Rule

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

            // initialize ship
            PlayerShip = new Ship(20,20);
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

            // Draw Ship
            e.Graphics.FillRectangle(Brushes.Black, PlayerShip.m_Pos[0] * GridSquareSize, PlayerShip.m_Pos[1] * GridSquareSize, GridSquareSize, GridSquareSize);

            ////////////////////////////////
            // Game State Update
            ////////////////////////////////
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

                        CellGrid[i][j].m_NextState = R.GetNewCellStateFromRule(count, CellGrid[i][j].m_State);
                        
                        /*
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
                        */
                     
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

            if (e.KeyCode == Keys.T)
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

            if (e.KeyCode == Keys.A)
            {
                PlayerShip.m_Pos[0]--;
            }
            if (e.KeyCode == Keys.D)
            {
                PlayerShip.m_Pos[0]++;
            }
            if (e.KeyCode == Keys.W)
            {
                PlayerShip.m_Pos[1]--;
            }
            if (e.KeyCode == Keys.S)
            {
                PlayerShip.m_Pos[1]++;
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
