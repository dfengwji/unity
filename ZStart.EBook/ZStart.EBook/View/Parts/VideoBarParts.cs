using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using ZStart.EBook.Common;

namespace ZStart.EBook.View.Parts
{
    public class VideoBarParts:BaseParts
    {
        public Text timeLabel;
        public Image audioImage;
        public Button exitButton;
        public Button pauseButton;
        public Button playButton;
        public Slider videoProgress;
        public Slider audioProgress;

        public AudioSource[] audioSources;
        public Image volumeSp;
        public Image muteSp;

        private VideoPlayer player;
        private Coroutine playCoroutine;
        public long TotalTime
        {
            get
            {
                float val = player.frameCount / player.frameRate;
                if (val < 1)
                    val = 1;
                return (long)val;
            }
        }

        public long CurrentTime
        {
            get
            {
                float val = player.frame / player.frameRate;
                return (long)val;
            }
        }

        public float Progress
        {
            get
            {
                return player.frame / (float)player.frameCount;
            }
        }

        public float Volume
        {
            set
            {
                float vol = value;
                if (vol > 1)
                {
                    vol = 1;
                }
                else if (vol < 0)
                    vol = 0;
                for (int i = 0; i < audioSources.Length; i++)
                {
                    audioSources[i].volume = vol;
                }
            }
            get
            {
                return audioSources[0].volume;
            }
        }

        public bool IsMute
        {
            set
            {
                for (int i = 0; i < audioSources.Length; i++)
                {
                    audioSources[i].mute = value;
                }
            }
            get
            {
                return audioSources[0].mute;
            }
        }

        protected override void Start()
        {
            Init();
            base.Start();
            videoProgress.onValueChanged.AddListener(OnVideoProgress);
            audioProgress.onValueChanged.AddListener(OnAudioProgress);
            playButton.onClick.AddListener(OnPlayHandler);
            pauseButton.onClick.AddListener(OnPauseHandler);
        }

        private void Init()
        {
            if(timeLabel != null)
            {
                return;
            }
            timeLabel = mTransform.Find("TimeLabel").GetComponent<Text>();
            audioImage = mTransform.Find("AudioImage").GetComponent<Image>();
            exitButton = mTransform.Find("ScreenButton").GetComponent<Button>();
            pauseButton = mTransform.Find("PauseButton").GetComponent<Button>();
            playButton = mTransform.Find("PlayButton").GetComponent<Button>();
            videoProgress = mTransform.Find("VideoProgress").GetComponent<Slider>();
            audioProgress = mTransform.Find("AudioVolume").GetComponent<Slider>();
            audioSources = mTransform.GetComponentsInParent<AudioSource>();
            volumeSp = mTransform.Find("VoiceSp").GetComponent<Image>();
            muteSp = mTransform.Find("MuteSp").GetComponent<Image>();
        }

        public void SetPlayer(VideoPlayer pl)
        {
            player = pl;
            player.prepareCompleted += PrepareHandle;
            player.started += StartPlayHandle;
            player.loopPointReached += CompletePlayHandle;
            player.seekCompleted += SeekCompleteHanle;
            timeLabel.text = Utility.FormatTime((int)CurrentTime);
        }

        public override void Show()
        {
            base.Show();
            if (player.isPlaying && playCoroutine == null)
                playCoroutine = StartCoroutine(PlayInspector());
            timeLabel.text = Utility.FormatTime((int)CurrentTime);
            audioProgress.value = Volume;
            if (Volume < 0.1)
            {
                audioImage.sprite = muteSp.sprite;
            }
            else
            {
                audioImage.sprite = volumeSp.sprite;
            }
            playButton.gameObject.SetActive(!player.isPlaying);
            pauseButton.gameObject.SetActive(player.isPlaying);
        }

        public override void Hide()
        {
            base.Hide();
            playCoroutine = null;
            StopAllCoroutines();
        }

        private void PrepareHandle(VideoPlayer source)
        {
            Core.ZLog.Warning("video player controller::prepare complete = " + source.isPrepared);
            OnVideoProgress(videoProgress.value);
            playButton.gameObject.SetActive(!player.isPlaying);
            pauseButton.gameObject.SetActive(player.isPlaying);
        }

        private void SeekCompleteHanle(VideoPlayer source)
        {
            //this.LogWarning("video player controller::seek complete = " + source.frame);
        }

        private void StartPlayHandle(VideoPlayer source)
        {
            Core.ZLog.Warning("video player controller::start play = " + player.isPlaying);
            if (isActiveAndEnabled && playCoroutine == null && player.isPlaying)
                playCoroutine = StartCoroutine(PlayInspector());
            playButton.gameObject.SetActive(!player.isPlaying);
            pauseButton.gameObject.SetActive(player.isPlaying);
        }

        private void CompletePlayHandle(VideoPlayer source)
        {

        }

        IEnumerator PlayInspector()
        {
            while (true)
            {
                if (player.isPrepared)
                    videoProgress.value = Progress;
                yield return new WaitForSeconds(1f);
            }
        }

        public void Play(string path)
        {
            if (path != player.url)
            {
                player.url = path;
                videoProgress.value = 0;
            }
            player.Play();
        }

        public void OnPlayHandler()
        {
            if (player.isPlaying)
                return;
            player.Play();
        }

        public void OnPauseHandler()
        {
            if (playCoroutine != null)
                StopCoroutine(playCoroutine);
            playCoroutine = null;
            if (!player.isPlaying)
                return;
            player.Pause();
            playButton.gameObject.SetActive(true);
            pauseButton.gameObject.SetActive(false);
        }

        private void OnVideoProgress(float amount)
        {
            if (!player.isPrepared)
                return;
            long frame = Mathf.RoundToInt(player.frameCount * amount);
            timeLabel.text = Utility.FormatTime((int)(frame / player.frameRate));
            if (player.frame == frame)
                return;
            player.frame = frame;
        }

        private void OnAudioProgress(float amount)
        {
            Volume = amount;
            if (amount < 0.1)
            {
                audioImage.sprite = muteSp.sprite;
            }
            else
            {
                audioImage.sprite = volumeSp.sprite;
            }
        }
    }
}
