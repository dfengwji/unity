using UnityEngine;

namespace ZStart.VRoom.Item
{
    public abstract class InteractiveItem:MonoBehaviour
    {
        public string identify = "";

        protected virtual void Start()
        {
            identify = "item-" + GetInstanceID();
            RoomSession.Instance.Register(this);
        }

        public virtual void OnGazeHover()
        {

        }

        public virtual void OnGazeEnter()
        {
            
        }

        public virtual void OnGazeOut()
        {
        }

        public virtual void OnGazeActive()
        {

        }
    }
}
