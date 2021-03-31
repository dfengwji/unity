using ZStart.Core.Controller;
using ZStart.VRoom.Item;
using ZStart.VRoom.Manager;
using ZStart.VRoom.View.Parts;

namespace ZStart.VRoom.View
{
    public class MainPanel: Core.ZUIBehaviour
    {
        public string lookingItem = "";
        public ImageParts imageParts;
        public LabelParts labelParts;
        protected override void Start()
        {
            VRNotifyManager.AddListener(Enum.NotifyType.OnUIImage, OnImageShow);
            VRNotifyManager.AddListener(Enum.NotifyType.OnUILabel, OnLabelShow);
            VRNotifyManager.AddListener(Enum.NotifyType.OnUIVideo, OnVideoShow);
            VRNotifyManager.AddListener(Enum.NotifyType.OnLookOut, OnLookOut);
        }

        protected void Appear()
        {
            gameObject.SetActive(true);
        }

        protected void Disappear()
        {
            gameObject.SetActive(false);
        }

        private void OnLabelShow(object data)
        {
            if (data == null)
            {
                return;
            }
            var tip = data as string;
            labelParts.UpdateLabel(tip);
            labelParts.Show();
        }

        private void OnImageShow(object data)
        {
            if(data == null)
            {
                return;
            }
            var uid = data as string;
            if (lookingItem == uid)
                return;
            lookingItem = uid;
            var item = RoomSession.Instance.GetItem(uid);
            if (item != null)
            {
                var im = item as ImageItem;
                imageParts.UpdateLabel(im.desc);
                imageParts.UpdateTexture(im.source);
                imageParts.Show();
            }
            //else
            //{
            //    string path = data as string;
            //    ZImageController.Instance.Load(path, path, OnImageComplete);
            //}
        }

        private void OnImageComplete(ZImageController.ImageInfo info)
        {
            imageParts.UpdateTexture(info.texture);
        }

        private void OnVideoShow(object data)
        {
            if (data == null)
            {
                return;
            }
            var uid = data as string;
            if (lookingItem == uid)
                return;
            lookingItem = uid;
            var item = RoomSession.Instance.GetItem(uid);
            if (item != null) { }
        }

        private void OnLookOut(object data)
        {
            if (data == null)
            {
                return;
            }
            imageParts.UnShow();
            labelParts.UnShow();
            lookingItem = "";
        }
    }
}
