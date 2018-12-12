using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ZStart.Common.Controller;

namespace ZStart.Common.View.Parts
{
    [RequireComponent(typeof(RectMask2D))]
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollTextParts:AppParts,IPointerEnterHandler,IPointerExitHandler
    {
        public bool isAutoScroll = false;
        public MoveDirection loopDirection = MoveDirection.Left;
        public float speed = 0.005f;
        private ScrollRect mScroll;
        private Text mTarget;

        private bool isHover = false;
        private float fVelocity = 0;
   
        // Use this for initialization
        protected override void Awake()
        {
            if(mScroll == null)
                mScroll = GetComponent<ScrollRect>();
            if(mTarget == null)
                mTarget = GetComponentInChildren<Text>();
            mScroll.movementType = ScrollRect.MovementType.Unrestricted;
            mScroll.horizontal = false;
            mScroll.viewport.anchorMax = new Vector2(0.5f,0.5f);
            mScroll.viewport.anchorMin = new Vector2(0.5f,0.5f);
            mScroll.content.pivot = new Vector2(0.5f, 0.5f);
        }

        protected override void OnEnable()
        {
            isHover = false;
            CheckNormalizedPos();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            CallbackController.Instance.UnRegisterUpdateEvent(this.gameObject);
        }

        public void UpdateLabel(string tip)
        {
            if (mTarget != null)
            {
                mTarget.text = tip;
            }
        }
      
        void UpdateAction()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ScrollContent(MoveDirection.Left);
            }else if(Input.GetKeyDown(KeyCode.RightArrow)){
                ScrollContent(MoveDirection.Right);
            }
#endif
            if(isAutoScroll){
                ScrollContent();
            }
            else
            {
                if (isHover)
                {
                    ScrollContent();
                }
            }
            
        }

        private void ScrollContent(MoveDirection move)
        {
            Vector2 size = mScroll.content.sizeDelta;
            Vector2 pos = mScroll.content.anchoredPosition;
            Vector2 viewSize = mScroll.viewport.sizeDelta;
            float diff = 0f;
            float ammount = 0f;
            if (move == MoveDirection.Left || move == MoveDirection.Right)
            {
                ammount = mScroll.horizontalNormalizedPosition;
                diff = size.x * 0.5f + viewSize.x * 0.5f;
                if (move == MoveDirection.Left)
                {
                    if (pos.x < 0 && Mathf.Abs(pos.x) > diff)
                        return;
                    ammount += mScroll.viewport.sizeDelta.x / size.x;
                }
                else
                {
                    if (pos.x > 0 && pos.x > diff)
                        return;
                    ammount -= mScroll.viewport.sizeDelta.x / size.x;
                }
                mScroll.horizontalNormalizedPosition = ammount;
            }
        }

        private void ScrollContent()
        {
            Vector2 targetSize = mScroll.content.sizeDelta;
            Vector2 pos = mScroll.content.anchoredPosition;
            Vector2 viewSize = mScroll.viewport.sizeDelta;
            float diff = 0f;
            float ammount = 0f;
            if (loopDirection == MoveDirection.Left || loopDirection == MoveDirection.Right)
            {
                ammount = mScroll.horizontalNormalizedPosition;
                diff = targetSize.x * 0.5f + viewSize.x * 0.5f;
                if (loopDirection == MoveDirection.Left)
                {
                    if (pos.x < 0 && Mathf.Abs(pos.x) > diff)
                    {
                        mTarget.rectTransform.anchoredPosition = new Vector2(diff,pos.y);
                    }
                    else
                    {
                        ammount += viewSize.x / targetSize.x * speed;
                        mScroll.horizontalNormalizedPosition = ammount;
                    }
                }
                else
                {
                    if (pos.x > 0 && pos.x > diff)
                    {
                        mTarget.rectTransform.anchoredPosition = new Vector2(-diff, pos.y);
                    }
                    else
                    {
                        ammount -= viewSize.x / targetSize.x * speed;
                        mScroll.horizontalNormalizedPosition = ammount;
                    }
                }
                
            }else if(loopDirection == MoveDirection.None){
                ScrollMove();
            }
        }

        private void ScrollMove()
        {
            if (mScroll.viewport.sizeDelta.x > mScroll.content.sizeDelta.x) 
                return;
          
            float amount = mScroll.horizontalNormalizedPosition;
            if (amount >= 0.99f)
            {
                fVelocity = -0.005f * mScroll.viewport.sizeDelta.x / mScroll.content.sizeDelta.x;
            }
            else if (amount < 0.01f)
            {
                fVelocity = 0.005f * mScroll.viewport.sizeDelta.x / mScroll.content.sizeDelta.x;
            }
            mScroll.horizontalNormalizedPosition = Mathf.Clamp01(amount + fVelocity);
        }

        public void CheckNormalizedPos()
        {
            StartCoroutine(PosInspector());
        }

        public IEnumerator PosInspector()
        {
            yield return new WaitForSeconds(0.05f);
            if (mScroll.viewport.sizeDelta.x > mScroll.content.sizeDelta.x)
                mScroll.horizontalNormalizedPosition = 0.5f;
            else
            {
                if(loopDirection == MoveDirection.Left)
                    mScroll.horizontalNormalizedPosition = 0f;
                else
                    mScroll.horizontalNormalizedPosition = 1f;
            }
               
            yield return null;
            CallbackController.Instance.RegisterUpdateEvent(this.gameObject, UpdateAction);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
           
        }
#endif

        public override void Clear()
        {
            isHover = false;
            mScroll.horizontalNormalizedPosition = 0f;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isAutoScroll)
                return;
            mScroll.horizontalNormalizedPosition = 0f;
            isHover = false;
        }
    }
}
