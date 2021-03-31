using DG.Tweening;
using SuperScrollView;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ZStart.RGraph.Manager;
using ZStart.RGraph.Model;
using ZStart.RGraph.View.Parts;

namespace ZStart.RGraph.View.Group
{
    public class TimeLineGroup:AppUIGroup
    {
        public TextMeshProUGUI titleLabel;
        //public List<TimeParts> items;
        //public RectTransform content;
        public LoopListView2 scrollViewer;
        public CanvasGroup canvasGroup;

        public RectTransform anchorTarget;
        public float anchorX = 15f;
        public Vector2 yearRange = new Vector2(1880f,1978f);
        public float step = 0f;
        private List<AffairInfo> data = null;
        float lastOffY = 0.0f;
        public string lastAffair = "";
        public Vector2 targetStep = Vector2.zero;
        public TimeParts selectedItem;
        protected override void Start()
        {
            base.Start();
            float dif = yearRange.y - yearRange.x;
            if (dif < 1)
                dif = 100;
            step = Screen.height / (dif * 12f);
        }

        private void Update()
        {
            anchorTarget.anchoredPosition = Vector2.Lerp(anchorTarget.anchoredPosition, targetStep, Time.deltaTime);
        }

        public override void Appear()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0f;
            //titleLabel.text = LocalManager.GetValue("Relation.Menu.Experience");
        }

        public override void Disappear()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.DOFade(0f, 0.3f).OnComplete(Clear);
        }

        public override void Clear()
        {
            scrollViewer.Clear();
        }

        public void ShowSelected(NodeInfo node)
        {
            lastAffair = "";
            lastOffY = 0f;
            data = node.entity.experiences;
            gameObject.SetActive(true);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, 0.3f);
            scrollViewer.InitListView(data.Count, GetItemHandle);
            if(data != null && data.Count > 0)
                UpdateAnchorStep(data[0]);
            //StartCoroutine(CreateItemInspector(node.personage.experiences));
        }

        private LoopListViewItem2 GetItemHandle(LoopListView2 listView, int index)
        {
            if (index < 0 || index >= data.Count)
            {
                return null;
            }

            AffairInfo info = data[index];
            if (string.IsNullOrEmpty(info.uid))
            {
                return null;
            }
            LoopListViewItem2 item = listView.NewListViewItem("TimeParts");
            TimeParts parts = item.GetComponent<TimeParts>();
            if (item.IsInitHandlerCalled == false)
            {
                item.IsInitHandlerCalled = true;
            }
            //parts.Clear();
            parts.AddClickEvent(ClickItemHandler);
            parts.UpdateInfo(info, index);
            return item;
        }

        //IEnumerator CreateItemInspector(List<AffairInfo> list)
        //{
        //    ClearItems();
        //    yield return null;
        //    int length = list.Count;
        //    for (int i = 0; i < length; i += 1)
        //    {
        //        var item = ZAssetController.Instance.ActivateAsset<TimeParts>(content);
        //        item.UpdateInfo(list[i], i);
        //        items.Add(item);
        //        yield return null;
        //    }
        //}

        private void ClickItemHandler(TimeParts parts, string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                if (selectedItem != null && parts.identify == selectedItem.identify)
                    return;
                if (selectedItem != null)
                {
                    selectedItem.Selected = false;
                }
                selectedItem = parts;
                selectedItem.Selected = true;
            }
            else
            {
                //Debug.LogWarning("try expend node = " + keyword);
                DFNotifyManager.SendNotify(Enum.DFNotifyType.OnNodeHighlight, new PairInfo(selectedItem.identify, keyword) );
            }
        }

        //private void ClearItems()
        //{
        //    for (int i = 0; i < items.Count; i += 1)
        //    {
        //        items[i].Clear();
        //        ZAssetController.Instance.DeActivateAsset(items[i].mTransform);
        //    }
        //    items.Clear();
        //}

        private AffairInfo GetAffairInfo(string uid)
        {
            for (int i = 0;i < data.Count;i += 1)
            {
                if(data[i].uid == uid)
                {
                    return data[i];
                }
            }
            return new AffairInfo();
        }

        private void UpdateAnchorStep(AffairInfo info)
        {
            if (string.IsNullOrEmpty(info.uid) || info.year < 1 || lastAffair == info.uid)
                return;
            lastAffair = info.uid;
            var y = ((info.year - yearRange.x) * 12 + info.month) * step;
            if (y < 0)
                y = 0;
            else if (y > Screen.height)
                y = Screen.height - 30;
            targetStep = new Vector2(anchorX, -y);
            //Debug.LogWarning("UpdateAnchorStep...uid = " +info.year + ";pos = " + targetStep);
        }
       
        public void Handle_ScrollChanged(Vector2 offset)
        {
            if (Mathf.Abs(lastOffY - offset.y) > 0.01f)
            {
                lastOffY = offset.y;
                if(scrollViewer.ShownItemCount > 0)
                {
                    int i = 2;
                    var val = scrollViewer.ScrollRect.verticalScrollbar.value;
                    if(val > 0.99f)
                    {
                        i = 0;
                    }else if(val < 0.01f)
                    {
                        i = scrollViewer.ShownItemCount - 1;
                    }
                    else
                    {
                        if (i >= scrollViewer.ShownItemCount)
                            i = scrollViewer.ShownItemCount - 1;
                    }
                    var item = scrollViewer.GetShownItemByIndex(i);
                    if (item == null)
                        return;
                    var parts = item.GetComponent<TimeParts>();
                    if (parts != null)
                    {
                        var info = GetAffairInfo(parts.identify);
                        UpdateAnchorStep(info);
                    }
                }
            }
        }
    }
}
