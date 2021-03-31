using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZStart.Core.Controller
{
    public class ZCallbackController : ZSingletonBehaviour<ZCallbackController>
    {
        public class CallbackInfo
        {
            public string uid;
            public UnityAction callback;
            public GameObject target;

            public CallbackInfo(string uuid, UnityAction action)
            {
                uid = uuid;
                callback = action;
            }

            public CallbackInfo(GameObject go, UnityAction action)
            {
                target = go;
                uid = go.name + "-" + go.GetInstanceID();
                callback = action;
            }

            public void Clear()
            {
                target = null;
                uid = "";
                callback = null;
            }

            public override string ToString()
            {
                return uid;
            }
        }

        List<CallbackInfo> updateCallbacks;

        protected override void Awake()
        {
            base.Awake();
            updateCallbacks = new List<CallbackInfo>();
        }

        void Update()
        {
            if (updateCallbacks != null && updateCallbacks.Count > 0)
            {
                for (int i = 0, max = updateCallbacks.Count; i < max; i++)
                {
                    if (updateCallbacks[i].target != null)
                    {
                        if (updateCallbacks[i].target.activeSelf)
                            updateCallbacks[i].callback.Invoke();
                    }
                    else if (!string.IsNullOrEmpty(updateCallbacks[i].uid))
                        updateCallbacks[i].callback.Invoke();
                }
            }
        }

        public void RegisterUpdateEvent(string uid, UnityAction callback)
        {
            if (callback == null || string.IsNullOrEmpty(uid))
                return;
            if (HasUpdateEvent(uid))
            {
                Debug.Log("Callback Controller... had register uid = " + uid);
                return;
            }
            updateCallbacks.Add(new CallbackInfo(uid, callback));
        }

        public void RegisterUpdateEvent(GameObject target, UnityAction callback)
        {
            if (callback == null || target == null)
                return;
            if (HasUpdateEvent(target))
            {
                Debug.Log("Callback Controller... had register target = " + target);
                return;
            }
            updateCallbacks.Add(new CallbackInfo(target, callback));
        }

        public void UnRegisterUpdateEvent(string uid)
        {
            for (int i = 0, max = updateCallbacks.Count; i < max; i++)
            {
                if (updateCallbacks[i].uid == uid)
                {
                    updateCallbacks[i].Clear();
                    updateCallbacks.RemoveAt(i);
                    break;
                }
            }
        }

        public void UnRegisterUpdateEvent(GameObject target)
        {
            for (int i = 0, max = updateCallbacks.Count; i < max; i++)
            {
                if (updateCallbacks[i].target == target)
                {
                    updateCallbacks[i].Clear();
                    updateCallbacks.RemoveAt(i);
                    break;
                }
            }
        }

        private bool HasUpdateEvent(string uid)
        {
            for (int i = 0, max = updateCallbacks.Count; i < max; i++)
            {
                if (updateCallbacks[i].uid == uid)
                    return true;
            }
            return false;
        }

        private bool HasUpdateEvent(GameObject target)
        {
            for (int i = 0, max = updateCallbacks.Count; i < max; i++)
            {
                if (updateCallbacks[i].target == target)
                    return true;
            }
            return false;
        }
    }
}
