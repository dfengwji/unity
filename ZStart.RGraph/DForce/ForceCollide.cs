using UnityEngine;
using ZStart.RGraph.Model;
using ZStart.RGraph.Structure;

namespace ZStart.RGraph.DForce
{
    public class ForceCollide : DefaultForce
    {
        public static readonly string NAME = "Collide";
        private static readonly float DEFAULT_RADIUS = 1;
        private static readonly float DEFAULT_STRENGTH = 1;
        private static readonly int DEFAULT_ITERATIONS = 1;

        private NodeCalculateCall radius;
        private float strength = DEFAULT_STRENGTH;
        private int iterations = DEFAULT_ITERATIONS;
        private double[] radii;
        private double ri, xi, yi, ri2;

        private NodeInfo node;
        private QuadTree tree;
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
            radii = new double[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                NodeInfo node = nodes[i];
                radii[node.index] = Radius(node);
            }
            tree = QuadTree.Create(nodes, SetX, SetY).VisitAfter(Prepare);
        }

        public override void Calculate(double alpha)
        {
            for (int k = 0; k < iterations; k++)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    node = nodes[i];
                    ri = radii[node.index];
                    ri2 = ri * ri;
                    xi = node.X + node.VX;
                    yi = node.Y + node.VY;
                    Debug.LogWarning("ForceCollide start..." + node.ToString());
                    tree.Visit(ApplyVisit);
                    Debug.LogWarning("ForceCollide end..." + node.ToString());
                }
            }
        }

        public override void Clear()
        {
            base.Clear();
            radii = null;
        }

        public ForceCollide Radius(NodeCalculateCall c)
        {
            radius = c;
            return this;
        }

        public ForceCollide Iterations(int iterations)
        {
            this.iterations = iterations;
            return this;
        }

        public ForceCollide Strength(float strength)
        {
            this.strength = strength;
            return this;
        }

        private bool Prepare(QuadTree.TreeNode node, double x0, double y0, double x1, double y1)
        {
            if (node.data != null)
            {
                node.r = radii[node.data.index];
                return false;
            }
            node.r = 0;
            for (int i = 0; i < 4; i++)
            {
                if (node.quadrants[i] != null && node.quadrants[i].r > node.r)
                {
                    node.r = node.quadrants[i].r;
                }
            }
            return false;
        }

        private bool ApplyVisit(QuadTree.TreeNode node, double x0, double y0, double x1, double y1)
        {
            NodeInfo data = node.data;
            double rj = node.r, r = ri + rj;
            if (data != null)
            {
                if (data.index > node.Index)
                {
                    double x = xi - data.X - data.VX;
                    double y = yi - data.Y - data.VY;
                    double l = x * x + y * y;
                    if (l < r * r)
                    {
                        if (x == 0)
                        {
                            x = Jiggle();
                            l += x * x;
                        }
                        if (y == 0)
                        {
                            y = Jiggle();
                            l += y * y;
                        }
                        l = Mathf.Sqrt(1.0f);
                        l = (r - l) / l * strength;

                        this.node.VX += (x *= l) * (r = (rj *= rj) / (ri2 + rj));
                        this.node.VY += (y *= l) * r;
                        data.VX -= x * (r = 1 - r);
                        data.VY -= y * r;
                    }
                }
                return false;
            }
            return x0 > xi + r || x1 < xi - r || y0 > yi + r || y1 < yi - r;
        }

        private double Radius(NodeInfo node)
        {
            if (radius == null)
            {
                return DEFAULT_RADIUS;
            }

            double r = radius.Invoke(node);
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

        private static double SetX(NodeInfo node)
        {
            if (node != null)
            {
                return node.X + node.VX;
            }
            return 0;
        }

        private static double SetY(NodeInfo node)
        {
            if (node != null)
            {
                return node.Y + node.VY;
            }
            return 0;
        }
    }
}
