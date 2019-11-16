using ZStart.Common.Manager;
using ZStart.Common.View.Widget;
using ZStart.Core.Model;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace ZStart.Common.View.Window
{
    public class AlertTipWindow:AppWindow
    {
        public enum ButtonType
        {
            Left,
            Right
        }

        public enum IconType
        {
            None = -1,
            Info = 0,
            Success = 1,
            Failed = 2,
        }

        public Text titleLabel;
        public Image iconImage;
        public Text tipLabel;
        public IconLabelButton leftButton;
        public IconLabelButton rightButton;

        public List<Sprite> mIconList;

        private UnityAction<ButtonType,UIParamInfo> clickFun;
      
        protected override void Start()
        {
            base.Start();
            rightButton.onClick.AddListener(ClickRightHandler);
            leftButton.onClick.AddListener(ClickLeftHandler);
        }

        public override void Appear(UIParamInfo info)
        {
            base.Appear(info);
        }

        public void UpdateInfo(IconType type, string title, string content = "", string left = "", string right = "")
        {
            //switch (type)
            //{
            //    case IconType.None:
            //        iconImage.enabled = false;
            //        break;
            //    case IconType.Info:
            //    case IconType.Success:
            //    case IconType.Failed:
            //        iconImage.enabled = true;
            //        iconImage.overrideSprite = mIconList[(int)type];
            //        break;
            //}
            UpdateInfo(title, content, left, right);
        }

        public void UpdateInfo(string title, string content, string left = "", string right = "")
        {
            if (!string.IsNullOrEmpty(left))
                leftButton.UpdateLabel(left);
            else
                leftButton.UpdateLabel(LocalManager.GetValue("Button.Cancel.Tip"));
            if (!string.IsNullOrEmpty(right))
                rightButton.UpdateLabel(right);
            else
                rightButton.UpdateLabel(LocalManager.GetValue("Button.Sure.Tip"));
            titleLabel.text = title;
            tipLabel.text = content;
        }

        public void AddLinstener(UnityAction<ButtonType, UIParamInfo> callFun)
        {
            clickFun = callFun;
        }

        private void ClickRightHandler()
        {
            if (clickFun != null)
            {
                clickFun.Invoke(ButtonType.Right,paramInfo);
            }
        }

        private void ClickLeftHandler()
        {
            if (clickFun != null)
            {
                clickFun.Invoke(ButtonType.Left, paramInfo);
            }
        }

        public override void Clear()
        {
          
        }
    }
}
