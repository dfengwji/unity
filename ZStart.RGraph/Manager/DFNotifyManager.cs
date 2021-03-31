using UnityEngine;
using UnityEngine.Events;
using ZStart.Core.Common;
using ZStart.RGraph.Enum;

namespace ZStart.RGraph.Manager
{
    public class DFNotifyManager
    {
        private static DFNotifyManager mInstance = null;

        private NotifyProxy<object> proxy = null;
        private DFNotifyManager()
        {
            proxy = new NotifyProxy<object>();
        }

        public static DFNotifyManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new DFNotifyManager();
                return mInstance;
            }
        }

        public void Update()
        {
            proxy.UpdateNotify();
        }

        #region Static Function
        public static void AddListener(DFNotifyType notify, UnityAction<object> action)
        {
            Instance.proxy.AddNotify((int)notify, action);
        }

        public static void AddListener(DFNotifyType notify, GameObject tareget, UnityAction<object> action)
        {
            Instance.proxy.AddNotify((int)notify, tareget, action);
        }

        public static void RemoveListener(DFNotifyType notify)
        {
            Instance.proxy.RemoveNotify((int)notify);
        }

        public static void RemoveListener(DFNotifyType notify, GameObject target)
        {
            Instance.proxy.RemoveNotify((int)notify, target);
        }

        public static void SendNotify(DFNotifyType notify, object data)
        {
            Instance.proxy.PushNotify((int)notify, null, data);
        }

        public static void SendNotify(DFNotifyType notify, GameObject target, object data)
        {
            Instance.proxy.PushNotify((int)notify, target, data);
        }
        #endregion
    }
}
