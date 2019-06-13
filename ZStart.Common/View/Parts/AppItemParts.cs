using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZStart.Common.View.Parts
{
    public abstract class AppItemParts : AppParts, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public enum OptionType
        {
            None,
            Play,
            Detail,
            Delete,
            Stop,
            Capture,
        }
        public Image iconMask;
        public RawImage iconImage;
        public Image defaultImage;
        public Text tipLabel;

        public Image maskImage;
        public GameObject downAndroid;
        public GameObject downWindows;
        private RectTransform _tipRect;
        public RectTransform tipTransform
        {
            get
            {
                if (_tipRect == null)
                    _tipRect = tipLabel.GetComponent<RectTransform>();
                return _tipRect;
            }
        }

        public CanvasGroup canvasGroup;
        public CanvasRenderer canvasRender;

        protected bool isHover = true;
        public string identify = "";
        protected UnityAction<AppItemParts, OptionType> clickCallFun;
        private bool isLocked = false;

        public bool ShowAndroid
        {
            set
            {
                if (downAndroid != null)
                    downAndroid.SetActive(value);
            }
        }

        public bool ShowWindows
        {
            set
            {
                if (downWindows != null)
                    downWindows.SetActive(value);
            }
        }

        protected override void OnDisable()
        {
            SetHighlight(false);
            base.OnDisable();
        }

        public virtual void AddListener(UnityAction<AppItemParts, OptionType> clickFun)
        {

        }

        public virtual void UpdateOption(OptionType type, bool enable)
        {

        }

        public virtual void UpdateInfo(string uid, string title, string price)
        {

        }

        public virtual void UpdateInfo(string title, string price)
        {
            if (title != tipLabel.text)
            {
                tipLabel.text = title;
                tipLabel.SetAllDirty();
                tipLabel.GetComponent<CanvasRenderer>().cull = false;
                // fitter.SetLayoutHorizontal();
                /*if (scroll != null)
                {
                    if (tipLabel.rectTransform.sizeDelta.x > scroll.viewport.sizeDelta.x)
                        tipLabel.rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, tipLabel.rectTransform.sizeDelta.x);
                    scroll.horizontalNormalizedPosition = 0f;
                }*/
            }
        }

        public virtual void SetHighlight(bool hover)
        {
            if (isHover == hover)
                return;
            isHover = hover;
            if (maskImage != null)
            {
                if (hover)
                    maskImage.enabled = true;
                else
                    maskImage.enabled = false;
            }
        }

        public virtual void UpdateDownload(Sprite sp, float amount, bool pause)
        {

        }

        public virtual void UpdateType(OptionType type) { }

        public virtual void UpdateIcon(Texture2D tex, Vector2 size)
        {
            if (iconImage != null)
            {
                iconImage.rectTransform.sizeDelta = size;
                iconImage.texture = tex;
                bool show = tex == null ? false : true;
                defaultImage.enabled = !show;
                iconImage.enabled = show;
                if (iconMask != null)
                    iconMask.enabled = show;
                if (show)
                {
                    //canvasRender.cull = false;
                    iconImage.SetVerticesDirty();
                }
            }
            else
            {
                iconImage.enabled = false;
                if (iconMask != null)
                    iconMask.enabled = false;
                defaultImage.enabled = true;
            }
        }

        public virtual void UpdateIcon(Texture2D tex)
        {
            if (iconImage != null)
            {
                iconImage.texture = tex;
                bool show = tex == null ? false : true;
                defaultImage.enabled = !show;
                iconImage.enabled = show;
                if (iconMask != null)
                    iconMask.enabled = show;
                if (show)
                {
                    //canvasRender.cull = false;
                    //iconImage.SetVerticesDirty();
                }
            }
            else
            {
                iconImage.enabled = false;
                if (iconMask != null)
                    iconMask.enabled = false;
                defaultImage.enabled = true;
            }
        }

        public override void Show()
        {
            isLocked = false;
            SetHighlight(false);
            base.Show();
        }

        public virtual void ResetDefault()
        {
            isLocked = false;
            canvasGroup.alpha = 0;
            //mTransform.localPosition = new Vector3(-Width * 0.5f, 0f, 0f);
        }

        public override void UnShow()
        {
           
        }

        public override void Clear()
        {
            SetHighlight(false);
            //if(scroll != null)
            //    scroll.horizontalNormalizedPosition = 0f;
            identify = "";
            //tipLabel.text = "";
            iconImage.enabled = false;
            defaultImage.enabled = true;
            isLocked = false;
        }

        public virtual void Lock(bool locked)
        {
            isLocked = locked;
            if (locked)
            {

            }
            else
            {

            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            SetHighlight(true);
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            SetHighlight(false);
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {

        }
    }
}
