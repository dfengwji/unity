using UnityEngine;
using UnityEngine.EventSystems;

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
        
        [SerializeField]
        private bool _isStartEnd = false;
        public bool isStartEnd
        {
            get { return _isStartEnd; }
        }


        protected override void Start()
        {
            //Debug.LogWarning("Can not override the Start,replace of Init that script name = "+this.name);
            _isStartEnd = true;
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
