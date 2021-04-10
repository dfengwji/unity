using UnityEngine;

namespace ZStart.VRoom.Item
{
    public class AudioItem: InteractiveItem
    {
        public AudioSource source;
        public int delay = 0;
        public override void OnGazeEnter()
        {
        }

        public override void OnGazeActive()
        {
            source.Play((ulong)delay);
        }

        public override void OnGazeOut()
        {
            source.Stop();
        }
    }
}
