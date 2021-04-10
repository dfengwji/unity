using UnityEngine;
using UnityEngine.UI;

namespace ZStart.EBook.View.Widget
{
    public class IconButton : LabelButton
    {
        public Image icon;
        public RawImage rawIcon;

        public MaskableGraphic Graphic
        {
            get
            {
                InitComponents();
                if (icon != null)
                {
                    return icon;
                }
                return rawIcon;
            }
        }

        protected override void Start()
        {
            InitComponents();
            base.Start();
        }

        private void InitComponents()
        {
            if (icon != null || rawIcon != null)
            {
                return;
            }
            if (transform.childCount > 0)
            {
                if (icon == null)
                    icon = GetComponentInChildren<Image>();
                if (rawIcon == null)
                    rawIcon = GetComponentInChildren<RawImage>();
            }
            else
            {
                if (icon == null)
                    icon = GetComponent<Image>();
                if (rawIcon == null)
                    rawIcon = GetComponent<RawImage>();
            }
        }

        public void ShowImage(bool show)
        {
            InitComponents();
            if (icon != null)
                icon.enabled = show;
            if (rawIcon != null)
                rawIcon.enabled = show;
        }

        public void UpdateSprite(Sprite sp)
        {
            InitComponents();
            if (icon == null)
                return;
            if (sp != null)
                icon.sprite = sp;
            icon.enabled = sp == null ? false : true;
        }

        public void UpdateTexture(Texture2D tex)
        {
            InitComponents();
            if (rawIcon == null)
                return;
            if (tex != null)
                rawIcon.texture = tex;
            rawIcon.enabled = tex == null ? false : true;
        }

        public void UpdateAdaptTexture(Texture2D tex)
        {
            if (rawIcon == null)
                return;
            rawIcon.texture = tex;
            rawIcon.enabled = tex == null ? false : true;
            if (tex != null)
                rawIcon.SetNativeSize();
        }

        public void UpdateAdaptSprite(Sprite sp)
        {
            if (icon == null)
                return;
            icon.sprite = sp;
            icon.enabled = sp == null ? false : true;
            if (sp != null)
                icon.SetNativeSize();
        }

        public void UpdateAmount(float amount)
        {
            if (icon != null && icon.type == Image.Type.Filled)
                icon.fillAmount = amount;
        }
    }
}
