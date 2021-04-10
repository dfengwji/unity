using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZStart.EBook.View.Widget
{
    public class LabelButton : Button
    {
        public string identify = "";

        public Text first;
        public Text second;
        private UnityAction<LabelButton> clickFun;
        private RectTransform _mRectTrans;
        public RectTransform mRectTransform
        {
            get
            {
                if (_mRectTrans == null)
                    _mRectTrans = transform as RectTransform;
                return _mRectTrans;
            }
        }

        private bool isOn = false;
        public bool IsHighlight
        {
            set
            {
                isOn = value;
                if (value)
                    DoStateTransition(SelectionState.Highlighted, false);
                else
                    DoStateTransition(SelectionState.Normal, false);
            }
        }

        public bool IsDisabled
        {
            set
            {
                interactable = !value;
            }
        }

        private bool defaultShow = true;
        public bool DefaultShow
        {
            set
            {
                defaultShow = value;
            }
            get
            {
                return defaultShow;
            }
        }

        protected override void Start()
        {
            base.Start();
            if (first == null)
            {
                first = mRectTransform.Find("FirstLabel").GetComponent<Text>();
            }
            if (second == null)
            {
                second = mRectTransform.Find("SecondLabel").GetComponent<Text>();
            }
            if (DefaultShow)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public void UpdateLabel(string text)
        {
            if (first != null)
                first.text = text;
        }

        public void UpdateLabel(string one, string two)
        {
            if (first != null)
                first.text = one;
            if (second != null)
                second.text = two;
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Clear()
        {
            clickFun = null;
            identify = "";
        }

        public void AddClickListener(UnityAction<LabelButton> callback)
        {
            clickFun = callback;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (clickFun != null)
                clickFun.Invoke(this);
        }
    }
}
