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

        public static int CurrentPopulationSize = 0;

        public static int GridSquareSize = 10;

        public static int PopulationMax = 200;

        InfoView info = null;

        public enum CellType
        {
            Water,
            Food,
            Grass,
            Mud,
            Rock,
            Agent
        }

        public class Cell
        {
            public CellType Type = CellType.Grass;

            public ulong LocalMaximumSustainability = 50000;
            public ulong ResourceCount = 50000;
            public ulong ReplenishRate = 1;

            public CellType OldType = CellType.Grass;
            public IAgent HoldingAgent = null;

            public void Update()
            {
                if (ResourceCount < LocalMaximumSustainability)
                {
                    ResourceCount += ReplenishRate;
                }
            }
            public void AssignAgent(IAgent a)
            {
                OldType = Type;
                Type = CellType.Agent;
                HoldingAgent = a;
                ++Form1.CurrentPopulationSize;
            }
            public void ClearAgent()
            {
                Type = OldType;
                HoldingAgent = null;
                --Form1.CurrentPopulationSize;
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

        public interface IAgent
        {
            ulong IterationIndex { get; set; }
            Brush Faction { get; set; }
            IAgent Rand(int x, int y, Grid grid, Brush color);
            bool ExecuteOneInstruction(Grid grid);
            bool CanBreed(Grid grid);
            IEnumerable<IAgent> BreedWith(IAgent other, Grid grid);
            String GetStats();
            ulong GetFitness();

            void SetTracking(bool bTrack);
            bool GetTracking();
            IEnumerable<Point> GetTrackingPoints();
        }

        public class Agent_1 : IAgent
        {

            public static Random r = new Random();

            bool bTracking = false;
            List<Point> TrackingPoints = new List<Point>();

            public void SetTracking(bool bTrack)
            {
                bTracking = bTrack;
                if (!bTrack)
                {
                    TrackingPoints.Clear();
                }
            }
            public bool GetTracking()
            {
                return bTracking;
            }
            public IEnumerable<Point> GetTrackingPoints() { return TrackingPoints; }

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

                    ret.Instructions = new InstructionSet[Agent_1.r.Next(200, 800)];
                    ret.JumpLocations = new int[ret.Instructions.Length];
                    ret.DoActions = new Action[ret.Instructions.Length];
                    ret.DoDirections = new Direction[ret.Instructions.Length];

                    for (int i = 0; i < ret.Instructions.Length; ++i)
                    {
                        int RandomOpCode = Agent_1.r.Next(0, 50);
                        if (RandomOpCode > (int)InstructionSet.Do)
                            RandomOpCode = (int)InstructionSet.Do;

                        InstructionSet ins = (InstructionSet)RandomOpCode;
                        ret.Instructions[i] = ins;
                        if (ins == InstructionSet.JumpIfEqual)
                        {
                            ret.JumpLocations[i] = Agent_1.r.Next(0, ret.Instructions.Length);
                        }
                        else if (ins == InstructionSet.Do || ins == InstructionSet.DoIfEqual)
                        {
                            ret.DoActions[i] = (Action)Agent_1.r.Next(0, typeof(Action).GetEnumValues().Length);
                            ret.DoDirections[i] = (Direction)Agent_1.r.Next(0, typeof(Direction).GetEnumValues().Length);
                        }
                    }

                    return ret;
                }
            }

            public int InstructionGroupPointer = 0;
            public int InstructionPointer = 0;

            public Brush Faction { get; set; }

            public ulong LOCAL_MAX = 1000000;
            public ulong ENERGY = 10000;
            public ulong FOOD = 10000;
            public ulong WATER = 10000;
            public Agent_1()
            {
                ENERGY = LOCAL_MAX;
                FOOD = LOCAL_MAX;
                WATER = LOCAL_MAX;
            }

            public int Generation = 1;

            public ulong BreedAge = 0;
            public ulong Age = 0;

            public int x = 0, y = 0;

            public int RegisterIndex = 0;

            public ulong IterationIndex { get; set; }

            public Object[] Registers = new Object[2];
            public List<InstructionGroup> Brain = new List<InstructionGroup>();

            public IAgent Rand(int x, int y, Grid grid, Brush color)
            {
                Agent_1 ret = new Agent_1();

                ret.Faction = color;

                int instructionGroupCount = Agent_1.r.Next(50, 500);
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

                if (a.GetType() == typeof(Cell) && b.GetType() == typeof(CellType))
                {
                    return ((Cell)a).Type == ((CellType)b);
                }
                if (b.GetType() == typeof(Cell) && a.GetType() == typeof(CellType))
                {
                    return ((Cell)b).Type == ((CellType)a);
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
                        if (actOn.Type == CellType.Water && actOn.ResourceCount >= 1000)
                        {
                            actOn.ResourceCount -= 1000;
                            if (WATER + 1000 < LOCAL_MAX)
                            {
                                WATER += 1000;
                            }
                        }
                        break;
                    case Action.Eat:
                        if (actOn.Type == CellType.Food && actOn.ResourceCount >= 1000)
                        {
                            actOn.ResourceCount -= 1000;
                            if (FOOD + 1000 < LOCAL_MAX)
                            {
                                FOOD += 1000;
                            }
                        }
                        break;
                    case Action.Move:
                        if (actOn.Type != CellType.Rock && actOn.Type != CellType.Water && actOn.Type != CellType.Agent)
                        {
                            x = actOnX;
                            y = actOnY;

                            if (bTracking)
                            {
                                TrackingPoints.Add(new Point((int)(x * Form1.GridSquareSize + ((float)Form1.GridSquareSize) / 2), (int)(y * Form1.GridSquareSize + ((float)Form1.GridSquareSize) / 2)));
                            }

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

            Cell GetFirstAvailableCell(Grid grid, out int xOut, out int yOut)
            {
                int sx = x - 10;
                int sy = y - 10;
                for (; sx < x + 10; ++sx)
                {
                    for (sy = y - 10; sy < y + 10; ++sy)
                    {
                        Cell tryCell = grid[sx, sy];
                        if (tryCell.Type == CellType.Grass || tryCell.Type == CellType.Mud)
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
                if (Agent_1.r.Next(0, 1000) < 2)
                {
                    Brain.Add(InstructionGroup.Rand());
                }
                else
                {
                    Brain.Add(addTo);
                }
            }

            // Agent API
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
                        Registers[RegisterIndex] = CellType.Water;
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadFood:
                        Registers[RegisterIndex] = CellType.Food;
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadGrass:
                        Registers[RegisterIndex] = CellType.Grass;
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadMud:
                        Registers[RegisterIndex] = CellType.Mud;
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadRock:
                        Registers[RegisterIndex] = CellType.Rock;
                        if (++RegisterIndex == Registers.Length)
                        {
                            RegisterIndex = 0;
                        }
                        break;
                    case InstructionSet.LoadAgent:
                        Registers[RegisterIndex] = CellType.Agent;
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

                if (--FOOD == 0 || --ENERGY == 0 || --WATER == 0 || Age >= LOCAL_MAX * 100 || r.Next(1000000) == 1)
                {
                    // Dead
                    grid[x, y].ClearAgent();
                    return false;
                }

                if (Age > LOCAL_MAX)
                {
                    this.Faction = Brushes.Aquamarine;
                }
                return true;
            }
            public IEnumerable<IAgent> BreedWith(IAgent other, Grid grid)
            {
                this.BreedAge = 0;
                ((Agent_1)other).BreedAge = 0;

                int AInstructionGroupCount = Brain.Count;
                int BInstructionGroupCount = ((Agent_1)other).Brain.Count;

                int CInstructionGroupCount = (int)Math.Max(50, ((AInstructionGroupCount + BInstructionGroupCount) / 2.0) + r.Next(-50, 50));

                Agent_1 ret = new Agent_1();

                ret.Generation = this.Generation + 1;
                int gx, gy;

                Cell firstAvailable = GetFirstAvailableCell(grid, out gx, out gy);
                if (firstAvailable != null)
                {
                    ret.x = gx;
                    ret.y = gy;
                    ret.Faction = Brushes.DarkOrange;

                    for (int i = 0; i < CInstructionGroupCount / 2 && i < AInstructionGroupCount; ++i)
                    {
                        AddInstructionGroupOrRandom(ret.Brain, this.Brain[i]);
                    }
                    for (int i = 0; i < CInstructionGroupCount / 2 && i < BInstructionGroupCount; ++i)
                    {
                        AddInstructionGroupOrRandom(ret.Brain, ((Agent_1)other).Brain[i]);
                    }

                    firstAvailable.AssignAgent(ret);
                }

                yield return ret;

                ret = new Agent_1();
                ret.Generation = this.Generation + 1;

                CInstructionGroupCount = Math.Max(50, (AInstructionGroupCount + BInstructionGroupCount - r.Next(-50, 50)));
                firstAvailable = GetFirstAvailableCell(grid, out gx, out gy);
                if (firstAvailable != null)
                {
                    ret.x = gx;
                    ret.y = gy;
                    ret.Faction = Brushes.DarkOrange;

                    for (int i = 0; i < CInstructionGroupCount; ++i)
                    {
                        if (i < AInstructionGroupCount)
                            AddInstructionGroupOrRandom(ret.Brain, this.Brain[i]);
                        ++i;
                        if (i < BInstructionGroupCount)
                            AddInstructionGroupOrRandom(ret.Brain, ((Agent_1)other).Brain[i]);
                    }

                    firstAvailable.AssignAgent(ret);
                }

                yield return ret;

                ret = new Agent_1();
                ret.Generation = this.Generation + 1;

                CInstructionGroupCount = Math.Max(50, (Math.Max(AInstructionGroupCount, BInstructionGroupCount) - r.Next(-50, 50)));
                firstAvailable = GetFirstAvailableCell(grid, out gx, out gy);
                if (firstAvailable != null)
                {
                    ret.x = gx;
                    ret.y = gy;
                    ret.Faction = Brushes.DarkOrange;
                     
                    for (int i = 0; i < CInstructionGroupCount; ++i)
                    {
                        if (i < AInstructionGroupCount)
                            AddInstructionGroupOrRandom(ret.Brain, this.Brain[i]);
                        ++i;
                        if (i < BInstructionGroupCount)
                            AddInstructionGroupOrRandom(ret.Brain, ((Agent_1)other).Brain[i]);
                    }

                    firstAvailable.AssignAgent(ret);
                }

                yield return ret;


            }
            public String GetStats()
            {
                String ret = "";
                ret += "Faction: " + (new Pen(Faction).Color) + "\n";
                ret += "Food: " + FOOD + "\n";
                ret += "Water: " + WATER + "\n";
                ret += "Energy: " + ENERGY + "\n";
                ret += "Generation: " + Generation + "\n";
                ret += "Age: " + Age + "\n"; ;

                ulong instructionCount = 0;

                for (int i = 0; i < Brain.Count; ++i)
                {
                    instructionCount += (ulong)Brain[i].Instructions.Length;
                }

                ret += "Instruction Count: " + instructionCount.ToString() + "\n";
                ret += "Instruction Groups: " + Brain.Count.ToString();
                return
                    ret;
            }
            public bool CanBreed(Grid grid)
            {
                if (FOOD > 0 && WATER > 0 && ENERGY > 0)
                {
                    if (BreedAge >= LOCAL_MAX + 1000 || (FOOD != WATER && Agent_1.r.Next(0, 10000) == 1))
                    {
                        return true;
                    }
                }
                else
                {
                    if (FOOD > 0 || WATER > 0 && Agent_1.r.Next(0, 1000) == 1)
                    {
                        return true;
                    }
                }
                return false;
            }

            public ulong GetFitness()
            {
                return (FOOD + WATER) * 10 + ENERGY + Age;
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

        bool bshift = false;

        CellType painter = CellType.Grass;
        int paintSize = 1;
        bool sparseBrush = false;

        public Grid CellGrid = null;
        bool bNeedsUpdate = false;

        ulong LastTotalPopulation = 0;
        public ulong IterationIndex = 0;

        IAgent agentFactory = new Agent_1();

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
                                if (painter == CellType.Agent && CellGrid[rx + i, ry + j].Type != CellType.Agent)
                                {
                                    updateCell.AssignAgent(agentFactory.Rand(rx + i, ry + j, CellGrid, Brushes.Bisque));
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
                                if (painter == CellType.Agent && CellGrid[rx + i, ry + j].Type != CellType.Agent)
                                {
                                    updateCell.AssignAgent(agentFactory.Rand(rx + i, ry + j, CellGrid, Brushes.Bisque));
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

                List<IAgent> Parents = new List<IAgent>();

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
                            if (CurrentCell.Type == CellType.Agent && CurrentCell.HoldingAgent.IterationIndex != IterationIndex)
                            {
                                ++TotalPopulation;
                                IAgent CurrentAgent = CurrentCell.HoldingAgent;

                                CurrentAgent.ExecuteOneInstruction(CellGrid);
                                if (CurrentAgent.CanBreed(CellGrid))
                                {
                                    Parents.Add(CurrentAgent);
                                    ++AgentsLeft;
                                }
                                CurrentAgent.IterationIndex = IterationIndex;
                            }
                            else if (info.InsertRandomAgents() && (CurrentCell.Type == CellType.Grass || CellGrid[i,j].Type == CellType.Mud) && LastTotalPopulation < 80 && Agent_1.r.Next(0, 10000) == 1)
                            {
                                IAgent newRandomAgent = agentFactory.Rand(i, j, CellGrid, Brushes.Black);
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

                    Parents.Sort((Comparison<IAgent>)((IAgent a, IAgent b) => b.GetFitness().CompareTo(a.GetFitness())));
                    int canMake = Parents.Count;

                    int added = 0;
                    for (int i = 0; i < AgentsLeft;)
                    {
                        added = 0;
                        for (int j = 1; j < AgentsLeft && added <= canMake; ++j)
                        {
                            if (Agent_1.r.Next(1, 100) > 50 && CurrentPopulationSize < PopulationMax)
                            {
                                foreach (Agent_1 newAgent in Parents[i].BreedWith(Parents[j], CellGrid))
                                {
                                    //info.Log("Breeding ... " + added + "/" + canMake + " (out of " + Parents.Count + " parents)");
                                    if (++added == canMake)
                                    {
                                        break;
                                    }
                                }
                            }
                        }

                        canMake -= 2;

                        --AgentsLeft;
                        Parents.RemoveAt(0);
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
                        case CellType.Grass:
                            e.Graphics.FillRectangle(Brushes.Green, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                            break;
                        case CellType.Mud:
                            e.Graphics.FillRectangle(Brushes.Maroon, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                            break;
                        case CellType.Water:
                            e.Graphics.FillRectangle(Brushes.Blue, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                            break;
                        case CellType.Rock:
                            e.Graphics.FillRectangle(Brushes.Gray, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                            break;
                        case CellType.Food:
                            e.Graphics.FillRectangle(Brushes.PaleVioletRed, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
                            break;
                        case CellType.Agent:
                            e.Graphics.FillRectangle(CellGrid[i,j].HoldingAgent.Faction, i * GridSquareSize, j * GridSquareSize, GridSquareSize, GridSquareSize);
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
            
            for (int i = 0; i < CellGrid.Width; ++i)
            {
                for (int j = 0; j < CellGrid.Height; ++j)
                {
                    if (CellGrid[i, j].Type == CellType.Agent && CellGrid[i, j].HoldingAgent.GetTracking())
                    {
                        Point[] points = CellGrid[i, j].HoldingAgent.GetTrackingPoints().ToArray();
                        if (points.Length > 1)
                        {
                            e.Graphics.DrawLines(Pens.Azure, points);
                        }
                    }
                }
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
            if (!bshift)
            {
                mx = e.X;
                my = e.Y;
            }
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

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                bshift = false;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.I)
            {
                info.Show();
            }

            if (e.KeyCode == Keys.D0)
            {
                IterationIndex = 0;
            }

            if (e.Shift)
            {
                bshift = true;
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
                painter = CellType.Grass;
            }
            else if (e.KeyCode == Keys.D2)
            {
                painter = CellType.Water;
            }
            else if (e.KeyCode == Keys.D3)
            {
                painter = CellType.Rock;
            }
            else if (e.KeyCode == Keys.D4)
            {
                painter = CellType.Food;
            }
            else if (e.KeyCode == Keys.D5)
            {
                painter = CellType.Agent;
            }
            else if (e.KeyCode == Keys.D6)
            {
                painter = CellType.Mud;
            }

            bNeedsUpdate = true;
        }
    }
}
