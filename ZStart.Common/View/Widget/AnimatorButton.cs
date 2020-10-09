using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZStart.Common.View.Widget
{
    public class AnimatorButton:GazeButton
    {
        public Text label;
        public bool isHover = false;
        private Vector3 initPos = Vector3.zero;
        private UnityAction<AnimatorButton> clickFun;
        
        protected override void Start()
        {
            base.Start();
            isHover = false;
            initPos = mRectTransform.position;
        }

        protected override void OnDisable()
        {
            mRectTransform.position = initPos;
            isHover = false;
            base.OnDisable();
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (clickFun != null)
                clickFun.Invoke(this);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (isHover)
                return;
            base.OnPointerEnter(eventData);
            isHover = true;
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (!isHover)
                return;
            //base.OnPointerExit(eventData);
            isHover = false;
            //mRectTransform.DOMove(initPos, 0.15f).OnComplete(() => { isHover = false; });
        }

        public void AddClickListener(UnityAction<AnimatorButton> callback)
        {
            clickFun = callback;
        }

        public void UpdateLabel(string txt)
        {
            if (label != null)
                label.text = txt;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (label == null)
                label = GetComponentInChildren<Text>();
            if (animat == null)
                animat = GetComponentInChildren<Animator>();
        }
#endif
    }
}
