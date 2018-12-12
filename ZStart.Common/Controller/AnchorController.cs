using ZStart.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ZStart.Common.Controller
{
    public class AnchorController : ZSingletonBehaviour<AnchorController>
    {
        public GameObject anchorGaze;
        public Image anchorRing;
        
        void Start()
        {
            Hide();
        }

        public void Show()
        {
            
        }

        public void Hide()
        {
           
        }

        public void Gaze(float time)
        {
            if (time < 0.01f)
            {
                anchorGaze.SetActive(false);
                anchorRing.fillAmount = 0;
                StopAllCoroutines();
                return;
            }
            StartCoroutine(GazeInspector(time));

        }

        IEnumerator GazeInspector(float time)
        {
            anchorGaze.SetActive(true);
            anchorRing.fillAmount = 0;
            float start = 0f;
            while (start < time)
            {
                start += Time.deltaTime;
                anchorRing.fillAmount = Mathf.Lerp(0f, 1f, start / time);
                yield return null;
            }
            anchorGaze.SetActive(false);
            anchorRing.fillAmount = 0;
        }
    }
}
