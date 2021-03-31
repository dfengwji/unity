using System.Collections.Generic;
using ZStart.RGraph.Model;

namespace ZStart.RGraph.DForce
{
    public interface IForce
    {
        /**
           * optionally implement force.initialize to receive the simulation.
           */
        void Init(Simulation simulation);

        void Update(Simulation simulation);
        /**
         * apply this force to modify nodes’ positions or velocities.
         * @param alpha from 0 (inclusive) to 1 (inclusive), default 1
         */
        void Calculate(double alpha);

        void Clear();
    }

    public abstract class DefaultForce : IForce
    {
        protected NodeInfo[] nodes;

        protected static double Jiggle()
        {
            return (UnityEngine.Random.Range(0.0f, 1.0f) - 0.5f) * 1e-6;
        }

        public virtual void Calculate(double alpha){}

        public virtual void Init(Simulation simulation)
        {
            //nodes = simulation.GetNodes();
        }

        public virtual void Update(Simulation simulation)
        {
            nodes = simulation.GetNodes();
        }

        public virtual void Clear()
        {
            nodes = null;
        }
    }
}
