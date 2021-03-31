using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZStart.Core;
using ZStart.Core.Controller;
using ZStart.RGraph.Common;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Manager;
using ZStart.RGraph.Model;
using ZStart.RGraph.View.Parts;

namespace ZStart.RGraph.Layout
{
    public class BaseLayout
    {
        protected List<RGNode> allNodes = new List<RGNode>(10);
        protected List<RGEdge> allEdges = new List<RGEdge>(10);

        protected List<GraphBox> graphBoxs = new List<GraphBox>(10);

        public virtual void Step() { }

        public virtual void Begin() { }

        public virtual void Stop() { }

        private DataManager proxy;

        public GraphBox GetGraphBox(string uid)
        {
            for (int i = 0; i < graphBoxs.Count; i += 1)
            {
                if (graphBoxs[i].Center == uid)
                {
                    return graphBoxs[i];
                }
            }
            return null;
        }

        public Vector3 GetRandomPosition(Transform center, float radius)
        {
            //return Vector3.zero;
            Vector2 c = Random.insideUnitCircle * 1;
            Vector2 p = c.normalized * (radius + c.magnitude);
            if (center != null)
            {
                return new Vector3(center.localPosition.x + p.x, center.localPosition.y + p.y, 0f);
            }
            else
            {
                if (radius > 0)
                    return new Vector3(p.x, p.y, 0f);
                else
                    return Vector3.zero;
            }
        }

        public RGNode CreateNode(NodeInfo info, Transform center, Transform box, float radius, string parent)
        {
            if (info == null)
                return null;
            RGNode item = GetNode(info.UID);
            bool add = false;
            bool icon = File.Exists(info.avatar);
            if (item == null)
            {
                GNodeParts view = null;
                if (info.IsVirtual || !icon)
                {
                    view = ZAssetController.Instance.ActivateAsset<GNodeParts>(box);
                }
                else
                {
                    view = ZAssetController.Instance.ActivateAsset<GNodePersonParts>(box);
                }
                item = new RGNode(info, view);
                var pos = GetRandomPosition(center, radius);
                item.Data.LastPosition = pos;
                item.Data.X = pos.x;
                item.Data.Y = pos.y;
                item.distPos = new Vector2(pos.x, pos.y);
                item.mTransform.localPosition = pos;
                add = true;
            }
            AddNode(item, parent, add);
            if(icon)
                ZImageController.Instance.Load(info.UID, info.avatar, OnImageLoad);
            return item;
        }

        private void OnImageLoad(ZImageController.ImageInfo info)
        {
            if (info == null)
                return;
            var item = GetNode(info.identify);
            if (item != null)
            {
                item.viewer.UpdateTexture(info.texture);
            }
        }

        public RGEdge CreateEdge(LinkInfo info, Transform box, string centre)
        {
            RGEdge item = GetEdge(info.UID);
            if (item == null)
            {
                var from = GetNode(info.from.UID);
                var to = GetNode(info.to.UID);
                if (from != null && to != null)
                {
                    var view = ZAssetController.Instance.ActivateAsset<GEdgeParts>(box);
                    item = new RGEdge();
                    item.UpdateInfo(info, view, from, to);
                    item.DrawLine();
                    item.Show();
                    AddEdge(item, centre);
                }
            }
            else
            {
                item.Show();
            }
            return item;
        }

        public virtual void AddNode(RGNode node, string parent, bool add)
        {
            if (add)
            {
                node.Data.mass = 1.0f;
                allNodes.Add(node);
            }
            
            var graph = GetGraphBox(parent);
            if (graph == null)
            {
                graph = new GraphBox(parent);
                graphBoxs.Add(graph);
            }
            //graph.AddNode(node);
        }

        public bool HadNode(string uid)
        {
            for (int i = 0; i < allNodes.Count; i += 1)
            {
                if (allNodes[i].Data.UID == uid)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HadEdge(string uid)
        {
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                if (allEdges[i].Data.UID == uid)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void AddEdge(RGEdge edge, string parent)
        {
            allEdges.Add(edge);
            var graph = GetGraphBox(parent);
            if (graph == null)
            {
                graph = new GraphBox(parent);
                graphBoxs.Add(graph);
            }
            //graph.AddEdge(edge);
        }

        public void UpdateLines()
        {
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                allEdges[i].DrawLine();
            }
        }

        public RGNode GetNode(string uid)
        {
            for (int i = 0; i < allNodes.Count; i += 1)
            {
                if (allNodes[i].Data.UID == uid)
                {
                    return allNodes[i];
                }
            }
            return null;
        }

        public RGNode GetNode(GameObject go)
        {
            for (int i = 0; i < allNodes.Count; i += 1)
            {
                if (allNodes[i].viewer.gameObject == go)
                {
                    return allNodes[i];
                }
            }
            return null;
        }

        public void RemoveNode(string uid)
        {
            for (int i = 0; i < allNodes.Count; i += 1)
            {
                if (allNodes[i].Data.UID == uid)
                {
                    allNodes[i].Hide();
                    allNodes.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveEdge(string uid)
        {
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                if (allEdges[i].Data.UID == uid)
                {
                    allEdges[i].Hide();
                    allEdges.RemoveAt(i);
                    break;
                }
            }
        }

        public RGEdge GetEdge(string uid)
        {
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                if (allEdges[i].Data.UID == uid)
                {
                    return allEdges[i];
                }
            }
            return null;
        }

        protected bool HadLink(string from, string to)
        {
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                if (allEdges[i].Data.IsLink(from, to))
                {
                    return true;
                }
            }
            return false;
        }

        // 检查节点能否展开
        public bool CheckExpend(string root, RGNode node)
        {
            if (node == null)
            {
                return false;
            }
            var path = Path.Combine(root, "graph/graph_" + node.Data.UID + ".json");
            if (!File.Exists(path))
            {
                ZLog.Warning("the file not exist that path = " + path);
                node.Expend = ExpendStatus.Disable;
                return false;
            }
            else
            {
                if (node.Expend == ExpendStatus.Disable)
                    node.Expend = ExpendStatus.Closed;
                return true;
            }
        }

        // 展开节点
        public void ExpendNode(string uid)
        {
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                if (allEdges[i].Data.HadNode(uid))
                {
                    allEdges[i].Data.offLength = Random.Range(10.0f, 30.0f);
                }
            }
        }

        // 隐藏指定的节点
        public void HideNode(RGNode node)
        {
            if (node == null)
                return;
            RemoveNode(node.Data.UID);
            List<RGEdge> edges = GetEdgesByTarget(node.Data.UID);
            for (int i = 0; i < edges.Count; i += 1)
            {
                RGNode other = edges[i].GetAnother(node.Data.UID);
                var count = proxy.GetLeafNodeCount(other.Data.UID);
                if (count < 2 && !other.Data.Pinned)
                {
                    RemoveNode(other.Data.UID);
                }
                RemoveEdge(edges[i].Data.UID);
            }
        }

        // 收缩节点
        public void ShrinkNode(RGNode node)
        {
            if (node == null)
                return;
            node.Expend = ExpendStatus.Closed;
            List<RGEdge> edges = GetEdgesByTarget(node.Data.UID);
            for (int i = 0; i < edges.Count; i += 1)
            {
                RGNode other = edges[i].GetAnother(node.Data.UID);
                if(other != null && !other.Data.IsVirtual)
                {
                    var count = proxy.GetLeafNodeCount(other.Data.UID);
                    if (count < 2 && !other.Data.Pinned)
                    {
                        RemoveNode(other.Data.UID);
                        RemoveEdge(edges[i].Data.UID);
                    }
                    else
                    {
                        if (edges[i].Data.offLength < 0.01f)
                            RemoveEdge(edges[i].Data.UID);
                    }
                }
            }
        }

        // 高亮关系
        public void HighlightRelation(string target)
        {
            List<string> list = new List<string>(10);
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                if (string.IsNullOrEmpty(target))
                {
                    allEdges[i].NodeFrom.IsHighlight = false;
                    allEdges[i].NodeTo.IsHighlight = false;
                    allEdges[i].IsHighlight = false;
                }
                else
                {
                    if (allEdges[i].Data.HadNode(target))
                    {
                        allEdges[i].IsHighlight = true;
                        list.Add(allEdges[i].NodeFrom.Data.UID);
                        list.Add(allEdges[i].NodeTo.Data.UID);
                    }
                    else
                    {
                        allEdges[i].IsHighlight = false;
                    }
                }
            }
            if (!string.IsNullOrEmpty(target))
            {
                for (int j = 0; j < allNodes.Count; j += 1)
                {
                    if (list.Contains(allNodes[j].Data.UID))
                    {
                        allNodes[j].IsHighlight = true;
                    }
                    else
                    {
                        allNodes[j].IsHighlight = false;
                    }
                }
            }
        }

        public RGEdge GetEdge(string from, string to)
        {
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                if (allEdges[i].Data.IsLink(from, to))
                {
                    return allEdges[i];
                }
            }
            return null;
        }

        public bool IsLink(string from, string to)
        {
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                if (allEdges[i].Data.IsLink(from, to))
                {
                    return true;
                }
            }
            return false;
        }

        public List<RGNode> GetNodesByTarget(string target)
        {
            List<RGNode> list = new List<RGNode>(10);
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                if (allEdges[i].NodeFrom.Data.UID == target)
                {
                    list.Add(allEdges[i].NodeTo);
                }
                else if (allEdges[i].NodeTo.Data.UID == target)
                {
                    list.Add(allEdges[i].NodeFrom);
                }
            }
            return list;
        }

        public List<RGEdge> GetEdgesByTarget(string target)
        {
            List<RGEdge> list = new List<RGEdge>(10);
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                if (allEdges[i].Data.HadNode(target))
                {
                    list.Add(allEdges[i]);
                }
            }
            return list;
        }

        public void Dispose()
        {
            for (int i = 0; i < allEdges.Count; i += 1)
            {
                allEdges[i].Clear();
                ZAssetController.Instance.DeActivateAsset(allEdges[i].mTransform);
            }

            for (int i = 0; i < allNodes.Count; i += 1)
            {
                allNodes[i].Clear();
                ZAssetController.Instance.DeActivateAsset(allNodes[i].mTransform);
            }
            allEdges.Clear();
            allNodes.Clear();
            graphBoxs.Clear();
        }
    }
}
