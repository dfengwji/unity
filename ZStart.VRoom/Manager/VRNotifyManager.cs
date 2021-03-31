

using UnityEngine;
using UnityEngine.Events;
using ZStart.Core.Common;
using ZStart.VRoom.Enum;

namespace ZStart.VRoom.Manager
{
    public class VRNotifyManager
    {
        private static VRNotifyManager mInstance = null;

        private NotifyProxy<object> proxy = null;
        private VRNotifyManager()
        {
            proxy = new NotifyProxy<object>();
        }

        public static VRNotifyManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new VRNotifyManager();
                return mInstance;
            }
        }

        public void UpdateNotify()
        {
            proxy.UpdateNotify();
        }

        #region Static Function
        public static void AddListener(NotifyType notify, UnityAction<object> action)
        {
            Instance.proxy.AddNotify((int)notify, action);
        }

        public static void AddListener(NotifyType notify, GameObject tareget, UnityAction<object> action)
        {
            Instance.proxy.AddNotify((int)notify, tareget, action);
        }

        public static void RemoveListener(NotifyType notify)
        {
            Instance.proxy.RemoveNotify((int)notify);
        }

        public static void RemoveListener(NotifyType notify, GameObject target)
        {
            Instance.proxy.RemoveNotify((int)notify, target);
        }

        public static void SendNotify(NotifyType notify, object data)
        {
            Instance.proxy.PushNotify((int)notify, null, data);
        }

        public static void SendNotify(NotifyType notify, GameObject target, object data)
        {
            Instance.proxy.PushNotify((int)notify, target, data);
        }
        #endregion
    }
}
