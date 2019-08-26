using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZStart.Common.View;
using ZStart.Common.View.Parts;
using ZStart.Core;
using ZStart.Core.Controller;
using ZStart.Core.Model;

namespace ZStart.Common.Controller
{
    internal class AlertController : ZSingletonBehaviour<AlertController>
    {
        static int Comparison(AlertParts a, AlertParts b)
        {
            if (a.movementStart < b.movementStart) return -1;
            if (a.movementStart > b.movementStart) return 1;
            return 0;
        }
        public Canvas rootCanvas;
        public AlertParts iconLabelPrefab;
        public AlertParts labelPrefab;
        public Font defFont;

        public GameObject overlayMask;
       
        public AnimationCurve offsetYCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 40f) });

        public AnimationCurve offsetXCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 40f) });

        /// <summary>
        /// Curve used to fade out entries with time.
        /// </summary>

        public AnimationCurve alphaCurve = new AnimationCurve(new Keyframe[] { new Keyframe(1f, 1f), new Keyframe(2f, 0f) });

        /// <summary>
        /// Curve used to scale the entries.
        /// </summary>

        public AnimationCurve scaleCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.25f, 1f) });

        public List<AlertParts> mList = new List<AlertParts>();
        public List<AlertParts> mUnused = new List<AlertParts>();
        int counter = 0;

        public bool isTween = false;
        public Transform effectBox = null;
        public RectTransform windowBox = null;
        public string openedWindow = "";
        private AppWindow _alertWindow;
        public BaseEffect mShowVfx = null;
        public BaseEffect mCloseVfx = null;
        public Dictionary<string,UIParamInfo> historyWindows;
        public float openDelay = 1.5f;
        public float hideDelay = 1.5f;
        public bool hasOpenedWindow
        {
            get
            {
                return _alertWindow == null ? false : true;
            }
        }

        void Start()
        {
            historyWindows = new Dictionary<string, UIParamInfo>();
            if(mShowVfx != null)
                mShowVfx.Init();
            if(mCloseVfx != null)
                mCloseVfx.Init();
        }

        public bool IsOpened<T>() where T : AppWindow
        {
            if (_alertWindow == null)
                return false;
            if ((typeof(T) == _alertWindow.GetType() || _alertWindow.GetType().IsSubclassOf(typeof(T))))
                return true;
            else
                return false;
        }

        public T GetCurrentWindow<T>() where T : AppWindow
        {
            if (_alertWindow == null)
                return null;
            return _alertWindow as T;
        }

        public AppWindow GetOpenedWindow()
        {
            if (_alertWindow == null)
                return null;
            return _alertWindow;
        }

        public T ShowWindow<T>(UIParamInfo info, bool effect, bool mask) where T : AppWindow
        {
            if(overlayMask != null)
                overlayMask.SetActive(mask);
            T clone = ZAssetController.Instance.ActivateAsset<T>(windowBox);
            _alertWindow = clone;
            openedWindow = typeof(T).Name;
            clone.effectPoint = effectBox;
            if (historyWindows.ContainsKey(openedWindow))
                historyWindows.Remove(openedWindow);
            historyWindows.Add(openedWindow,info);
           
            if (effect && openDelay > 0.01f)
                StartCoroutine(ShowInspector(clone,info));
            else
            {
                StartCoroutine(OpenDelay(clone, info));
            }
            return clone;
        }

        public T ShowNow<T>(UIParamInfo info, bool effect, bool mask) where T : AppWindow
        {
            if(overlayMask != null)
                overlayMask.SetActive(mask);
            if (_alertWindow != null && typeof(T) == _alertWindow.GetType())
                return _alertWindow as T;

            T clone = ZAssetController.Instance.ActivateAsset<T>(windowBox);
            _alertWindow = clone;
            openedWindow = typeof(T).Name;
            clone.effectPoint = effectBox;
            if (historyWindows.ContainsKey(openedWindow))
                historyWindows.Remove(openedWindow);
            historyWindows.Add(openedWindow,info);
            StartCoroutine(OpenDelay(clone, info));
            return clone;
        }

        IEnumerator OpenDelay(AppWindow window, UIParamInfo info)
        {
            while (!window.IsStarted)
            {
                yield return null;
            }
            window.AddListeners();
            window.Appear(info);
        }

        public void CloseWindow()
        {
            if(overlayMask != null)
                overlayMask.SetActive(false);
            openedWindow = "";
            if (_alertWindow != null)
            {
                if (mShowVfx != null && mShowVfx.isActiveAndEnabled)
                    StartCoroutine(HideInspector());
                else
                {
                    CloseNow();
                }
            }
        }

        public void CloseNow()
        {
            openedWindow = "";
        
            _alertWindow.RemoveListeners();
            _alertWindow.Disappear();
            _alertWindow.Clear();
            _alertWindow = null;
        }

        public void Clear()
        {
            historyWindows.Clear();
        }

        IEnumerator ShowInspector(AppWindow window, UIParamInfo info)
        {
            isTween = true;
            if(mShowVfx != null)
            {
                mShowVfx.gameObject.SetActive(true);
                mShowVfx.Play();
            }
            if (mCloseVfx != null)
            {
                mCloseVfx.Stop();
                mCloseVfx.gameObject.SetActive(false);
            }
            
            window.AddListeners();
            yield return new WaitForSeconds(openDelay);
            window.Appear(info);
            isTween = false;
        }

        IEnumerator HideInspector()
        {
            isTween = true;
            if (mShowVfx != null)
            {
                mShowVfx.Stop();
                mShowVfx.gameObject.SetActive(false);
            }
            if (mCloseVfx != null)
            {
                mCloseVfx.gameObject.SetActive(true);
                mCloseVfx.Play();
            }
           
            _alertWindow.RemoveListeners();
            _alertWindow.Clear();
            _alertWindow.Disappear();
            yield return new WaitForSeconds(hideDelay);
            _alertWindow = null;
            isTween = false;
        }

        void Update()
        {
            if (mList.Count == 0) return;
            float time = Time.realtimeSinceStartup;

            Keyframe[] offsetYs = offsetYCurve.keys;
            Keyframe[] offsetXs = offsetXCurve.keys;
            Keyframe[] alphas = alphaCurve.keys;
            Keyframe[] scales = scaleCurve.keys;

            float offsetYEnd = offsetYs[offsetYs.Length - 1].time;
            float offsetXEnd = offsetYs[offsetXs.Length - 1].time;
            float alphaEnd = alphas[alphas.Length - 1].time;
            float scalesEnd = scales[scales.Length - 1].time;

            float totalEnd = Mathf.Max(scalesEnd, Mathf.Max(offsetYEnd, alphaEnd, offsetXEnd));

            // Adjust alpha and delete old entries
            for (int i = mList.Count; i > 0;)
            {
                AlertParts ent = mList[--i];
                float currentTime = time - ent.movementStart;
                ent.offsetY = offsetYCurve.Evaluate(currentTime);
                ent.offsetX = offsetXCurve.Evaluate(currentTime);
                float alp = alphaCurve.Evaluate(currentTime);
                Color orgColor = ent.label.color;
                ent.label.color = new Color(orgColor.r, orgColor.g, orgColor.b, alp);

                // Make the label scale in
                float s = scaleCurve.Evaluate(time - ent.time) * 1;
                if (s < 0.001f) s = 0.001f;
                ent.mTransform.localScale = new Vector3(s, s, s);

                // Delete the entry when needed
                if (currentTime > totalEnd) Delete(ent);
                else ent.enabled = true;
            }

            float offsetY = 0f;
            float offsetX = 0f;
            // Move the entries
            for (int i = mList.Count; i > 0;)
            {
                AlertParts ent = mList[--i];
                offsetY = Mathf.Max(offsetY, ent.offsetY) + ent.initOffY + ent.initPos.y;
                offsetX = Mathf.Max(offsetX, ent.offsetX) * ent.sign + ent.initPos.x;
                ent.mTransform.localPosition = new Vector3(offsetX, offsetY, 0f);
                offsetY += Mathf.Round(ent.mTransform.localScale.y);
            }
        }

        void Delete(AlertParts ent)
        {
            mList.Remove(ent);
            mUnused.Add(ent);
            ent.gameObject.SetActive(false);
        }

        AlertParts Create(bool icon)
        {
            // See if an unused entry can be reused
            if (mUnused.Count > 0)
            {
                for (int i = 0; i < mUnused.Count; i++)
                {
                    AlertParts comp = mUnused[i];
                    if (comp.isIcon == icon)
                    {
                        mUnused.RemoveAt(i);
                        comp.time = Time.realtimeSinceStartup;
                        comp.gameObject.SetActive(true);
                        comp.offsetY = 0f;
                        comp.offsetX = 0;
                        comp.sign = 0;
                        mList.Add(comp);
                        return comp;
                    }
                }
            }

            // New entry
            AlertParts item = null;
            if (icon)
                item = Instantiate(iconLabelPrefab);
            else
                item = Instantiate(labelPrefab);
            item.gameObject.SetActive(true);
            item.mTransform.SetParent(windowBox);
            item.mTransform.localPosition = Vector3.zero;
            item.time = Time.realtimeSinceStartup;
            item.sign = 0;
            //ne.label = NGUITools.AddWidget<UILabel>(gameObject);
            item.name = counter.ToString();
           
            // Make it small so that it's invisible to start with
            item.mTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            mList.Add(item);
            ++counter;
            return item;
        }

        public void ShowAlert(Sprite sp, string tip, Vector2 initPos)
        {
            if (!enabled) return;

            //float time = Time.realtimeSinceStartup;
            float val = 0f;

            // Create a new entry
            AlertParts ne = Create(sp != null ? true : false);
            ne.stay = 0;
            //ne.label.color = c;
            ne.val = val;
            ne.initOffY = 0;
            ne.initPos = initPos;
            ne.UpdateImage(sp);
            ne.label.text = tip;

            // Sort the list
            mList.Sort(Comparison);
        }
    }

    public class Alert
    {
        private Alert()
        {
        }

        public static void SetWindowBox(RectTransform box)
        {
            AlertController.Instance.windowBox = box;
        }

        public static Canvas RootCanvas
        {
            get
            {
                return AlertController.Instance.rootCanvas;
            }
        }

        public static void ShowTip(string tip)
        {
            if (string.IsNullOrEmpty(tip))
                return;
            ZLog.Warning(tip);
            AlertController.Instance.ShowAlert(null, tip, Vector2.zero);
        }

        public static bool HasWindow
        {
            get
            {
                return AlertController.Instance.hasOpenedWindow;
            }
        }

        public static AppWindow GetOpenedWindow()
        {
            return AlertController.Instance.GetOpenedWindow();
        }

        public static bool IsOpend<T>() where T : AppWindow
        {
            return AlertController.Instance.IsOpened<T>();
        }

        /// <summary>
        /// 带特效动画的
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ShowWindow<T>() where T : AppWindow
        {
            if (AlertController.Instance == null)
                Debug.LogError("AlertController is not find!!!");
          
            return ShowWindow<T>(new UIParamInfo(),true,true);
        }

        /// <summary>
        /// 不带特效动画
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T ShowSoloWindow<T>() where T : AppWindow
        {
            if (AlertController.Instance == null)
                Debug.LogError("AlertController is not find!!!");

            return ShowWindow<T>(new UIParamInfo(),false,true);
        }

        public static T ShowSoloWindow<T>(UIParamInfo info,bool mask = true) where T : AppWindow
        {
            if (AlertController.Instance == null)
                Debug.LogError("AlertController is not find!!!");

            return ShowWindow<T>(info,false,mask);
        }

        public static T ShowWindow<T>(UIParamInfo info, bool mask = true) where T : AppWindow
        {
            if (AlertController.Instance == null)
                Debug.LogError("AlertController is not find!!!");
            return ShowWindow<T>(info, true,mask);
        }

        private static T ShowWindow<T>(UIParamInfo info, bool effect,bool mask = true) where T : AppWindow
        {
            if (AlertController.Instance == null)
                Debug.LogError("AlertController is not find!!!");
            if (Alert.HasWindow)
            {
                if (AlertController.Instance.IsOpened<T>())
                {
                    AppWindow tmp = AlertController.Instance.GetCurrentWindow<T>();
                    tmp.Appear(info);
                    return tmp as T;
                }
                AlertController.Instance.CloseNow();
                return AlertController.Instance.ShowNow<T>(info, effect, true);
            }
            else
            {
                return AlertController.Instance.ShowWindow<T>(info, effect, mask);
            }
        }

        public static void Close()
        {
            if (AlertController.Instance.isTween)
                return;
            AlertController.Instance.CloseWindow();
        }

        public static void Clear()
        {
            AlertController.Instance.Clear();
        }
    }
}
