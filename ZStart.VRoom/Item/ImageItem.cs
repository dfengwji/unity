using UnityEngine;
using ZStart.VRoom.Manager;

namespace ZStart.VRoom.Item
{
    public class ImageItem: InteractiveItem
    {
        public Texture2D source;
        public string desc = "";
        public override void OnGazeEnter()
        {
            //Debug.LogWarning("image gaze enter...name = " + name);
            VRNotifyManager.SendNotify(Enum.NotifyType.OnLookIn, identify);
        }

        public override void OnGazeActive()
        {
            //Debug.LogWarning("image gaze active...name = " + name);
            VRNotifyManager.SendNotify(Enum.NotifyType.OnUIImage, identify);
        }

        public override void OnGazeOut()
        {
            //Debug.LogError("image gaze out...name = " + name);
            VRNotifyManager.SendNotify(Enum.NotifyType.OnLookOut, identify);
        }
    }
}
