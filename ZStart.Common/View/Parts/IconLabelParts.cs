using UnityEngine;
using UnityEngine.UI;
namespace ZStart.Common.View.Parts
{
    public class IconLabelParts : IconParts
    {
        public Text label;

        public void UpdateLabel(string text)
        {
            if (label != null)
                label.text = text;
        }

       
        public void UpdateContent(Sprite sp,string text)
        {
            if (icon != null)
                icon.overrideSprite = sp;
            if (label != null)
                label.text = text;
        }

        public override void Clear()
        {
            
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if(icon == null)
                icon = GetComponentInChildren<Image>();
            if(label == null)
                label = GetComponentInChildren<Text>();
        }
#endif
    }
}
