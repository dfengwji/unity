using UnityEngine.UI;

namespace ZStart.Common.View.Widget
{
    public class HoverButton:GazeButton
    {
        private bool _hover = false;
        public bool hover
        {
            get
            {
                return _hover;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _hover = false;
        }

        public override void OnSelect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            //base.OnSelect(eventData);
        }

        public override void OnDeselect(UnityEngine.EventSystems.BaseEventData eventData)
        {
            //base.OnDeselect(eventData);
        }
    
        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (state == SelectionState.Highlighted)
                _hover = true;
            else
                _hover = false;
            base.DoStateTransition(state, instant);
        }
    }
}
