using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

namespace ZStart.RGraph.View.Parts
{
    public class ThemeParts: AppUIParts, IEventSystemHandler, IPointerClickHandler
    {
        public TextMeshProUGUI vNameLabel;
        public TextMeshProUGUI hNameLabel;
        public Text remarkLabel;
        public RawImage coverImage;
        public CanvasGroup expendBox;
        public int index = 0;
        private UnityAction<ThemeParts, bool> clickFun;

        public bool Selected
        {
            set
            {
                vNameLabel.enabled = !value;
                expendBox.gameObject.SetActive(value);
                if (value)
                {
                    expendBox.alpha = 0;
                    expendBox.DOFade(1f, 0.3f);
                }
            }
        }

        public void UpdateInfo(string uid, string title, string remark, UnityAction<ThemeParts, bool> click)
        {
            identify = uid;
            vNameLabel.text = title;
            hNameLabel.text = title;
            remarkLabel.text = remark;
            expendBox.gameObject.SetActive(false);
            clickFun = click;
        }

        public void UpdateCover(Texture2D texture)
        {
            coverImage.texture = texture;
            coverImage.enabled = texture == null ? false : true;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickFun != null)
                clickFun.Invoke(this, false);
        }

        public void Handler_Enter()
        {
            if (clickFun != null)
                clickFun.Invoke(this, true);
        }
    }
}
