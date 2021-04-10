using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZStart.EBook.View.Widget;

namespace ZStart.EBook.View.Parts
{
    public class MenuParts: PageParts
    {
        public Transform menuBox;
        public List<LabelButton> menus;
        public RectTransform highImage;
        public Button jumpButton;
        public LabelButton selectedMenu;

        protected override void Start()
        {
            Init();
            base.Start();
        }

        private void Init()
        {
            if (menuBox != null)
                return;
            menuBox = mTransform.Find("MenuBox");
            highImage = mTransform.Find("MenuBox/HighImage").GetComponent<RectTransform>();
            jumpButton = mTransform.Find("MenuBox/HighImage/SwitchButton").GetComponent<Button>();
            menus = new List<LabelButton>(menuBox.childCount);
            for (int i = 0;i < menuBox.childCount;i += 1)
            {
                var child = menuBox.GetChild(i);
                var btn = child.gameObject.AddComponent<LabelButton>();
                menus.Add(btn);
            }
        }

        public override void UpdateInfo(BookModel.PageInfo info)
        {
            selectedMenu = null;
            this.identify = info.uid;
            var list = info.menus;
            for (int i = 0; i < menus.Count; i += 1)
            {
                if (i < list.Count)
                {
                    menus[i].Show();
                    menus[i].identify = "index-" + i;
                    menus[i].UpdateLabel(list[i].name, list[i].page + "");
                    menus[i].AddClickListener(MenuClickHandler);
                }
                else
                {
                    menus[i].Hide();
                    menus[i].AddClickListener(null);
                }
                if (i == 0)
                {
                    //selectedMenu = menus[i];
                    highImage.anchoredPosition = menus[i].mRectTransform.anchoredPosition;
                }
            }
            //if (info.page % 2 == 0)
            //{
            //    jumpButton.rectTransform().anchoredPosition = new Vector2(301, 0);
            //}
            //else
            //{
            //    jumpButton.rectTransform().anchoredPosition = new Vector2(-301, 0);
            //}
            highImage.gameObject.SetActive(false);
            jumpButton.gameObject.SetActive(false);
        }

        public override void Show()
        {
            base.Show();
        }

        private void MenuClickHandler(LabelButton btn)
        {
            if (btn == selectedMenu)
            {
                return;
            }
            highImage.gameObject.SetActive(true);
            jumpButton.gameObject.SetActive(false);
            if (selectedMenu != null)
            {
                highImage.DOAnchorPos(btn.mRectTransform.anchoredPosition, 0.3f).OnComplete(() => {
                    jumpButton.gameObject.SetActive(true);
                });
            }
            else
            {
                highImage.anchoredPosition = btn.mRectTransform.anchoredPosition;
                jumpButton.gameObject.SetActive(true);
            }

            selectedMenu = btn;
        }

        public void Handler_SwitchClick()
        {
            if (selectedMenu == null)
                return;
            int page = int.Parse(selectedMenu.identify);
            if (page % 2 != 0)
            {
                page = page - 1;
            }
            if (clickFun != null)
            {
                clickFun.Invoke(this, Option.None, page);
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (menuBox == null)
                return;
            var array = menuBox.GetComponentsInChildren<LabelButton>();
            menus.Clear();
            menus.AddRange(array);
        }
#endif
    }
}
