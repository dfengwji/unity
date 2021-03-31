using UnityEngine;

namespace ZStart.RGraph.View
{
    public abstract class AppUIParts: Core.View.ZUIComponent
    {
        public string identify = "";
        //private RectTransform target = null;
        //public RectTransform mTransform
        //{
        //    get
        //    {
        //        if(target == null)
        //        {
        //            target = GetComponent<RectTransform>();
        //        }
        //        return target;
        //    }
        //}
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

        public virtual void Lock(bool locked)
        {

        }

        public override void Clear()
        {

        }
    }
}
