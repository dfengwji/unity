using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZStart.RGraph.Model;

namespace ZStart.RGraph.DForce
{
    public class Simulation
    {
        public static readonly float INITIAL_RADIUS = 2f;
        public static readonly float INITIAL_ANGLE = Mathf.PI * (3 - Mathf.Sqrt(5));
        public static readonly int DEFAULT_ITERATIONS = 1;

        public enum Status
        {
            Running,
            Pause,
            Stop
        }

        private float initialRadius;
        private float initialAngle;
        private float alpha;
        private float alphaMin;
        private float alphaDecay;
        private float alphaTarget;
        private float velocityDecay;
        private Status status = Status.Stop;
        private Dictionary<string, IForce> forces;
        private int iterations = DEFAULT_ITERATIONS;

        private List<NodeInfo> nodes = new List<NodeInfo>(20);
        private List<LinkInfo> links = new List<LinkInfo>(20);

        private UnityAction<object> callBack = default;
       
        public Simulation SetIterations(int i)
        {
            iterations = i;
            return this;
        }

        public NodeInfo[] GetNodes()
        {
            return nodes.ToArray();
        }

        public LinkInfo[] GetLinks()
        {
            return links.ToArray();
        }

        public double Alpha
        {
            get { return alpha; }
        }

        public double AlphaMin
        {
            get { return alphaMin; }
        }

        public double AlphaDecay
        {
            get { return alphaDecay; }
        }

        public double AlphaTarget
        {
            get { return alphaTarget; }
        }

        public double VelocityDecay
        {
            get { return 1.0f - velocityDecay; }
        }

        public IForce GetForce(string name)
        {
            if (!string.IsNullOrEmpty(name) && forces.ContainsKey(name))
            {
                return forces[name];
            }
            return null;
        }

        public void RemoveForce(string name)
        {
            if (string.IsNullOrEmpty(name) || !forces.ContainsKey(name))
            {
                return;
            }
            forces.Remove(name);
        }

        public void SetCallback(UnityAction<object> callback)
        {
            this.callBack = callback;
        }

        public void Start()
        {
            InitializeNodes();
            foreach (IForce force in forces.Values)
            {
                force.Update(this);
            }
            status = Status.Running;
        }

        public void Pause()
        {
            status = Status.Pause;
        }

        public void Stop()
        {
            nodes.Clear();
            links.Clear();
            foreach (IForce force in forces.Values)
            {
                force.Clear();
            }
            status = Status.Stop;
        }
        
        public void AddNode(NodeInfo info)
        {
            if (info == null || HadNode(info.UID))
                return;
            nodes.Add(info);
        }

        public void AddLink (LinkInfo info)
        {
            if (info == null || HadLink(info.UID))
                return;
           
            links.Add(info);
        }

        public bool HadNode(string uid)
        {
            for (int i = 0;i < nodes.Count; i += 1)
            {
                if(nodes[i].UID == uid)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HadLink(string uid)
        {
            for (int i = 0; i < links.Count; i += 1)
            {
                if (links[i].UID == uid)
                {
                    return true;
                }
            }
            return false;
        }

        public NodeInfo Find(double x, double y, double radius)
        {
            NodeInfo closest = null;
            double radius2;
            if (radius <= 0)
            {
                radius2 = Double.PositiveInfinity;
            }
            else
            {
                radius2 = radius * radius;
            }

            foreach (var node in nodes)
            {
                double dx = x - node.LastPosition.x;
                double dy = y - node.LastPosition.y;
                double d2 = dx * dx + dy * dy;
                if (d2 < radius2)
                {
                    closest = node;
                    break;
                }
            }

            return closest;
        }

        public void Step()
        {
            if (status != Status.Running)
                return;
            Tick(iterations);
            if (callBack != null)
            {
                callBack.Invoke(false);
            }
            if (alpha < alphaMin)
            {
                if (callBack != null)
                {
                    callBack.Invoke(true);
                }
            }
        }

        private void Tick(int iterations)
        {
            if (iterations <= 0)
            {
                iterations = DEFAULT_ITERATIONS;
            }

            for (int k = 0; k < iterations; k++)
            {
                alpha += (alphaTarget - alpha) * alphaDecay;

                foreach (IForce force in forces.Values)
                {
                    force.Calculate(alpha);
                }

                foreach (NodeInfo node in nodes)
                {
                    Debug.LogWarning("Simulation start..." + node.ToString());
                    if (node.fx == Double.MaxValue)
                    {
                        node.X += node.VX *= velocityDecay;
                    }
                    else
                    {
                        node.X = node.fx;
                        node.VX = 0;
                    }
                    if (node.fy == Double.MaxValue)
                    {
                        node.Y += node.VY *= velocityDecay;
                    }
                    else
                    {
                        node.Y = node.fy;
                        node.VY = 0;
                    }
                    Debug.LogWarning("Simulation end..."+node.ToString());
                }
            }
        }

        private void InitializeNodes()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                NodeInfo node = nodes[i];
                node.index = i;
                if (node.fx != Double.MaxValue)
                {
                    node.X = node.fx;
                }
                if (node.fy != Double.MaxValue)
                {
                    node.Y = node.fy;
                }
                if (node.fx == Double.MaxValue || node.fy == Double.MaxValue)
                {
                    float radius = initialRadius * Mathf.Sqrt(i);
                    float angle = i * initialAngle;
                    node.X = radius * Mathf.Cos(angle);
                    node.Y = radius * Mathf.Sin(angle);
                }
            }
        }

        public Simulation(Builder builder)
        {
            this.initialRadius = builder.initialRadius;
            this.initialAngle = builder.initialAngle;
            this.alpha = builder.alpha;
            this.alphaMin = builder.alphaMin;
            this.alphaDecay = builder.alphaDecay;
            this.alphaTarget = builder.alphaTarget;
            this.velocityDecay = builder.velocityDecay;

            if (builder.nodes == null)
            {
                this.nodes = new List<NodeInfo>(10);
            }
            else
            {
                this.nodes = builder.nodes;
            }

            if (builder.links == null)
            {
                this.links = new List<LinkInfo>(10);
            }
            else
            {
                this.links = builder.links;
            }

            if (builder.forces == null)
            {
                this.forces = new Dictionary<string, IForce>(5);
            }
            else
            {
                this.forces = builder.forces;
            }

            InitializeNodes();

            foreach (IForce force in forces.Values)
            {
                force.Init(this);
            }
        }

        public class Builder
        {
            public float initialRadius;
            public float initialAngle;
            public float alpha;
            public float alphaMin;
            public float alphaDecay;
            public float alphaTarget;
            public float velocityDecay;

            public List<NodeInfo> nodes = new List<NodeInfo>(20);
            public List<LinkInfo> links = new List<LinkInfo>(20);
            public Dictionary<string, IForce> forces;

            public Builder()
            {
                this.initialRadius = INITIAL_RADIUS;
                this.initialAngle = INITIAL_ANGLE;
                this.alpha = 0.1f;
                this.alphaMin = 0.001f;
                this.alphaDecay = 1 - Mathf.Pow(alphaMin, 1.0f / 300f);
                this.alphaTarget = 0f;
                this.velocityDecay = 0.3f;
            }

            public Builder Nodes(List<NodeInfo> array)
            {
                nodes.Clear();
                if(array != null)
                {
                    nodes.AddRange(array);
                }
                return this;
            }

            public Builder Links(List<LinkInfo> array)
            {
                links.Clear();
                if (array != null)
                {
                    links.AddRange(array);
                }
                return this;
            }

            public Builder AddForce(string name, IForce force)
            {
                if (name == null || force == null)
                {
                    return this;
                }
                if (forces == null)
                {
                    forces = new Dictionary<string, IForce>(10);
                }
                forces.Add(name, force);
                return this;
            }

            public Builder InitialRadius(float radius)
            {
                if (radius > 0)
                {
                    this.initialRadius = radius;
                }
                return this;
            }

            public Builder InitialAngle(float angle)
            {
                if (angle > 0)
                {
                    this.initialAngle = angle;
                }
                return this;
            }

            public Simulation Build()
            {
                return new Simulation(this);
            }
        }
    }
}
