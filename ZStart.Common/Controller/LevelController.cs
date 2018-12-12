using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZStart.Common.Level;
using ZStart.Common.Manager;
using ZStart.Core;
using ZStart.Core.Controller;
using ZStart.Core.Manager;

namespace ZStart.Common.Controller
{
    public class LevelController : ZSingletonBehaviour<LevelController>
    {
        public string launcherScene = "LauncherStage";
        public Camera[] cameraEyes;
        public float maxShowTime = 1.0f;
        public GameObject guideModule;
        public Transform loadingObj;
        public string currentScene = "";

        public BaseLevel loadedLevel;
        public bool showGuide = false;
        public bool IsLoading { get; private set; } = false;

        private bool showLoad = false;
        public bool showLoading{
            set{
                loadingObj.gameObject.SetActive(value);
                showLoad = value;
             
                ShowScene(!value);
            }
            get
            {
                return showLoad;
            }
        }

        private bool isInit = false;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneLoaded;
        }

        void Start()
        {
            showLoading = true;
            CallbackController.Instance.RegisterUpdateEvent("LevelController-"+GetInstanceID(),UpdateAction);
        }

        private void ShowScene(bool show)
        {
            int layer = 1 << 0;
            if (show)
            {
                for (int i = 0; i < cameraEyes.Length; i++)
                {
                    cameraEyes[i].cullingMask = -1;
                }
            }
            else
            {
                for (int i = 0; i < cameraEyes.Length; i++)
                {
                    cameraEyes[i].cullingMask &= ~layer;
                }
            }
        }

        //void OnApplicationPause(bool isPause)
        //{
        //    if (isPause == false && isInit)
        //    {
        //        string path = AppIOManager.Instance.GetTheme();
        //        SwitchLevel(path);
        //    }
        //}

        void UpdateAction()
        {
            if (showLoad && loadingObj != null)
                loadingObj.Rotate(Vector3.back, Time.deltaTime * 300);
#if UNITY_EDITOR
            if(Input.GetKeyDown(KeyCode.Alpha0)){
                SwitchLevel("");
            }else if(Input.GetKeyDown(KeyCode.Alpha1)){
                SwitchLevel(Application.streamingAssetsPath + "/screen80s.hd");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwitchLevel(Application.streamingAssetsPath + "/daylight-1.hd");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SwitchLevel(Application.streamingAssetsPath + "/daylight-2.hd");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                SwitchLevel(Application.streamingAssetsPath + "/daylight-3.hd");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                SwitchLevel(Application.streamingAssetsPath + "/daylight-3.hd");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                SwitchLevel(Application.streamingAssetsPath + "/daylight-5.hd");
            }
#endif
        }

        private void SceneLoaded(Scene scene,LoadSceneMode mode)
        {
            ZLog.Log("SceneLoaded....." + scene.name + ";mode = " + mode);
            currentScene = scene.name;
            if(loadedLevel == null)
                loadedLevel = FindObjectOfType<BaseLevel>();
        }

        public void Show()
        {
            if (loadedLevel != null)
            {
                loadedLevel.Show();
            }
        }

        public void Hide()
        {
            if (loadedLevel != null)
            {
                loadedLevel.UnShow();
            }
        }

        IEnumerator InitInspector()
        {
            showLoading = true;
            IsLoading = true;

            AsyncOperation async = SceneManager.LoadSceneAsync(launcherScene);
            while (!async.isDone)
            {
                yield return null;
            }
            async.allowSceneActivation = true;
            yield return null;
            loadedLevel = FindObjectOfType<BaseLevel>();
            yield return new WaitForSeconds(0.2f);
            loadedLevel.ShowSky();

            showLoading = false;
            Debug.LogWarning("InitInspector. guideModule == " + guideModule);
            if (showGuide)
            {
                if (guideModule == null)
                {
                    NotifyManager.SendNotify(Enum.NotifyType.OnGuideComplete, null);
                }
                else
                    guideModule.SetActive(true);
            }
            else
            {
                if (guideModule != null)
                    guideModule.SetActive(false);
                NotifyManager.SendNotify(Enum.NotifyType.OnGuideComplete, null);
            }
            IsLoading = false;
            yield return null;
            ZLog.Log("LevelController... InitInspector.....");
            SwitchLevel("");
        }

        IEnumerator LoadInspector(string scene,string flag)
        {
            ZLog.Log("switch scene!!!!from = " + currentScene + " to " + scene + " and flag = " + flag);
            if (currentScene == scene)
                yield break;
            IsLoading = true;
            yield return new WaitForSeconds(0.5f);
            RenderSettings.skybox = null;
            showLoading = true;
            yield return new WaitForSeconds(0.1f);
           
            AsyncOperation async = SceneManager.LoadSceneAsync(scene,LoadSceneMode.Single);
            while (!async.isDone)
            {
                yield return null;
            }
            async.allowSceneActivation = true;
            yield return null;
            showLoading = false;
            loadedLevel = GameObject.FindObjectOfType<BaseLevel>();
            NotifyManager.SendNotify(Enum.NotifyType.OnSceneChanged, currentScene);
            yield return new WaitForSeconds(0.2f);
            loadedLevel.ShowSky();
            yield return null;
            IsLoading = false;
        }

        public void InitScene()
        {
            isInit = true;
            StartCoroutine(InitInspector());
        }

        public void SwitchLevel(string path)
        {
            ZLog.Log("LevelController switch level = " + path+";loading = "+IsLoading);
            if (IsLoading)
                return;
            if (string.IsNullOrEmpty(path))
            {
                StartCoroutine(LoadInspector(launcherScene,""));
            }
            else
            {
                AssetBundle bundle = ZBundleManager.Instance.GetBundle(path);
                if (bundle != null)
                {
                    TryLoadScene(bundle);
                }
                else
                    ZBundleController.Instance.LoadByPath(path, Core.Enum.BundleType.Scene, BundleCompleteHandle);
            }
        }

        public void LoadLevel(string stage)
        {
            ZLog.Log("LevelController switch level = " + stage + ";loading = " + IsLoading);
            if (IsLoading)
                return;
            if (string.IsNullOrEmpty(stage))
            {
                StartCoroutine(LoadInspector(launcherScene, ""));
            }
            else
            {
                StartCoroutine(LoadInspector(stage, ""));
            }
        }

        private void BundleCompleteHandle(string uid, bool success)
        {
            if (success)
            {
                AssetBundle bundle = ZBundleManager.Instance.GetBundle(uid);
                if (bundle == null || bundle.GetAllScenePaths() == null || bundle.GetAllScenePaths().Length < 1)
                    return;
                showLoading = true;
                TryLoadScene(bundle);
            }
            else
            {
                ZLog.Warning("can not load bundle!!! try check path = " + uid + " is valid!!");
            }
        }

        private void TryLoadScene(AssetBundle bundle)
        {
            string scenePath = bundle.GetAllScenePaths()[0];
            string[] array = scenePath.Split('/');
            string scene = array[array.Length - 1].Replace(".unity", "");
            ZLog.Log("load level inspector....scene = " + scene);
            StartCoroutine(LoadInspector(scene, ""));
        }
    }
}
