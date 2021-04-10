using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using ZStart.Core;
using ZStart.Core.Controller;
using ZStart.Core.Manager;
using ZStart.Core.Util;
using ZStart.EBook.Common;
using ZStart.EBook.View.Parts;

namespace ZStart.EBook
{
    public class Simulator:ZBehaviourBase
    {
        public Button preButton;
        public Button nextButton;
        public Button toTopButton;
        public Button autoButton;
        public RawImage frontCover;
        public RawImage backCover;
        public GameObject progressBox;
        public Slider progressSlider;
        public GameObject eventBox;
        public VideoPlayer videoPlayer;
        public RectTransform videoBox;
        public RectTransform videoRender;
        public VideoBarParts videoTools;
        public MenuParts menuPrefab;
        public BookPro book;
        public AutoFlip autoFlip;

        public PageParts selectedPage;
        public Vector2 pageSize = new Vector2(700, 1000);
        public bool isPageFlipping = false;
        public float pageFlipTime = 1.5f;
        public bool isAutoFlipping = false;
        public List<PageParts> leftItems = new List<PageParts>(20);
        public List<PageParts> rightItems = new List<PageParts>(20);
        public Sprite turnOffSp;
        public Sprite turnOnSp;
        public string bookUID = "";
        private TouchSwipe touchSwipe = null;

        void Awake()
        {
            touchSwipe = new TouchSwipe(OnSwipeHandle);
        }

        private void Start()
        {
            Init();
            preButton.onClick.AddListener(PrePageHandle);
            nextButton.onClick.AddListener(NextPageHandle);
            toTopButton.onClick.AddListener(BackFirstHandle);
            autoButton.onClick.AddListener(AutoFlip);
            videoBox.GetComponent<Button>().onClick.AddListener(VideoBGClickHandle);
            videoBox.gameObject.SetActive(false);
            videoTools.gameObject.SetActive(false);
            videoTools.SetPlayer(videoPlayer);
            videoTools.exitButton.onClick.AddListener(OnVideoRestore);
        }

        private void Init()
        {
            if (book != null)
                return;
            book = mTransform.Find("BookPro").gameObject.AddComponent<BookPro>();
            menuPrefab = mTransform.Find("BookPro/MenuPrefab").gameObject.AddComponent<MenuParts>();
            mTransform.Find("BookPro/LeftPage").gameObject.AddComponent<MenuParts>();
            mTransform.Find("BookPro/RightPage").gameObject.AddComponent<MenuParts>();
            eventBox = mTransform.Find("EventBox").gameObject;
            preButton = mTransform.Find("EventBox/PreButton").GetComponent<Button>();
            nextButton = mTransform.Find("EventBox/NextButton").GetComponent<Button>();
            toTopButton = mTransform.Find("EventBox/BackButton").GetComponent<Button>();
            autoButton = mTransform.Find("EventBox/AutoButton").GetComponent<Button>();
            frontCover = mTransform.Find("BGBox/Front_Cover").GetComponent<RawImage>();
            backCover = mTransform.Find("BGBox/Back_Cover").GetComponent<RawImage>();
            progressBox = mTransform.Find("Progress_Box").gameObject;
            progressSlider = mTransform.Find("Progress_Box").GetComponentInChildren<Slider>();
            videoPlayer = mTransform.Find("VideoPlayer").GetComponent<VideoPlayer>();
            videoBox = mTransform.Find("VideoPlayer/VideoBox").GetComponent<RectTransform>();
            videoRender = mTransform.Find("VideoPlayer/VideoBox/RenderImage").GetComponent<RectTransform>();
            videoTools = mTransform.Find("VideoPlayer/VideoBar").gameObject.AddComponent<VideoBarParts>();
        }

        void Update()
        {
            if (Input.touchCount > 0)
            {
                touchSwipe.CheckTouchSwipe();
            }
            else
            {
                touchSwipe.CheckMouseSwipe();
            }
        }
       
        private void OnDisable()
        {
            if (videoPlayer && (videoPlayer.isPlaying || videoPlayer.isPrepared))
            {
                videoPlayer.Stop();
            }
        }

        public void Appear(BookModel model)
        {
            if (model == null)
            {
                ZLog.Warning(" book pro...open failed that config is error!!!path = ");
                return;
            }
            if (bookUID == model.uid)
                return;
            bookUID = model.uid;
            gameObject.SetActive(true);
            eventBox.SetActive(false);
            progressBox.SetActive(true);
            progressSlider.value = 0f;
            StartCoroutine(PageCreateInspector(model));
        }

        public void Appear(string dir)
        {
            bookUID = MD5Util.ToMD5(dir);
            frontCover.enabled = false;
            backCover.enabled = false;
            ReadImges(dir);
        }

        public void Disappear()
        {
            videoRender.SetParent(videoBox);
            gameObject.SetActive(false);
        }

        public void AutoFlip()
        {
            isAutoFlipping = !isAutoFlipping;
        }

        private void ReadImges(string path)
        {
            string[] files = Simulator.GetImagesInDirectory(path);
            if (files == null || files.Length < 1)
            {
                ZLog.Warning("look book...that directory is empty!!! path = " + path);
                return;
            }

            StartCoroutine(InitBookInspector(files));
        }

        private IEnumerator InitBookInspector(string[] files)
        {
            book.ClearPapers();
            progressBox.SetActive(true);
            progressSlider.value = 0f;
            float max = files.Length;
            List<GameObject> list = new List<GameObject>(2);
            for (int i = 0; i < max; i++)
            {
                bool right = list.Count == 1 ? true : false;
                BookModel.PageInfo info = new BookModel.PageInfo
                {
                    image = files[i]
                };
                PageParts page = CreatePage(info, right);
                list.Add(page.gameObject);
                if (list.Count > 1)
                {
                    book.AddPaper(list[0], list[1]);
                    list.Clear();
                }
                progressSlider.value = i / max;
                yield return null;
            }
            ZLog.Log("look book that paper count = " + book.papers.Count + "; files num = " + max);
            book.CurrentPaper = 1;
            book.UpdatePages();

            book.StartFlippingPaper = 0;
            book.EndFlippingPaper = book.papers.Count - 1;
            yield return null;
            progressBox.SetActive(false);
            eventBox.SetActive(true);
        }

        private IEnumerator PageCreateInspector(BookModel model)
        {
            book.ClearPapers();
            progressBox.SetActive(true);
            progressSlider.value = 0f;
            float max = model.AllPages.Count;
            string front = MD5Util.ToMD5(model.frontCover);
            string back = MD5Util.ToMD5(model.backCover);
            Texture2D frontTex = ZFileManager.Instance.ReadTexture(model.frontCover, new Vector2(700, 996));
            Texture2D backTex = ZFileManager.Instance.ReadTexture(model.backCover, new Vector2(700, 996));
            frontCover.texture = frontTex;
            frontCover.enabled = frontTex == null ? false : true;

            backCover.texture = backTex;
            backCover.enabled = backTex == null ? false : true;
            preButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
            List<BookModel.PaperModel> pages = model.AllPages;
            for (int i = 0; i < max; i++)
            {
                PageParts left = CreatePage(pages[i].left, false);
                PageParts right = CreatePage(pages[i].right, true);
                book.AddPaper(left.gameObject, right.gameObject);
                progressSlider.value = i / max;
                yield return null;
            }
            ZLog.Log("look book that paper count = " + book.papers.Count + "; files num = " + max);
            book.CurrentPaper = 0;
            book.UpdatePages();

            book.StartFlippingPaper = 0;
            book.EndFlippingPaper = book.papers.Count - 1;
            yield return null;
            progressBox.SetActive(false);
            eventBox.SetActive(true);
        }

        private PageParts CreatePage(BookModel.PageInfo info, bool right)
        {
            string path = "";
            PageParts page = ActivePage(right, info.menus.Count > 0 ? true : false);
            page.UpdateInfo(info);
            if (!string.IsNullOrEmpty(info.image))
            {
                path = info.image;
                ZImageController.Instance.Load(info.uid, path, pageSize, ImageLoadComplete);
            }
            page.PlayAudio(info.audio, turnOnSp, turnOffSp);
            page.CheckVideo(info.video, info.cover);
            page.Show();

            page.mTransform.SetParent(book.transform, false);
            if (right)
            {
                page.mTransform.sizeDelta = book.RightPage.sizeDelta;
                page.mTransform.pivot = book.RightPage.pivot;
                page.mTransform.anchoredPosition = book.RightPage.anchoredPosition;
                page.mTransform.localScale = book.RightPage.localScale;
                page.name = "Page" + ((book.papers.Count - 1) * 2);
            }
            else
            {
                page.mTransform.sizeDelta = book.LeftPage.sizeDelta;
                page.mTransform.pivot = book.LeftPage.pivot;
                page.mTransform.anchoredPosition = book.LeftPage.anchoredPosition;
                page.mTransform.localScale = book.LeftPage.localScale;
                page.name = "Page" + ((book.papers.Count - 1) * 2 + 1);
            }
            page.UpdatePage(info.page);
            page.AddListener(PageVideoHandle);
            return page;
        }

        private void ImageLoadComplete(ZImageController.ImageInfo info)
        {
            var item = GetPageItem(info.identify);
            if (item != null)
            {
                item.UpdateTexture(info.texture);
            }
        }

        private PageParts GetPageItem(string uid)
        {
            for (int i = 0; i < leftItems.Count; i += 1)
            {
                if (leftItems[i].identify == uid)
                {
                    return leftItems[i];
                }
            }
            for (int i = 0; i < rightItems.Count; i += 1)
            {
                if (rightItems[i].identify == uid)
                {
                    return rightItems[i];
                }
            }
            return null;
        }

        private void OnSwipeHandle(TouchSwipe.Direction dire, bool start)
        {
            if (start)
            {
                book.OnMouseDragLeftPage(null);
                book.OnMouseDragRightPage(null);
            }
            else
            {
                if (dire == TouchSwipe.Direction.Left)
                {
                    videoPlayer.Stop();
                    book.OnMouseRelease(null);
                    CheckPage(book.CurrentPaper);
                }
                else if (dire == TouchSwipe.Direction.Right)
                {
                    videoPlayer.Stop();
                    book.OnMouseRelease(null);
                    CheckPage(book.CurrentPaper - 1);
                }
            }
           
        }

        private PageParts ActivePage(bool right, bool menu)
        {
            List<PageParts> items = null;
            if (right)
                items = rightItems;
            else
                items = leftItems;
            for (int i = 0; i < items.Count; i += 1)
            {
                if (!items[i].isActiveAndEnabled)
                {
                    items[i].Show();
                    return items[i];
                }
            }
            PageParts item = null;
            if (right)
            {
                if (menu)
                {
                    item = Instantiate(menuPrefab);
                    rightItems.Add(item);
                }
                else
                {
                    GameObject obj = Instantiate(book.RightPage.gameObject);
                    item = obj.GetComponent<PageParts>();
                    rightItems.Add(item);
                }
            }
            else
            {
                if (menu)
                {
                    item = Instantiate(menuPrefab);
                    leftItems.Add(item);
                }
                else
                {
                    GameObject obj = Instantiate(book.LeftPage.gameObject);
                    item = obj.GetComponent<PageParts>();
                    leftItems.Add(item);
                }
            }
            item.Show();
            return item;
        }

        private void PageVideoHandle(PageParts item, PageParts.Option opt, int page)
        {
            if (selectedPage != null && selectedPage != item)
                selectedPage.VideoPause();
            selectedPage = item;
            if (opt == PageParts.Option.None)
            {
                if (page > 0)
                    PageFlip(page);
            }
            else if (opt == PageParts.Option.FullScreen)
            {
                videoBox.gameObject.SetActive(true);
                videoRender.SetParent(videoBox);
                videoRender.DOSizeDelta(new Vector2(1920, 1080), 0.3f);
                videoRender.DOAnchorPos(Vector2.zero, 0.3f);
                if (!videoPlayer.isPlaying)
                {
                    videoTools.Play(item.videoPath);
                }
            }
            else if (opt == PageParts.Option.Pause)
            {
                videoPlayer.Pause();
            }
            else if (opt == PageParts.Option.Play)
            {
                item.VideoRestore(videoRender, false);
                if (videoPlayer.url != item.videoPath)
                {
                    videoPlayer.url = item.videoPath;
                }
                videoPlayer.Play();
            }
        }

        private void OnVideoRestore()
        {
            if (selectedPage == null)
                return;
            videoPlayer.Pause();
            selectedPage.VideoRestore(videoRender, true);
            selectedPage = null;
            videoBox.gameObject.SetActive(false);
            videoTools.mTransform.DOAnchorPosY(-180, 0.3f).OnComplete(() => {
                videoTools.gameObject.SetActive(false);
            });
        }

        private void VideoBGClickHandle()
        {
            if (videoTools.isActiveAndEnabled)
            {
                videoTools.mTransform.DOAnchorPosY(-180, 0.3f).OnComplete(() => {
                    videoTools.Hide();
                });
            }
            else
            {
                videoTools.Show();
                videoTools.mTransform.DOAnchorPosY(0, 0.3f);
            }
        }

        private void PageFlip(int pageNum)
        {
            videoPlayer.Stop();
            if (pageNum < 0) pageNum = 0;
            if (pageNum > autoFlip.ControledBook.papers.Count * 2) pageNum = autoFlip.ControledBook.papers.Count * 2 - 1;
            autoFlip.enabled = true;
            autoFlip.PageFlipTime = 0.13f;
            autoFlip.TimeBetweenPages = 0;
            autoFlip.StartFlipping((pageNum + 1) / 2);
            CheckPage((pageNum + 1) / 2);
        }

        private void BackFirstHandle()
        {
            PageFlip(0);
        }

        private void CheckPage(int page)
        {
            nextButton.gameObject.SetActive(page >= book.papers.Count ? false : true);
            preButton.gameObject.SetActive(page < 1 ? false : true);
        }

        private void NextPageHandle()
        {
            if (isPageFlipping || book.CurrentPaper >= book.papers.Count) return;
            isPageFlipping = true;
            videoPlayer.Stop();
            PageFlipper.FlipPage(book, pageFlipTime, FlipMode.RightToLeft, () => {
                isPageFlipping = false;
                CheckPage(book.CurrentPaper);
            });
        }

        private void PrePageHandle()
        {
            if (isPageFlipping || book.CurrentPaper <= 0) return;
            isPageFlipping = true;
            videoPlayer.Stop();
            PageFlipper.FlipPage(book, pageFlipTime, FlipMode.LeftToRight, () => {
                isPageFlipping = false;
                CheckPage(book.CurrentPaper);
            });
        }

        public void Clear()
        {
            book.ClearPapers();
            for (int i = 0; i < leftItems.Count; i++)
            {
                leftItems[i].Hide();
            }
            for (int i = 0; i < rightItems.Count; i++)
            {
                rightItems[i].Hide();
            }
        }

        public static string[] GetImagesInDirectory(string path)
        {
            string[] fileNameX = Directory.GetFiles(path, "*.*");
            List<string> lists = new List<string>();
            for (int i = 0; i < fileNameX.Length; i++)
            {
                string fileNamei = fileNameX[i].ToLower();
                if (fileNamei.EndsWith(".jpg") || fileNamei.EndsWith(".png"))
                {
                    lists.Add(fileNamei);
                }
            }
            return lists.ToArray();
        }
    }
}
