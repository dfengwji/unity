using UnityEngine;
using UnityEngine.UI;
using XTC.MVCS;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Manager;
using ZStart.RGraph.Model;
using ZStart.RGraph.View.Group;
using ZStart.Core.Controller;
using System.Collections.Generic;
using ZStart.RGraph.View.Parts;
using System.IO;
using ZStart.RGraph.Layout;
using ZStart.RGraph.View.Item;

namespace ZStart.RGraph.View
{
    public class AppUIPanel: Core.ZUIBehaviour
    {
        public Text titleLabel;
        public RectTransform titleBox;

        public RawImage bigImage;
        public Image imageMask;

        public Button exitButton;

        public RecordGroup recordGroup;
        public TimeLineGroup timeGroup;
        public RecommendGroup recommendGroup;
        public NodeInfo selectedNode = null;
        public MenuType selectedMenu = MenuType.Unknown;
        public bool isSearch = false;
        protected override void Start()
        {
           
            exitButton.gameObject.SetActive(true);
        }

        protected override void OnEnable()
        {
            AddListeners();
        }

        protected override void OnDisable()
        {
            RemoveListeners();
        }

        public void Appear(string title, bool search = true)
        {
            timeGroup.Disappear();
            recordGroup.Disappear();
            isSearch = search;
            if (search)
            {
                recommendGroup.Appear();
                recommendGroup.Shrink();
            }
            else
            {
                recommendGroup.Disappear();
            }
            SetTitle(title);
            imageMask.enabled = false;
        }


        private void SetTitle(string title)
        {
            titleBox.gameObject.SetActive(!string.IsNullOrEmpty(title));
            titleLabel.text = title;
        }

        public void AddListeners()
        {
            DFNotifyManager.AddListener(DFNotifyType.OnNodeSelected, gameObject, OnNodeSelected);
            DFNotifyManager.AddListener(DFNotifyType.OnUIRefresh, gameObject, OnUIUpdate);
            DFNotifyManager.AddListener(DFNotifyType.OnNodeMenuUpdate, gameObject, OnMenuUpdate);
            DFNotifyManager.AddListener(DFNotifyType.OnThemesUpdate, gameObject, OnThemesUpdate);
            timeGroup.AddListeners();
        }

        public void RemoveListeners()
        {
            DFNotifyManager.RemoveListener(DFNotifyType.OnNodeSelected, gameObject);
            DFNotifyManager.RemoveListener(DFNotifyType.OnUIRefresh, gameObject);
            DFNotifyManager.RemoveListener(DFNotifyType.OnNodeMenuUpdate, gameObject);
            DFNotifyManager.RemoveListener(DFNotifyType.OnThemesUpdate, gameObject);
            timeGroup.RemoveListeners();
        }

        private void OnUIUpdate(object data)
        {

        }

        private void OnNodeSelected(object data)
        {
            if (data == null)
            {
                selectedMenu = MenuType.Unknown;
                selectedNode = null;
                timeGroup.Disappear();
                recordGroup.Disappear();
                if (isSearch)
                {
                    recommendGroup.Appear();
                    recommendGroup.Shrink();
                }
                imageMask.enabled = false;
            }
            else
            {
                selectedNode = (NodeInfo)data;
            }
        }

        private void OnMenuUpdate(object data)
        {
            var menu = (int)data;
            SwitchMenu((MenuType)menu);
        }

        private void OnImageUpdate(ZImageController.ImageInfo info)
        {
            if (info == null)
                return;
            if (recordGroup.identify == info.identify)
            {
                recordGroup.UpdateHeader(info.texture);
            }
            else
            {
                if (bigImage.enabled)
                {
                    bigImage.texture = info.texture;
                    bigImage.SetNativeSize();
                }
            }
        }

        private void OnThemesUpdate(object data)
        {
            if (data == null)
                return;
            var list = data as List<ThemeInfo>;
            recommendGroup.UpdateInfo(list.ToArray(), WordsClickHandle);
        }

        private void ImageClickHandle(ImageParts parts)
        {
            if (selectedNode == null)
                return;
            AffairInfo info = selectedNode.entity.GetAffair(parts.identify);
            imageMask.enabled = true;
            ZImageController.Instance.Load(parts.identify, info.images[0], OnImageUpdate);
            bigImage.enabled = true;
        }

        private void WordsClickHandle(LabelParts parts)
        {
            SetTitle(parts.Name);
            // proxy.GetGraphByNode(parts.identify);
        }

        private void SwitchMenu(MenuType menu)
        {
            if (menu == selectedMenu || selectedNode == null)
                return;
            selectedMenu = menu;
            if (menu == MenuType.Remark)
            {
                timeGroup.Disappear();
                var path = Path.Combine(MessageManager.Instance.RootPath, selectedNode.avatar);
                ZImageController.Instance.Load(selectedNode.UID, path, OnImageUpdate);
                recordGroup.UpdateInfo(selectedNode.entity);
                recommendGroup.Disappear();
            }
            else if (menu == MenuType.Experience)
            {
                recordGroup.Disappear();
                timeGroup.ShowSelected(selectedNode);
                recommendGroup.Disappear();
            }
        }

        public void Handler_ExpendNode()
        {
            DFNotifyManager.SendNotify(DFNotifyType.OnNodeExpend, selectedNode.UID);
        }

        public void Handler_PinNode()
        {
            DFNotifyManager.SendNotify(DFNotifyType.OnNodePin, selectedNode.UID);
        }

        public void Handler_Exit()
        {
        }

        public void Handler_BackThemes()
        {
            DFNotifyManager.SendNotify(DFNotifyType.OnNodeSelected, null);
            //AppSession.Instance.Proxy.OpenThemes();
        }

        public void Handler_ClosePhoto()
        {
            imageMask.enabled = false;
            bigImage.enabled = false;
        }

    }
}
