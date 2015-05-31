using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VecLib;

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
                Dead,
                Ship, // Also alive
                Bullet
            }

            public CellState State { get; set; }
            public CellState NextState { get; set; }

            public bool Alive {  get { return State == CellState.Alive || State == CellState.Ship; } }

            public Cell()
            {
                State = CellState.Dead;
            }
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

            public static int WrapMod(int x, int m)
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
                    aliveCount += m_AllDirections[k](i, j).Alive ? 1 : 0;
                }
                return aliveCount;
            }

            public int ShipNeighbors(int i, int j)
            {
                int shipCount = 0;
                for (int k = 0; k < m_AllDirections.Length; ++k)
                {
                    shipCount += m_AllDirections[k](i, j).State == Cell.CellState.Ship ? 1 : 0;
                }
                return shipCount;
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

            public Cell.CellState GetNewCellStateFromRule(int numNeighbors, bool isAlive)
            {
                /* interface between cell and rule when updating cells */
                return isAlive ? m_RuleBehavior[1][numNeighbors] : m_RuleBehavior[0][numNeighbors];
            }
        }

        class Ship
        {
            public List<vec2> m_CellPositions = new List<vec2>();
            public Rectangle m_Bounds; // (x, y, w, h) bounds of existence of the ship

            public Ship() { }

            public Ship(Rectangle Bounds)
            {
                m_Bounds = Bounds;
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
            // Do not restrict the ship
            PlayerShip = new Ship(new Rectangle(0, 0, CellGrid.Width, CellGrid.Height));
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

        bool bPaintShip = false;

        bool[] KeysDown = new bool[256];

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            int rx = mx / GridSquareSize;
            int ry = my / GridSquareSize;

            if (bPaintShip)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Black)), rx * GridSquareSize, ry * GridSquareSize, GridSquareSize, GridSquareSize);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.LightGreen)), rx * GridSquareSize, ry * GridSquareSize, GridSquareSize, GridSquareSize);
            }

            if (mdown)
            {
                if (mbutton == MouseButtons.Left)
                {
                    if (bPaintShip)
                    {
                        CellGrid[rx, ry].State = Cell.CellState.Ship;
                    }
                    else
                    {
                        CellGrid[rx, ry].State = Cell.CellState.Alive;
                    }
                }
                else
                    CellGrid[rx, ry].State = Cell.CellState.Dead;
            }

            for (int i = 0; i < CellGrid.Width; ++i)
            {
                for (int j = 0; j < CellGrid.Height; ++j)
                {
                    if (CellGrid[i, j].State == Cell.CellState.Alive)
                    {
                        e.Graphics.FillRectangle(Brushes.LightGreen, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                    }
                    else if (CellGrid[i, j].State == Cell.CellState.Ship)
                    {
                        e.Graphics.FillRectangle(Brushes.Black, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                    }
                    else if (CellGrid[i, j].State == Cell.CellState.Bullet)
                    {
                        e.Graphics.FillRectangle(Brushes.Orange, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
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

            ////////////////////////////////
            // Game State Update
            ////////////////////////////////
            if (brunning  && (DateTime.Now.Ticks - llLastUpdate) > Properties.IterationDelay)
            {
                llLastUpdate = DateTime.Now.Ticks;

                vec2 vecCannon = new vec2(0, CellGrid.Height);

                // Regular update loop
                for (int i = 0; i < CellGrid.Width; ++i)
                {
                    for (int j = 0; j < CellGrid.Height; ++j)
                    {
                        Cell.CellState currentState = CellGrid[i, j].State;
                        
                        if (currentState != Cell.CellState.Bullet)
                        {
                            Cell.CellState nextState = R.GetNewCellStateFromRule(CellGrid.AliveNeighbors(i, j), CellGrid[i, j].Alive);

                            if (nextState == Cell.CellState.Dead)
                            {
                                CellGrid[i, j].NextState = Cell.CellState.Dead;
                            }
                            else if (currentState == Cell.CellState.Ship)
                            {
                                if (j < vecCannon.y)
                                {
                                    vecCannon = new vec2(i, j);
                                }
                                CellGrid[i, j].NextState = Cell.CellState.Ship;
                            }
                            else if (currentState == Cell.CellState.Dead)
                            {
                                if (CellGrid.ShipNeighbors(i, j) > CellGrid.AliveNeighbors(i, j) / 2)
                                {
                                    CellGrid[i, j].NextState = Cell.CellState.Ship;
                                }
                                else
                                {
                                    CellGrid[i, j].NextState = Cell.CellState.Alive;
                                }
                            }
                            else
                            {
                                CellGrid[i, j].NextState = Cell.CellState.Alive;
                            }
                        }
                    }
                }

                // Bullet update loop
                for (int i = 0; i < CellGrid.Width; ++i)
                {
                    for (int j = 0; j < CellGrid.Height; ++j)
                    {
                        if (CellGrid[i, j].State == Cell.CellState.Bullet)
                        {
                            if (j == 0)
                            {
                                CellGrid[i, j].NextState = Cell.CellState.Dead;
                            }
                            else
                            {
                                if (CellGrid[i, j + 1].State != Cell.CellState.Bullet && CellGrid[i, j + 1].State != Cell.CellState.Ship)
                                {
                                    CellGrid[i, j].NextState = Cell.CellState.Dead;
                                }
                                if (CellGrid[i, j - 1].NextState != Cell.CellState.Ship)
                                {
                                    CellGrid[i, j - 1].NextState = Cell.CellState.Bullet;
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < CellGrid.Width; ++i)
                {
                    for (int j = 0; j < CellGrid.Height; ++j)
                    {
                        CellGrid[i, j].State = CellGrid[i, j].NextState;
                    }
                }

                // Move the ship, process one set of directions at a time

                if (KeysDown[(int)Keys.A])
                {
                    for (int i = 0; i < CellGrid.Width; ++i)
                    {
                        for (int j = 0; j < CellGrid.Height; ++j)
                        {
                            if (CellGrid[i, j].State == Cell.CellState.Ship)
                            {
                                if (CellGrid[i + 1, j].State != Cell.CellState.Ship)
                                {
                                    CellGrid[i, j].NextState = Cell.CellState.Dead;
                                }
                                CellGrid[i - 1, j].NextState = Cell.CellState.Ship;
                            }
                        }
                    }

                    for (int i = 0; i < CellGrid.Width; ++i)
                    {
                        for (int j = 0; j < CellGrid.Height; ++j)
                        {
                            CellGrid[i, j].State = CellGrid[i, j].NextState;
                        }
                    }
                }

                if (KeysDown[(int)Keys.D])
                {
                    for (int i = 0; i < CellGrid.Width; ++i)
                    {
                        for (int j = 0; j < CellGrid.Height; ++j)
                        {
                            if (CellGrid[i, j].State == Cell.CellState.Ship)
                            {
                                if (CellGrid[i - 1, j].State != Cell.CellState.Ship)
                                {
                                    CellGrid[i, j].NextState = Cell.CellState.Dead;
                                }
                                CellGrid[i + 1, j].NextState = Cell.CellState.Ship;
                            }
                        }
                    }

                    for (int i = 0; i < CellGrid.Width; ++i)
                    {
                        for (int j = 0; j < CellGrid.Height; ++j)
                        {
                            CellGrid[i, j].State = CellGrid[i, j].NextState;
                        }
                    }
                }

                if (KeysDown[(int)Keys.W])
                {
                    for (int i = 0; i < CellGrid.Width; ++i)
                    {
                        for (int j = 0; j < CellGrid.Height; ++j)
                        {
                            if (CellGrid[i, j].State == Cell.CellState.Ship)
                            {
                                if (CellGrid[i, j + 1].State != Cell.CellState.Ship)
                                {
                                    CellGrid[i, j].NextState = Cell.CellState.Dead;
                                }
                                CellGrid[i, j - 1].NextState = Cell.CellState.Ship;
                            }
                        }
                    }

                    for (int i = 0; i < CellGrid.Width; ++i)
                    {
                        for (int j = 0; j < CellGrid.Height; ++j)
                        {
                            CellGrid[i, j].State = CellGrid[i, j].NextState;
                        }
                    }
                }

                if (KeysDown[(int)Keys.S])
                {
                    for (int i = 0; i < CellGrid.Width; ++i)
                    {
                        for (int j = 0; j < CellGrid.Height; ++j)
                        {
                            if (CellGrid[i, j].State == Cell.CellState.Ship)
                            {
                                if (CellGrid[i, j - 1].State != Cell.CellState.Ship)
                                {
                                    CellGrid[i, j].NextState = Cell.CellState.Dead;
                                }
                                CellGrid[i, j + 1].NextState = Cell.CellState.Ship;
                            }
                        }
                    }

                    for (int i = 0; i < CellGrid.Width; ++i)
                    {
                        for (int j = 0; j < CellGrid.Height; ++j)
                        {
                            CellGrid[i, j].State = CellGrid[i, j].NextState;
                        }
                    }
                }
                
                if (KeysDown[(int)Keys.ControlKey])
                { 
                   CellGrid[vecCannon.x, vecCannon.y - 1].State = Cell.CellState.Bullet;
                }

            }

            if (bstepping && brunning)
            {
                brunning = false;
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

            if (e.KeyCode == Keys.D2)
            {
                bPaintShip = true;
            }
            else if (e.KeyCode == Keys.D1)
            {
                bPaintShip = false;
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
