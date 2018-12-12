using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZStart.Common.View.Widget
{
    public class GazeButton:Button
    {
        public static float gazeTime = 5f;

        public long id = 0;
        public string identify = "";
        private UnityAction<int> gazeCallFun;

        private RectTransform _mRectTrans;
        public RectTransform mRectTransform
        {
            get
            {
                if (_mRectTrans == null)
                    _mRectTrans = transform as RectTransform;
                return _mRectTrans;
            }
        }

        private bool isRaycst = false;
        public bool rayCastTarget
        {
            set
            {
                isRaycst = value;
            }
            get
            {
                return isRaycst;
            }
        }

        public void AddGazeListener(UnityAction<int> callback)
        {
            gazeCallFun = callback;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            //if (gazeTime > 0.01f)
            //{
            //    AnchorController.Instance.Gaze(gazeTime);
            //    StartCoroutine(GazeInspector());
            //}
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
           // StopAllCoroutines();
           // AnchorController.Instance.Gaze(0);
        }

        IEnumerator GazeInspector()
        {
            yield return new WaitForSeconds(gazeTime);
            if (gazeCallFun != null)
                gazeCallFun.Invoke(0);
        }

        public virtual void Show()
        {
            //if (isActiveAndEnabled == false)
                gameObject.SetActive(true);
        }

        public virtual void UnShow()
        {
            //if (isActiveAndEnabled)
                gameObject.SetActive(false);
        }
    }
}
