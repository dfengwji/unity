using System;
using UnityEngine;
using ZStart.RGraph.Model;
using ZStart.RGraph.Structure;

namespace ZStart.RGraph.DForce
{
    public class ForceBody:DefaultForce
    {
        public static readonly string NAME = "Charge";
        private static readonly double DEFAULT_STRENGTH = -30;
        private static readonly double DEFAULT_THETA2 = 0.81;

        private double distanceMin2 = 1;
        private double distanceMax2 = Double.PositiveInfinity;

        private double[] strengths;
        private NodeCalculateCall strengthCall;
        private double theta2 = DEFAULT_THETA2;
        private double alpha;

        private NodeInfo nowNode;
        QuadTree tree;
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
            for (int i = 0; i < nodes.Length; i++)
            {
                NodeInfo node = nodes[i];
                strengths[node.index] = Strength(i);
            }
            tree = QuadTree.Create(nodes, SetX, SetY).VisitAfter(Accumulate);
        }

        public override void Calculate(double alpha)
        {
            this.alpha = alpha;
            
            for (int i = 0;i < nodes.Length;i += 1)
            {
                this.nowNode = nodes[i];
                tree.Visit(ApplyVisit);
            }
        }

        public override void Clear()
        {
            base.Clear();
            strengths = null;
        }

        public ForceBody Strength(NodeCalculateCall c)
        {
            strengthCall = c;
            return this;
        }

        public ForceBody Theta(double theta)
        {
            theta2 = theta * theta;
            return this;
        }

        public ForceBody DistanceMin(double distanceMin)
        {
            distanceMin2 = distanceMin * distanceMin;
            return this;
        }

        public ForceBody DistanceMax(double distanceMax)
        {
            distanceMax2 = distanceMax * distanceMax;
            return this;
        }

      
        private double Strength(int i)
        {
            if (strengthCall == null)
            {
                return DEFAULT_STRENGTH;
            }
            return strengthCall.Invoke(nodes[i]);
        }

        private bool Accumulate(QuadTree.TreeNode node, double x0, double y0, double x1, double y1)
        {
            double strength = 0, weight = 0;
            double x = 0, y = 0;
            QuadTree.TreeNode tmp;
            if (!node.IsLeaf())
            {
                for (int i = 0; i < 4; i++)
                {
                    double c;
                    tmp = node.quadrants[i];
                    if (tmp != null && (c = Mathf.Abs((float)tmp.strength)) > 0)
                    {
                        strength += tmp.strength;
                        weight += c;
                        x += c * tmp.x;
                        y += c * tmp.y;
                    }
                }
                node.x = x / weight;
                node.y = y / weight;
            }
            else
            {
                tmp = node;
                tmp.x = tmp.data.X;
                tmp.y = tmp.data.Y;
                do
                {
                    strength += strengths[tmp.data.index];
                } while ((tmp = tmp.next) != null);
            }
            node.strength = strength;
            Debug.LogWarning("ForceBody Accumulate..." + strength);
            return false;
        }

        private bool ApplyVisit(QuadTree.TreeNode node, double x0, double y0, double x1, double y1)
        {
            if (node.strength == 0)
            {
                return true;
            }
            double x = node.x - this.nowNode.X, y = node.y - this.nowNode.Y;
            double w = x1 - x0;
            double l = x * x + y * y;

            if (w * w / theta2 < l)
            {
                if (l < distanceMax2)
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
                    if (l < distanceMin2)
                    {
                        l = Mathf.Sqrt((float)(distanceMin2 * l));
                    }
                    this.nowNode.VX += x * node.strength * alpha / l;
                    this.nowNode.VY += y * node.strength * alpha / l;
                }
                Debug.LogWarning("ForceBody apply..."+this.nowNode.ToString());
                return true;
            }
            else if (!node.IsLeaf() || l >= distanceMax2)
            {
                return false;
            }

            if (node.data != this.nowNode || node.next != null)
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
                if (l < distanceMin2)
                {
                    l = Mathf.Sqrt((float)(distanceMin2 * l));
                }
            }

            do
            {
                if (node.data != this.nowNode)
                {
                    w = strengths[node.data.index] * alpha / l;
                    this.nowNode.VX += x * w;
                    this.nowNode.VY += y * w;
                }
            } while ((node = node.next) != null);

            return false;
        }

        private static double SetX(NodeInfo node)
        {
            if (node != null)
            {
                return node.X;
            }
            return 0;
        }

        private static double SetY(NodeInfo node)
        {
            if (node != null)
            {
                return node.Y;
            }
            return 0;
        }
    }
}
