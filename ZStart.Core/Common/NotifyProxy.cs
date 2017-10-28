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

    internal class NotifyObject<T>
    {
        public int notify;
        public GameObject target;
        public T param;
        public NotifyObject()
        {

        }
    }

    public class NotifyProxy<T>
    {
        private Dictionary<int, List<NotifyData<T>>> notifyDic = null;
        private List<NotifyObject<T>> notifyList;
        private List<NotifyObject<T>> cacheNotifys;
        public NotifyProxy()
        {
            notifyDic = new Dictionary<int, List<NotifyData<T>>>();
            notifyList = new List<NotifyObject<T>>();
            cacheNotifys = new List<NotifyObject<T>>();
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

        public void AddNotify(int notify, GameObject target, UnityAction<T> action)
        {
            List<NotifyData<T>> list = null;
            if (notifyDic.ContainsKey(notify))
            {
                list = notifyDic[notify];
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].target == target)
                    {
                        list[i].uEvent.RemoveListener(action);
                        list[i].uEvent.AddListener(action);
                        return;
                    }
                }
                NotifyData<T> info = new NotifyData<T>();
                info.target = target;
                info.notify = notify;
                info.uEvent = new NotifyEvent<T>();
                info.uEvent.AddListener(action);
                list.Add(info);
            }
            else
            {
                list = new List<NotifyData<T>>();
                NotifyData<T> info = new NotifyData<T>();
                info.target = target;
                info.notify = notify;
                info.uEvent = new NotifyEvent<T>();
                info.uEvent.AddListener(action);
                list.Add(info);
                notifyDic.Add(notify, list);
            }
        }

        public void AddNotify(int notify, UnityAction<T> action)
        {
            AddNotify(notify, null, action);
        }

        public void RemoveNotify(int notify, GameObject target)
        {
            if (notifyDic.ContainsKey(notify))
            {
                List<NotifyData<T>> list = notifyDic[notify];
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].target == target)
                    {
                        list[i].Clear();
                        list.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        public void RemoveNotify(int notify)
        {
            if (notifyDic.ContainsKey(notify))
            {
                List<NotifyData<T>> list = notifyDic[notify];
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Clear();
                }
                list.Clear();
                notifyDic.Remove(notify);
            }
        }

        public void ExcuteAction(int notify, GameObject target, T data)
        {
            if (notifyDic.ContainsKey(notify))
            {
                List<NotifyData<T>> list = notifyDic[notify];
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].target == target && list[i].target.activeInHierarchy)
                    {
                        list[i].uEvent.Invoke(data);
                        break;
                    }
                }
            }
        }

        public void ExcuteAction(int notify, T data)
        {
            if (notifyDic.ContainsKey(notify))
            {
                List<NotifyData<T>> list = notifyDic[notify];
                for (int i = 0; i < list.Count; i++)
                {
                    NotifyData<T> act = list[i];
                    if (act.target == null)
                    {
                        act.uEvent.Invoke(data);
                    }
                    else
                    {

                        if (act.target.activeInHierarchy)
                            act.uEvent.Invoke(data);
                    }

                }
            }
        }

        public void PushNotify(int notify, GameObject target, T data)
        {
            NotifyObject<T> obj = new NotifyObject<T>();
            obj.notify = notify;
            obj.target = target;
            obj.param = data;
            cacheNotifys.Add(obj);
        }

    }
}
