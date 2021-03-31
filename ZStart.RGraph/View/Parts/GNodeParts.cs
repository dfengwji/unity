using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZStart.Core.Controller;

namespace ZStart.RGraph.View.Parts
{
    public class GNodeParts: AppUIParts,IPointerClickHandler,IPointerUpHandler,IPointerDownHandler
    {
        public Collider2D colliderBox;
        public Image maskRender;
        public Image normalRender;
        public Image pinRender;
        public Image highRender;
        public RectTransform highlight;
        public Text label;
        public Color color = Color.white;
        public RawImage header;
        private UnityAction clickAction;
        private UnityAction<TouchPhase> touchAction;
        public bool Pinned
        {
            set
            {
                if (pinRender)
                    pinRender.enabled = value;
                if (highRender)
                    highRender.enabled = !value;
                if (normalRender)
                    normalRender.enabled = !value;
            }
        }

        public bool Selected
        {
            set
            {
                if(highRender)
                    highRender.enabled = value;
                if(normalRender)
                    normalRender.enabled = !value;
            }
        }

        public bool IsHighlight
        {
            set
            {
                if (maskRender != null)
                {
                    if (value)
                    {
                        maskRender.color = new Color(color.r, color.g, color.b, 0f);
                    }
                    else
                    {
                        maskRender.color = new Color(color.r, color.g, color.b, 0.5f);
                    }
                }

                if (normalRender != null)
                {
                    if (value)
                    {
                        normalRender.color = new Color(color.r, color.g, color.b, 1f);
                    }
                    else
                    {
                        normalRender.color = new Color(color.r, color.g, color.b, 0.5f);
                    }
                }
            }
        }

        protected override void Start()
        {
            if (colliderBox == null)
            {
                colliderBox = GetComponent<CircleCollider2D>();
                maskRender = mTransform.Find("avatar/mask").GetComponent<Image>();
                maskRender.gameObject.SetActive(true);

                normalRender = mTransform.Find("avatar/normal").GetComponent<Image>();
                normalRender.gameObject.SetActive(true);

                highlight = mTransform.Find("avatar/high").GetComponent<RectTransform>();
                highRender = highlight.GetComponent<Image>();
                highRender.gameObject.SetActive(true);

                pinRender = mTransform.Find("avatar/pin").GetComponent<Image>();
                pinRender.gameObject.SetActive(true);

                label = mTransform.Find("text").GetComponent<Text>();
               
                header = mTransform.Find("avatar/head").GetComponent<RawImage>();
                gameObject.AddComponent<Lean.Gui.LeanMoveToTop>();
                gameObject.SetActive(false);
            }
            base.Start();
            IsHighlight = false;
            Pinned = false;
            Selected = false;
        }
        
        public virtual void UpdateLabel(string tip)
        {
            label.text = tip;
            gameObject.SetActive(true);
            mTransform.localScale = Vector3.one * 0.01f;
        }

        public void AddListeners(UnityAction click, UnityAction<TouchPhase> action)
        {
            clickAction = click;
            touchAction = action;
        }

        public virtual void UpdateTexture(Texture2D texture)
        {
            header.texture = texture;
            header.enabled = texture == null ? false:true;
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            mTransform.DOScale(Vector3.one, 0.2f);
        }

        public override void UnShow()
        {
            mTransform.DOScale(Vector3.one * 0.01f, 0.2f).OnComplete(() => {
                gameObject.SetActive(false);
                ZAssetController.Instance.DeActivateAsset(mTransform);
            });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(clickAction != null)
            {
                clickAction.Invoke();
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(touchAction != null){
                touchAction.Invoke(TouchPhase.Ended);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (touchAction != null)
            {
                touchAction.Invoke(TouchPhase.Began);
            }
        }
    }
}
