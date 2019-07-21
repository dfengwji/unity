﻿using System.Collections;
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
        public Transform loadingObj;
        public string currentScene = "";

        public BaseLevel loadedLevel;
        public float roateSpeed = 300f;
        private bool showLoad = false;
        public bool ShowLoading{
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

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += SceneLoaded;
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

        void UpdateAction()
        {
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
            ShowLoading = true;
            AsyncOperation async = SceneManager.LoadSceneAsync(launcherScene);
            async.allowSceneActivation = false;
            while (!async.isDone)
            {
                if (async.progress > 0.8999f)
                {
                    async.allowSceneActivation = true;
                }
                if (loadingObj != null)
                    loadingObj.Rotate(Vector3.back, Time.deltaTime * roateSpeed);
                yield return null;
            }
            yield return null;
            loadedLevel = FindObjectOfType<BaseLevel>();
            yield return new WaitForSeconds(0.2f);
            loadedLevel.ShowSky();

            ShowLoading = false;
            ZLog.Log("LevelController... InitInspector.....complete");
            NotifyManager.SendNotify(Enum.NotifyType.OnSceneChanged, currentScene);
            SwitchLevel("");
        }

        IEnumerator LoadInspector(string scene,string flag)
        {
            ZLog.Log("switch scene!!!!from = " + currentScene + " to " + scene + " and flag = " + flag);
            if (currentScene == scene)
                yield break;
           
            yield return new WaitForSeconds(0.5f);
            RenderSettings.skybox = null;
            ShowLoading = true;
            yield return new WaitForSeconds(0.1f);
           
            AsyncOperation async = SceneManager.LoadSceneAsync(scene,LoadSceneMode.Single);
            async.allowSceneActivation = false;
            while (!async.isDone)
            {
                if (async.progress > 0.8999f)
                {
                    async.allowSceneActivation = true;
                }
                if (loadingObj != null)
                    loadingObj.Rotate(Vector3.back, Time.deltaTime * roateSpeed);
                yield return null;
            }
          
            yield return null;
            ShowLoading = false;
            loadedLevel = GameObject.FindObjectOfType<BaseLevel>();
            NotifyManager.SendNotify(Enum.NotifyType.OnSceneChanged, currentScene);
            yield return new WaitForSeconds(0.2f);
            loadedLevel.ShowSky();
            yield return null;
        }

        public void InitScene()
        {
            StartCoroutine(InitInspector());
        }

        public void SwitchLevel(string path)
        {
            ZLog.Log("LevelController switch level = " + path);
           
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
            ZLog.Log("LevelController switch level = " + stage);
           
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
                ShowLoading = true;
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
