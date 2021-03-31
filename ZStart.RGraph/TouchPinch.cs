using DG.Tweening;
using HedgehogTeam.EasyTouch;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZStart.RGraph.Util;

namespace ZStart.RGraph
{
    public class TouchPinch: Core.ZUIBehaviour
    {
        public Vector2 range = new Vector2(0.3f, 3f);
        public float speed = 3f;
        public Image image;
        public RectTransform target;
        public Camera uiCamera;
        public RGSimulator simulator;
        private float lastDistance = 0;
        private Rect bound = new Rect();
        private bool isTween = false;
        private System.Action<float> callAction;
        protected override void Start()
        {
            image = GetComponent<Image>();
            image.color = new Color(0, 0, 0, 0);
        }

        void Update()
        {
            if (Input.touchCount > 1 && target != null)
            {
                var arr = GetTouchedFingers(Input.touches);
                if (arr.Count > 1)
                {
                    if (simulator != null && simulator.isActiveAndEnabled)
                        simulator.SwitchTouch(RGSimulator.TouchType.Pinch);
                    var dist = TouchUtil.GetScreenDistance(arr);
                    CheckPinch(dist);
                    if (callAction != null)
                        callAction.Invoke(dist);
                }
            }
            else
            {
                if(simulator != null && simulator.isActiveAndEnabled)
                    simulator.SwitchTouch(RGSimulator.TouchType.Empty);
            }
        }

        protected override void OnEnable()
        {
            ScaleTarget();
        }

        protected override void OnDisable()
        {
            isTween = false;
            StopAllCoroutines();
        }

        private void ScaleTarget()
        {
            if (simulator == null || !simulator.isActiveAndEnabled || target == null || isTween)
                return;
            isTween = true;
            lastDistance = 0;
            target.localScale = Vector3.one;
            target.DOScale(new Vector3(range.x, range.x, 1), 0.2f);
            StartCoroutine(DelayZoomOut());
        }

        IEnumerator DelayZoomOut()
        {
            yield return new WaitForSeconds(1f);
            target.DOScale(Vector3.one, 0.2f);
            yield return new WaitForSeconds(0.2f);
            isTween = false;
        }
        
        public void ActiveRaycast(bool act)
        {
            image.enabled = act;
        }

        public void AddAction(System.Action<float> fun)
        {
            callAction = fun;
        }

        public void ResetData()
        {
            if(target != null)
                target.localScale = Vector3.one;
        }

        public void SetCamera(Camera cam, RGSimulator tool)
        {
            simulator = tool;
            uiCamera = cam;
            if (target == null)
                target = simulator.scrollRect.content;
            var screen = RectTransformUtility.WorldToScreenPoint(uiCamera, mTransform.position);
            var width = mTransform.rect.width;
            var height = mTransform.rect.height;
            bound.Set(screen.x - width * 0.5f, screen.y - height * 0.5f, width,height);
            ScaleTarget();
        }

        public void Zoom(float val)
        {
            if (target == null || DOTween.IsTweening(target.gameObject))
                return;
            float scale = val;
            if (scale > range.y)
                scale = range.y;
            else if (scale < range.x)
                scale = range.x;
            target.DOScale(scale, 0.2f);
        }

        public void SetRangeAndSpeed(float min, float max, float sp)
        {
            range = new Vector2(min, max);
            speed = sp;
        }

        public void HandlePinch(Gesture gesture)
        {
            //if (!isTouch)
            //    return;
            var scale = target.localScale.x - gesture.deltaPinch * Time.deltaTime;
            if (scale > range.y)
                scale = range.y;
            else if (scale < range.x)
                scale = range.x;
            target.localScale = new Vector3(scale, scale, 1);
        }

        private List<LeanFinger> GetTouchedFingers(List<LeanFinger> all)
        {
            List<LeanFinger> list = new List<LeanFinger>(2);
            for (int i = 0;i < all.Count;i += 1)
            {
                if (bound.Contains(all[i].ScreenPosition))
                {
                    list.Add(all[i]);
                    if (list.Count == 2)
                        break;
                }
            }
#if UNITY_EDITOR
            list.Clear();
            list.AddRange(all);
#endif
            return list;
        }

        private List<Touch> GetTouchedFingers(Touch[] all)
        {
            List<Touch> list = new List<Touch>(2);
            for (int i = 0; i < all.Length; i += 1)
            {
                if (bound.Contains(all[i].position))
                {
                    list.Add(all[i]);
                    if (list.Count == 2)
                        break;
                }
            }
            
#if UNITY_EDITOR
           // list.Clear();
           // list.AddRange(all);
#endif
            return list;
        }

        private void CheckPinch(float dist)
        {
            if (lastDistance < 0.001f)
                lastDistance = dist;
            var delta = dist - lastDistance;
            if (delta > 0)
                delta = 1;
            else if (delta < 0)
                delta = -1;
            else
                delta = 0;
            lastDistance = dist;
            var scale = target.localScale.x + delta * Time.deltaTime * speed;
            if (scale > range.y)
                scale = range.y;
            else if (scale < range.x)
                scale = range.x;
            target.localScale = new Vector3(scale, scale, 1);
        }

        private void HandleGesture(List<LeanFinger> fingers)
        {
            if (target == null)
                return;
            var arr = GetTouchedFingers(fingers);
            if (arr.Count < 2)
            {
                return;
            }
            var dist = LeanGesture.GetScreenDistance(arr);
            //CheckPinch(dist);
        }
    }
}
