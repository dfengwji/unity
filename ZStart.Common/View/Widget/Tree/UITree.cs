using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ZStart.Common.View.Widget.Tree
{
    public class UITree : UIBehaviour
    {
        public Sprite openIcon = null;
        public Sprite closeIcon = null;
        public Sprite leafIcon = null;

        public UITreeNode nodePrefab = null;
        public Transform container = null;

        private readonly List<UITreeNode> poolList = new List<UITreeNode>();
        public Transform poolParent = null;

        private UnityAction<string> clickFun;
        public void Inject(UITreeData rootData)
        {
            AddItem(rootData,0);
        }

        public void AddLeafEvent(UnityAction<string> callFun)
        {
            clickFun = callFun;
        }

        public List<UITreeNode> AddItems(List<UITreeData> datas, int siblingIndex)
        {
            List<UITreeNode> result = new List<UITreeNode>();
            for (int i = datas.Count - 1; i >= 0; i--)
                result.Add(AddItem(datas[i], siblingIndex));
            return result;
        }
        public UITreeNode AddItem(UITreeData data, int siblingIndex)
        {
            UITreeNode treeNode = null;
            if (poolList.Count > 0)
            {
                treeNode = poolList[0];
                poolList.RemoveAt(0);
            }
            else
                treeNode = CloneTreeNode();
            treeNode.transform.SetParent(container);
            treeNode.gameObject.SetActive(true);
            treeNode.transform.localScale = Vector3.one;
            treeNode.Inject(data,clickFun);
            treeNode.transform.SetSiblingIndex(siblingIndex + 1);
            return treeNode;
        }

        public void RemoveItems(List<UITreeNode> treeNodes)
        {
            foreach (UITreeNode node in treeNodes)
                RemoveItem(node);
        }
        public void RemoveItem(UITreeNode treeNode)
        {
            if (null == poolParent)
                poolParent = new GameObject("CachePool").transform;
            treeNode.transform.SetParent(poolParent);
            //treeNode.gameObject.SetActive(false);
            poolList.Add(treeNode);
        }

        private UITreeNode CloneTreeNode()
        {
            UITreeNode result = Instantiate(nodePrefab);
            result.transform.SetParent(container);
            return result;
        }
    }
}
