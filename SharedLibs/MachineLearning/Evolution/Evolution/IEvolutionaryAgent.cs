using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning
{
    public interface IEvolutionaryAgent
    {
        // Evolutionary agorithm requirements
        IComparable GetFitness();
        IEnumerable<IEvolutionaryAgent> BreedWith(IEvolutionaryAgent agentB);

        // Tracking requirements
        void Update(Object objParam);
        bool GetIsAlive();
        bool GetCanBreed();
        IEvolutionaryAgent GenerateRandomAgent(Object objParam);
    }
}
