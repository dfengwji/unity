using System.Collections.Generic;
using UnityEngine;

namespace ZStart.RGraph.Util
{
    public static class TouchUtil
    {
        public static float GetScreenDistance(List<Touch> fingers)
        {
            var distance = default(float);
            var center = default(Vector2);

            if (TryGetScreenCenter(fingers, ref center) == true)
            {
                TryGetScreenDistance(fingers, center, ref distance);
            }

            return distance;
        }

        public static bool TryGetScreenDistance(List<Touch> fingers, Vector2 center, ref float distance)
        {
            if (fingers != null)
            {
                var total = 0.0f;
                var count = 0;

                for (var i = fingers.Count - 1; i >= 0; i--)
                {
                    var finger = fingers[i];
                    total += Vector2.Distance(finger.position, center);
                    count += 1;
                }

                if (count > 0)
                {
                    distance = total / count; return true;
                }
            }

            return false;
        }

        public static bool TryGetScreenCenter(List<Touch> fingers, ref Vector2 center)
        {
            if (fingers != null)
            {
                var total = Vector2.zero;
                var count = 0;

                for (var i = fingers.Count - 1; i >= 0; i--)
                {
                    total += fingers[i].position;
                    count += 1;
                }

                if (count > 0)
                {
                    center = total / count; return true;
                }
            }

            return false;
        }
    }
}
