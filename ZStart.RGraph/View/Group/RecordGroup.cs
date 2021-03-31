using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZStart.Core.Controller;
using ZStart.RGraph.Manager;
using ZStart.RGraph.Model;
using ZStart.RGraph.View.Parts;

namespace ZStart.RGraph.View.Group
{
    public class RecordGroup:AppUIGroup
    {
        public RawImage image;
        public TextMeshProUGUI titleLabel;
        public Text nameLabel;
        public Text remarkLabel;
        public string identify = "";
        public CanvasGroup canvasGroup;
        public RectTransform content;
        public List<RecordParts> items = new List<RecordParts>(5);

        protected override void Start()
        {
            base.Start();
            //titleLabel.text = LocalManager.GetValue("Relation.Menu.Record");
        }

        public void UpdateInfo(EntityInfo info)
        {
            identify = info.uid;
            nameLabel.text = info.name;
            remarkLabel.text = info.remark;
            Appear();
            StartCoroutine(CreateItemInspector(info.properties));
        }

        public void UpdateHeader(Texture2D texture)
        {
            image.texture = texture;
            image.enabled = true;
        }

        public override void Appear()
        {
            base.Appear();
            image.enabled = false;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.3f);
        }

        public override void Disappear()
        {
            image.enabled = false;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.DOFade(0f, 0.3f).OnComplete(()=> {
                ClearItems();
                gameObject.SetActive(false);
            });

        }

        IEnumerator CreateItemInspector(List<PropertyInfo> list)
        {
            ClearItems();
            yield return null;
            int length = list.Count;
            for (int i = 0; i < length; i += 1)
            {
                var item = ZAssetController.Instance.ActivateAsset<RecordParts>(content);
                item.UpdateInfo(list[i].key, list[i].entities, ClickLabelHandler);
                items.Add(item);
                yield return null;
            }
        }

        private void ClickLabelHandler(LabelParts parts)
        {
            //Debug.LogWarning(parts.label.text + "---" + parts.identify);
            DFNotifyManager.SendNotify(Enum.DFNotifyType.OnNodeHighlight, new PairInfo(identify, parts.identify));
        }

        private void ClearItems()
        {
            for (int i = 0; i < items.Count; i += 1)
            {
                items[i].Clear();
                ZAssetController.Instance.DeActivateAsset(items[i].mTransform);
            }
            items.Clear();
        }
    }
}
