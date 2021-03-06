﻿using ZStart.Core;
using ZStart.Core.Controller;
using ZStart.Core.Model;
using UnityEngine;
namespace ZStart.Common.View
{
    public abstract class AppWindow : ZUIBehaviour
    {
        public Transform effectPoint;
        public RectTransform contentBox;
        public GameObject loadingBox;

        [SerializeField]
        protected UIParamInfo paramInfo;
        public UIParamInfo UIParam
        {
            get
            {
                return paramInfo;
            }
        }
        private bool isStarted = false;
        public bool IsStarted
        {
            get
            {
                return isStarted;
            }
        }

        public bool IsLoading
        {
            set
            {
                if (loadingBox != null)
                    loadingBox.SetActive(value);
            }
            get
            {
                if (loadingBox != null)
                    return loadingBox.activeSelf;
                else
                    return false;
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
