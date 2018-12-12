using UnityEngine;
using UnityEngine.UI;

namespace ZStart.Common.View.Parts
{
    public class AlertParts : AppParts
    {
        public Text label;
        public Image image;
        public float time;          // Timestamp of when this entry was added
        public float stay = 0f;     // How long the text will appear to stay stationary on the screen
        public float offsetY = 0f;  // How far the object has moved based on time
        public float offsetX = 0f;
        public float initOffY = 0f;
        public int sign = 0;
        public float dispear = 0f;
        public float val = 0f;
        public Vector3 initPos = Vector3.zero;
        public float movementStart { get { return time + stay; } }

        public bool isIcon
        {
            get
            {
                if (image == null)
                    return false;
                else
                    return true;
            }
        }

        public void UpdateImage(Sprite sp)
        {
            if (image == null)
                return;
            image.sprite = sp;
            image.enabled = true;
        }

        public override void Clear()
        {

        }
    }
}
