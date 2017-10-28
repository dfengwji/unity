using UnityEngine;
using UnityEngine.UI;
namespace ZStart.Core.View
{
    public abstract class ZUIComponent:ZUIBehaviour
    {
        public virtual void Show() { }

        public virtual void UnShow() { }

        public void ChildrenEnable(Transform parent, bool able)
        {

        }

        public void OpenCollider(bool turnOn)
        {
            ColliderEnable(mTransform, turnOn);
        }

        protected void ColliderEnable(Transform rootObj, bool enable)
        {
            if (rootObj == null)
                return;
            //Debug.LogWarning("ColliderEnable....." + this.name + " --- " +enable);
            Collider[] colliders = rootObj.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = enable;
            }
        }
    }
}
