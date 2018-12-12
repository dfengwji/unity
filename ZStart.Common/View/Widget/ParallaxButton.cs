using DG.Tweening;
using ZStart.Common.Enum;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace ZStart.Common.View.Widget
{
    public class ParallaxButton : GazeButton
    {
        public RectTransform box;
        public Image bgImage;
        public Image bgShadow;
        public Image iconImage;
        public Image iconShadow;

        public Text label;

        private UnityAction<ParallaxButton> clickFun;
        
        protected override void Start()
        {
            base.Start();
            bgShadow.enabled = false;
        }

        public void AddClickListener(UnityAction<ParallaxButton> callback)
        {
            clickFun = callback;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (clickFun != null)
                clickFun.Invoke(this);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            mRectTransform.DOScale(Vector3.one * 1.2f, 0.4f);
            bgShadow.enabled = true;
        }
#if MODE_IVR
        public void OnHover(IVRRayPointerEventData eventData)
        {
            if (!rayCastTarget)
                return;
            if (eventData.HitResults.Count > 0)
            {
                int ind = -1;
                for (int i = 0; i < eventData.HitResults.Count;i++ )
                {
                    if(eventData.HitResults[i].gameObject == gameObject)
                    {
                        ind = i;
                        break;
                    }
                }
                if (ind < 0)
                    return;
                Vector3 hitPos = eventData.HitPoints[ind];
                DirectionType direc = CalculateDirection(hitPos, mRectTransform.position,0f);
                float signX = 0f;
                float signY = 0f;
                if (direc == DirectionType.Right)
                {
                    signY = -1;
                }
                else if (direc == DirectionType.Left)
                {
                    signY = 1;
                }
                else if (direc == DirectionType.Up)
                {
                    signX = 1;
                }
                else if (direc == DirectionType.Down)
                {
                    signX = -1;
                }
                else if (direc == DirectionType.Down_Left)
                {
                    signX = -1;
                    signY = 1;
                }
                else if (direc == DirectionType.Down_Right)
                {
                    signX = -1;
                    signY = -1;
                }
                else if (direc == DirectionType.Up_Left)
                {
                    signX = 1;
                    signY = 1;
                }
                else
                {
                    signX = 1;
                    signY = -1;
                }

                float off = 15f;
               
                Vector3 direction = mRectTransform.position - hitPos;

                float upDot = Vector2.Dot(Vector2.down, direction);
                float rightDot = Vector2.Dot(Vector2.left, direction);
                float xx = Mathf.Abs(upDot) * off;
                float yy = Mathf.Abs(rightDot) * off;
                box.rotation = Quaternion.Euler(new Vector3(signX * xx,signY * yy, 0f));
            }
        }
#endif
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            bgShadow.enabled = false;
            box.localEulerAngles = Vector3.zero;
            mRectTransform.DOScale(Vector3.one, 0.4f);
        }

        public void UpdateLabel(string tip)
        {
            if (label != null)
                label.text = tip;
        }

        public static DirectionType CalculateDirection(Vector2 from, Vector2 to, float threshold)
        {
            Vector2 direc = to - from;

            float upDot = Vector2.Dot(Vector2.down, direc);
            float rightDot = Vector2.Dot(Vector2.left, direc);
            if (upDot > threshold)
            {
                if (rightDot > threshold)
                    return DirectionType.Up_Right;
                else if (rightDot < -threshold)
                    return DirectionType.Up_Left;
                else
                    return DirectionType.Up;
            }
            else if (upDot < -threshold)
            {
                if (rightDot > threshold)
                    return DirectionType.Down_Right;
                else if (rightDot < -threshold)
                    return DirectionType.Down_Left;
                else
                    return DirectionType.Down;
            }
            else
            {
                if (rightDot > 0)
                    return DirectionType.Right;
                else
                    return DirectionType.Left;
            }
        }
    }
}
