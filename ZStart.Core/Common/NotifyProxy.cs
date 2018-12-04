using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace ZStart.Core.Common
{
    internal class NotifyData<T>
    {
        public GameObject target = null;
        public int notify = 0;
        public UnityEvent<T> uEvent = null;

        public void Clear()
        {
            target = null;
            notify = 0;
            if (uEvent != null)
                uEvent.RemoveAllListeners();
            uEvent = null;
        }
    }

    internal class NotifyEvent<T> : UnityEvent<T>
    {
        public NotifyEvent() { }
    }

    public class NotifyObject<T>
    {
        public int notify;
        public GameObject target;
        public T param;
        public NotifyObject()
        {

        }
    }

    internal class NotifySet<T>
    {
        public int notify = 0;
        private List<NotifyData<T>> set = null;
        public int Count
        {
            get
            {
                return set.Count;
            }
        }

        public NotifySet()
        {
            set = new List<NotifyData<T>>();
        }

        public void RemoveData(GameObject target)
        {
            for (int i = 0;i < set.Count;i++)
            {
                if (set[i].target != null && set[i].target == target)
                {
                    set.RemoveAt(i);
                    break;
                }
            }
        }

        public void ExceteAction(GameObject target,T data)
        {
            for (int i = 0; i < set.Count; i++)
            {
                if (set[i].target == target && set[i].target.activeInHierarchy)
                {
                    set[i].uEvent.Invoke(data);
                    break;
                }
            }
        }

        public void ExceteAction(T data)
        {
            for (int i = 0; i < set.Count; i++)
            {
                if (set[i].target == null)
                {
                    set[i].uEvent.Invoke(data);
                }
                else
                {
                    if (set[i].target.activeInHierarchy)
                        set[i].uEvent.Invoke(data);
                }
            }
        }

        public void Clear()
        {
            notify = -1;
            set.Clear();
        }

        public void AddData(NotifyData<T> data)
        {
            set.Add(data);
        }
    }

    public class NotifyProxy<T>
    {
        private List<NotifyObject<T>> notifyList;
        private List<NotifyObject<T>> cacheNotifys;
        private List<NotifySet<T>> registerSets;

        public int RegistedCount
        {
            get
            {
                return registerSets.Count;
            }
        }

        public NotifyProxy()
        {
            notifyList = new List<NotifyObject<T>>();
            cacheNotifys = new List<NotifyObject<T>>();
            registerSets = new List<NotifySet<T>>();
            ZLog.Log("NotifyProxy...create!!!"+typeof(T).Name);
        }

        public void UpdateNotify()
        {
            if (notifyList.Count > 0)
            {
                for (int i = 0, max = notifyList.Count; i < max; i++)
                {
                    NotifyObject<T> obj = notifyList[i];
                    if (obj.target == null)
                        ExcuteAction((int)obj.notify, obj.param);
                    else
                        ExcuteAction((int)obj.notify, obj.target, obj.param);
                }
                notifyList.Clear();
            }
            if (cacheNotifys.Count > 0)
            {
                notifyList.AddRange(cacheNotifys.ToArray());
                cacheNotifys.Clear();
            }
        }

        private NotifySet<T> GetNotifySet(int notify)
        {
            for (int i = 0;i < registerSets.Count;i++)
            {
                if (registerSets[i].notify == notify)
                {
                    return registerSets[i];
                }
            }
            return null;
        }

        public void AddNotify(int notify, GameObject target, UnityAction<T> action)
        {
            NotifySet<T> set = GetNotifySet(notify);
            if (set == null)
            {
                set = new NotifySet<T>();
                set.notify = notify;
                registerSets.Add(set);
            }
            else
            {
                set.RemoveData(target);
            }
            NotifyData<T> info = new NotifyData<T>
            {
                target = target,
                notify = notify,
                uEvent = new NotifyEvent<T>()
            };
            info.uEvent.AddListener(action);
            set.AddData(info);
        }

        public void AddNotify(int notify, UnityAction<T> action)
        {
            AddNotify(notify, null, action);
        }

        public void RemoveNotify(int notify, GameObject target)
        {
            NotifySet<T> set = GetNotifySet(notify);
            if (set != null)
            {
                set.RemoveData(target);
            }
        }

        public void RemoveNotify(int notify)
        {
            for (int i = 0; i < registerSets.Count; i++)
            {
                if (registerSets[i].notify == notify)
                {
                    registerSets.RemoveAt(i);
                    return;
                }
            }
        }

        public void ExcuteAction(int notify, GameObject target, T data)
        {
            NotifySet<T> set = GetNotifySet(notify);
            if (set != null)
            {
                set.ExceteAction(target, data);
            }
        }

        public void ExcuteAction(int notify, T data)
        {
            NotifySet<T> set = GetNotifySet(notify);
            if (set != null)
            {
                set.ExceteAction(data);
            }
        }

        public void PushNotify(int notify, GameObject target, T data)
        {
            NotifyObject<T> obj = new NotifyObject<T>
            {
                notify = notify,
                target = target,
                param = data
            };
            cacheNotifys.Add(obj);
        }

    }
}
