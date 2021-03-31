using UnityEngine;
using ZStart.RGraph.Model;
using ZStart.RGraph.Structure;

namespace ZStart.RGraph.DForce
{
    public class ForceRadial:DefaultForce
    {
        public static readonly string NAME = "Radial";
        private static readonly double DEFAULT_RADIUS = 1;

        private NodeCalculateCall radiusCall, strengthCall;
        private double x, y;
        private double[] radiuses;
        private double[] strengths;

        public ForceRadial() { }

        public ForceRadial(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

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
            radiuses = new double[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                if (radiusCall != null)
                    radiuses[i] = radiusCall(nodes[i]);
                else
                    radiuses[i] = 0f;
                if (strengthCall != null)
                    strengths[i] = strengthCall(nodes[i]);
                else
                    strengths[i] = 0f;
            }
        }

        public override void Calculate(double alpha)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                NodeInfo node = nodes[i];
                double dx = node.X - x, dy = node.Y - y;
                dx = dx == 0 ? 1e-6 : dx;
                dy = dy == 0 ? 1e-6 : dy;
                double r = Mathf.Sqrt((float)(dx * dx + dy * dy));
                double k = (radiuses[i] - r) * strengths[i] * alpha / r;

                node.VX += dx * k;
                node.VY += dy * k;
                Debug.LogWarning("ForceRadial Calculate..." + node.ToString());
            }
        }

        public ForceRadial Rradius(NodeCalculateCall c)
        {
            radiusCall = c;
            return this;
        }

        public ForceRadial Strength(NodeCalculateCall c)
        {
            strengthCall = c;
            return this;
        }

        public ForceRadial SetX(double x)
        {
            this.x = x;
            return this;
        }

        public ForceRadial SetY(double y)
        {
            this.y = y;
            return this;
        }

        private double Radius(NodeInfo node)
        {
            if (radiusCall == null)
            {
                return DEFAULT_RADIUS;
            }

            double r = radiusCall.Invoke(node);
            if (r < 0)
            {
                return -r;
            }
            if (r == 0)
            {
                return DEFAULT_RADIUS;
            }
            return r;
        }

        private double Strength(NodeInfo node)
        {
            if (strengthCall == null)
            {
                return 0;
            }
            return strengthCall.Invoke(node);
        }
    }
}
