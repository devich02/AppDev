using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CellularEvolution
{
    public partial class Form1 : Form
    {

        int GridSquareSize = 10;

        InfoView info = null;

        public class Cell
        {
            public enum CellType
            {
                Water,
                Food,
                Grass,
                Mud,
                Rock,
                Agent
            }

            public CellType Type = CellType.Grass;

            public ulong LocalMaximumSustainability = 50000;
            public ulong ResourceCount = 50000;
            public ulong ReplenishRate = 1;

            public CellType OldType = CellType.Grass;
            public Agent HoldingAgent = new Agent();

            public void Update()
            {
                if (ResourceCount < LocalMaximumSustainability)
                {
                    ResourceCount += ReplenishRate;
                }
            }
            public void AssignAgent(Agent a)
            {
                OldType = Type;
                Type = CellType.Agent;
                HoldingAgent = a;
            }
            public void ClearAgent()
            {
                Type = OldType;
                HoldingAgent = null;
            }
        }

        public class Grid
        {
            public Cell[][] Cells;

            public Grid() { }
            public Grid(int x, int y)
            {
                Cells = new Cell[x][];
                for (int i = 0; i < x; ++i)
                {
                    Cells[i] = new Cell[y];
                    for (int j = 0; j < y; ++j)
                    {
                        Cells[i][j] = new Cell();
                    }
                }
            }

            public int Width {  get { return Cells.Length; } }
            public int Height { get { return Cells[0].Length; } }

            public Cell this[int x, int y]
            {
                get
                {
                    if (x < 0)
                        x += Cells.Length;
                    if (x >= Cells.Length)
                        x -= Cells.Length;
                    if (y < 0)
                        y += Cells[x].Length;
                    if (y >= Cells[x].Length)
                        y -= Cells[x].Length;

                    return Cells[x][y];
                }
            }
        }

        public class Agent
        {

            public static Random r = new Random();

            public enum Action
            {
                Move,
                Eat,
                Drink,
                Sleep
            }
            public enum Direction
            {
                TopLeft,
                Top,
                TopRight,
                Right,
                BottomRight,
                Bottom,
                BottomLeft,
                Left
            }
            public enum InstructionSet
            {
                LoadTopLeft = 0,
                LoadTop = 1,
                LoadTopRight = 2,
                LoadRight = 3,
                LoadBottomRight = 4,
                LoadBottom = 5,
                LoadBottomLeft = 6,
                LoadLeft = 7,

                LoadWater = 8,
                LoadFood = 9,
                LoadGrass = 10,
                LoadMud = 11,
                LoadRock = 12,
                LoadAgent = 13,

                JumpIfEqual = 14,
                DoIfEqual = 15,

                Do = 16
            }
            public class InstructionGroup
            {
                public InstructionSet[] Instructions;
                public int[] JumpLocations;
                public Action[] DoActions;
                public Direction[] DoDirections;

                public static InstructionGroup Rand()
                {
                    InstructionGroup ret = new InstructionGroup();

                    ret.Instructions = new InstructionSet[Agent.r.Next(200, 800)];
                    ret.JumpLocations = new int[ret.Instructions.Length];
                    ret.DoActions = new Action[ret.Instructions.Length];
                    ret.DoDirections = new Direction[ret.Instructions.Length];

                    for (int i = 0; i < ret.Instructions.Length; ++i)
                    {
                        int RandomOpCode = Agent.r.Next(0, 50);
                        if (RandomOpCode > (int)InstructionSet.Do)
                            RandomOpCode = (int)InstructionSet.Do;

                        InstructionSet ins = (InstructionSet)RandomOpCode;
                        ret.Instructions[i] = ins;
                        if (ins == InstructionSet.JumpIfEqual)
                        {
                            ret.JumpLocations[i] = Agent.r.Next(0, ret.Instructions.Length);
                        }
                        else if (ins == InstructionSet.Do || ins == InstructionSet.DoIfEqual)
                        {
                            ret.DoActions[i] = (Action)Agent.r.Next(0, typeof(Action).GetEnumValues().Length);
                            ret.DoDirections[i] = (Direction)Agent.r.Next(0, typeof(Direction).GetEnumValues().Length);
                        }
                    }

                    return ret;
                }
            }

            public int InstructionGroupPointer = 0;
            public int InstructionPointer = 0;

            public Brush AgentColor = Brushes.Black;

            public ulong LOCAL_MAX = 500000;
            public ulong ENERGY = 500000;
            public ulong FOOD = 500000;
            public ulong WATER = 500000;

            public int Generation = 1;

            public ulong BreedAge = 0;
            public ulong Age = 0;

            public int x = 0, y = 0;

            public int RegisterIndex = 0;

            public ulong IterationIndex = 0;

            public Object[] Registers = new Object[2];
            public List<InstructionGroup> Brain = new List<InstructionGroup>();

            public static Agent Rand(int x, int y, Brush color)
            {
                Agent ret = new Agent();

                ret.AgentColor = color;

                int instructionGroupCount = Agent.r.Next(50, 500);
                for (int i = 0; i < instructionGroupCount; ++i)
                {
                    ret.Brain.Add(InstructionGroup.Rand());
                }
                ret.x = x;
                ret.y = y;

                return ret;
            }

            bool Compare(Object a, Object b)
            {
                if (a == null && b == null)
                {
                    return true;
                }

                if (a == null)
                {
                    return false;
                }

                if (b == null)
                {
                    return false;
                }

                if (a.GetType() == typeof(Cell) && b.GetType() == typeof(Cell.CellType))
                {
                    return ((Cell)a).Type == ((Cell.CellType)b);
                }
                if (b.GetType() == typeof(Cell) && a.GetType() == typeof(Cell.CellType))
                {
                    return ((Cell)b).Type == ((Cell.CellType)a);
                }

                if (a.GetType() == typeof(Cell) && b.GetType() == typeof(Cell))
                {
                    return ((Cell)a).Type == ((Cell)b).Type;
                }

                return false;
            }
            void Do(Action action, Direction direction, Grid grid)
            {
                int actOnX = 0, actOnY = 0;
                switch (direction)
                {
                    case Direction.TopLeft: actOnX = x - 1; actOnY = y - 1; break;
                    case Direction.Top: actOnX = x; actOnY = y - 1; break;
                    case Direction.TopRight: actOnX = x + 1; actOnY = y - 1; break;
                    case Direction.Right: actOnX = x + 1; actOnY = y; break;
                    case Direction.BottomRight: actOnX = x + 1; actOnY = y + 1; break;
                    case Direction.Bottom: actOnX = x; actOnY = y + 1; break;
                    case Direction.BottomLeft: actOnX = x - 1; actOnY = y + 1; break;
                    case Direction.Left: actOnX = x - 1; actOnY = y; break;
                }

                Cell currentlyIn = grid[x, y];
                Cell actOn = grid[actOnX, actOnY];

                switch (action)
                {
                    case Action.Drink:
                        if (actOn.Type == Cell.CellType.Water && actOn.ResourceCount >= 1000)
                        {
                            actOn.ResourceCount -= 1000;
                            if (WATER + 1000 < LOCAL_MAX)
                            {
                                WATER += 1000;
                            }
                        }
                        break;
                    case Action.Eat:
                        if (actOn.Type == Cell.CellType.Food && actOn.ResourceCount >= 1000)
                        {
                            actOn.ResourceCount -= 1000;
                            if (FOOD + 1000 < LOCAL_MAX)
                            {
                                FOOD += 1000;
                            }
                        }
                        break;
                    case Action.Move:
                        if (actOn.Type != Cell.CellType.Rock && actOn.Type != Cell.CellType.Water && actOn.Type != Cell.CellType.Agent)
                        {
                            x = actOnX;
                            y = actOnY;

                            if (x < 0)
                                x += grid.Width;
                            if (x >= grid.Width)
                                x -= grid.Width;
                            if (y < 0)
                                y += grid.Height;
                            if (y >= grid.Height)
                                y -= grid.Height;

                            currentlyIn.ClearAgent();
                            actOn.AssignAgent(this);

                        }
                        break;
                    case Action.Sleep:
                        if (ENERGY + 10 < LOCAL_MAX)
                        {
                            ENERGY += 10;
                        }
                        break;
                }

            }

            // Returns true if still alive, false if dead
            public bool ExecuteOneInstruction(Grid grid)
            {

                InstructionGroup curGroup = Brain[InstructionGroupPointer];
                InstructionSet curInstruction = curGroup.Instructions[InstructionPointer];

                switch (curInstruction)
                {
                    case InstructionSet.LoadTopLeft:
                        Registers[RegisterIndex] = grid[x - 1, y - 1];
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadTop:
                        Registers[RegisterIndex] = grid[x, y - 1];
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadTopRight:
                        Registers[RegisterIndex] = grid[x + 1, y - 1];
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadRight:
                        Registers[RegisterIndex] = grid[x + 1, y];
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadBottomRight:
                        Registers[RegisterIndex] = grid[x + 1, y + 1];
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadBottom:
                        Registers[RegisterIndex] = grid[x, y + 1];
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadBottomLeft:
                        Registers[RegisterIndex] = grid[x - 1, y + 1];
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadLeft:
                        Registers[RegisterIndex] = grid[x - 1, y];
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;

                    case InstructionSet.LoadWater:
                        Registers[RegisterIndex] = Cell.CellType.Water;
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadFood:
                        Registers[RegisterIndex] = Cell.CellType.Food;
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadGrass:
                        Registers[RegisterIndex] = Cell.CellType.Grass;
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadMud:
                        Registers[RegisterIndex] = Cell.CellType.Mud;
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadRock:
                        Registers[RegisterIndex] = Cell.CellType.Rock;
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadAgent:
                        Registers[RegisterIndex] = Cell.CellType.Agent;
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;

                    case InstructionSet.JumpIfEqual:
                        if (Compare(Registers[0], Registers[1]))
                        {
                            InstructionPointer = curGroup.JumpLocations[InstructionPointer];
                        }
                        break;

                    case InstructionSet.Do:
                        Do(curGroup.DoActions[InstructionPointer], curGroup.DoDirections[InstructionPointer], grid);
                        break;
                    case InstructionSet.DoIfEqual:
                        if (Compare(Registers[0], Registers[1]))
                        {
                            Do(curGroup.DoActions[InstructionPointer], curGroup.DoDirections[InstructionPointer], grid);
                        }
                        break;
                }

                if (++InstructionPointer == curGroup.Instructions.Length)
                {
                    if (++InstructionGroupPointer == Brain.Count)
                    {
                        InstructionGroupPointer = 0;
                    }
                    InstructionPointer = 0;
                }

                ++Age;
                ++BreedAge;

                if (--FOOD == 0 || --ENERGY == 0 || --WATER == 0)
                {
                    // Dead
                    grid[x, y].ClearAgent();
                    return false;
                }
                return true;
            }

            public Cell GetFirstAvailableCell(Grid grid, out int xOut, out int yOut)
            {
                int sx = x - 10;
                int sy = y - 10;
                for (; sx < x + 10; ++sx)
                {
                    for (sy = y - 10; sy < y + 10; ++sy)
                    {
                        Cell tryCell = grid[sx, sy];
                        if (tryCell.Type == Cell.CellType.Grass || tryCell.Type == Cell.CellType.Mud)
                        {
                            xOut = sx;
                            yOut = sy;
                            return tryCell;
                        }
                    }
                }
                xOut = 0;
                yOut = 0;
                return null;
            }

            void AddInstructionGroupOrRandom(List<InstructionGroup> Brain, InstructionGroup addTo)
            {
                if (Agent.r.Next(0, 100) < 2)
                {
                    Brain.Add(InstructionGroup.Rand());
                }
                else
                {
                    Brain.Add(addTo);
                }
            }

            public IEnumerable<Agent> BreedWith(Agent other, Grid grid)
            {
                int AInstructionGroupCount = Brain.Count;
                int BInstructionGroupCount = other.Brain.Count;

                int CInstructionGroupCount = (int)((AInstructionGroupCount + BInstructionGroupCount) / 2.0);

                Agent ret = new Agent();

                ret.Generation = this.Generation + 1;
                int gx, gy;

                Cell firstAvailable = GetFirstAvailableCell(grid, out gx, out gy);
                if (firstAvailable != null)
                {
                    ret.x = gx;
                    ret.y = gy;
                    ret.AgentColor = Brushes.DarkOrange;

                    for (int i = 0; i < CInstructionGroupCount / 2 && i < AInstructionGroupCount; ++i)
                    {
                        AddInstructionGroupOrRandom(ret.Brain, this.Brain[i]);
                    }
                    for (int i = 0; i < CInstructionGroupCount / 2 && i < BInstructionGroupCount; ++i)
                    {
                        AddInstructionGroupOrRandom(ret.Brain, other.Brain[i]);
                    }

                    firstAvailable.AssignAgent(ret);
                }

                yield return ret;

                ret = new Agent();
                ret.Generation = this.Generation + 1;

                CInstructionGroupCount = AInstructionGroupCount + BInstructionGroupCount;
                firstAvailable = GetFirstAvailableCell(grid, out gx, out gy);
                if (firstAvailable != null)
                {
                    ret.x = gx;
                    ret.y = gy;
                    ret.AgentColor = Brushes.DarkOrange;

                    for (int i = 0; i < CInstructionGroupCount; ++i)
                    {
                        if (i < AInstructionGroupCount)
                            AddInstructionGroupOrRandom(ret.Brain, this.Brain[i]);
                        ++i;
                        if (i < BInstructionGroupCount)
                            AddInstructionGroupOrRandom(ret.Brain, other.Brain[i]);
                    }

                    firstAvailable.AssignAgent(ret);
                }

                yield return ret;

                ret = new Agent();
                ret.Generation = this.Generation + 1;

                CInstructionGroupCount = Math.Max(AInstructionGroupCount, BInstructionGroupCount);
                firstAvailable = GetFirstAvailableCell(grid, out gx, out gy);
                if (firstAvailable != null)
                {
                    ret.x = gx;
                    ret.y = gy;
                    ret.AgentColor = Brushes.DarkOrange;

                    for (int i = 0; i < CInstructionGroupCount; ++i)
                    {
                        if (i < AInstructionGroupCount)
                            AddInstructionGroupOrRandom(ret.Brain, this.Brain[i]);
                        ++i;
                        if (i < BInstructionGroupCount)
                            AddInstructionGroupOrRandom(ret.Brain, other.Brain[i]);
                    }

                    firstAvailable.AssignAgent(ret);
                }

                yield return ret;


            }
        }

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(800, 600);
            CellGrid = new Grid(800 / GridSquareSize, 600 / GridSquareSize);
            info = new InfoView(this);
        }

        int mx = 0, my = 0;
        bool mdown = false;
        MouseButtons mbutton = MouseButtons.Left;

        bool brunning = false;
        bool bstep = false;

        Cell.CellType painter = Cell.CellType.Grass;
        int paintSize = 1;
        bool sparseBrush = false;

        public Grid CellGrid = null;
        bool bNeedsUpdate = false;

        ulong LastTotalPopulation = 0;
        ulong IterationIndex = 0;

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            int rx = mx / GridSquareSize;
            int ry = my / GridSquareSize;

            if (bNeedsUpdate)
            {
                info.Update(rx, ry, CellGrid, painter, paintSize);
                bNeedsUpdate = false;
            }

            if (mdown)
            {
                if (mbutton == MouseButtons.Left)
                {
                    if (sparseBrush)
                    {
                        for (int i = 0; i < paintSize * 2; i += 2)
                        {
                            for (int j = 0; j < paintSize * 2; j += 2)
                            {
                                Cell updateCell = CellGrid[rx + i, ry + j];
                                if (painter == Cell.CellType.Agent && CellGrid[rx + i, ry + j].Type != Cell.CellType.Agent)
                                {
                                    updateCell.OldType = CellGrid[rx + i, ry + j].Type;
                                    updateCell.HoldingAgent = Agent.Rand(rx + i, ry + j, Brushes.Bisque);
                                }
                                updateCell.Type = painter;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < paintSize; ++i)
                        {
                            for (int j = 0; j < paintSize; ++j)
                            {
                                Cell updateCell = CellGrid[rx + i, ry + j];
                                if (painter == Cell.CellType.Agent && CellGrid[rx + i, ry + j].Type != Cell.CellType.Agent)
                                {
                                    updateCell.OldType = CellGrid[rx + i, ry + j].Type;
                                    updateCell.HoldingAgent = Agent.Rand(rx + i, ry + j, Brushes.Bisque);
                                }
                                updateCell.Type = painter;
                            }
                        }
                    }
                }
            }

            if (brunning)
            {
                int AgentsLeft = 0;

                List<Agent> Parents = new List<Agent>();

                int iterCount = info.GetIterationsPerVisualRefresh();
                for (int k = 0; k < iterCount; ++k)
                {
                    ++IterationIndex;
                    Parents.Clear();
                    AgentsLeft = 0;

                    ulong TotalPopulation = 0;

                    for (int i = 0; i < CellGrid.Width; ++i)
                    {
                        for (int j = 0; j < CellGrid.Height; ++j)
                        {
                            Cell CurrentCell = CellGrid[i, j];
                            if (CurrentCell.Type == Cell.CellType.Agent && CurrentCell.HoldingAgent.IterationIndex != IterationIndex)
                            {
                                ++TotalPopulation;
                                Agent CurrentAgent = CurrentCell.HoldingAgent;
                                if (CurrentAgent.ExecuteOneInstruction(CellGrid))
                                {
                                    if (CurrentAgent.BreedAge >= CurrentAgent.LOCAL_MAX + 1000 || (CurrentAgent.FOOD != CurrentAgent.WATER && Agent.r.Next(0, 100) < 2))
                                    {
                                        Parents.Add(CurrentAgent);
                                        ++AgentsLeft;
                                    }
                                }
                                else
                                {
                                    if (CurrentAgent.FOOD > 0 || CurrentAgent.WATER > 0)
                                    {
                                        Parents.Add(CurrentAgent);
                                        ++AgentsLeft;
                                    }
                                }
                                CurrentAgent.IterationIndex = IterationIndex;
                            }
                            else if (info.InsertRandomAgents() && (CurrentCell.Type == Cell.CellType.Grass || CellGrid[i,j].Type == Cell.CellType.Mud) && LastTotalPopulation < 80 && Agent.r.Next(0, 10000) == 1)
                            {
                                Agent newRandomAgent = Agent.Rand(i, j, Brushes.Black);
                                CurrentCell.AssignAgent(newRandomAgent);
                            }
                            else
                            {
                                if (CurrentCell.ResourceCount + CurrentCell.ReplenishRate < CurrentCell.LocalMaximumSustainability)
                                {
                                    CurrentCell.ResourceCount += CurrentCell.ReplenishRate;
                                }
                            }
                        }
                    }

                    LastTotalPopulation = TotalPopulation;

                    int added = 0;
                    for (int i = 0; i < AgentsLeft; ++i)
                    {
                        for (int j = 0; j < AgentsLeft; ++j)
                        {
                            if (i == j || Parents[i].BreedAge < Parents[i].LOCAL_MAX + 1000 || Parents[j].BreedAge < Parents[j].LOCAL_MAX + 1000)
                            {
                                continue;
                            }
                            if (Agent.r.Next(1, 100) > 33)
                            {
                                foreach (Agent newAgent in Parents[i].BreedWith(Parents[j], CellGrid))
                                {
                                    ++added;
                                }
                                Parents[i].BreedAge = 0;
                                Parents[j].BreedAge = 0;
                            }
                        }
                    }
                }

                bNeedsUpdate = true;
            }

            for (int i = 0; i < CellGrid.Width; ++i)
            {
                for (int j = 0; j < CellGrid.Height; ++j)
                {
                    switch (CellGrid[i,j].Type)
                    {
                        case Cell.CellType.Grass:
                            e.Graphics.FillRectangle(Brushes.Green, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                            break;
                        case Cell.CellType.Mud:
                            e.Graphics.FillRectangle(Brushes.Maroon, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                            break;
                        case Cell.CellType.Water:
                            e.Graphics.FillRectangle(Brushes.Blue, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                            break;
                        case Cell.CellType.Rock:
                            e.Graphics.FillRectangle(Brushes.Gray, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                            break;
                        case Cell.CellType.Food:
                            e.Graphics.FillRectangle(Brushes.PaleVioletRed, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                            break;
                        case Cell.CellType.Agent:
                            e.Graphics.FillRectangle(CellGrid[i,j].HoldingAgent.AgentColor, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                            break;
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

            if (sparseBrush)
            {
                for (int i = 0; i < paintSize * 2; i += 2)
                {
                    for (int j = 0; j < paintSize * 2; j += 2)
                    {
                        e.Graphics.FillRectangle(Brushes.Orange, (rx + i) * GridSquareSize, (ry + j) * GridSquareSize, GridSquareSize, GridSquareSize);
                    }
                }
            }
            else
            {
                for (int i = 0; i < paintSize; ++i)
                {
                    for (int j = 0; j < paintSize; ++j)
                    {
                        e.Graphics.FillRectangle(Brushes.Orange, (rx + i) * GridSquareSize, (ry + j) * GridSquareSize, GridSquareSize, GridSquareSize);
                    }
                }
            }


            if (bstep && brunning)
            {
                brunning = false;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            mx = e.X;
            my = e.Y;
            bNeedsUpdate = true;
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.I)
            {
                info.Show();
            }

            if (e.KeyCode == Keys.Z)
            {
                if (sparseBrush)
                {
                    sparseBrush = false;
                }
                else
                {
                    sparseBrush = true;
                }
            }

            if (e.KeyCode == Keys.S)
            {
                if (bstep)
                {
                    bstep = false;
                }
                else
                {
                    bstep = true;
                }
            }

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

            if (e.KeyCode == Keys.Q)
            {
                paintSize = 1;
            }
            else if (e.KeyCode == Keys.W)
            {
                paintSize = 2;
            }
            else if (e.KeyCode == Keys.E)
            {
                paintSize = 3;
            }
            else if (e.KeyCode == Keys.R)
            {
                paintSize = 4;
            }

            if (e.KeyCode == Keys.D1)
            {
                painter = Cell.CellType.Grass;
            }
            else if (e.KeyCode == Keys.D2)
            {
                painter = Cell.CellType.Water;
            }
            else if (e.KeyCode == Keys.D3)
            {
                painter = Cell.CellType.Rock;
            }
            else if (e.KeyCode == Keys.D4)
            {
                painter = Cell.CellType.Food;
            }
            else if (e.KeyCode == Keys.D5)
            {
                painter = Cell.CellType.Agent;
            }
            else if (e.KeyCode == Keys.D6)
            {
                painter = Cell.CellType.Mud;
            }

            bNeedsUpdate = true;
        }
    }
}
