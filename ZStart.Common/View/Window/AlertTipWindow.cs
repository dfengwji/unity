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
            Cancel,
            Sure
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
            rightButton.onClick.AddListener(ClickSureHandler);
            leftButton.onClick.AddListener(ClickCancelHandler);
            rightButton.UpdateLabel(LocalManager.GetValue("Button.Sure.Tip"));
            leftButton.UpdateLabel(LocalManager.GetValue("Button.Cancel.Tip"));
        }

        public override void Appear(UIParamInfo info)
        {
            base.Appear(info);
        }

        public void UpdateInfo(IconType type, string title, string content = "")
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
           
            titleLabel.text = title;
            tipLabel.text = content;
        }

        public void UpdateButtons()
        {

        }

        public void AddLinstener(UnityAction<ButtonType, UIParamInfo> callFun)
        {
            clickFun = callFun;
        }

        private void ClickSureHandler()
        {
            if (clickFun != null)
            {
                clickFun.Invoke(ButtonType.Sure,paramInfo);
            }
        }

        private void ClickCancelHandler()
        {
            if (clickFun != null)
            {
                clickFun.Invoke(ButtonType.Cancel, paramInfo);
            }
        }

        public override void Clear()
        {
          
        }
    }
}
