using ZStart.Core;
using ZStart.Core.Util;
using System.Collections;
using UnityEngine;

namespace ZStart.Common.Controller
{
    public class WebCameraController : ZSingletonBehaviour<WebCameraController>
    {
        public Vector2 Size = new Vector2(800, 600);
        public float FPS = 0;

        //接收返回的图片数据  
        WebCamTexture webCamera;
        public Camera vcamera;
        public Renderer render;//作为显示摄像头的面板

        void Start()
        {
           
        }

        /// <summary>  
        /// 初始化摄像头
        /// </summary>  
        public IEnumerator InitInspector()
        {
            ZLog.Log("InitInspector web camera!!!! ");
            render.enabled = true;
            vcamera.enabled = true;
            yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            if (Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                InitCamera();
                yield break;
            }
            else
            {
                render.enabled = false;
                vcamera.enabled = false;
            }
        }

        private void InitCamera()
        {
            ZLog.Log("init web camera!!!! ");
            WebCamDevice[] devices = WebCamTexture.devices;
            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].isFrontFacing)
                {
                    string device = devices[i].name;
                    webCamera = new WebCamTexture(device, (int)Size.x, (int)Size.y, (int)FPS);

                    render.material.mainTexture = webCamera;
                    render.enabled = true;
                    vcamera.enabled = true;
                    webCamera.Play();
                    ZLog.Log("active web camera that name = " + device);
                    break;
                }
            }
        }

        public void Play()
        {
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                StartCoroutine(InitInspector());
            }
            else
            {
                if (webCamera != null)
                {
                    vcamera.enabled = true;
                    render.enabled = true;
                    webCamera.Play();
                }
                else
                {
                    InitCamera();
                }
            }
        }


        public void Stop()
        {
            vcamera.enabled = false;
            render.enabled = false;
            if(webCamera != null)
                webCamera.Stop();
        }

      
    }
}
