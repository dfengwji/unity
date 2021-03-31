using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ZStart.Core.Controller;
using ZStart.RGraph.Model;
using ZStart.RGraph.Util;

namespace ZStart.RGraph.View.Parts
{
    public class RecordParts: AppUIParts
    {
        public Text keyLabel;
        public Text valueLabel;
        public Vector2 defaultSize = new Vector2(425f, 72f);
        public float charLineHeight = 35;
        public int columnMax = 18;
        public RectTransform itemBox;
        public List<LabelParts> items;
        public void UpdateInfo(string key, PairInfo[] array, UnityAction<LabelParts> action)
        {
            keyLabel.text = key;
            string tip = "";
            if(array != null)
            {
                for (int i = 0;i < array.Length;i += 1)
                {
                    if (string.IsNullOrEmpty(array[i].key))
                    {
                        tip += array[i].value;
                    }
                    else
                    {
                        var item = ZAssetController.Instance.ActivateAsset<LabelParts>(itemBox);
                        item.UpdateInfo(array[i].key, array[i].value, action);
                        items.Add(item);
                    }
                }
            }
            valueLabel.text = tip;
            valueLabel.gameObject.SetActive(!string.IsNullOrEmpty(tip));
            UpdateExpendSize(tip);
        }

        private void UpdateExpendSize(string desc)
        {
            if (string.IsNullOrEmpty(desc))
                return;
            if(desc.Length < columnMax)
            {
                mTransform.sizeDelta = new Vector2(defaultSize.x, defaultSize.y);
                return;
            }
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
            if (maxLine < 0)
                maxLine = 1;
            float offY = defaultSize.y + maxLine * charLineHeight;
            mTransform.sizeDelta = new Vector2(defaultSize.x, offY);
        }

        public override void Clear()
        {
            for (int i = 0;i < items.Count;i += 1)
            {
                items[i].Clear();
                ZAssetController.Instance.DeActivateAsset(items[i].mTransform);
            }
            items.Clear();
            identify = "";
        }

        public override void Show()
        {
            base.Show();
        }

        public override void UnShow()
        {
            identify = "";
            base.UnShow();
            
        }
    }
}
