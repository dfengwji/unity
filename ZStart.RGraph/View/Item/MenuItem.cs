using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using ZStart.Core.Controller;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Model;

namespace ZStart.RGraph.View.Item
{
    public class MenuItem : Core.ZBehaviourBase
    {
        public MenuSprite[] sprites;
        public SpriteRenderer[] buttons;

        public void Show()
        {
            UpdateState(MenuType.Remark, ExpendStatus.Closed);
            UpdateState(MenuType.Experience, ExpendStatus.Closed);
            mTransform.localScale = new Vector3(0.1f, 0.1f, 1f);
            mTransform.DOScale(new Vector3(2.5f, 2.5f, 1f), 0.3f);
        }

        public void Hide()
        {
            mTransform.DOScale(new Vector3(0.1f, 0.1f, 1f), 0.5f).OnComplete(()=> {
                ZAssetController.Instance.DeActivateAsset(mTransform);
            });
        }

        public MenuType SelectState(string uname)
        {
            if (string.IsNullOrEmpty(uname) || !uname.Contains("-"))
                return MenuType.Unknown;
            var m = (MenuType)int.Parse(uname.Split('-')[1]);
            List<SpriteRenderer> list = new List<SpriteRenderer>(3);
            for (int i = 0;i < buttons.Length;i += 1)
            {
                if (m == MenuType.Remark || m == MenuType.Experience)
                {
                    list.Add(buttons[i]);
                }
            }
            SelectState(m,ref list);

            return m;
        }

        private void SelectState(MenuType type,ref List<SpriteRenderer> list)
        {
            for (int i = 0; i < list.Count; i += 1)
            {
                var t = (MenuType)int.Parse(buttons[i].name.Split('-')[1]);
                if (t == type)
                {
                    list[i].sprite = GetSprite(type, ExpendStatus.Opened);
                }
                else
                {
                    list[i].sprite = GetSprite(t, ExpendStatus.Closed);
                }
            }
        }

        public void UpdateState(MenuType type, ExpendStatus state)
        {
            for (int i = 0; i < buttons.Length; i += 1)
            {
                var t = (MenuType)int.Parse(buttons[i].name.Split('-')[1]);
                if (t == type)
                {
                    buttons[i].sprite = GetSprite(t, state);
                    break;
                }
            }
        }

        private Sprite GetSprite(MenuType type, ExpendStatus status)
        {
            for (int i = 0; i < sprites.Length; i += 1)
            {
                if (sprites[i].menu == type)
                {
                    if (status == ExpendStatus.Closed)
                        return sprites[i].normal;
                    else if (status == ExpendStatus.Opened)
                        return sprites[i].highlight;
                    else
                        return sprites[i].disable;
                }
            }
            return null;
        }
    }
}
