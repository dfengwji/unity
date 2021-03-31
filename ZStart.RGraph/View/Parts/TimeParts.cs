using DG.Tweening;
using SuperScrollView;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZStart.Core.Controller;
using ZStart.RGraph.Model;
using ZStart.RGraph.Util;

namespace ZStart.RGraph.View.Parts
{
    public class TimeParts:AppUIParts, IEventSystemHandler, IPointerClickHandler
    {
        public Image bgImage;
        public Text timeLabel;
        public Text descLabel;
        public Image arrowImage;
        public float defaultHeight = 119f;
        public float charLineHeight = 35;
        public float tagHeight = 40;
        public float imageHeight = 100;
        public int columnMax = 18;
        public int index = 0;

        public Sprite normalSp;
        public Sprite highlightSp;

        public RectTransform imageContent;
       
        public RectTransform labelContent;
        public List<LabelParts> labelItems;
        private AffairInfo data = default;
        private UnityAction<TimeParts, string> callFun;

        private LoopListViewItem2 viewItem;
        public LoopListViewItem2 ViewItem
        {
            get
            {
                if (viewItem == null)
                    viewItem = GetComponent<LoopListViewItem2>();
                return viewItem;
            }
        }
        public bool Selected
        {
            set
            {
                ExpendArea(value);
            }
        }

        protected override void OnDisable()
        {
            Clear();
        }

        private void ExpendArea(bool selected)
        {
            if (data == null)
                return;
            
            if (selected)
            {
                bgImage.sprite = highlightSp;
                arrowImage.enabled = false;
                int imgCol = Mathf.CeilToInt(data.images.Length / 4f);
                imageContent.gameObject.SetActive(imgCol < 1 ? false : true);
                descLabel.text = data.description;
                UpdateExpendSize(data.description, data.keywords.Length > 0 ? true : false, imgCol);
                SwitchLabelButtons(true);
            }
            else
            {
                SwitchLabelButtons(false);
                bgImage.sprite = normalSp;
                imageContent.gameObject.SetActive(false);
                bool sub = data.description.Length > columnMax ? true : false;
                arrowImage.enabled = sub || data.keywords.Length > 0;
                if (sub)
                    descLabel.text = data.description.Substring(0, columnMax - 3) + "...";
                else
                    descLabel.text = data.description;
                //mTransform.sizeDelta = new Vector2(mTransform.sizeDelta.x, defaultHeight);
                mTransform.DOSizeDelta(new Vector2(mTransform.sizeDelta.x, defaultHeight), 0.2f).OnUpdate(()=> {
                    ViewItem.ParentListView.OnItemSizeChanged(viewItem.ItemIndex);
                });
            }
        }

        private void UpdateExpendSize(string desc, bool tag, int images)
        {
            int chinaNum = 0;
            int letterNum = 0;
            int otherNum = 0;
            for (int i = 0; i < desc.Length; i += 1)
            {
                if (LogicUtil.IsChinaChar(desc.Substring(i, 1)))
                {
                    chinaNum += 1;
                }
                else if (LogicUtil.IsLetterOrNumber(desc.Substring(i, 1)))
                {
                    letterNum += 1;
                }
                else
                {
                    otherNum += 1;
                }
            }
            int count = chinaNum + letterNum / 2 + otherNum / 2;
            int maxLine = Mathf.CeilToInt(count / (float)columnMax) - 1;
            if (maxLine < 1)
                maxLine = 1;
            descLabel.rectTransform.sizeDelta = new Vector2(descLabel.rectTransform.sizeDelta.x, maxLine * charLineHeight);
            imageContent.sizeDelta = new Vector2(imageContent.sizeDelta.x, images * imageHeight);
            float offY = defaultHeight + maxLine * charLineHeight + images * imageHeight;
            if (tag)
                offY += tagHeight;
            mTransform.DOSizeDelta(new Vector2(mTransform.sizeDelta.x, offY), 0.2f).OnUpdate(() => {
                ViewItem.ParentListView.OnItemSizeChanged(viewItem.ItemIndex);
            });
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (callFun != null)
                callFun.Invoke(this, "");
        }

        public void AddClickEvent(UnityAction<TimeParts, string> action)
        {
            callFun = action;
        }

        private void ClickButtonHandle(LabelParts button)
        {
            if (callFun != null)
                callFun.Invoke(this, button.identify);
        }

        private void SwitchLabelButtons(bool show)
        {
            if (show)
            {
                int length = data.keywords.Length;
                for (int i = 0; i < length; i += 1)
                {
                    var item = ZAssetController.Instance.ActivateAsset<LabelParts>(labelContent);
                    item.UpdateInfo(data.keywords[i].key, data.keywords[i].value, ClickButtonHandle);
                    labelItems.Add(item);
                }
            }
            else
            {
                for (int i = 0;i < labelItems.Count;i += 1 )
                {
                    labelItems[i].Clear();
                    ZAssetController.Instance.DeActivateAsset(labelItems[i].mTransform);
                }
                labelItems.Clear();
            }
        }

        public void UpdateInfo(AffairInfo info, int index)
        {
            data = info;
            this.index = index;
            identify = info.uid;
            timeLabel.text = info.time;
            bgImage.sprite = normalSp;
            bool sub = info.description.Length > columnMax ? true : false;
            if (sub)
            {
                descLabel.text = data.description.Substring(0, columnMax - 3) + "...";
            }
            else
            {
                descLabel.text = data.description;
            }
            arrowImage.enabled = sub || data.keywords.Length > 0;
            SwitchLabelButtons(false);
            imageContent.gameObject.SetActive(false);
            mTransform.sizeDelta = new Vector2(mTransform.sizeDelta.x, defaultHeight);
            ////descLabel.text = string.Format("<size=24><b>{0}</b></size>", name) + "\n" + desc;
        }

        public override void Clear()
        {
            Selected = false;
            identify = "";
        }
    }
}
