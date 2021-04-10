using UnityEngine;
using UnityEngine.EventSystems;

namespace ZStart.EBook.View
{
    public abstract class BaseParts : UIBehaviour
    {
        public string identify = "";
        private RectTransform _mTransform;
        public RectTransform mTransform
        {
            get
            {
                if (_mTransform == null)
                {
                    _mTransform = transform as RectTransform;
                }
                return _mTransform;
            }
        }

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

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Clear() { }

        public virtual void Lock(bool locked)
        {

        }
    }
}
