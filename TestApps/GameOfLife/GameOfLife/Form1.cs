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

        Props Properties = new Props();

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

            public Cell(){}
        }

        class Grid
        {
            private Cell[,] m_GridCells;
            private int m_Width;
            private int m_Height;

            public int Width {  get { return m_Width; } }
            public int Height {  get { return m_Height; } }

            private delegate Cell GetCellInDirection(int i, int j);

            public Cell TopLeft(int i, int j) { return this[i - 1, j - 1]; }
            public Cell Top(int i, int j) { return this[i, j - 1]; }
            public Cell TopRight(int i, int j) { return this[i + 1, j - 1]; }
            public Cell Right(int i, int j) { return this[i + 1, j]; }
            public Cell BottomRight(int i, int j) { return this[i + 1, j + 1]; }
            public Cell Bottom(int i, int j) { return this[i, j + 1]; }
            public Cell BottomLeft(int i, int j) { return this[i - 1, j + 1]; }
            public Cell Left(int i, int j) { return this[i - 1, j]; }

            private GetCellInDirection[] m_AllDirections;

            public Grid() { }
            public Grid(int width, int height)
            {
                m_AllDirections = new GetCellInDirection[]
                {
                    TopLeft,
                    Top,
                    TopRight,
                    Right,
                    BottomRight,
                    Bottom,
                    BottomLeft,
                    Left
                };

                m_Width = width;
                m_Height = height;

                m_GridCells = new Cell[width, height];
                for (int i = 0; i < width; ++i)
                {
                    for (int j = 0; j < height; ++j)
                    {
                        m_GridCells[i, j] = new Cell();
                    }
                }
            }

            private int WrapMod(int x, int m)
            {
                int r = x % m;
                return r < 0 ? r + m : r;
            }
            public Cell this[int i, int j]
            {
                get
                {
                    return m_GridCells[WrapMod(i, m_Width), WrapMod(j, m_Height)];
                }
            }

            public int AliveNeighbors(int i, int j)
            {
                int aliveCount = 0;
                for (int k = 0; k < m_AllDirections.Length; ++k)
                {
                    aliveCount += m_AllDirections[k](i, j).m_State == Cell.CellState.Alive ? 1 : 0;
                }
                return aliveCount;
            }
        }

        class Rule
        {
            public Cell.CellState[][] m_RuleBehavior; // bool[2][9] with 0,1 -> aliveness, and 0-8 possible neighbor counts
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
                this.m_RuleBehavior = new Cell.CellState[2][];
                this.m_RuleBehavior[0] = new Cell.CellState[9];
                this.m_RuleBehavior[1] = new Cell.CellState[9];

                SetRuleFromClassic(2, 3, 3, 3);
            }

            public Rule(int a,int b,int c,int d)
            {
                // better way to do this?
                this.m_RuleBehavior = new Cell.CellState[2][];
                this.m_RuleBehavior[0] = new Cell.CellState[9];
                this.m_RuleBehavior[1] = new Cell.CellState[9];

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
                    this.m_RuleBehavior[0][i] = (i >= c && i <= d) ? Cell.CellState.Alive : Cell.CellState.Dead; // currently dead updates
                    this.m_RuleBehavior[1][i] = (i >= a && i <= b) ? Cell.CellState.Alive : Cell.CellState.Dead; // currently alive updates
                }

            }

            /* should implement custom rule creation (i.e., anything other than classic) here */

            public Cell.CellState GetNewCellStateFromRule(int numNeighbors, Cell.CellState alive)
            {
                /* interface between cell and rule when updating cells */
                return (alive == Cell.CellState.Alive) ? m_RuleBehavior[1][numNeighbors] : m_RuleBehavior[0][numNeighbors];
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

        Grid CellGrid;
        Ship PlayerShip;
        Rule R;

        public Form1()
        {
            InitializeComponent();

            R = new Rule(); // this is the default constructor --> Game of Life Rule

            this.ClientSize = new Size(800, 600);
            CellGrid = new Grid(800 / GridSquareSize, 600 / GridSquareSize);

            // initialize ship
            PlayerShip = new Ship(20,20);
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
        long llLastUpdate = 0;

        bool[] KeysDown = new bool[256];

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            int rx = mx / GridSquareSize;
            int ry = my / GridSquareSize;

            e.Graphics.FillRectangle(Brushes.Orange, rx * GridSquareSize, ry * GridSquareSize, GridSquareSize, GridSquareSize);

            if (mdown)
            {
                if (mbutton == MouseButtons.Left)
                    CellGrid[rx, ry].m_State = Cell.CellState.Alive;
                else
                    CellGrid[rx, ry].m_State = Cell.CellState.Dead;
            }

            for (int i = 0; i < CellGrid.Width; ++i)
            {
                for (int j = 0; j < CellGrid.Height; ++j)
                {
                    if (CellGrid[i, j].m_State == Cell.CellState.Alive)
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
            if (brunning  && (DateTime.Now.Ticks - llLastUpdate) > Properties.IterationDelay)
            {
                llLastUpdate = DateTime.Now.Ticks;

                for (int i = 0; i < CellGrid.Width; ++i)
                {
                    for (int j = 0; j < CellGrid.Height; ++j)
                    {
                        CellGrid[i, j].m_NextState = R.GetNewCellStateFromRule(CellGrid.AliveNeighbors(i, j), CellGrid[i, j].m_State);
                    }
                }

                for (int i =0; i < CellGrid.Width; ++i)
                {
                    for (int j = 0; j < CellGrid.Height; ++j)
                    {
                        CellGrid[i, j].m_State = CellGrid[i, j].m_NextState;
                    }
                }
            }

            if (bstepping && brunning)
            {
                brunning = false;
            }



            if (KeysDown[(int)Keys.A])
            {
                PlayerShip.m_Pos[0]--;
            }
            if (KeysDown[(int)Keys.D])
            {
                PlayerShip.m_Pos[0]++;
            }
            if (KeysDown[(int)Keys.W])
            {
                PlayerShip.m_Pos[1]--;
            }
            if (KeysDown[(int)Keys.S])
            {
                PlayerShip.m_Pos[1]++;
            }
            
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

            KeysDown[(int)e.KeyCode] = true;

            if (e.KeyCode == Keys.I)
            {
                Properties.Show();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            KeysDown[(int)e.KeyCode] = false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            mx = e.X;
            my = e.Y;
        }
    }
}
