using UnityEngine;
using ZStart.Core.Enum;

namespace ZStart.Core.Util
{
    public class LayerMaskUtil
    {
        private LayerMaskUtil() { }

        public static bool CompareLayer(int ly, LayerMaskType flag)
        {
            int layer = LayerMask.NameToLayer(flag.ToString());
            if (ly == layer)
                return true;
            return false;
        }

        public static int GetRaycastLayer(LayerMaskType flag)
        {
            int layer = LayerMask.NameToLayer(flag.ToString());
            return 1 << layer;
        }

        public static int GetRaycastLayer(params LayerMaskType[] flags)
        {
            if (flags == null || flags.Length == 0)
                return 0;
            int layer = 0;
            for (int i = 0; i < flags.Length; i++)
            {
                LayerMaskType mask = flags[i];
                int temp = LayerMask.NameToLayer(mask.ToString());
                layer += 1 << temp;
            }
            return layer;
        }
    }
}
