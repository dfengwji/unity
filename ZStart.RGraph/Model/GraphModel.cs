using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZStart.RGraph.Structure;

namespace ZStart.RGraph.Model
{
    public class GraphModel:Core.ZDataBase
    {
        public string Parent
        {
            get;
            set;
        }
        public string Center {
            get;
            private set;
        }
        public List<NodeInfo> Nodes
        {
            get;
            private set;
        }
        public List<LinkInfo> Links
        {
            get;
            private set;
        }

        private string rootPath = "";
        public GraphModel(string path = "")
        {
            rootPath = path;
            Nodes = new List<NodeInfo>(10);
            Links = new List<LinkInfo>(10);
        }
        public void Clear()
        {
            Nodes.Clear();
            Links.Clear();
        }

        public void SetCenter(string center)
        {
            Center = center;
        }

        public NodeInfo CenterInfo()
        {
            return GetNode(Center);
        }

        public void AddNode(NodeInfo node, NodeInfo center)
        {
            if (node == null || HadNode(node.UID))
                return;
            Nodes.Add(node);
            if (center != null)
            {
                LinkInfo edge = new LinkInfo
                {
                    UID = center + "-" + node.UID,
                    from = center,
                    to = node
                };
                AddEdge(edge);
            }
        }

        public void AddEdge(LinkInfo edge)
        {
            if(edge == null || HadEdge(edge.UID))
                return;
            Links.Add(edge);
        }

        public bool HadNode(string uid)
        {
            for (int i = 0; i < Nodes.Count; i += 1)
            {
                if (Nodes[i].UID == uid)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HadEdge(string uid)
        {
            for (int i = 0; i < Links.Count; i += 1)
            {
                if (Links[i].UID == uid)
                {
                    return true;
                }
            }
            return false;
        }

        public LinkInfo GetEdge(string from, string to)
        {
            for (int i = 0;i < Links.Count;i += 1)
            {
                if (Links[i].IsLink(from, to))
                {
                    return Links[i];
                }
            }
            return null;
        }

        public List<LinkInfo> GetLink(string node)
        {
            List<LinkInfo> list = new List<LinkInfo>(5);
            for (int i = 0; i < Links.Count; i += 1)
            {
                if (Links[i].HadNode(node))
                {
                    list.Add(Links[i]);
                }
            }
            return null;
        }

        public int GetLinkCount(string node)
        {
            int num = 0;
            for (int i = 0; i < Links.Count; i += 1)
            {
                if (Links[i].HadNode(node))
                {
                    num += 1;
                }
            }
            return num;
        }

        public Pair<List<NodeInfo>,List<LinkInfo>> GetByCenter(string uid, int layer)
        {
            Pair<List<NodeInfo>, List<LinkInfo>> pair = new Pair<List<NodeInfo>, List<LinkInfo>>();
            List<NodeInfo> nodes = new List<NodeInfo>(10);
            List<LinkInfo> edges = new List<LinkInfo>(10);
            CheckNodesByCenter(uid, layer, ref nodes, ref edges);
            pair.first = nodes;
            pair.second = edges;
            return pair;
        }

        private bool HadItem<T>(ref List<T> list, string uid) where T:Core.ZDataBase
        {
            for (int j = 0; j < list.Count; j += 1)
            {
                if (list[j].UID == uid)
                {
                    return true;
                }
            }
            return false;
        }

        public NodeInfo GetNode(string uid)
        {
            for (int i = 0;i <Nodes.Count;i += 1)
            {
                if(Nodes[i].UID == uid)
                {
                    return Nodes[i];
                }
            }
            return null;
        }

        private void CheckNodesByCenter(string uid, int layer, ref List<NodeInfo> nodes, ref List<LinkInfo> edges)
        {
            if(!HadItem(ref nodes, uid))
            {
                var node = GetNode(uid);
                if (node != null)
                    nodes.Add(node);
            }
            if (layer < 1)
                return;
           
            for (int i = 0;i < Links.Count;i += 1)
            {
                string other = Links[i].GetOther(uid);
               
                if (!string.IsNullOrEmpty(other))
                {
                    if (!HadItem(ref edges, Links[i].UID))
                    {
                        edges.Add(Links[i]);
                    }
                    CheckNodesByCenter(other, layer - 1, ref nodes, ref edges);
                }
            }
        }

        public void CheckVirtualNodes()
        {
            var array = GetLinkTypes();
            for (int i = 0; i < array.Count; i += 1)
            {
                NodeInfo info = new NodeInfo
                {
                    UID = Core.Util.MD5Util.MD5String(Center + "-" + array[i]),
                    vName = array[i],
                    name = "",
                    parent = Center,
                    avatar = Path.Combine(rootPath, "icon/"+array[i]+".png")
                };

                AddNode(info, CenterInfo());
            }
            for (int i = 0;i < Links.Count;i += 1)
            {
                if (!string.IsNullOrEmpty(Links[i].type))
                {
                    string uid = Core.Util.MD5Util.MD5String(Center + "-" + Links[i].type);
                    var node = GetNode(uid);
                    if(Links[i].from.UID == Center)
                    {
                        Links[i].from = node;
                    }
                    else if (Links[i].to.UID == Center)
                    {
                        Links[i].to = node;
                    }
                }
            }
        }

        public List<NodeInfo> GetVirtualNodes()
        {
            List<NodeInfo> nodes = new List<NodeInfo>(5);
            for (int i = 0;i < Nodes.Count;i += 1)
            {
                if (Nodes[i].IsVirtual)
                {
                    nodes.Add(Nodes[i]);
                }
            }
            return nodes;
        }

        public List<NodeInfo> GetNodes()
        {
            List<NodeInfo> nodes = new List<NodeInfo>(Nodes.Count);
            for (int i = 0; i < Nodes.Count; i += 1)
            {
                if (!Nodes[i].IsVirtual)
                {
                    nodes.Add(Nodes[i]);
                }
            }
            return nodes;
        }

        public List<NodeInfo> GetNodesByVLink(string vName, string node)
        {
            List<NodeInfo> nodes = new List<NodeInfo>(Nodes.Count);
            for (int i = 0; i < Links.Count; i += 1)
            {
                if (Links[i].type == vName)
                {
                    if(Links[i].from.UID == node)
                    {
                        nodes.Add(Links[i].to);
                    }else if (Links[i].to.UID == node)
                    {
                        nodes.Add(Links[i].from);
                    }
                }
            }
            return nodes;
        }

        public List<NodeInfo> GetNodesByParent(string uid)
        {
            List<NodeInfo> nodes = new List<NodeInfo>(Nodes.Count);
            for (int i = 0; i < Nodes.Count; i += 1)
            {
                if (!Nodes[i].IsVirtual && Nodes[i].parent == uid)
                {
                    nodes.Add(Nodes[i]);
                }
            }
            return nodes;
        }

        public List<LinkInfo> GetVirtualLinks()
        {
            List<LinkInfo> list = new List<LinkInfo>(5);
            for (int i = 0; i < Links.Count; i += 1)
            {
                if (Links[i].to.IsVirtual)
                {
                    list.Add(Links[i]);
                }
            }
            return list;
        }

        public List<LinkInfo> GetLinks()
        {
            List<LinkInfo> list = new List<LinkInfo>(5);
            for (int i = 0; i < Links.Count; i += 1)
            {
                if (!Links[i].to.IsVirtual)
                {
                    list.Add(Links[i]);
                }
            }
            return list;
        }

        public List<LinkInfo> GetLinksByType(string name)
        {
            List<LinkInfo> list = new List<LinkInfo>(10);
            for (int i = 0;i < Links.Count;i += 1)
            {
                if (Links[i].type == name)
                {
                    list.Add(Links[i]);
                }
            }
            return list;
        }

        private List<string> GetLinkTypes()
        {
            List<string> array = new List<string>(5);
            for (int i = 0;i < Links.Count;i += 1)
            {
                if (!array.Contains(Links[i].type))
                {
                    array.Add(Links[i].type);
                }
            }
            return array;
        }

        public List<LinkInfo> GetLinks(string from, string to)
        {
            List<LinkInfo> array = new List<LinkInfo>(5);
            for (int i = 0;i < Links.Count;i += 1)
            {
                if(Links[i].IsLink(from, to))
                {
                    array.Add(Links[i]);
                }
            }
            return array;
        }

        public void SortRelations()
        {
            Dictionary<PairInfo, List<LinkInfo>> ids = new Dictionary<PairInfo, List<LinkInfo>>(20);
            for (int i = 0;i < Links.Count;i += 1)
            {
                PairInfo pair = new PairInfo
                {
                    key = Links[i].from.UID + "_" + Links[i].to.UID,
                    value = Links[i].to.UID + "_" + Links[i].from.UID
                };
                List<LinkInfo> array = GetLinks(Links[i].from.UID, Links[i].to.UID);
                if (!ids.ContainsKey(pair))
                {
                    ids.Add(pair, array);
                    int length = array.Count;
                    for (int j = 0; j < length; j += 1)
                    {
                        array[j].number = length % 2 != 0 ? j : j+1;
                        array[j].repeat = j;
                        array[j].RandomLength((length-1) * 50f);
                    }
                }
            }
        }

        private bool HadPair(ref List<PairInfo> list, PairInfo pair)
        {
            for (int i = 0;i < list.Count;i += 1)
            {
                if(list[i].Equals(pair))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyGraph(GraphModel graph)
        {
            for (int i = 0;i < graph.Nodes.Count;i += 1)
            {
                AddNode(graph.Nodes[i], null);
            }

            for (int i = 0; i < graph.Links.Count; i += 1)
            {
                AddEdge(graph.Links[i]);
            }
        }
    }
}
