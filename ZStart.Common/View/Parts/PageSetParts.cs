using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using ZStart.Core.Controller;

namespace ZStart.Common.View.Parts
{
    public class PageSetParts : Core.ZBehaviourBase
    {
        public List<AppItemParts> items;

        public Transform[] spawnPoints;

        public bool isMoving = false;
        public float moveTime = 0.2f;

        public bool isFilled
        {
            get
            {
                if (items == null)
                    return false;
                return items.Count > 0 ? true : false;
            }
        }

        public void Show()
        {
            if (isActiveAndEnabled == false)
                gameObject.SetActive(true);
        }

        public void Hide()
        {
            if (isActiveAndEnabled)
                gameObject.SetActive(false);
        }

        public void HideItems()
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].UnShow();
            }
            items.Clear();
        }

        public void MoveTo(Vector3 to, bool hide)
        {
            if (DOTween.IsTweening(mTransform))
                DOTween.Kill(mTransform, true);
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Lock(true);
            }
            mTransform.DOLocalMove(to, moveTime, true).SetEase(Ease.OutBack).OnComplete(() =>
            {
                isMoving = true;
                if (hide)
                {
                    Dispose();
                    gameObject.SetActive(false);
                }
                else
                {
                    for (int i = 0; i < items.Count; i++)
                    {
                        items[i].Lock(false);
                    }
                }

            });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Clear();
                ZAssetController.Instance.DeActivateAsset(items[i].mTransform);
            }
            items.Clear();
        }

        public Transform GetSpawnPoint(int index)
        {
            if (index < spawnPoints.Length)
                return spawnPoints[index];
            else
                return null;
        }

        public void RemoveItem(string identify)
        {
            AppItemParts item = GetItem(identify);
            if (item == null)
                return;
            items.Remove(item);
            item.Clear();
            ZAssetController.Instance.DeActivateAsset(item.mTransform);
        }

        public void AddItem(AppItemParts item)
        {
            if (items == null)
                items = new List<AppItemParts>();
            items.Add(item);
        }

        public AppItemParts GetItem(string identify)
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].identify == identify)
                    return items[i];
            }
            return null;
        }

        [ContextMenu("ResetSpawns")]
        public void ResetSpawns()
        {
            Transform[] array = GetComponentsInChildren<Transform>();
            if (array == null)
            {
                Debug.LogWarning("spawn array == null!!!!");
                return;
            }
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != mTransform)
                {
                    list.Add(array[i]);
                }
            }
            spawnPoints = new Transform[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                spawnPoints[i] = list[i];
            }
        }
    }
}
