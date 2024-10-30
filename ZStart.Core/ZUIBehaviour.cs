using UnityEngine;

namespace ZStart.Core
{ 
    public abstract class ZUIBehaviour : UIBehaviour
    {
        protected ZUIBehaviour(){}
        private RectTransform _mTransform;
        public RectTransform mTransform
        {
            get
            {
                if (_mTransform == null)
                    _mTransform = GetComponent<RectTransform>();
                return _mTransform;
            }
        }
        
        [HideInInspector]
        private bool isStartEnd = false;
        public bool IsStartEnd
        {
            get { return isStartEnd; }
        }


        protected override void Start()
        {
            //Debug.LogWarning("Can not override the Start,replace of Init that script name = "+this.name);
            isStartEnd = true;
        }

        public virtual void Clear()
        {
        }

        public virtual void Select()
        {
            if (EventSystem.current.alreadySelecting)
                return;

            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }
}
