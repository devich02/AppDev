using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning
{

    public class EvEngine
    {
        public class AgentTracker
        {
            public IEvolutionaryAgent Agent { get; internal set; }
            public ulong Age { get; internal set; }
            public ulong GenerationalAge { get; internal set; }
            public AgentTracker() { }
            public AgentTracker(IEvolutionaryAgent agent)
            {
                this.Agent = agent;
                this.Age = 0;
                this.GenerationalAge = 0;
            }
        }

        private List<AgentTracker> m_listAgents = new List<AgentTracker>();
        public IReadOnlyList<AgentTracker> CurrentPopulation {  get { return m_listAgents; } }

        /// <summary>
        /// Gets or sets the maximum population count. Population may exceed PopulationMax, but not by the engine breeding agents.
        /// </summary>
        public long PopulationMax { get; set; }

        /// <summary>
        /// Gets or sets the minimum population count. Can be zero. In non-zero, forces a breeding cycle before removing the agents which would bring the total population lower than the minimum.
        /// </summary>
        public long PopulationMin { get; set; }
        
        /// <summary>
        /// The number of agents in the population.
        /// </summary>
        public long Population { get { return m_listAgents.Count; } }

        /// <summary>
        /// Gets or sets a value indicating whether the global maximum should be kept. Default true.
        /// </summary>
        public bool PopulationKeepGlobalMax { get; set; }

        /// <summary>
        /// Gets or sets the maximum age of any member of the population. Once a member has reached this age, it will be deleted no matter what. If this value is 0, it is ignored.
        /// </summary>
        public ulong PopulationMaximumAge { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of generations a member of the population can be alive fore. If this value is 0, it is ignored.
        /// </summary>
        public ulong PopulationMaximumGenerationalAge { get; set; }

        /// <summary>
        /// Gets the global maximum (best fit solution thus far).
        /// </summary>
        public IEvolutionaryAgent GlobalMaximum { get; private set; }

        /// <summary>
        /// For agents elgible for breeding, this is the chance that they will breed.
        /// </summary>
        public double BreedChance { get; set; }

        public enum BreedingType
        {
            /// <summary>
            /// Waits for the entire generation to be ready to breed before breeding
            /// </summary>
            Generations,

            /// <summary>
            /// Breeds as soon as at least two individuals are capabale
            /// </summary>
            Dynamic
        }

        public enum BreedScanType
        {
            /// <summary>
            /// Given a list of breedable agents, L, performs a simple scan until the population maximum is met.
            /// <para>for (i = 0; i &lt; Count(L); ++i)</para>
            /// <para>for (j = 0; j &lt; Count(L); ++j)</para>
            /// <para>if (i != j) breed L[i] and L[j];</para>
            /// </summary>
            MostFitToLeastFit,

            /// <summary>
            /// Given a list of breedable agents, L, performs a permutation of breeding where the most fit agents are bred, completely ignoring the least fit at first, until the population maximum is met.
            /// </summary>
            MostFitPermutations
        }

        /// <summary>
        /// Gets or sets the breeding selection algorithm. See the enum BreedScanType for more information about how these affect the algorithm. Default is MostFitPermutations.
        /// </summary>
        public BreedScanType BreedSelectionAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the breeding algorithm. See the enum BreedingType for more information about how these affect the algorithm. Default is Generations.
        /// </summary>
        public BreedingType BreedAlgorithm { get; set; }

        /// <summary>
        /// Gets or sets the prune percentage. Valid when BreedAlgorithm = Generations, will simply remove the lower BreedPruneLeastFitPercent percentage of the population. Default is 5%.
        /// </summary>
        public double BreedPruneLeastFitPercent { get; set; }

        /// <summary>
        /// The number of times Update has been called for this population.
        /// </summary>
        public long IterationCount { get; private set; }

        public EvEngine()
        {
            PopulationMax = 100;
            PopulationMin = 0;
            BreedChance = 100;
            BreedSelectionAlgorithm = BreedScanType.MostFitPermutations;
            BreedAlgorithm = BreedingType.Generations;
            BreedPruneLeastFitPercent = .05;
            GlobalMaximum = null;
            PopulationKeepGlobalMax = true;
        }

        public void Initialize<T>(int initialPopulationCount, Object objParam) where T : IEvolutionaryAgent, new()
        {
            T generator = new T();

            m_listAgents.Clear();
            for (int i = 0; i < initialPopulationCount; ++i)
            {
                m_listAgents.Add(new AgentTracker(generator.GenerateRandomAgent(objParam)));
            }
        }

        private List<IEvolutionaryAgent> m_listBreedableAgents = new List<IEvolutionaryAgent>();
        public void Update(Object objParam)
        {
            foreach (AgentTracker agent in m_listAgents)
            {
                ++agent.Age;
                agent.Agent.Update(objParam);
            }

            m_listBreedableAgents.Clear();

            for (int i = 0; i < m_listAgents.Count;)
            {
                if ((!PopulationKeepGlobalMax || m_listAgents[i].Agent != GlobalMaximum) 
                    && 
                    (!m_listAgents[i].Agent.GetIsAlive()
                    || (PopulationMaximumGenerationalAge != 0 && m_listAgents[i].GenerationalAge > PopulationMaximumGenerationalAge)
                    || (PopulationMaximumAge != 0 && m_listAgents[i].Age > PopulationMaximumAge)))
                {
                    if (m_listAgents.Count - 1 < PopulationMin)
                    {
                        m_listBreedableAgents.Add(m_listAgents[i].Agent);
                    }
                    m_listAgents.RemoveAt(i);
                }
                else
                {
                    ++i;
                }
            }

            foreach (AgentTracker agent in m_listAgents)
            {
                if (agent.Agent.GetCanBreed())
                {
                    m_listBreedableAgents.Add(agent.Agent);
                }
            }

            if (((BreedAlgorithm == BreedingType.Dynamic && m_listBreedableAgents.Count > 1)  || (BreedAlgorithm == BreedingType.Generations && m_listBreedableAgents.Count == Population))
                && Population < PopulationMax)
            {
                m_listBreedableAgents.Sort((Comparison<IEvolutionaryAgent>)((IEvolutionaryAgent a, IEvolutionaryAgent b) => b.GetFitness().CompareTo(a.GetFitness())));

                m_listAgents.ForEach(a => ++a.GenerationalAge);

                if (GlobalMaximum == null)
                {
                    GlobalMaximum = m_listBreedableAgents[0];
                }
                else if (m_listBreedableAgents[0].GetFitness().CompareTo(GlobalMaximum.GetFitness()) == 1)
                {
                    GlobalMaximum = m_listBreedableAgents[0];
                }

                if (BreedAlgorithm == BreedingType.Generations && BreedPruneLeastFitPercent < 1)
                {
                    int removeCount = (int)(BreedPruneLeastFitPercent * m_listBreedableAgents.Count);

                    if (m_listAgents.Count - removeCount >= PopulationMin)
                    {
                        for (int i = 0; i < removeCount; ++i)
                        {
                            for (int j = 0; j < m_listAgents.Count; ++j)
                            {
                                if (m_listAgents[j].Agent == m_listBreedableAgents[m_listBreedableAgents.Count - 1])
                                {
                                    m_listAgents.RemoveAt(j);
                                    break;
                                }
                            }
                            m_listBreedableAgents.RemoveAt(m_listBreedableAgents.Count - 1);
                        }
                    }
                }

                if (BreedSelectionAlgorithm == BreedScanType.MostFitToLeastFit)
                {
                    for (int i = 0; i < m_listBreedableAgents.Count && Population < PopulationMax; ++i)
                    {
                        for (int j = 1; j < m_listBreedableAgents.Count && Population < PopulationMax; ++j)
                        {
                            if (i == j) continue;
                            foreach (IEvolutionaryAgent child in m_listBreedableAgents[i].BreedWith(m_listBreedableAgents[j]))
                            {
                                m_listAgents.Add(new AgentTracker(child));
                                if (Population >= PopulationMax)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (BreedSelectionAlgorithm == BreedScanType.MostFitPermutations)
                {
                    List<IEvolutionaryAgent> listAParents = new List<IEvolutionaryAgent>();
                    listAParents.Add(m_listBreedableAgents[0]);
                    IEvolutionaryAgent BParent = m_listBreedableAgents[1];
                    int iNextBParent = 1;

                    do
                    {
                        foreach (IEvolutionaryAgent A in listAParents)
                        {
                            foreach (IEvolutionaryAgent child in A.BreedWith(BParent))
                            {
                                m_listAgents.Add(new AgentTracker(child));
                                if (Population >= PopulationMax)
                                {
                                    break;
                                }
                            }
                        }

                        if (Population >= PopulationMax)
                        {
                            break;
                        }
                        else if (++iNextBParent == m_listBreedableAgents.Count)
                        {
                            listAParents.Add(BParent);
                        }
                        else if (iNextBParent > m_listBreedableAgents.Count)
                        {
                            break;
                        }
                        else
                        {
                            BParent = m_listBreedableAgents[iNextBParent];
                        }

                    } while (true);
                }
            }

        }
    }
}
