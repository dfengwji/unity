using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace ZStart.Common.View.Widget
{
    public class IconButton:GazeButton
    {
        public Image icon;
        public RawImage rawIcon;
        private UnityAction<IconButton> clickFun;
        private UnityAction<IconButton, bool> callFun;
        private UnityAction<IconButton, bool> hoverCallFun;

        private bool isOn = false;
        public bool isHighlight
        {
            set
            {
                isOn = value;
                if (value)
                    DoStateTransition(SelectionState.Highlighted, false);
                else
                    DoStateTransition(SelectionState.Normal, false);
            }
        }

        public bool IsDisabled
        {
            set
            {
                interactable = !value;
                if (icon != null && icon.material.HasProperty("_TrunOn"))
                {
                    icon.material.SetFloat("_TrunOn", value ? 0f : 1f);
                }
            }
        }
        protected override void Start()
        {
            base.Start();
            if (icon == null)
                icon = GetComponentInChildren<Image>();
            if (rawIcon == null)
                rawIcon = GetComponentInChildren<RawImage>();
        }

        public void AddClickListener(UnityAction<IconButton> callback)
        {
            clickFun = callback;
        }

        public void AddListener(UnityAction<IconButton, bool> callback, UnityAction<IconButton, bool> hoverback = null)
        {
            callFun = callback;
            hoverCallFun = hoverback;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (isOn) DoStateTransition(SelectionState.Highlighted, true);
            else DoStateTransition(SelectionState.Normal, true);
        }

        public override void OnSelect(BaseEventData eventData)
        {
            if (hoverCallFun != null && interactable)
                hoverCallFun.Invoke(this, true);
        }

        public override void OnDeselect(BaseEventData eventData)
        {

        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (clickFun != null)
                clickFun.Invoke(this);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if(isOn == false)
                base.OnPointerEnter(eventData);
            if (hoverCallFun != null && interactable)
                hoverCallFun.Invoke(this, true);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (isOn == false)
                base.OnPointerExit(eventData);
            if (hoverCallFun != null && interactable)
                hoverCallFun.Invoke(this, false);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (isOn == false)
                base.OnPointerDown(eventData);
            if (callFun != null && interactable)
                callFun.Invoke(this, true);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (isOn == false)
                base.OnPointerUp(eventData);
            if (callFun != null && interactable)
                callFun(this, false);
        }

        public void Clear()
        {
            callFun = null;
            hoverCallFun = null;
            clickFun = null;
            id = 0;
        }

        public void ShowIcon(bool show)
        {
            if (icon != null)
                icon.enabled = show;
        }

        public void ShowTexture(bool show)
        {
            if (rawIcon != null)
                rawIcon.enabled = show;
        }

        public void SwitchImage(bool raw)
        {
            if (rawIcon != null)
                rawIcon.enabled = raw;
            if (icon != null)
                icon.enabled = !raw;
        }

        public void UpdateSprite(Sprite sp)
        {
            if (sp != null)
                icon.sprite = sp;
            icon.enabled = sp == null ? false : true;
        }

        public void UpdateTexture(Texture2D tex)
        {
            if (tex != null)
                rawIcon.texture = tex;
            rawIcon.enabled = tex == null ? false : true;
        }

        public void UpdateAdaptTexture(Texture2D tex)
        {
            rawIcon.texture = tex;
            rawIcon.enabled = tex == null ? false : true;
            if (tex != null)
                rawIcon.SetNativeSize();
        }

        public void UpdateAdaptSprite(Sprite sp)
        {
            icon.sprite = sp;
            icon.enabled = sp == null ? false : true;
            if (sp != null)
                icon.SetNativeSize();
        }

        public void UpdateAmount(float amount)
        {
            if (icon != null && icon.type == Image.Type.Filled)
                icon.fillAmount = amount;
        }

        public void UpdateState(bool act)
        {
            interactable = act;
            if (icon != null && icon.material.HasProperty("_TrunOn"))
            {
                icon.material.SetFloat("_TrunOn", act ? 1f : 0f);
            }
        }


    }
}
