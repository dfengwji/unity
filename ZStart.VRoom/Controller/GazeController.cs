using UnityEngine;
using UnityEngine.UI;
using ZStart.VRoom.Item;

namespace ZStart.VRoom.Controller
{
    public class GazeController: MonoBehaviour
    {
        public Transform headerTarget;
        public RectTransform canvasTarget;
        public Transform tipsFooter;
        public Image reticleImage;
        public float distance = 0f;
        public RaycastHit hit;

        private GameObject nowLookObj;
        private InteractiveItem[] lookingItems;

        public float durationTime = 2.0f;
        public float surfaceOffset = 0.01f;
        private float countTime;
        private bool isActived = false;
        private Vector3 canvasOriginalPosition;
        private Vector3 canvasOriginalRotation;
        private Vector3 canvasOriginalScale;

        void Start()
        {
            canvasOriginalPosition = new Vector3(0, 0, distance);
            canvasOriginalRotation = canvasTarget.transform.eulerAngles;
            canvasOriginalScale = new Vector3(0.02f, 0.02f, 0.02f);
            reticleImage.fillAmount = 0;
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = new Ray(headerTarget.position, headerTarget.forward);
            if (Physics.Raycast(ray, out hit, 15))
            {
                canvasTarget.localScale = hit.distance * canvasOriginalScale * 0.05f;
                canvasTarget.position = hit.point;
                canvasTarget.forward = hit.normal;
                if (hit.transform.gameObject != nowLookObj)
                {
                    if (nowLookObj != null)
                    {
                        var items = nowLookObj.GetComponents<InteractiveItem>();
                        if (items != null)
                        {
                            for (int i = 0;i < items.Length;i += 1)
                            {
                                items[i].OnGazeOut();
                            }
                        }
                    }
                    isActived = false;
                    nowLookObj = hit.transform.gameObject;
                    lookingItems = nowLookObj.GetComponents<InteractiveItem>();
                    if (lookingItems != null)
                    {
                        for (int i = 0; i < lookingItems.Length; i += 1)
                        {
                            lookingItems[i].OnGazeEnter();
                        }
                    }
                }
                else
                {
                    countTime += Time.deltaTime;
                    //if (lookingItem)
                    //{
                    //    lookingItem.OnGazeHover();
                    //}
                    if (countTime < durationTime)
                    {
                        //如果用户提前按下按钮
                        if (Input.GetKeyDown(KeyCode.Joystick1Button0))
                        {
                            reticleImage.fillAmount = 0.0f;
                            countTime = 0;
                            if (lookingItems != null && !isActived)
                            {
                                isActived = true;
                                for (int i = 0; i < lookingItems.Length; i += 1)
                                {
                                    lookingItems[i].OnGazeActive();
                                }
                            }
                        }
                        reticleImage.fillAmount = countTime / durationTime;
                    }
                    else
                    {
                        reticleImage.fillAmount = 0.0f;
                        countTime = 0;
                        if (lookingItems != null && !isActived)
                        {
                            isActived = true;
                            for (int i = 0; i < lookingItems.Length; i += 1)
                            {
                                lookingItems[i].OnGazeActive();
                            }
                        }
                    }
                }
            }
            else
            {
                canvasTarget.localPosition = canvasOriginalPosition;
                canvasTarget.localEulerAngles = Vector3.zero;
                canvasTarget.localScale = canvasOriginalScale;
                countTime = 0;
                reticleImage.fillAmount = 0;
                if (lookingItems != null)
                {
                    for (int i = 0; i < lookingItems.Length; i += 1)
                    {
                        lookingItems[i].OnGazeOut();
                    }
                    isActived = false;
                    nowLookObj = null;
                    lookingItems = null;
                }
            }

            if (hit.transform != null && hit.transform.tag == "floor")
            {
                var pos = hit.point + hit.normal * surfaceOffset;
                tipsFooter.position = pos;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(headerTarget.position, "AutoMan.png");
        }
    }
}
