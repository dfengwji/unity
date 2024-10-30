using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZStart.Core.Event;

namespace ZStart.Core.Manager
{
    public class ZTimerManager
    {
        private static ZTimerManager _instance = null;

        public static ZTimerManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ZTimerManager();
                }
                return _instance;
            }
        }

        private List<ITimerHandler> timerList = null;

        public ZTimerManager()
        {
            timerList = new List<ITimerHandler>();
        }

        public void AddTimer(ITimerHandler timer)
        {
            if (timer == null) return;
            timerList.Add(timer);
        }

        public void UpdateTimer()
        {
            List<ITimerHandler> indexs = new List<ITimerHandler>();
            for (int i = 0; i < timerList.Count;i++ )
            {
                int cd = timerList[i].OnTimerUpdate();
                if (cd < 1)
                {
                    timerList[i].OnTimerOver();
                    indexs.Add(timerList[i]);
                }
            }
            for (int i = 0; i < indexs.Count;i++ )
            {
                if(timerList.Contains(indexs[i]))
                    timerList.Remove(indexs[i]);
            }
        }

        public void RemoveTimer(string uid)
        {
            for(int i = 0;i < timerList.Count;i++){
                if(timerList[i].uid == uid){
                    timerList.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveTimer(ITimerHandler timer)
        {
            if(timerList.Contains(timer))
                timerList.Remove(timer);
        }

        public ITimerHandler GetTimer(string uid)
        {
            for (int i = 0; i < timerList.Count; i++)
            {
                if (timerList[i].uid == uid)
                    return timerList[i];
            }
            return null;
        }

        public int GetTimerCD(string uid)
        {
            ITimerHandler timer = GetTimer(uid);
            if (timer == null)
                return 0;
            else
                return timer.cdTime;
        }

        public static void Add(ITimerHandler timer)
        {
            Instance.AddTimer(timer);
        }

        public static void Update()
        {
            Instance.UpdateTimer();
        }

        public static ITimerHandler Get(string uid)
        {
            return Instance.GetTimer(uid);
        }

        public static int GetCD(string uid)
        {
            return Instance.GetTimerCD(uid);
        }

        public static void Remove(string uid)
        {
            Instance.RemoveTimer(uid);
        }
    }
}
