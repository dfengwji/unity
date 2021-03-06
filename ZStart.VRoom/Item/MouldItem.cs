﻿using UnityEngine;
using ZStart.VRoom.Manager;

namespace ZStart.VRoom.Item
{
    public class MouldItem:InteractiveItem
    {
        public Transform target;
        public string desc;
        public float speed = 1.2f;
        public Vector3 direction = Vector3.up;
        public bool isLooking = false;
        private void Update()
        {
            if (isLooking)
            {
                target.Rotate(direction, Time.deltaTime * speed);
            }
        }

        public override void OnGazeEnter()
        {
        }

        public override void OnGazeActive()
        {
            isLooking = true;
            VRNotifyManager.SendNotify(Enum.NotifyType.OnUILabel, desc);
        }

        public override void OnGazeOut()
        {
            isLooking = false;
            VRNotifyManager.SendNotify(Enum.NotifyType.OnLookOut, identify);
        }
    }
}
