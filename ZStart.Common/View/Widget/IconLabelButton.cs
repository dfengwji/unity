using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZStart.Common.View.Widget
{
    public class IconLabelButton : IconButton
    {
        public Text label;

        public Color colorNormal = Color.white;
        public Color colorHighlight = Color.white;
        protected override void Start()
        {
            base.Start();
            if (label == null)
            {
                label = GetComponentInChildren<Text>();
                label.color = colorNormal;
            }
        }

        public void UpdateLabel(string text)
        {
            if (label != null)
                label.text = text;
        }

        public virtual void UpdateContent(Sprite sp,string text)
        {
            if (label != null)
                label.text = text;
            if (icon != null)
                icon.overrideSprite = sp;
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if(label != null)
                label.color = colorHighlight;
            base.OnPointerEnter(eventData);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (label != null)
                label.color = colorNormal;
            base.OnPointerExit(eventData);
        }
    }
}
