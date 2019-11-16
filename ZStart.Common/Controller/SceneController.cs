using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZStart.Common.Level;
using ZStart.Common.Manager;
using ZStart.Core;
using ZStart.Core.Controller;
using ZStart.Core.Manager;

namespace ZStart.Common.Controller
{
    public class SceneController : ZSingletonBehaviour<SceneController>
    {
        public string defaultScene = "LauncherStage";
        public Camera[] cameraEyes;
        public float maxShowTime = 1.0f;
        public Transform loadingObj;
        public string currentScene = "";

        public List<BaseScene> loadedScenes;
        public float roateSpeed = 300f;
        private bool showLoad = false;
        public bool ShowLoading{
            set{
                if(loadingObj != null)
                    loadingObj.gameObject.SetActive(value);
                showLoad = value;
             
                ShowScene(!value);
            }
            get
            {
                return showLoad;
            }
        }

        public float Progress
        {
            get
            {
                if (asyncOperation != null)
                    return asyncOperation.progress;
                else
                    return -1f;
            }
        }

        private AsyncOperation asyncOperation;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneLoaded;
            SceneManager.sceneUnloaded += SceneUnloaded;
            loadedScenes = new List<BaseScene>();
        }

        void Start()
        {
            ShowLoading = true;
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

        private void SceneLoaded(Scene scene,LoadSceneMode mode)
        {
            ZLog.Log("SceneController...the scene had loaded that name = " + scene.name + ";mode = " + mode);
            currentScene = scene.name;
            AddOne(scene);
            NotifyManager.SendNotify(Enum.NotifyType.OnSceneLoaded, currentScene);
        }

        private void SceneUnloaded(Scene scene)
        {
            ZLog.Warning("SceneController...the scene had unloaded that name = " + scene.name);
            Remove(scene.name);
            NotifyManager.SendNotify(Enum.NotifyType.OnSceneUnloaded, scene.name);
            if (loadedScenes.Count > 0)
            {
                currentScene = loadedScenes[loadedScenes.Count - 1].name;
            }
            else
            {
                currentScene = "";
            }
        }

        private void AddOne(Scene scene)
        {
            if (scene == null)
                return;
            if (HadOne(scene.name))
                return;
            GameObject[] array = scene.GetRootGameObjects();
            bool had = false;
            for (int i = 0; i < array.Length; i += 1)
            {
                var tmp = array[i].GetComponent<BaseScene>();
                if (tmp != null)
                {
                    tmp.uname = scene.name;
                    loadedScenes.Add(tmp);
                    had = true;
                    break;
                }
            }
            if (!had && array.Length > 0)
            {
                var tmp = array[0].AddComponent<DefaultScene>();
                if(tmp != null)
                {
                    tmp.uname = scene.name;
                    loadedScenes.Add(tmp);
                }
            }
        }

        private void Remove(string uname)
        {
            for (int i = 0; i < loadedScenes.Count; i += 1)
            {
                if (loadedScenes[i].uname == uname)
                {
                    loadedScenes.RemoveAt(i);
                    break;
                }
            }
        }

        public BaseScene Get(string uname)
        {
            for (int i = 0; i < loadedScenes.Count; i += 1)
            {
                if (loadedScenes[i].uname == uname)
                    return loadedScenes[i];
            }
            return null;
        }

        public bool HadOne(string uname)
        {
            for (int i = 0; i < loadedScenes.Count; i += 1)
            {
                if (loadedScenes[i].uname == uname)
                    return true;
            }
            return false;
        }

        public void Show()
        {
            BaseScene scene = Get(currentScene);
            if (scene != null)
            {
                scene.Show();
            }
        }

        public void Hide()
        {
            BaseScene scene = Get(currentScene);
            if (scene != null)
            {
                scene.UnShow();
            }
        }

        public void AllowActivation()
        {
            if (asyncOperation != null)
            {
                asyncOperation.allowSceneActivation = true;
                asyncOperation = null;
            }
        }

        private void Update()
        {
            if (asyncOperation != null)
            {
                if (loadingObj != null)
                    loadingObj.Rotate(Vector3.back, Time.deltaTime * roateSpeed);
            }
        }

        IEnumerator InitInspector()
        {
            ShowLoading = true;
            yield return new WaitForEndOfFrame();
            asyncOperation = SceneManager.LoadSceneAsync(defaultScene);
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                if (asyncOperation.progress >0.899f)
                {
                    asyncOperation.allowSceneActivation = true;
                }
                yield return null;
            }
            ShowLoading = false;
            ZLog.Log("SceneController... InitInspector.....complete");
            //Switch("");
        }

        IEnumerator LoadInspector(string scene,string flag, LoadSceneMode mode, bool autoAct)
        {
            ZLog.Log("SceneController...load scene!!!!from = " + currentScene + " to " + scene + " and flag = " + flag);
            if (currentScene == scene)
                yield break;
            ShowLoading = true;
            RenderSettings.skybox = null;
            yield return new WaitForEndOfFrame();
            asyncOperation = SceneManager.LoadSceneAsync(scene, mode);
            asyncOperation.allowSceneActivation = false;
            if (autoAct)
            {
                while (!asyncOperation.isDone)
                {
                    if (asyncOperation.progress > 0.899f)
                    {
                        asyncOperation.allowSceneActivation = true;
                    }
                    yield return null;
                }
            }
            else
            {
                yield return asyncOperation;
            }
           
            ShowLoading = false;
        }

        IEnumerator UnLoadInspector(string scene)
        {
            ZLog.Log("SceneController...try unload scene that name = " + scene);
            ShowLoading = true;
            RenderSettings.skybox = null;
            yield return new WaitForEndOfFrame();
            asyncOperation = SceneManager.UnloadSceneAsync(scene);
            asyncOperation.allowSceneActivation = false;
            yield return asyncOperation;
            //while (!async.isDone)
            //{
            //    progress = async.progress;
            //    if (loadingObj != null)
            //        loadingObj.Rotate(Vector3.back, Time.deltaTime * roateSpeed);
            //    yield return null;
            //}
            yield return null;
            ShowLoading = false;
        }
        
        public void Init()
        {
            StartCoroutine(InitInspector());
        }

        public void Unload(string uname)
        {
            if (!HadOne(uname))
                return;
            StartCoroutine(UnLoadInspector(uname));
        }

        public void Switch(string path, LoadSceneMode mode = LoadSceneMode.Single, bool act = false)
        {
            ZLog.Log("SceneController...try switch scene = " + path);
           
            if (string.IsNullOrEmpty(path))
            {
                StartCoroutine(LoadInspector(defaultScene,"", mode, act));
            }
            else
            {
                AssetBundle bundle = ZBundleManager.Instance.GetBundle(path);
                if (bundle != null)
                {
                    TryLoad(bundle, mode, act);
                }
                else
                    ZBundleController.Instance.Load(path, Core.Enum.BundleType.Scene, BundleCompleteHandle);
            }
        }

        public void Load(string stage, LoadSceneMode mode = LoadSceneMode.Single, bool act = false)
        {
            ZLog.Log("SceneController...try load scene = " + stage);
           
            if (string.IsNullOrEmpty(stage))
            {
                StartCoroutine(LoadInspector(defaultScene, "", mode, act));
            }
            else
            {
                StartCoroutine(LoadInspector(stage, "", mode, act));
            }
        }

        private void BundleCompleteHandle(string uid, bool success)
        {
            if (success)
            {
                AssetBundle bundle = ZBundleManager.Instance.GetBundle(uid);
                if (bundle == null || bundle.GetAllScenePaths() == null || bundle.GetAllScenePaths().Length < 1)
                    return;
                ShowLoading = true;
                TryLoad(bundle, LoadSceneMode.Additive, true);
            }
            else
            {
                ZLog.Warning("SceneController...can not load bundle!!! try check path = " + uid + " is valid!!");
            }
        }

        private void TryLoad(AssetBundle bundle, LoadSceneMode mode, bool act)
        {
            string scenePath = bundle.GetAllScenePaths()[0];
            string[] array = scenePath.Split('/');
            string scene = array[array.Length - 1].Replace(".unity", "");
            ZLog.Log("SceneController...load scene inspector....scene = " + scene);
            StartCoroutine(LoadInspector(scene, "", mode, act));
        }
    }
}
