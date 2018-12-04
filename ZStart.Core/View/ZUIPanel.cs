using System.Collections;
using UnityEngine;
using ZStart.Core.Enum;
using ZStart.Core.Model;
using ZStart.Core.Util;
namespace ZStart.Core.View
{
    public abstract class ZUIPanel:ZUIBehaviour, IZUIPanel
    {
        public RectTransform effectPoint;
       
        [SerializeField]
        protected UIParamInfo windowInfo;
        public UIParamInfo Info
        {
            get
            {
                return windowInfo;
            }
        }

        protected bool _isOpen = false;
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

        public void WakenUp()
        {
            if(gameObject.activeInHierarchy == false)
                GameObjectUtil.ActiveChildren(gameObject, true);
            mTransform.localScale = Vector3.one;
            mTransform.localPosition = Vector3.zero;
            mTransform.localRotation = Quaternion.identity;
        }

        public virtual void Appear() { }

        public virtual void Disappear() {
            
        }

        public virtual void AddListeners()
        {

        }

        public virtual void RemoveListeners()
        {

        }

        public void DelayOpen(UIParamInfo info)
        {
            StopAllCoroutines();
            if (gameObject.activeInHierarchy)
                StartCoroutine(OpenInspector(info));
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

        IEnumerator OpenInspector(UIParamInfo info)
        {
            while (isStartEnd == false)
            {
                yield return null;
            }
            Open(info);
        }

        public void Open(UIParamInfo info)
        {
            windowInfo = info;
            _isOpen = true;
            AddListeners();
            Appear();
        }

        public void Close()
        {
            RemoveListeners();
            Clear();
            _isOpen = false;
            StartCoroutine(CloseInspector());
        }

        IEnumerator CloseInspector()
        {
            yield return null;
            Disappear();
        }
    }
}
