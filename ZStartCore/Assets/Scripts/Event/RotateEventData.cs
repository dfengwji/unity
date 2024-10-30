using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace ZStart.Core.Event
{
    public class RotateEventData : BaseEventData
    {
        public List<PointerEventData> data;
        public float rotateDelta;

        public RotateEventData(EventSystem sys, float d = 0)
            : base(sys)
        {
            data = new List<PointerEventData>();
            rotateDelta = d;
        }
    }
}
