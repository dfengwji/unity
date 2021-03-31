using System.Collections.Generic;

namespace ZStart.RGraph.Common
{
    public class GraphBox
    {
        public string Center {
            get;
            private set;
        }
        public List<RGNode> nodes;
        public List<RGEdge> edges;

        public GraphBox(string uid)
        {
            Center = uid;
            nodes = new List<RGNode>(10);
            edges = new List<RGEdge>(10);
        }

        public RGNode GetCenter()
        {
            for (int i = 0; i < nodes.Count; i += 1)
            {
                if (nodes[i].Data.UID == Center)
                {
                    return nodes[i];
                }
            }
            return null;
        }

        public void AddNode(RGNode node)
        {
            if (node == null || HadNode(node.Data.UID))
                return;
            nodes.Add(node);
        }

        public bool HadNode(string uid)
        {
            for (int i = 0;i < nodes.Count;i += 1)
            {
                if (nodes[i].Data.UID == uid)
                {
                    return true;
                }
            }
            return false;
        }

        public RGNode GetNode(string uid)
        {
            for (int i = 0; i < nodes.Count; i += 1)
            {
                if (nodes[i].Data.UID == uid)
                {
                    return nodes[i];
                }
            }
            return null;
        }

        public void RemoveNode(string uid)
        {
            for (int i = 0; i < nodes.Count; i += 1)
            {
                if (nodes[i].Data.UID == uid)
                {
                    nodes.RemoveAt(i);
                    break;
                }
            }
        }

        public void AddEdge(RGEdge edge)
        {
            edges.Add(edge);
        }

        public bool HadEdge(string uid)
        {
            for (int i = 0; i < edges.Count; i += 1)
            {
                if (edges[i].Data.UID == uid)
                {
                    return true;
                }
            }
            return false;
        }

        public RGEdge GetEdge(string uid)
        {
            for (int i = 0; i < edges.Count; i += 1)
            {
                if (edges[i].Data.UID == uid)
                {
                    return edges[i];
                }
            }
            return null;
        }

        public void RemoveEdge(string uid)
        {
            for (int i = 0; i < edges.Count; i += 1)
            {
                if (edges[i].Data.UID == uid)
                {
                    edges.RemoveAt(i);
                    break;
                }
            }
        }
    }
}
