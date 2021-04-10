using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ZStart.Core.Controller;
using ZStart.Core.Manager;

namespace ZStart.EBook.View.Parts
{
    public class PageParts:BaseParts
    {
        public enum VideoPosition
        {
            Top = 0,
            Middle = 1,
            Bottom = 2
        }

        public enum Option
        {
            None = 0,
            Play = 1,
            FullScreen = 2,
            Pause = 3,
        }

        public RawImage image;
        public Button audioButton;
        public Button playButton;
        public Button fullButton;
        public RectTransform videoBox;
        public CanvasGroup videoTools;
        public RectTransform videoRender;
        public RawImage videoCover;
        public GameObject footBox;
        public Text footLabel;
        public Vector2 positionTop = new Vector2(0, 260);
        public Vector2 positionMiddle = new Vector2(0, 0);
        public Vector2 positionBottom = new Vector2(0, -260);
        protected string audioPath;
        public string videoPath;
        protected Sprite turnOnSp;
        protected Sprite turnOffSp;
        private bool isPlaying = false;
        protected UnityAction<PageParts, Option, int> clickFun;

        protected override void Start()
        {
            Init();
            base.Start();
            //audioButton.onClick.AddListener(PlayAudioHandler);
            if (videoBox)
                videoBox.GetComponent<Button>().onClick.AddListener(OnClickVideoBG);
        }

        private void Init()
        {
            if (image != null)
                return;
            image = gameObject.GetComponent<RawImage>();
            audioButton = mTransform.Find("AudioButton").GetComponent<Button>();
            videoBox = mTransform.Find("VideoBox").GetComponent<RectTransform>();
            if (videoBox != null)
            {
                playButton = mTransform.Find("VideoBox/ToolBar/PlayButton").GetComponent<Button>();
                fullButton = mTransform.Find("VideoBox/ToolBar/FullButton").GetComponent<Button>();
                videoTools = mTransform.Find("VideoBox/ToolBar").GetComponent<CanvasGroup>();
                videoRender = mTransform.Find("VideoBox/RenderBox").GetComponent<RectTransform>();
                videoCover = mTransform.Find("VideoBox/CoverImage").GetComponent<RawImage>();
            }
            footBox = mTransform.Find("FootBox").gameObject;
            footLabel = mTransform.Find("FootBox/Text").GetComponent<Text>();
        }

        public override void Show()
        {
            base.Show();
            if (image == null)
                image = gameObject.AddComponent<RawImage>();
            if (GetComponent<Mask>() == null)
                gameObject.AddComponent<Mask>().showMaskGraphic = true;
            if (GetComponent<CanvasGroup>() == null)
                gameObject.AddComponent<CanvasGroup>();
            //if (playButton)
            //    playButton.onClick.AddListener(OnPlayVideoHandle);
            //if (fullButton)
            //    fullButton.onClick.AddListener(OnFullScreenHandle);
        }

        public virtual void UpdateInfo(BookModel.PageInfo info)
        {
            this.identify = info.uid;
        }

        public virtual void AddListener(UnityAction<PageParts, Option, int> fun)
        {
            this.clickFun = fun;
        }

        public void UpdatePage(int page)
        {
            if (footBox != null)
            {
                footLabel.text = string.Format("{0:D2}", page);
            }
        }

        public void UpdateTexture(Texture2D texture)
        {
            image.texture = texture;
        }

        public override void Hide()
        {
            base.Hide();
            //if (playButton)
            //    playButton.onClick.RemoveAllListeners();
            //if (fullButton)
            //    fullButton.onClick.RemoveAllListeners();
        }

        public void CheckVideo(string path, string cover)
        {
            if (!string.IsNullOrEmpty(path))
            {
                var array = path.Split(';');
                var pos = VideoPosition.Top;
                var video = "";
                if (array.Length == 1)
                {
                    video = array[0];
                }
                else if (array.Length > 1)
                {
                    video = array[0];
                    pos = (VideoPosition)int.Parse(array[1]);
                }
                PlayVideo(video, pos);

                videoCover.enabled = !string.IsNullOrEmpty(cover);
                if (!string.IsNullOrEmpty(cover))
                    ZImageController.Instance.Load(identify, cover, OnVideoCoverComplete);
            }
            else
            {
                PlayVideo("", PageParts.VideoPosition.Top);
            }
        }

        private void OnVideoCoverComplete(ZImageController.ImageInfo info)
        {
            videoCover.texture = info.texture;
        }

        public void PlayVideo(string path, VideoPosition pos)
        {
            if (videoBox == null)
            {
                return;
            }
            videoPath = path;
            videoBox.gameObject.SetActive(!string.IsNullOrEmpty(path));
            if (videoTools)
            {
                videoTools.alpha = 1;
                videoTools.gameObject.SetActive(true);
            }

            if (pos == VideoPosition.Top)
                videoBox.anchoredPosition = positionTop;
            else if (pos == VideoPosition.Middle)
                videoBox.anchoredPosition = positionMiddle;
            else
                videoBox.anchoredPosition = positionBottom;
        }

        public void VideoPause()
        {
            if (videoTools == null)
                return;
            videoTools.gameObject.SetActive(true);
            videoTools.DOFade(1, 0.5f);
        }

        public void VideoRestore(RectTransform render, bool tween)
        {
            if (render == null || videoBox == null)
                return;
            render.SetParent(videoRender);
            if (tween)
            {
                render.DOAnchorPos(Vector2.zero, 0.3f);
                render.DOSizeDelta(new Vector2(550, 310), 0.3f);
            }
            else
            {
                render.anchoredPosition = Vector2.zero;
                render.sizeDelta = new Vector2(550, 310);
            }
        }

        public void PlayAudio(string path, Sprite turnOn, Sprite turnOff)
        {
            turnOnSp = turnOn;
            turnOffSp = turnOff;
            audioPath = path;
            audioButton.gameObject.SetActive(!string.IsNullOrEmpty(path));
        }

        private void PlayAudioHandler()
        {
            isPlaying = !isPlaying;
            var img = audioButton.GetComponent<Image>();
            if (isPlaying)
            {
                img.sprite = turnOffSp;
                //ZAudioManager.PlayMusic(audioPath, mTransform.parent.gameObject, false);
            }
            else
            {
                img.sprite = turnOnSp;
                ZAudioManager.Stop(mTransform.parent.gameObject);
            }
        }

        private void OnClickVideoBG()
        {
            if (videoBox == null)
                return;
            if (videoTools && !videoTools.isActiveAndEnabled)
            {
                videoTools.alpha = 0;
                videoTools.gameObject.SetActive(true);
                videoTools.DOFade(1, 0.5f);
            }

            if (clickFun != null)
                clickFun.Invoke(this, Option.Pause, -1);
        }

        private void OnPlayVideoHandle()
        {
            if (clickFun != null)
                clickFun.Invoke(this, Option.Play, -1);
            if (videoTools != null)
            {
                videoTools.DOFade(0, 0.3f).OnComplete(() =>
                {
                    videoTools.gameObject.SetActive(false);
                });
            }
        }

        private void OnFullScreenHandle()
        {
            if (clickFun != null)
                clickFun.Invoke(this, Option.FullScreen, -1);
        }
    }
}
