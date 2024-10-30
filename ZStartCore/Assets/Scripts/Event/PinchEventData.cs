using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace ZStart.Core.Event
{
    public class PinchEventData : BaseEventData
    {
        public List<PointerEventData> data;
        public float pinchDelta;

        public PinchEventData(EventSystem sys, float d = 0)
            : base(sys)
        {
            data = new List<PointerEventData>();
            pinchDelta = d;
        }
    }
}
