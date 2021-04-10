using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ZStart.VRoom.View.Parts
{
    public class LabelParts: AppParts
    {
        public RectTransform labelBox;
        public Text tipLabel;
        public CanvasGroup group;
        public void UpdateLabel(string tip)
        {
            labelBox.gameObject.SetActive(!string.IsNullOrEmpty(tip));
            tipLabel.text = tip;
        }

        public override void Show()
        {
            base.Show();
            group.alpha = 0;
            group.DOFade(1f, 0.3f);
        }

        public override void UnShow()
        {
            group.DOFade(0f, 0.3f).OnComplete(() => {
                base.UnShow();
            });
        }
    }
}
