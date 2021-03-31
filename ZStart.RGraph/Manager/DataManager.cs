using System.Collections.Generic;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Model;

namespace ZStart.RGraph.Manager
{
    public class DataManager
    {
        private static DataManager mInstance = null;
        public static DataManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new DataManager();
                return mInstance;
            }
        }

        private List<GraphModel> allGraph;
        private List<ThemeInfo> themes;
        public DataManager()
        {
            allGraph = new List<GraphModel>(10);
            themes = new List<ThemeInfo>(5);
        }

        public void UpdateThemes(List<ThemeInfo> list)
        {
            themes = list;
        }

        public void GetThemes()
        {
            if (themes != null && themes.Count > 0)
                DFNotifyManager.SendNotify(DFNotifyType.OnThemesUpdate, themes);
            else
            {
                MessageManager.Instance.RequestThemes();
            }
        }

        public ThemeInfo GetTheme(string uid)
        {
            for (int i = 0;i < themes.Count;i += 1)
            {
                if (themes[i].uid == uid)
                    return themes[i];
            }
            return new ThemeInfo();
        }

        public bool AddGraph(GraphModel graph)
        {
            if (graph == null || graph.Nodes.Count < 2)
                return false;
            if (graph.Nodes.Count == 2 && HadNode(graph.Center))
                return false;
            if (HadGraph(graph.Center))
                return true;
            allGraph.Add(graph);
            return true;
        }

        public bool HadGraph(string center)
        {
            for (int i = 0;i < allGraph.Count;i += 1)
            {
                if (allGraph[i].Center == center)
                {
                    return true;
                }
            }
            return false;
        }

        public GraphModel GetGraph(string center)
        {
            for (int i = 0; i < allGraph.Count; i += 1)
            {
                if (allGraph[i].Center == center)
                {
                    return allGraph[i];
                }
            }
            return null;
        }

        public bool HadNode(string node)
        {
            for (int i = 0; i < allGraph.Count; i += 1)
            {
                var info = allGraph[i].GetNode(node);
                if (info != null)
                {
                    return true;
                }
            }
            return false;
        }

        public NodeInfo GetNode(string node)
        {
            for (int i = 0; i < allGraph.Count; i += 1)
            {
                var info = allGraph[i].GetNode(node);
                if(info != null)
                {
                    return info;
                }
            }
            return null;
        }

        public int GetLeafNodeCount(string node)
        {
            int count = 0;
            for (int i = 0; i < allGraph.Count; i += 1)
            {
                if(allGraph[i].HadNode(node))
                {
                    count += 1;
                }
            }
            return count;
        }

        public int GetEdgeCount(string node)
        {
            List<string> links = new List<string>(5);
            for (int i = 0;i < allGraph.Count;i += 1)
            {
                foreach (var edge in allGraph[i].Links)
                {
                    if (edge.HadNode(node) && !links.Contains(edge.UID))
                        links.Add(edge.UID);
                }
            }
            return links.Count;
        }

        public void CheckGraph(string uid, string parent)
        {
            if (string.IsNullOrEmpty(uid))
                return;
            GraphModel model = GetGraph(uid);
            //var center = Data.GetNode(uid);
            //if (center != null)
            //{
            //    model.SetCenter(center.UID);
            //    model.AddNode(center);
            //}
            //for (int i = 0; i < Data.Edges.Count; i += 1)
            //{
            //    if (Data.Edges[i].HadNode(uid))
            //    {
            //        model.AddEdge(Data.Edges[i]);
            //        var other = Data.Edges[i].GetOther(uid);
            //        var node = Data.GetNode(other);
            //        model.AddNode(node);
            //    }
            //}
            if (model != null)
            {
                model.Parent = parent;
                DFNotifyManager.SendNotify(DFNotifyType.OnGraphDataUpdate, model);
            }
            else
            {
                MessageManager.Instance.RequestGraph(uid, parent);
            }
        }

        public void GetGraphByTheme(string uid)
        {
            var info = GetTheme(uid);
            CheckGraph(info.node, "");
        }

        public void GetGraphByNode(string uid)
        {
            CheckGraph(uid, "");
        }

        public void OpenThemes()
        {
            
        }

        public void OpenWindow(string path)
        {
          
        }
    }
}
