
using UnityEngine;
namespace ZStart.Core.View
{
    public abstract class ZUIGroup:ZUIBehaviour
    {
        protected bool _isOpen = false;
        public bool isOpen
        {
            get { return _isOpen; }
        }
        public virtual void Appear() { }

        public virtual void Disappear() { }
    }
}
