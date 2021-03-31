using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZStart.RGraph.View.Parts
{
    public class LabelParts : AppUIParts, IEventSystemHandler, IPointerClickHandler
    {
        public Image bgImage;
        public TextMeshProUGUI label;
        public float defWidth = 16f;
        public float unitW = 10f;
        private UnityAction<LabelParts> callFun;

        public string Name
        {
            get
            {
                return label.text;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (callFun != null)
                callFun.Invoke(this);
        }

        public void UpdateTip(string uid, string tip, UnityAction<LabelParts> action)
        {
            identify = uid;
            label.text = tip;
            callFun = action;
            bgImage.enabled = true;
            label.alignment = TextAlignmentOptions.Center;
            mTransform.sizeDelta = new Vector2(defWidth + tip.Length * unitW, mTransform.sizeDelta.y);
        }

        public void UpdateInfo(string uid, string tip, UnityAction<LabelParts> action)
        {
            identify = uid;
            label.text = tip;
            if (string.IsNullOrEmpty(uid))
            {
                bgImage.enabled = false;
                label.alignment = TextAlignmentOptions.Left;
            }
            else
            {
                callFun = action;
                bgImage.enabled = true;
                label.alignment = TextAlignmentOptions.Center;
            }
            mTransform.sizeDelta = new Vector2(defWidth + tip.Length * unitW, mTransform.sizeDelta.y);
        }
    }
}
