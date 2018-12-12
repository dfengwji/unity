using UnityEngine;
using UnityEngine.UI;

namespace ZStart.Common.View.Parts
{
    public class ItemStateParts : AppParts
    {
        public Image image;
        public Text label;
        public Image fillImage;
        public void UpdateImage(Sprite sp)
        {
            if (image != null)
                image.sprite = sp;
        }

        public void ShowImage(bool show)
        {
            if (image != null && image.enabled != show)
                image.enabled = show;
        }

        public void UpdateLabel(string tip)
        {
            if (label != null)
                label.text = tip;
        }

        public void UpdateColor(Color color)
        {
            if (fillImage != null)
                fillImage.color = color;
        }

        public void UpdateAmount(float amount)
        {
            if (fillImage != null)
                fillImage.fillAmount = amount;
        }

        public override void Clear()
        {

        }
    }
}
