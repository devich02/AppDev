using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VecLib;
using MachineLearning;

namespace LibTest
{
    class EvEngineTest : ILibTest
    {
        
        class Platform
        {
            vec2 m_Position;
            public RectangleF CheckRect { get; private set; }
            public Platform() { }
            public Platform(vec2 position)
            {
                m_Position = position;
                CheckRect = new RectangleF(position.x, position.y - 2, 15, 2);
            }
            public Platform(int x, int y) : this(new vec2(x, y))
            {
            }

            public void Draw(Graphics g)
            {
                g.DrawLine(Pens.Black, m_Position, m_Position + new vec2(15, 0));
                g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Green)), CheckRect);
            }
        }

        class Game
        {
            Platform[] platforms = new Platform[]
            {
                new Platform(0, 100),
                new Platform (15, 100),
                new Platform (30,100),
                new Platform (50, 80),
                new Platform (65, 80),
                new Platform (85, 60),
                new Platform (100, 60),
                new Platform(115, 60),
                new Platform (120, 65),
                new Platform (125, 70),
                new Platform (170, 70),
                new Platform (190, 70),
                new Platform (210, 70),
                new Platform (250, 70),
                new Platform (290, 70),
                new Platform (305, 60),
                new Platform (345, 60),
                new Platform (385, 60),
                new Platform (425, 60),
                new Platform (465, 60),
                new Platform (505, 60),
                new Platform (545, 60),
                new Platform (585, 60),
                new Platform (625, 60),
                new Platform (665, 60),
                new Platform (705, 60),
            };

            public float MostRightPoint {  get { return platforms[platforms.Length - 1].CheckRect.X + 15; } }

            public void Draw(Graphics g)
            {
                foreach (Platform p in platforms)
                {
                    p.Draw(g);
                }
            }

            public IReadOnlyList<Platform> GetPlatforms() { return platforms; }
        }

        class Player : IEvolutionaryAgent
        {
            enum Action
            {
                MoveLeft,
                MoveRight,
                Jump
            }
            enum ActionState
            {
                On,
                Off
            }

            static Random r = new Random();

            int m_iActionIndex = 0;
            Action[] m_Actions = null;
            ActionState[] m_ActionStates = null;

            bool m_bMoveLeft = false;
            bool m_bMoveRight = false;

            bool m_bJump = false;

            bool m_bJumping = false;
            int iChangeXWhileJumping = 0;
            float fChangeYWhileJumping = -2.0f;
            float fAccYWhileJumping = .08f;

            bool m_bGrounded = true;

            bool m_bAlive = true;
            bool m_bHasBred = false;

            RectangleF m_Body;
            Game m_Game;

            public Player()
            {
                m_Body = new RectangleF(0, 90, 10, 10);
            }
            public Player(Game game)
            {
                m_Game = game;
                m_Body = new RectangleF(0, 90, 10, 10);

                int iActionCount = r.Next(1000, 50000);

                m_Actions = new Action[iActionCount];
                m_ActionStates = new ActionState[iActionCount];

                for (int i = 0; i < iActionCount; ++i)
                {
                    m_Actions[i] = (Action)((Action[])Enum.GetValues(typeof(Action)))[r.Next(3)];
                    m_ActionStates[i] = (ActionState)((ActionState[])Enum.GetValues(typeof(ActionState)))[r.Next(2)];
                }
            }
            public void Draw(Graphics g)
            {
                if (m_Body.X > m_Game.MostRightPoint)
                {
                    g.FillEllipse(Brushes.Green, m_Body);
                }
                else
                {
                    if (m_bAlive)
                        g.FillEllipse(Brushes.Orange, m_Body);
                    else
                        g.FillEllipse(Brushes.Red, m_Body);
                }
            }

            public void Update(object objParam)
            {
                if (!m_bAlive)
                {
                    return;
                }

                m_bGrounded = false;
                foreach (Platform p in m_Game.GetPlatforms())
                {
                    if (p.CheckRect.IntersectsWith(m_Body))
                    {
                        m_bGrounded = true;
                        break;
                    }
                }

                switch (m_Actions[m_iActionIndex])
                {
                    case Action.Jump:
                        m_bJump = m_ActionStates[m_iActionIndex] == ActionState.On;
                        break;
                    case Action.MoveLeft:
                        m_bMoveLeft = m_ActionStates[m_iActionIndex] == ActionState.On;
                        break;
                    case Action.MoveRight:
                        m_bMoveRight = m_ActionStates[m_iActionIndex] == ActionState.On;
                        break;
                }

                if (++m_iActionIndex == m_ActionStates.Length)
                {
                    m_bAlive = false;
                }

                if (m_bGrounded)
                {
                    m_bJumping = false;
                    fChangeYWhileJumping = -2.0f;
                    iChangeXWhileJumping = (m_bMoveRight ? 1 : 0) + (m_bMoveLeft ? -1 : 0);

                    if (m_bMoveRight)
                    {
                        ++m_Body.X;
                    }
                    if (m_bMoveLeft)
                    {
                        --m_Body.X;
                    }
                    if (m_bJump)
                    {
                        m_bJumping = true;
                    }
                }

                if (m_bJumping || !m_bGrounded)
                {
                    m_Body.X += iChangeXWhileJumping;
                    m_Body.Y += fChangeYWhileJumping;
                    fChangeYWhileJumping += fAccYWhileJumping;
                }

                if (m_Body.Y > 101)
                {
                    m_bAlive = false;
                }
            }

            public class ComparePlayers : IComparable
            {
                float m_X;
                int m_InstructionCount;
                float m_MostRight;
                public ComparePlayers(float x, int instructionCount, float mostRight)
                {
                    m_X = x;
                    m_InstructionCount = instructionCount;
                    m_MostRight = mostRight;
                }
                public int CompareTo(object obj)
                {
                    ComparePlayers cp = (ComparePlayers)obj;
                    if (m_X > m_MostRight && cp.m_X > m_MostRight)
                    {
                        return m_InstructionCount < cp.m_InstructionCount ? 1 : (m_InstructionCount == cp.m_InstructionCount ? 0 : -1);
                    }
                    if (m_X > cp.m_X) return 1;
                    if (m_X == cp.m_X) return 0;
                    return -1;
                }
            }
            public IComparable GetFitness()
            {
                return new ComparePlayers(m_Body.X, m_Actions.Length, m_Game.MostRightPoint);
            }

            public IEnumerable<IEvolutionaryAgent> BreedWith(IEvolutionaryAgent agentB)
            {
                Player A = this;
                Player B = (Player)agentB;

                A.m_bHasBred = true;
                B.m_bHasBred = true;

                for (int c = 0; c < 2; ++c)
                {
                    int actionCount = Math.Max((A.m_Actions.Length + B.m_Actions.Length) / 2 + r.Next(-100, 100), 50);

                    Player C = new Player();

                    C.m_Actions = new Action[actionCount];
                    C.m_ActionStates = new ActionState[actionCount];

                    int maxFitness = Math.Max((int)(A.m_Body.X + B.m_Body.X), 2);
                    int cutoff = Math.Max((int)(A.m_Body.X), 1);

                    for (int i = 0; i < actionCount;)
                    {
                        int sectionCounts = r.Next(5, 100);

                        if (r.Next(100) == 0)
                        {
                            for (int j = 0; j < sectionCounts && i < actionCount; ++j, ++i)
                            {
                                C.m_Actions[i] = (Action)((Action[])Enum.GetValues(typeof(Action)))[r.Next(3)];
                                C.m_ActionStates[i] = (ActionState)((ActionState[])Enum.GetValues(typeof(ActionState)))[r.Next(2)];
                            }
                        }
                        else
                        { 
                            if (r.Next(maxFitness) < cutoff && i + sectionCounts < actionCount && i + sectionCounts < A.m_Actions.Length)
                            {
                                for (int j = 0; j < sectionCounts; ++j, ++i)
                                {
                                    C.m_Actions[i] = A.m_Actions[i];
                                    C.m_ActionStates[i] = A.m_ActionStates[i];
                                }
                            }
                            else if (i + sectionCounts < B.m_Actions.Length && i + sectionCounts < actionCount)
                            {
                                for (int j = 0; j < sectionCounts; ++j, ++i)
                                {
                                    C.m_Actions[i] = B.m_Actions[i];
                                    C.m_ActionStates[i] = B.m_ActionStates[i];
                                }
                            }
                            else
                            {
                                if (i == 0)
                                {
                                    for (; i < actionCount; ++i)
                                    {
                                        C.m_Actions[i] = (Action)((Action[])Enum.GetValues(typeof(Action)))[r.Next(3)];
                                        C.m_ActionStates[i] = (ActionState)((ActionState[])Enum.GetValues(typeof(ActionState)))[r.Next(2)];
                                    }
                                }
                                else
                                {
                                    Array.Resize(ref C.m_Actions, i);
                                    Array.Resize(ref C.m_ActionStates, i);
                                }
                                break;
                            }
                        }
                    }

                    C.m_Game = m_Game;

                    yield return C;
                }

                if (r.Next(100) == 0)
                {
                    yield return new Player(m_Game);
                }
            }

            public IEvolutionaryAgent GenerateRandomAgent(object objParam)
            {
                return new Player((Game)objParam);
            }

            public bool GetCanBreed()
            {
                return !m_bAlive;
            }
           
            public bool GetIsAlive()
            {
                return !m_bHasBred;
            }

            public float GetX() { return m_Body.X; }
            public int GetActionCount() { return m_Actions.Length; }
        }

        Game game = new Game();

        EvEngine engine = new EvEngine();

        public int IterationCount { get; set; }

        public string LibTest(out bool testResult)
        {
            testResult = true;
            if (engine.GlobalMaximum == null)
                return "";
            return "Best: " + ((Player)engine.GlobalMaximum).GetX() + "/" + ((int)game.MostRightPoint) + " in " + ((Player)engine.GlobalMaximum).GetActionCount() + " actions";
        }

        int yoffset = 50;
        public void PaintTest(Graphics g, Rectangle viewPort)
        {
            for (int i = 0; i < IterationCount; ++i)
            {
                engine.Update(null);
            }

            int x = 50;
            int y = yoffset;

            IReadOnlyList<EvEngine.AgentTracker> agents = engine.CurrentPopulation;
            foreach (EvEngine.AgentTracker agent in agents)
            {
                g.ResetTransform();
                g.TranslateTransform(x, y);

                g.DrawRectangle(Pens.Blue, 0, 0, game.MostRightPoint, 100);
                game.Draw(g);
                ((Player)agent.Agent).Draw(g);

                if ((x += (int)(game.MostRightPoint + 10)) + game.MostRightPoint >= viewPort.X + viewPort.Width)
                {
                    x = 50;
                    if ((y += 120) > viewPort.Height)
                    {
                        break;
                    }
                }
            }

        }

        public void KeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                IterationCount = Math.Max(IterationCount - 10, 1);
            }
            if (e.KeyCode == Keys.Up)
            {
                IterationCount += 10;
            }
            if (e.KeyCode == Keys.T)
            {
                IterationCount = Math.Max(IterationCount - 5, 1);
            }
            if (e.KeyCode == Keys.G)
            {
                IterationCount += 5;
            }

            if (e.KeyCode == Keys.W)
            {
                yoffset += 10;
            }
            if (e.KeyCode == Keys.S)
            {
                yoffset -= 10;
            }
        }

        public void KeyUp(KeyEventArgs e)
        {
        }

        public void Initialize()
        {
            IterationCount = 10000;
            engine.PopulationMax = 10000;
            //engine.BreedAlgorithm = EvEngine.BreedingType.Dynamic;
            //engine.PopulationMaximumAge = 10000;
            engine.PopulationMaximumGenerationalAge = 3;
            // 25% prune rate
            engine.BreedPruneLeastFitPercent = .50;
            engine.Initialize<Player>(50, game);
        }
    }
}
