using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZStart.Core.Enum;
using ZStart.Core.Model;
using ZStart.Core.View;

namespace ZStart.Core.Controller
{
    public class ZUIPanelController:ZSingletonBehaviour<ZUIPanelController>
    {
        public static void Init()
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            Instance.InitPrefabs();
        }

        public static void DisplayPanel<T>(UIParamInfo info) where T:ZUIPanel
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            Instance.ShowPanel<T>(info);
        }

        public static void DisplayPanel<T>() where T : ZUIPanel
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            Instance.ShowPanel<T>(new UIParamInfo());
        }

        public static void DisplayBackPanel()
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            Instance.ShowBackPanel();
        }

        public static void DisplayPanel(string uname)
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            Instance.ShowPanel(uname, new UIParamInfo());
        }

        public static void HidePanel<T>()where T:ZUIPanel
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            Instance.CloseWakenPanel<T>();
        }

        public static void HideWakenPanels()
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");

            Instance.CloseWakenPanels();
        }

        public static void HideWakensExcept<T>()
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");

            Instance.CloseWakensExcept<T>();
        }

        public static ZUIPanel GetCurrentPanel()
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            return Instance.currentPanel;
        }

        public static T GetPanel<T>() where T:ZUIPanel
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            return Instance.GetExistPanel<T>();
        } 

        public static ZUIPanel[] GetWakendPanels()
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            List<ZUIPanel> list = new List<ZUIPanel>();
            for (int i = 0; i < Instance.panels.Count; i++)
            {
                ZUIPanel panel = Instance.panels[i] as ZUIPanel;
                if  (panel != null && panel.isOpen)
                    list.Add(panel);
            }

            return list.ToArray();
        }

        public static int GetWakendPanelCount()
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            int num = 0;
            for (int i = 0; i < Instance.panels.Count; i++)
            {
                IZUIPanel panel = Instance.panels[i];
                if (panel != null && panel.isOpen)
                    num++;
            }

            return num;
        }

        public static bool HasPanelWaken<T>() where T : ZUIPanel
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            return Instance.IsWaken<T>();
        }

        public static void WakenCurrentPanel(UIParamInfo info)
        {
            if (Instance == null)
                Debug.LogError("panel controller is not find!!!");
            Instance.RepeatOpenPanel(Instance.currentPanel, info);
        }

        public static Vector3 UIToScreen(Vector3 uiPos)
        {
            if (Instance.uiCamera == null) return Vector3.zero;
            return Instance.uiCamera.WorldToScreenPoint(uiPos);
        }

        public static Vector3 ScreenToUI(Vector3 wpos)
        {
            if (Instance.uiCamera == null) return Vector3.zero;
            return Instance.uiCamera.ScreenToWorldPoint(wpos);
        }

        private ZUIPanel _currentPanel = null;
        public ZUIPanel currentPanel
        {
            get { return _currentPanel; }
        }

        public RectTransform panelParent;
       
        public Camera uiCamera;
        public RectTransform effectPoint;
        public Vector3 canvasScale = Vector3.one;
        public Vector2 defaultScreen = new Vector2(640f,960f);
        public List<ZUIPanel> prefabs;
        public List<IZUIPanel> panels = null;

        public int historyGrade = 3;
        public string current = "";
        public List<string> history = null;
        
        public static string parent
        {
            get
            {
                string last = "";
                List<string> list = Instance.history;
                if (list.Count > 0)
                {
                    int ind = list.Count - 1;
                    last = list[ind];
                }
                return last;
            }
        }

        public static Vector2 screen
        {
            get
            {
                return Instance.defaultScreen;
            }
        }

        public static Vector3 scale
        {
            get
            {
                return Instance.canvasScale;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            prefabs = new List<ZUIPanel>();
            panels = new List<IZUIPanel>();
            history = new List<string>();
        }

        void Start()
        {
            if (panelParent == null)
                panelParent = this.GetComponent<RectTransform>();

            canvasScale = mTransform.localScale;
        }

        void OnDestroy()
        {
            _currentPanel = null;
            panels.Clear();
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {
                    Destroy(gameObject);
                }
            }
            _disposed = true;
        }

        private T GetSleepPanel<T>() where T : ZUIPanel
        {
            for (int i = 0; i < panels.Count;i++)
            {
                if (panels[i].isOpen == false && typeof(T) == panels[i].GetType())
                    return panels[i] as T;
            }
            return null;
        }

        private T GetExistPanel<T>() where T : ZUIPanel
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if (typeof(T) == panels[i].GetType())
                    return panels[i] as T;
            }
            return null;
        }

        public T GetWakenPanel<T>() where T : ZUIPanel
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i].isOpen && (typeof(T) == panels[i].GetType() || panels[i].GetType().IsSubclassOf(typeof(T))))
                {
                    return panels[i] as T;
                }
            }
            return null;
        }

        public void InitPrefabs()
        {
            List<AssetInfo> assets = ZAssetController.Instance.GetAssets(AssetType.Panel);
            InitPrefabs(assets);
        }

        public void InitPrefabs(List<AssetInfo> assets)
        {
            if (assets == null || assets.Count < 1) return;
            prefabs.Clear();
            for (int i = 0; i < assets.Count; i++)
            {
                AssetInfo info = assets[i];
                ZUIPanel prefab = ZAssetController.Instance.GetPrefab<ZUIPanel>(info.asset);
                if (prefab != null)
                {
                    prefab.name = info.name;
                    prefab.grade = info.grade;
                    prefabs.Add(prefab);
                    //CreatePanel<ZUIPanel>(prefab,panelParent);
                }
            }
        }

        public void ShowPanel(string uname,UIParamInfo info)
        {
            if (string.IsNullOrEmpty(uname))
                return;
            IZUIPanel panel = null;
            for (int i = 0;i < panels.Count;i++)
            {
                if(panels[i].definition == uname)
                {
                    panel = panels[i];
                    break;
                }
            }
            if (panel == null)
            {
                panel = CreatePanel(uname, panelParent);
                ActivePanel(panel, info);
            }else
            {
                if (panel.isOpen)
                {
                    RepeatOpenPanel(panel,info);
                }
                else
                {
                    ActivePanel(panel, info);
                }
            }
        }

        public void ShowBackPanel()
        {
            if (_currentPanel != null)
                ClosePanel(_currentPanel);
            string uname = parent;
            if (string.IsNullOrEmpty(uname))
                return;
            history.RemoveAt(history.Count-1);
            IZUIPanel panel = null;
            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i].definition == uname)
                {
                    panel = panels[i];
                    break;
                }
            }
            if (panel != null)
            {
                ZUIPanel tmp = panel as ZUIPanel;
                if (panel.isOpen)
                {
                    panel.Open(tmp.Info);
                }
                else
                {
                    panel.WakenUp(panelParent);
                    StartCoroutine(OpenInspector(tmp, tmp.Info));
                }
                _currentPanel = tmp;
                current = _currentPanel.name;
                ZLog.Warning("ShowBackPanel....."+current);
            }
        }

        public void ShowPanel<T>(UIParamInfo info) where T : ZUIPanel
        {
            if (IsWaken<T>())
            {
                T tmp = GetWakenPanel<T>();
                RepeatOpenPanel(tmp,info);
                return;
            }
            T panel = GetSleepPanel<T>();
            if (panel == null)
                panel = CreatePanel<T>(panelParent);

            ActivePanel(panel, info);
        }

        private void UpdateHistory()
        {
            if (_currentPanel == null)
                return;
            string uname = _currentPanel.name;
            if (_currentPanel.echelon < historyGrade)
            {
                if (history.Count < 1)
                    history.Add(uname);
                else if (history[history.Count - 1] != uname)
                    history.Add(uname);
            }
        }

        /// <summary>
        /// 重复打开
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="param"></param>
        public void RepeatOpenPanel(IZUIPanel panel, UIParamInfo param)
        {
            if (panel == null)
                return;
            UpdateHistory();
            panel.Open(panelParent, param);
            _currentPanel = panel as ZUIPanel;
            current = _currentPanel.name;
         
            ZLog.Warning("panel name = " + current + " is opened!!!");
        }

        private void ActivePanel(IZUIPanel panel, UIParamInfo param)
        {
            if (panel == null) return;
            UpdateHistory();
            panel.WakenUp(panelParent);
            _currentPanel = panel as ZUIPanel;
            current = _currentPanel.name;
            StartCoroutine(OpenInspector(_currentPanel, param));
           
        }

        IEnumerator OpenInspector(ZUIPanel panel, UIParamInfo info)
        {
            while (panel.IsStartEnd == false)
            {
                yield return null;
            }
            panel.Open(info);
        }

        public bool IsWaken<T>() where T : ZUIPanel
        {
            for (int i = 0; i < panels.Count; i++)
            {
                //ZUIPanel panel = panels[i] as ZUIPanel;
                //Debug.Log(panel + "---"+typeof(T) + "---"+panel.GetType());
                if ((typeof(T) == panels[i].GetType() || panels[i].GetType().IsSubclassOf(typeof(T))) && panels[i].isOpen)
                {
                    return true;
                }
            }
            return false;
        }

        private void CreatePanel<T>(T prefab, RectTransform parent) where T : ZUIPanel
        {
            if (prefab != null)
            {
                T panel = Instantiate<T>(prefab);
                if (panel != null)
                {
                    panel.mTransform.SetParent(parent, false);
                    panel.mTransform.localScale = Vector3.one;
                    panel.effectPoint = effectPoint;
                    panel.gameObject.SetActive(false);
                    panels.Add(panel);
                }
                else
                    ZLog.Warning("this is not ZUIPanel of these prefabs that is " + typeof(T) + "!!!");
            }
        }
        
        private T CreatePanel<T>(RectTransform parent)where T:ZUIPanel
        {
            T panel = null;
            for (int i = 0; i < prefabs.Count; i++)
            {
                T prefab = prefabs[i] as T;
                if (prefab != null)
                {
                    panel = Instantiate<T>(prefab);
                    if (panel != null)
                    {
                        panel.name = typeof(T).Name;
                        panel.mTransform.SetParent(parent,false);
                        panel.mTransform.localScale = Vector3.one;
                        panel.effectPoint = effectPoint;
                        panels.Add(panel);
                    }
                    else
                        ZLog.Warning("this is not ZUIPanel of these prefabs that is " + typeof(T) + "!!!");
                    break;
                }

            }
            if (panel == null)
                ZLog.Warning("can not find prefab that name = " + typeof(T));
            return panel;
        }

        private IZUIPanel CreatePanel(string uname, RectTransform parent)
        {
            ZUIPanel panel = null;
            for (int i = 0; i < prefabs.Count; i++)
            {
                ZUIPanel prefab = prefabs[i];
                if (prefab != null)
                {
                    panel = Instantiate(prefab);
                    if (panel != null)
                    {
                        panel.name = prefab.name;
                        panel.mTransform.SetParent(parent, false);
                        panel.mTransform.localScale = Vector3.one;
                        panel.effectPoint = effectPoint;
                        panels.Add(panel);
                    }
                    else
                        ZLog.Warning("this is not ZUIPanel of these prefabs that is " + uname + "!!!");
                    break;
                }

            }
            if (panel == null)
                ZLog.Warning("can not find prefab that name = " + uname);
            return panel;
        }

        private void ClosePanel(IZUIPanel panel)
        {
            if (panel == null) return;
            //if (_currentPanel == (ZUIPanel)panel)
            //    _currentPanel = null;
            panel.Close(mTransform);
        }

        private void CloseWakenPanels()
        {
            UpdateHistory();
            for (int i = 0; i < panels.Count;i++ )
            {
                if(panels[i].isOpen)
                    ClosePanel(panels[i]);
            }
            _currentPanel = null;
           
        }

        private void CloseWakensExcept<T>()
        {
            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i].isOpen && typeof(T) != panels[i].GetType() && !panels[i].GetType().IsSubclassOf(typeof(T)))
                    ClosePanel(panels[i]);
            }
            
            UpdateCurrentPanel();
        }

        private void CloseWakenPanel<T>()where T:ZUIPanel
        {
            T panel = GetWakenPanel<T>();
            if (panel == null)
            {
                ZLog.Warning("can not find the wakend panel that name = " + typeof(T).Name);
                return;
            }
            int len = history.Count;
            
            if(len > 0 && history[len - 1] == panel.name)
                history.RemoveAt(len-1);
            ClosePanel(panel);
            UpdateCurrentPanel();
        }

        private void UpdateCurrentPanel()
        {
            IZUIPanel panel = null;
            for (int i = 0;i < panels.Count;i++)
            {
                IZUIPanel tmp = panels[i];
                if (tmp.isOpen && panel == null)
                    panel = tmp;
                else if (tmp.isOpen && tmp.depth > panel.depth)
                    panel = tmp;
            }
            if (panel != null)
            {
                UpdateHistory();
                _currentPanel = panel as ZUIPanel;
                current = _currentPanel.name;
            }
            else
            {
                current = "";
                _currentPanel = null;
            }
        }
    }
}
