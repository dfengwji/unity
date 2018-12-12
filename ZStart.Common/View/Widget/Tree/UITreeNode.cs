using ZStart.Core.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZStart.Common.View.Widget.Tree
{
    public class UITreeNode : UIBehaviour
    {
        public RectTransform container = null;
        public Button button = null;
        public Image icon = null;
        public Text text = null;
    
        private UITreeData data = null;
        private UITree tree = null;
        private Transform mTransform = null;
        private List<UITreeNode> children = new List<UITreeNode>();
        private UnityAction<string> clickFun;
        public bool opened = false;
        public bool IsOpen
        {
            set
            {
                opened = value;
            }
            get
            {
                return opened;
            }
        }

        public bool IsLeaf
        {
            get
            {
                if (data.ChildNodes.Count < 1)
                    return true;
                else
                    return false;
            }
        }
      
        private void GetComponent()
        {
            mTransform = transform;
            tree = mTransform.GetComponentInParent<UITree>();
        }
        private void ResetComponent()
        {
            container.localPosition = new Vector3(0, container.localPosition.y, 0);
            icon.gameObject.SetActive(true);
        }

        public void Inject(UITreeData data, UnityAction<string> callback)
        {
            if (null == mTransform)
                GetComponent();
            clickFun = callback;
            ResetComponent();
            this.data = data;
            text.text = data.Name;
            button.onClick.AddListener(OpenOrCloseHandle);
            container.localPosition += new Vector3(container.sizeDelta.y * this.data.Layer, 0, 0);
            icon.gameObject.SetActive(true);
            if (IsLeaf)
            {
                icon.sprite = tree.leafIcon;  
            }
            else
            {
                icon.sprite = IsOpen ? tree.openIcon : tree.closeIcon;
            }
        }

        private void OpenOrCloseHandle()
        {
            if (IsLeaf)
            {
                if (clickFun != null)
                    clickFun(data.UID);
                return;
            }
            IsOpen = !IsOpen;
            if (IsOpen)
                OpenChildren();
            else
                CloseChildren();
            icon.sprite = IsOpen ? tree.openIcon : tree.closeIcon;
        }
        private void OpenChildren()
        {
            children = tree.AddItems(data.ChildNodes, transform.GetSiblingIndex());
        }

        protected void CloseChildren()
        {
            for (int i = 0; i < children.Count; i++)
            {
                UITreeNode node = children[i];
                node.RemoveListener();
                node.CloseChildren();
            }
            tree.RemoveItems(children);
            children.Clear();
        }

        private void RemoveListener()
        {
            button.onClick.RemoveListener(OpenOrCloseHandle);
        }
    }
}
