using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace ZStart.Common.View.Widget
{
    public class SelectedToggle : LabelToggle
    {
        private UnityAction<SelectedToggle, bool> callFun;
        private bool hover = false;
        public bool IsHighlight
        {
            set
            {
                if (value)
                    DoStateTransition(SelectionState.Highlighted, false);
                else
                    DoStateTransition(SelectionState.Normal, false);
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (isOn) DoStateTransition(SelectionState.Highlighted, true);
            else DoStateTransition(SelectionState.Normal, true);
        }

        public override void OnSelect(BaseEventData eventData)
        {

        }

        public override void OnDeselect(BaseEventData eventData)
        {

        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (isOn == false && interactable)
                base.OnPointerExit(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (isOn == false && interactable)
            {
                hover = true;
                base.OnPointerEnter(eventData);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (isOn == false && interactable)
                base.OnPointerUp(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (isOn == false && interactable)
                base.OnPointerDown(eventData);
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            isOn = !isOn;
            if (callFun != null)
                callFun.Invoke(this, isOn);
        } 

        public void AddListener(UnityAction<SelectedToggle, bool> callback)
        {
            callFun = callback;
        }

        public void RemoveListeners()
        {
            callFun = null;
        }
    }
}
