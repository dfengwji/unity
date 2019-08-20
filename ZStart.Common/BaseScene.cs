using UnityEngine;

namespace ZStart.Common.Level
{
    public abstract class BaseScene : Core.ZBehaviourBase
    {
        public Material skybox;
        public Camera sceneCamera;
        public string uname = "";
        public void ShowSky()
        {
            RenderSettings.skybox = skybox;
        }

        public virtual void Show()
        {
            //RenderSettings.skybox = skybox;
            gameObject.SetActive(true);
        }

        public virtual void UnShow()
        {
            //RenderSettings.skybox = null;
            gameObject.SetActive(false);
        }

    }
}
