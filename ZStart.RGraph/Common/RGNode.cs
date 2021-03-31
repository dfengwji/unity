using Lean.Gui;
using UnityEngine;
using UnityEngine.Events;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Model;
using ZStart.RGraph.View.Parts;

namespace ZStart.RGraph.Common
{
    public class RGNode
    {
        private UnityAction<RGNode> clickAction;
        private UnityAction<RGNode> dragStartAction;
        private UnityAction<RGNode> dragEndAction;
        private UnityAction<RGNode, TouchPhase> tapAction;
        public NodeInfo Data
        {
            private set;
            get;
        }

        public GNodeParts viewer;
        public GMenuParts menuItem;
        public Vector2 distPos = Vector2.zero;
        public RectTransform mTransform
        {
            get
            {
                return viewer.mTransform;
            }
        }

        public bool Pinned
        {
            set
            {
                Data.Pinned = value;
                viewer.Pinned = value;
                if (menuItem)
                {
                    menuItem.UpdateState(MenuType.Pin, value ? ExpendStatus.Closed : ExpendStatus.Opened);
                }
            }
            get
            {
                return Data.Pinned;
            }
        }

        public ExpendStatus Expend
        {
            set
            {
                Data.Expended = value;
                if (menuItem)
                    menuItem.UpdateState(MenuType.Expend, value);
            }
            get
            {
                return Data.Expended;
            }
        }

        private bool isSelected = false;
        public bool Selected
        {
            set
            {
                isSelected = value;
                if (value)
                {
                    Data.ClearForce();
                }
                viewer.Selected = value;
            }
            get
            {
                return isSelected;
            }
        }

        public bool IsActive
        {
            get
            {
                return viewer.isActiveAndEnabled;
            }
        }

        public bool IsHighlight
        {
            set
            {
                viewer.IsHighlight = value;
            }
        }

        public bool ShowMenu
        {
            set
            {
                if (value)
                {
                    //if (menuItem == null)
                    //{
                    //    menuItem = ZAssetController.Instance.ActivateAsset<GMenuParts>(mTransform);
                    //    menuItem.Show();
                    //    menuItem.UpdateState(MenuType.Pin, Data.Pinned ? ExpendStatus.Closed : ExpendStatus.Opened);
                    //    menuItem.UpdateState(MenuType.Expend, Data.Expended);
                    //}
                }
                else
                {
                    if (menuItem)
                        menuItem.UnShow();
                    menuItem = null;
                }
            }
        }

        public RGNode(NodeInfo info, GNodeParts view)
        {
            UpdateInfo(info, view);
        }

        public virtual void UpdateInfo(NodeInfo data, GNodeParts view)
        {
            Data = data;
            viewer = view;
            viewer.identify = data.UID;
            Data.Acceleration = Vector3.zero;
            Data.Velocity = Vector3.zero;
            viewer.UpdateLabel(data.name);
            var drag = viewer.mTransform.GetComponent<LeanDrag>();
            if (drag == null)
            {
                viewer.gameObject.AddComponent<LeanDrag>();
            }
            view.gameObject.AddComponent<LeanMoveToTop>();
        }

        public void AddListeners(UnityAction<RGNode> click, UnityAction<RGNode,TouchPhase> tapUp, UnityAction<RGNode> dragStart, UnityAction<RGNode> dragEnd)
        {
            clickAction = click;
            dragStartAction = dragStart;
            dragEndAction = dragEnd;
            tapAction = tapUp;
        }

        private void OnDragNodeStart()
        {
            if (dragStartAction != null)
            {
                dragStartAction.Invoke(this);
            }
        }

        private void OnDragNodeEnd()
        {
            if (dragEndAction != null)
            {
                dragEndAction.Invoke(this);
            }
        }

        private void OnClickNode()
        {
            if(clickAction != null)
            {
                clickAction.Invoke(this);
            }
        }

        private void OnTouchNode(TouchPhase phase)
        {
            if (tapAction != null)
            {
                tapAction.Invoke(this, phase);
            }
        }

        public void Show()
        {
            viewer.AddListeners(OnClickNode, OnTouchNode);
            viewer.Show();
            var drag = viewer.mTransform.GetComponent<LeanDrag>();
            if (drag != null)
            {
                drag.OnBegin.AddListener(OnDragNodeStart);
                drag.OnEnd.AddListener(OnDragNodeEnd);
            }
            if (menuItem)
                menuItem.UnShow();
        }

        public void Hide()
        {
            viewer.UnShow();
            menuItem = null;
        }

        public void ApplyForce(Vector3 force)
        {
            Data.Acceleration += force / Data.mass;
        }

        public void Move()
        {
            var pos = Data.Position;
            //Debug.LogWarning(Data.name + "---" + pos);
            if (!Data.Pinned)
                mTransform.localPosition = pos;
        }

        public void Clear()
        {
            Data.Dispose();
            Data = null;
        }
    }
}
