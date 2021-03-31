using DG.Tweening;
using SoftMasking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using ZStart.Core;
using ZStart.RGraph.Common;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Layout;
using ZStart.RGraph.Manager;
using ZStart.RGraph.Model;
using ZStart.RGraph.Util;
using ZStart.RGraph.View.Parts;

namespace ZStart.RGraph
{
    [DisallowMultipleComponent]
    public class RGSimulator: ZBehaviourBase
    {
        public enum TouchType
        {
            Empty,
            Pinch,
            Drag,
            Node
        }

        public Camera renderCamera;
        public ScrollRect scrollRect;
        public Image image;
        public Transform nodeBox;
        public Transform edgeBox;
        public TouchType touchType;
        // 数据充足的情况下最多展开多少层
        public int maxLevel = 2;
        public int nowLevel = 0;
        private BaseLayout layout = null;
        private RGNode selectedNode = null;
        private bool isSpawning = false;
        private System.Action<string> callAction;
        float radius = 230f;
        TouchPinch pinchTool;
        string cacheNode = "";
        void Start()
        {
            Init();
            ZLog.Warning("relation graph simulator start...center = " + cacheNode);
            if (isStartEnd)
            {
                DataManager.Instance.GetGraphByNode(cacheNode);
            }
        }

        private void OnEnable()
        {
            Init();
            AddListeners();
            ZLog.Warning("relation graph simulator enable...center = " + cacheNode);
            if (isStartEnd)
            {
                DataManager.Instance.GetGraphByNode(cacheNode);
            }
            pinchTool.ResetData();
        }

        private void OnDisable()
        {
            isSpawning = false;
            StopAllCoroutines();
            RemoveListeners();
        }

        private void LateUpdate()
        {
            if (isStartEnd)
            {
                layout.Step();
                if (selectedNode != null && selectedNode.viewer.highlight)
                {
                    selectedNode.viewer.highlight.Rotate(Vector3.back, Time.deltaTime * 30);
                }
            }
        }

        private void Init()
        {
            if (isStartEnd)
            {
                return;
            }
            if (renderCamera == null)
                renderCamera = Camera.main;
            scrollRect = mTransform.GetComponent<ScrollRect>();
            image = mTransform.GetComponent<Image>();
            nodeBox = mTransform.Find("Viewport/Content/Nodes").GetComponent<Transform>();
            edgeBox = mTransform.Find("Viewport/Content/Edges").GetComponent<Transform>();
            layout = new ForceDirectLayout(5f, 1f, 0.1f, radius, scrollRect.content);
            pinchTool = scrollRect.transform.parent.gameObject.AddComponent<TouchPinch>();
            pinchTool.SetCamera(renderCamera, this);

            var mask = scrollRect.viewport.gameObject.AddComponent<SoftMask>();
            mask.defaultShader = Shader.Find("Hidden/UI Default (Soft Masked)");

            isStartEnd = true;
        }

        private void ClearSelected()
        {
            if (selectedNode != null)
            {
                selectedNode.Data.VX = 0f;
                selectedNode.Data.VY = 0f;
                selectedNode.Data.X = selectedNode.Data.LastPosition.x;
                selectedNode.Data.Y = selectedNode.Data.LastPosition.y;

                selectedNode.Data.isDrag = false;
                selectedNode.Selected = false;
                selectedNode.ShowMenu = false;
                selectedNode = null;
                if (callAction != null)
                    callAction.Invoke("");
            }
        }

        public void SetPinch(float min, float max, float speed)
        {
            pinchTool.SetRangeAndSpeed(min, max, speed);
        }

        public void ZoomIn(float speed)
        {
            float sc = scrollRect.content.localScale.x;
            pinchTool.Zoom(sc + speed);
        }

        public void ZoomOut(float speed)
        {
            float sc = scrollRect.content.localScale.x;
            pinchTool.Zoom(sc - speed);
        }

        public void OpenNode(string uid, System.Action<string> fun)
        {
            cacheNode = uid;
            callAction = fun;
            ZLog.Warning("try open graph node..."+uid + ";" + isStartEnd);
            if (isStartEnd && isActiveAndEnabled)
            {
                DataManager.Instance.GetGraphByNode(uid);
            }
        }

        public void SetExpendLevel(int num)
        {
            maxLevel = num;
        }

        public void Close()
        {
            nowLevel = 0;
            ClearSelected();
            if(null != layout)
                layout.Dispose();
            cacheNode = "";
        }

        public void SwitchTouch(TouchType kind)
        {
            if (touchType == kind)
                return;
            touchType = kind;
            if (kind == TouchType.Empty)
            {
                scrollRect.enabled = true;
                image.enabled = true;
                pinchTool.ActiveRaycast(false);
            }
            else if (kind == TouchType.Node)
            {
                scrollRect.enabled = false;
                image.enabled = false;
                pinchTool.ActiveRaycast(false);
            }
            else if (kind == TouchType.Pinch)
            {
                scrollRect.enabled = false;
                image.enabled = false;
                pinchTool.ActiveRaycast(true);
            }
            else if (kind == TouchType.Drag)
            {
                scrollRect.enabled = true;
                image.enabled = true;
                pinchTool.ActiveRaycast(false);
            }
        }

        #region Event Handle
        private void AddListeners()
        {
            DFNotifyManager.AddListener(DFNotifyType.OnNodeExpend, gameObject, OnNodeExpend);
            DFNotifyManager.AddListener(DFNotifyType.OnNodePin, gameObject, OnNodePinned);
            DFNotifyManager.AddListener(DFNotifyType.OnNodeHighlight, gameObject, OnNodeHighlight);
            DFNotifyManager.AddListener(DFNotifyType.OnGraphDataUpdate, gameObject, OnGraphDataUpdate);
        }

        private void RemoveListeners()
        {
            DFNotifyManager.RemoveListener(DFNotifyType.OnNodeExpend, gameObject);
            DFNotifyManager.RemoveListener(DFNotifyType.OnNodePin, gameObject);
            DFNotifyManager.RemoveListener(DFNotifyType.OnGraphDataUpdate, gameObject);
            DFNotifyManager.RemoveListener(DFNotifyType.OnNodeHighlight, gameObject);
        }

        private void OnGraphDataUpdate(object data)
        {
            if (data == null)
                return;
            GraphModel graph = data as GraphModel;
            ZLog.Warning("create graph node = " + cacheNode + ";center = " + graph.Center + "; spawn = " + isSpawning);
            if (graph.Center != cacheNode || isSpawning)
            {
                return;
            }
            var success = DataManager.Instance.AddGraph(graph);
            if (success)
            {
                nowLevel += 1;
                layout.ExpendNode(graph.Center);
                StartCoroutine(CreateGraphInspector(graph));
            }
            else
            {
                var node = layout.GetNode(graph.Center);
                if (node != null)
                {
                    node.Expend = ExpendStatus.Disable;
                }
            }
        }

        private void OnNodeExpend(object data)
        {
            if (isSpawning)
                return;
            var uid = data as string;
            RGNode node = layout.GetNode(uid);
            ExpendNode(node);
            DFNotifyManager.SendNotify(DFNotifyType.OnUIRefresh, "");
        }

        private void OnNodePinned(object data)
        {
            var uid = data as string;
            RGNode item = layout.GetNode(uid);
            if (item != null)
            {
                item.Pinned = !item.Pinned;
            }
            DFNotifyManager.SendNotify(DFNotifyType.OnUIRefresh, "");
        }

        private void OnNodeHighlight(object data)
        {
            if (data == null)
                return;
            var pair = (PairInfo)data;
            var item = layout.GetNode(pair.value);
            if (item == null)
            {
                DataManager.Instance.CheckGraph(pair.value, pair.key);
            }
            else
            {
                item.IsHighlight = true;
            }
        }

        /// <summary>
        /// 选择一个节点
        /// </summary>
        /// <param name="gesture"></param>
        private void OnSimpleTap(RGNode node)
        {
            var obj = node.viewer.gameObject;
            if (obj == null)
            {
                layout.HighlightRelation("");
                ClearSelected();
                return;
            }
            if (obj.name.Contains("Menu"))
            {
                GMenuParts menu = obj.GetComponentInParent<GMenuParts>();
                MenuType type = menu.SelectState(node.viewer.name);
                CheckMenuType(type);
            }
            else
            {
                if (node == selectedNode)
                    return;
                ClearSelected();
                selectedNode = node;
                //node.ShowMenu = true;
                node.Selected = true;
                layout.HighlightRelation(node.Data.UID);
                if (node.Data.IsVirtual)
                {
                    ExpendNode(node);
                }
                else
                {
                    if (CheckNextGraph(node))
                    {
                        ExpendNode(node);
                    }
                }
                if (callAction != null)
                    callAction.Invoke(node.Data.UID);
            }
        }

        private void OnTouchNode(RGNode node, TouchPhase phase)
        {
            if (phase == TouchPhase.Began)
            {
                SwitchTouch(TouchType.Empty);
            }
            else
            {
                SwitchTouch(TouchType.Node);
            }
            if (phase == TouchPhase.Ended)
            {
                if (node.Pinned)
                {
                    node.Pinned = false;
                }
                else
                {
                    node.Pinned = true;
                }
            }
        }
       
        private void OnDragStart(RGNode node)
        {
            if (node == null)
                return;
            ClearSelected();
            node.ShowMenu = false;
            node.Selected = true;
            selectedNode = node;
            node.Data.isDrag = true;
            node.Pinned = true;
        }

        private void OnDragEnd(RGNode node)
        {
            ClearSelected();
        }
        #endregion

        private void ExpendNode(RGNode node)
        {
            if (isSpawning || node == null || nowLevel > maxLevel)
                return;
            if (node.Data.IsVirtual)
            {
                layout.ExpendNode(node.Data.UID);
                var graph = DataManager.Instance.GetGraph(node.Data.parent);
                var nodes = graph.GetNodesByVLink(node.Data.vName, node.Data.UID);
                var edges = graph.GetLinksByType(node.Data.vName);
                StartCoroutine(CreateNodesInspector(node.Data, nodes, edges));
            }
            else
            {
               
                if (node.Expend == ExpendStatus.Disable)
                    return;
                if (node.Expend == ExpendStatus.Closed)
                {
                    DataManager.Instance.CheckGraph(node.Data.UID, "");
                    node.Expend = ExpendStatus.Opened;
                }
                else
                {
                    layout.ShrinkNode(node);
                }
            }
        }

        IEnumerator CreateGraphInspector(GraphModel model)
        {
            isSpawning = true;
            layout.Stop();
            model.SortRelations();
            var parent = layout.GetNode(model.Parent);
            var center = layout.GetNode(model.Center);
            if (center == null)
            {
                center = layout.CreateNode(model.CenterInfo(), parent?.mTransform, nodeBox, 0f, model.Center);
                center.AddListeners(OnSimpleTap, OnTouchNode, OnDragStart, OnDragEnd);
            }
            yield return null;
            center.Show();
            List<NodeInfo> nodes = model.GetVirtualNodes(); 
            List<RGNode> viewers = new List<RGNode>(model.Nodes.Count);
            List<LinkInfo> edges = null;
            if (nodes.Count > 0)
            {
                edges = model.GetVirtualLinks();
            }
            else
            {
                nodes = model.GetNodes();
                edges = model.GetLinks();
            }
            for (int i = 0; i < nodes.Count; i += 1)
            {
                LinkInfo e = model.GetEdge(nodes[i].UID, model.Center);
                if (e != null)
                {
                    radius = e.InitLength;
                }
                var item = layout.CreateNode(nodes[i], center.mTransform, nodeBox, radius, model.Center);
                item.AddListeners(OnSimpleTap, OnTouchNode, OnDragStart, OnDragEnd);
                viewers.Add(item);
                yield return null;
            }
            for (int i = 0; i < viewers.Count; i += 1)
            {
                viewers[i].Show();
            }
            yield return new WaitForSeconds(0.2f);
            center.IsHighlight = true;
            isSpawning = false;
            for (int i = 0; i < edges.Count; i += 1)
            {
                layout.CreateEdge(edges[i], edgeBox, model.Center);
                yield return null;
            }
            layout.Begin();
            // DFNotifyManager.SendNotify(DFNotifyType.OnUIRefresh, "");
        }

        IEnumerator CreateNodesInspector(NodeInfo center, List<NodeInfo> nodes, List<LinkInfo> edges)
        {
            isSpawning = true;
            layout.Stop();
            var parentNode = layout.GetNode(center.parent);
            var centerNode = layout.GetNode(center.UID);
            if (centerNode == null)
            {
                centerNode = layout.CreateNode(center, parentNode?.mTransform, nodeBox, 0f, center.parent);
                centerNode.AddListeners(OnSimpleTap, OnTouchNode, OnDragStart, OnDragEnd);
            }
            yield return null;
            centerNode.Show();
            List<RGNode> list = new List<RGNode>(nodes.Count);
            for (int i = 0; i < nodes.Count; i += 1)
            {
                var item = layout.CreateNode(nodes[i], centerNode.mTransform, nodeBox, radius, center.UID);
                item.AddListeners(OnSimpleTap, OnTouchNode, OnDragStart, OnDragEnd);
                list.Add(item);
                yield return null;
            }
            for (int i = 0; i < list.Count; i += 1)
            {
                list[i].Show();
            }
            yield return new WaitForSeconds(0.2f);
            centerNode.IsHighlight = true;
            isSpawning = false;
            for (int i = 0; i < edges.Count; i += 1)
            {
                layout.CreateEdge(edges[i], edgeBox, center.UID);
                yield return null;
            }
            layout.Begin();
        }

        private bool CheckNextGraph(RGNode node)
        {
            string root = MessageManager.Instance.RootPath; 
            var expend = layout.CheckExpend(root, node);
            if(!expend)
            {
                return false;
            }
            if (node.Data.entity == null)
            {
                var file = Path.Combine(node.Data.fullPath);
                if (File.Exists(file))
                {
                    var json = File.ReadAllText(file);
                    var tmp = ParseUtil.ParseEntityJson(json, root);
                    if (tmp != null)
                    {
                        tmp.uid = node.Data.UID;
                        node.Data.entity = tmp;
                    }
                    else
                    {
                        node.Data.entity = new EntityInfo
                        {
                            uid = node.Data.UID,
                            name = node.Data.name
                        };
                    }
                }
                else
                {
                    node.Data.entity = new EntityInfo
                    {
                        uid = node.Data.UID,
                        name = node.Data.name
                    };
                }
            }
            node.Data.Screen = RectTransformUtility.WorldToScreenPoint(renderCamera, node.mTransform.position);
            // DFNotifyManager.SendNotify(DFNotifyType.OnNodeSelected, node.Data);
            return true;
        }

        private void CheckMenuType(MenuType menu)
        {
            if (menu == MenuType.Experience || menu == MenuType.Remark)
            {
                if (selectedNode == null)
                {
                    return;
                }
                if (menu == MenuType.Experience && selectedNode.Data.entity.HadEvents)
                {
                    DFNotifyManager.SendNotify(DFNotifyType.OnNodeMenuUpdate, (int)menu);
                }
                if (menu == MenuType.Remark && selectedNode.Data.entity.HadRecord)
                {
                    DFNotifyManager.SendNotify(DFNotifyType.OnNodeMenuUpdate, (int)menu);
                }
            }
            else if (menu == MenuType.Pin)
            {
                OnNodePinned(selectedNode.Data.UID);
            }
            else if (menu == MenuType.Expend)
            {
                OnNodeExpend(selectedNode.Data.UID);
            }
            else if (menu == MenuType.Hide)
            {
                layout.HideNode(selectedNode);
                ClearSelected();
            }
        }
        
        private RGNode GetNode(GameObject obj)
        {
            if (obj == null)
            {
                return null;
            }

            return layout.GetNode(obj);
        }
    }
}
