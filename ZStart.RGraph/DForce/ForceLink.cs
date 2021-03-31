using UnityEngine;
using ZStart.RGraph.Model;
using ZStart.RGraph.Structure;

namespace ZStart.RGraph.DForce
{
    public class ForceLink:DefaultForce
    {
        public static readonly string NAME = "Link";
        private static readonly int ITERATIONS = 1;
        private static readonly double DEFAULT_DISTANCE = 30;

        private LinkInfo[] links = default;
        private double[] distances;
        private double[] strengths;
        private double[] bias;
        private double[] count;

        private LinkCalculateCall strengthCall;
        private LinkCalculateCall distanceCall;

        public override void Init(Simulation simulation)
        {
            base.Init(simulation);
            Links(simulation.GetLinks());
            Update(simulation);
        }

        public override void Update(Simulation simulation)
        {
            base.Update(simulation);
            Links(simulation.GetLinks());
            if (nodes == null)
            {
                return;
            }
            count = new double[nodes.Length];
            for (int i = 0; i < links.Length; i++)
            {
                LinkInfo link = links[i];
                link.index = i;
                count[link.from.index] += 1;
                count[link.to.index] += 1;
            }

            bias = new double[links.Length];
            for (int i = 0; i < links.Length; i++)
            {
                LinkInfo link = links[i];
                bias[i] = count[link.from.index] / (count[link.from.index] + count[link.to.index]);
            }

            strengths = new double[links.Length];
            InitializeStrength();

            distances = new double[links.Length];
            InitializeDistance();
        }

        public override void Calculate(double alpha)
        {
            for (int k = 0; k < ITERATIONS; k++)
            {
                for (int i = 0; i < links.Length; i++)
                {
                    LinkInfo link = links[i];
                    NodeInfo source = link.from;
                    NodeInfo target = link.to;
                    double x = target.X + target.VX - source.X - source.VX;
                    double y = target.Y + target.VY - source.Y - source.VY;
                    if (x == 0)
                    {
                        x = Jiggle();
                    }
                    if (y == 0)
                    {
                        y = Jiggle();
                    }
                    double l = Mathf.Sqrt((float)(x * x + y * y));
                    l = (l - distances[i]) / l * alpha * strengths[i];
                    x *= l;
                    y *= l;
                    double b = bias[i];
                    target.VX -= x * b;
                    target.VY -= y * b;
                    b = 1 - b;
                    source.VX += x * b;
                    source.VY += y * b;
                    Debug.LogWarning("ForceLink Calculate..." + target.ToString());
                    Debug.LogWarning("ForceLink Calculate..." + source.ToString());
                }
            }
        }

        public ForceLink Strength(LinkCalculateCall c)
        {
            strengthCall = c;
            InitializeStrength();
            return this;
        }

        public ForceLink Distance(LinkCalculateCall c)
        {
            distanceCall = c;
            InitializeDistance();
            return this;
        }

        public ForceLink Links(LinkInfo[] array)
        {
            if (links == null)
            {
                this.links = new LinkInfo[array.Length];
            }
            this.links = array;
            return this;
        }

        private void InitializeStrength()
        {
            if (nodes == null)
            {
                return;
            }
            for (int i = 0; i < links.Length; i++)
            {
                strengths[i] = Strength(i);
            }
        }

        private void InitializeDistance()
        {
            if (nodes == null)
            {
                return;
            }
            for (int i = 0; i < links.Length; i++)
            {
                distances[i] = Distance(i);
            }
        }

        private double Strength(int i)
        {
            LinkInfo link = links[i];
            if (strengthCall != null)
            {
                return strengthCall.Invoke(link);
            }
            return 1 / Mathf.Min((float)count[link.from.index], (float)count[link.to.index]);
        }

        private double Distance(int i)
        {
            LinkInfo link = links[i];
            double distance = -1;
            if (distanceCall != null)
            {
                distance = distanceCall.Invoke(link);
            }
            if (distance < 0)
            {
                return DEFAULT_DISTANCE;
            }
            else
            {
                return distance;
            }
        }
    }
}
