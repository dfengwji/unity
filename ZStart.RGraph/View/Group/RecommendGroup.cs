using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZStart.Core.Controller;
using ZStart.RGraph.Model;
using ZStart.RGraph.View.Parts;

namespace ZStart.RGraph.View.Group
{
    public class RecommendGroup : AppUIGroup
    {
        public CanvasGroup canvasGroup;
        public float spacing = 10;
        public RectTransform labelBox;
        public RectTransform itemBox;
        public List<LabelParts> items;
        public Vector2 defaultSize = new Vector2(420, 164);

        protected override void Start()
        {
            base.Start();
            defaultSize = labelBox.sizeDelta;
        }

        public override void Appear()
        {
            if (canvasGroup.alpha > 0.9f)
                return;
            gameObject.SetActive(true);
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1f, 0.2f);
        }

        public override void Disappear()
        {
            canvasGroup.DOFade(0f, 0.2f).OnComplete(()=>
            {
                gameObject.SetActive(false);
            });
        }

        public void UpdateInfo(ThemeInfo[] array, UnityAction<LabelParts> action)
        {
            gameObject.SetActive(true);
            if (array != null)
            {
                StartCoroutine(CreateItemInspector(array, action));
            }
        }

        IEnumerator CreateItemInspector(ThemeInfo[] array, UnityAction<LabelParts> action)
        {
            float max = mTransform.sizeDelta.x;
            float width = 0f;
            float heigth = 30f;
            float row = 1;
            for (int i = 0; i < array.Length; i += 1)
            {
                var item = ZAssetController.Instance.ActivateAsset<LabelParts>(itemBox);
                item.UpdateTip(array[i].node, array[i].name, action);
                item.mTransform.anchorMax = new Vector2(0f, 1f);
                item.mTransform.anchorMin = new Vector2(0f, 1f);
                items.Add(item);
                var w = item.mTransform.sizeDelta.x;
                if (width + w > max)
                {
                    row += 1;
                    width = w;
                    item.mTransform.anchoredPosition = new Vector2(w * 0.5f, -((row - 0.5f) * heigth + spacing * (row - 1)));
                }
                else
                {
                    item.mTransform.anchoredPosition = new Vector2(width + w * 0.5f + spacing * (row - 1), -((row - 0.5f) * heigth + spacing * (row - 1)));
                    width += w + spacing;
                }
                yield return null;
            }
        }

        public override void Clear()
        {
            for (int i = 0; i < items.Count; i += 1)
            {
                items[i].Clear();
                ZAssetController.Instance.DeActivateAsset(items[i].mTransform);
            }
            items.Clear();
        }

        public void Expend()
        {
            itemBox.gameObject.SetActive(true);
            labelBox.DOSizeDelta(new Vector2(defaultSize.x, defaultSize.y), 0.2f);
        }

        public void Shrink()
        {
            itemBox.gameObject.SetActive(false);
            labelBox.DOSizeDelta(new Vector2(defaultSize.x, 64f), 0.2f);
        }

        public void Handler_Expend()
        {
            Expend();
        }
    }
}
