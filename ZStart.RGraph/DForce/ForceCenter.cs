using UnityEngine;
using ZStart.RGraph.Model;

namespace ZStart.RGraph.DForce
{
    public class ForceCenter : DefaultForce
    {
        public static readonly string NAME = "Center";
        private double x, y;

        public ForceCenter() { }

        public ForceCenter(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public override void Calculate(double alpha)
        {
            double sx = 0, sy = 0;
            int n = nodes.Length;

            foreach (NodeInfo node in nodes)
            {
                sx += node.X;
                sy += node.Y;
            }

            sx = sx / n - x;
            sy = sy / n - y;

            foreach (NodeInfo node in nodes)
            {
                node.X -= sx;
                node.Y -= sy;
                Debug.LogWarning("ForceCenter..." + node.ToString());
            }
        }

        public ForceCenter SetX(double x)
        {
            this.x = x;
            return this;
        }

        public ForceCenter SetY(double y)
        {
            this.y = y;
            return this;
        }
    }
}
