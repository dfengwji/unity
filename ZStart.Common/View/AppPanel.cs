using ZStart.Common.Manager;
using ZStart.Core.Controller;
using ZStart.Core.Manager;
using ZStart.Core.Model;
using UnityEngine;
namespace ZStart.Common.View
{
    public abstract class AppPanel:Core.View.ZUIPanel
    {
        public RectTransform contentBox;

        public CanvasGroup canvasGroup;

        protected override void Start()
        {
            base.Start();
            Init();
        }

        protected virtual void Init()
        {

        }

        public void ShowContent()
        {
            if (contentBox == null)
            {
                return;
            }
            //contentBox.localScale = new Vector3(0.5f, 0.5f, 1f);
            //contentBox.DOScale(1f, 0.2f).SetEase(Ease.OutExpo).OnComplete(AppearCompelte);
            //if (canvasGroup == null)
            //    canvasGroup = contentBox.GetComponent<CanvasGroup>();
            //if (canvasGroup == null)
            //    canvasGroup = contentBox.gameObject.AddComponent<CanvasGroup>();
            //canvasGroup.DOFade(1, 0.1f);
        }

        protected virtual void AppearCompelte()
        {
            if (contentBox != null)
                contentBox.localScale = Vector3.one;
        }

        public override void Disappear()
        {
            if (contentBox != null)
            {
                //canvasGroup.DOFade(0, 0.1f);
                //contentBox.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.1f).SetEase(Ease.InOutQuint).OnComplete(DisappearComplete);
            }
            else
                DisappearComplete();
        }

        protected virtual void DisappearComplete()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }

        protected void PlayMusic()
        {
            AssetInfo info = ConfigManager.Instance.GetAssetInfo(name);
            if (info.audio > 0)
            {
                AudioClip clip = ZAssetController.Instance.GetAudioClip(info.audio);
                ZAudioManager.PlayMusic(clip, gameObject, true);
            }
        }

        protected void StopMusic()
        {
            ZAudioManager.Stop(this.gameObject);
        }
    }
}
