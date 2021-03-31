
using System.Collections.Generic;
using ZStart.VRoom.Item;
using ZStart.VRoom.Manager;

namespace ZStart.VRoom
{
    public class RoomSession: Core.ZBehaviourBase
    {
        private static RoomSession instance = null;

        public static RoomSession Instance
        {
            get
            {
                return instance;
            }
        }

        public List<InteractiveItem> itItems;

        private void Awake()
        {
            instance = this;
            itItems = new List<InteractiveItem>(20);
        }

        private void Update()
        {
            VRNotifyManager.Instance.UpdateNotify();
        }

        public void Register(InteractiveItem item)
        {
            itItems.Add(item);
        }

        public InteractiveItem GetItem(string identify)
        {
            for (int i = 0;i < itItems.Count;i += 1)
            {
                if(itItems[i].identify == identify)
                {
                    return itItems[i];
                }
            }
            return null;
        }
    }
}
