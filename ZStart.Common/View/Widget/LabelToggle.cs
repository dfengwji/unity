using UnityEngine;
using UnityEngine.UI;

namespace ZStart.Common.View.Widget
{
    public class LabelToggle : Toggle
    {
        public Text label;
        public string UID = "";
        
        protected override void Start()
        {
            if (label == null)
                label = GetComponentInChildren<Text>();
        }

        public bool isEnable
        {
            set
            {
                interactable = value;
                if (animator != null)
                    animator.enabled = value;
                if (image == null)
                    image = GetComponentInChildren<Image>();
                if (image != null)
                    image.enabled = value;
            }
        }

        public void UpdateLabel(string tip)
        {
            if (label == null)
                label = GetComponentInChildren<Text>();
            label.text = tip;
        }

        public void UpdateLabel(float alpha)
        {
            if (label == null)
                label = GetComponentInChildren<Text>();
            label.color = new UnityEngine.Color(label.color.r,label.color.g,label.color.b,alpha);
        }

        public void Show()
        {
            if (isActiveAndEnabled == false)
                gameObject.SetActive(true);
        }

        public void UnShow()
        {
            if (isActiveAndEnabled == true)
                gameObject.SetActive(false);
        }
    }
}
