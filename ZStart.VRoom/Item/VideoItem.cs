using UnityEngine;
using UnityEngine.Video;

namespace ZStart.VRoom.Item
{
    public class VideoItem: InteractiveItem
    {
        public Transform playSp;
        public Transform pauseSp;
        public VideoPlayer player;

        protected override void Start()
        {
            base.Start();
            Pause();
        }

        public void Play()
        {
            if (player.isPlaying)
                return;
            player.Play();
            playSp.gameObject.SetActive(false);
            pauseSp.gameObject.SetActive(false);
        }

        public void Pause()
        {
            if (player.isPaused)
                return;
            player.Pause();
            playSp.gameObject.SetActive(true);
            pauseSp.gameObject.SetActive(false);
        }

        public override void OnGazeEnter()
        {
            
        }

        public override void OnGazeActive()
        {
            Play();
        }

        public override void OnGazeOut()
        {
            Pause();
        }
    }
}
