using UnityEngine;
namespace ZStart.Common.View
{
    public abstract class AppParts : Core.View.ZUIComponent
    {
       
        public virtual float Height
        {
            get
            {
                return mTransform.sizeDelta.y;
            }
            set
            {
                mTransform.sizeDelta = new Vector2(mTransform.sizeDelta.x, value);
            }
        }

        public virtual float Width
        {
            get
            {
                return mTransform.sizeDelta.x;
            }
            set
            {
                mTransform.sizeDelta = new Vector2(value, mTransform.sizeDelta.y);
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        public override void Show()
        {
            if (isActiveAndEnabled == false)
                gameObject.SetActive(true);
        }

        public override void UnShow()
        {
            if (isActiveAndEnabled)
                gameObject.SetActive(false);
        }
    }
}
