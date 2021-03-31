using ZStart.RGraph.Model;
using ZStart.RGraph.Structure;

namespace ZStart.RGraph.DForce
{
    public class ForceX:DefaultForce
    {
        public static readonly string NAME = "Node_X";
        private static readonly double DEFAULT_STRENGTH = 0.1;

        private double[] strengths;
        private double[] xz;
        private NodeCalculateCall xCallback;
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
            strengths = new double[nodes.Length];
            xz = new double[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                xz[i] = SetX(i);
                strengths[i] = Strength(i);
            }
        }

        public override void Calculate(double alpha)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                NodeInfo node = nodes[i];
                node.VX += (xz[i] - node.X) * strengths[i] * alpha;
            }
        }

        public ForceX X(NodeCalculateCall c)
        {
            xCallback = c;
            return this;
        }

        public ForceX Strength(NodeCalculateCall c)
        {
            strengthCall = c;
            return this;
        }
        
        private double SetX(int i)
        {
            if (xCallback == null)
            {
                return 0;
            }
            return xCallback.Invoke(nodes[i]);
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
