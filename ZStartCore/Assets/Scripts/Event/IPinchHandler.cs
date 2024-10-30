using UnityEngine.EventSystems;

namespace ZStart.Core.Event
{
    public interface IPinchHandler: IEventSystemHandler
    {
        void OnPinch (PinchEventData data);
    }
   
    public interface IRotateHandler : IEventSystemHandler
    {
        void OnRotate (RotateEventData data);
    }
}
