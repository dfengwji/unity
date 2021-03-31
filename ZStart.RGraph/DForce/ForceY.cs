using ZStart.RGraph.Model;
using ZStart.RGraph.Structure;

namespace ZStart.RGraph.DForce
{
    public class ForceY:DefaultForce
    {
        public static readonly string NAME = "Node_Y";
        private static readonly double DEFAULT_STRENGTH = 0.1;

        private double[] strengths;
        private double[] yz;
        private NodeCalculateCall yCall;
        private NodeCalculateCall strengthCall;

        public override void Init(Simulation simulation)
        {
            base.Init(simulation);
            Update(simulation);
        }

        public override void Update(Simulation simulation)
        {
            base.Update(simulation);
            if (nodes == null)
            {
                return;
            }
            int size = nodes.Length;
            strengths = new double[size];
            yz = new double[size];
            for (int i = 0; i < size; i++)
            {
                yz[i] = Y(i);
                strengths[i] = Strength(i);
            }
        }

        public override void Calculate(double alpha)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                NodeInfo node = nodes[i];
                node.VY += (yz[i] - node.Y) * strengths[i] * alpha;
            }
        }

        public ForceY SetY(NodeCalculateCall c)
        {
            yCall = c;
            return this;
        }

        public ForceY Strength(NodeCalculateCall c)
        {
            strengthCall = c;
            return this;
        }

        private double Y(int i)
        {
            if (yCall == null)
            {
                return 0;
            }
            return yCall.Invoke(nodes[i]);
        }

        private double Strength(int i)
        {
            if (strengthCall == null)
            {
                return DEFAULT_STRENGTH;
            }
            return strengthCall.Invoke(nodes[i]);
        }
    }
}
