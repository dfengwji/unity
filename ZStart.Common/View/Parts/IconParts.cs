using UnityEngine;
using UnityEngine.UI;
namespace ZStart.Common.View.Parts
{
    public class IconParts:AppParts
    {
        public string uid = "";
        public Image icon;

        public void UpdateSprite(Sprite sp)
        {
            if (icon != null)
            {
                icon.overrideSprite = sp;
            }
        }

        public void UpdateAdaptSprite(Sprite sp)
        {
            if(icon != null){
                icon.overrideSprite = sp;
                icon.enabled = sp == null ? false : true;
                if (sp != null)
                    icon.SetNativeSize();
            }
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            if(icon == null)
                icon = GetComponentInChildren<Image>();
        }
#endif

        public override void Clear()
        {
            
        }
    }
}
