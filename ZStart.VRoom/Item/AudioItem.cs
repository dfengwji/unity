using UnityEngine;

namespace ZStart.VRoom.Item
{
    public class AudioItem: InteractiveItem
    {
        public AudioSource bgSource;
        public AudioSource source;
        public int delay = 0;
        public override void OnGazeEnter()
        {
        }

        public override void OnGazeActive()
        {
            if(bgSource)
                bgSource.mute = true;
            if (delay > 0)
                source.Play((ulong)delay);
            else
                source.Play();
        }

        public override void OnGazeOut()
        {
            if(bgSource)
                bgSource.mute = false;
            source.Stop();
        }
    }
}
