using UnityEngine.EventSystems;

namespace ZStart.Core.Event
{
    public interface ITimerHandler : IEventSystemHandler
    {
        string uid { get; }
        int cdTime { get; }
        int OnTimerUpdate();
        void OnTimerOver();
    }
}
