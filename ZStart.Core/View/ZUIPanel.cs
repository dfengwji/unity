using UnityEngine;
using ZStart.Core.Enum;
using ZStart.Core.Model;
namespace ZStart.Core.View
{
    public abstract class ZUIPanel:ZUIBehaviour, IZUIPanel
    {
        public RectTransform effectPoint;
       
        [SerializeField]
        protected UIParamInfo paramInfo;
        public UIParamInfo Info
        {
            get
            {
                return paramInfo;
            }
        }

        private bool _isOpen = false;
        public bool isOpen
        {
            get { return _isOpen; }
        }

        public string definition
        {
            get { return name; }
        }

        public int grade = 0;
        public int echelon
        {
            get
            {
                return grade;
            }
        }

        public int depth
        {
            get
            {
                Transform parent = mTransform.parent;
                int length = parent.childCount;
                for (int i = 0;i < length;i++)
                {
                    Transform tmp = parent.GetChild(i);
                    if (tmp == mTransform)
                        return i;
                }
                return 0;
            }
        }

        public void WakenUp(Transform parent)
        {
            gameObject.SetActive(true);
            SetParent(parent);
            UpdateDepth(PanelDepthType.Top);
        }

        public virtual void Appear() { }

        public virtual void Disappear(Transform parent) {
            SetParent(mTransform);
            UpdateDepth(PanelDepthType.Bottom);
            Disappear();
        }

        public virtual void Disappear(){}

        public virtual void AddListeners()
        {

        }

        public virtual void RemoveListeners()
        {

        }

        public void UpdateDepth(int depth)
        {
            
        }

        public void UpdateDepth(PanelDepthType depth)
        {
            if (depth == PanelDepthType.Top)
            {
                mTransform.SetAsLastSibling();
            }
            else if(depth == PanelDepthType.Bottom)
            {
                //mTransform.SetAsFirstSibling();
            }
        }

        public void Open(UIParamInfo info)
        {
            paramInfo = info;
            _isOpen = true;
            AddListeners();
            Appear();
        }

        public void Open(Transform parent, UIParamInfo info)
        {
            SetParent(parent);
            UpdateDepth(PanelDepthType.Top);
            Open(info);
        }

        public void Close(Transform parent)
        {
            
            RemoveListeners();
            Clear();
            _isOpen = false;
            Disappear(parent);
        }
       
        public void SetParent(Transform parent)
        {
            mTransform.SetParent(parent);
            mTransform.localPosition = Vector3.zero;
            mTransform.localRotation = Quaternion.identity;
            mTransform.localScale = Vector3.one;
        }
    }
}
