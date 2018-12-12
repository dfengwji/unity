using ZStart.Core;
using ZStart.Core.Controller;
using ZStart.Core.Model;
using UnityEngine;
namespace ZStart.Common.View
{
    public abstract class AppWindow : ZUIBehaviour
    {
        public Transform effectPoint;
        public RectTransform contentBox;
        [SerializeField]
        protected UIParamInfo paramInfo;
        private bool isStarted = false;
        public bool IsStarted
        {
            get
            {
                return isStarted;
            }
        }
        protected override void Start()
        {
            isStarted = true;
        }

        public virtual void AddListeners()
        {

        }

        public virtual void RemoveListeners()
        {

        }

        public virtual void Appear(UIParamInfo info)
        {
            paramInfo = info;
            if (contentBox.gameObject.activeSelf == false)
                contentBox.gameObject.SetActive(true);
        }
        public virtual void Disappear()
        {
            if (contentBox.gameObject.activeSelf)
                contentBox.gameObject.SetActive(false);
            ZAssetController.Instance.DeActivateAsset(mTransform);
        }
    }
}
