using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZStart.RGraph.View.Parts
{
    public class ImageParts : AppUIParts, IEventSystemHandler, IPointerClickHandler
    {
        public RawImage image;
        public Text label;
        public Image mask;
        protected UnityAction<ImageParts> clickCallFun;

        public void AddListener(UnityAction<ImageParts> clickFun)
        {
            clickCallFun = clickFun;
        }

        public void UpdateImage(Texture2D texture)
        {
            image.texture = texture;
        }

        public override void Clear()
        {

        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickCallFun != null)
            {
                clickCallFun.Invoke(this);
            }
        }
    }
}
