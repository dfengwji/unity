using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZStart.Core.Controller;

namespace ZStart.RGraph.View.Item
{
    public class MouldItem : Core.ZBehaviourBase
    {
        public MeshRenderer body;
        public List<MeshRenderer> children;
       
        public Shader dissloveShader;
        public Shader highShader;
        public bool isDisslove = false;

        private void Awake()
        {
            children = new List<MeshRenderer>(5);
        }

        private void Start()
        {
            InitTargets();
        }

        private void InitTargets()
        {
            if (body != null)
                return;
            for (int i = 0; i < mTransform.childCount; i += 1)
            {
                Transform child = mTransform.GetChild(i);
                if (child.name == "Body")
                {
                    body = child.GetComponent<MeshRenderer>();
                }
                else if (child.name == "Head" || child.name.Contains("Child"))
                {
                    var tmp = child.GetComponent<MeshRenderer>();
                    children.Add(tmp);
                }
                else
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        public void ResetState(float delay)
        {
           
        }

        public void Show(float time)
        {
            InitTargets();
            UpdateShader(body, true);
            if (children.Count == 1)
            {
                UpdateShader(children[0], true);
                children[0].material.SetFloat("_Disslove", 0f);
            }
            body.material.SetFloat("_Disslove", 0f);
            FadeMesh(body, 5f, time, BodyShowComplete);
        }

        private void UpdateShader(MeshRenderer renderer, bool disslove)
        {
            if (renderer == null)
                return;
            isDisslove = disslove;
            if (disslove)
            {
                Texture tex = renderer.material.mainTexture;
                renderer.material.shader = dissloveShader;
                renderer.material.mainTexture = tex;
                renderer.material.SetColor("_BaseColor", Color.white);
            }
            else
            {
                Texture tex = renderer.material.mainTexture;
                renderer.material.shader = highShader;
                renderer.material.SetTexture("_Diffuse", tex);
                renderer.material.SetFloat("_Width", 5f);
                renderer.material.SetFloat("_Speed", 2f);
                renderer.material.SetColor("_Color", Color.white);
                renderer.material.SetFloat("_Smoothness", 0.2f);
            }
        }

        public void SwitchHighlight(bool on)
        {
            if (isDisslove)
                return;
            if (children.Count == 1)
            {
                children[0].material.SetFloat("_TurnOnHighLight", on ? 1f : 0f);
            }
            if (body)
            {
                body.material.SetFloat("_TurnOnHighLight", on ? 1f : 0f);
            }
        }

        public void OpenOrClose(bool open)
        {
            if (children.Count < 1)
                return;
            if (open)
            {
                if (children.Count == 1)
                {
                    children[0].GetComponent<Transform>().DOLocalMove(new Vector3(0, 2, 0), 1f);
                }
                else
                {

                }
            }
            else
            {
                if (children.Count == 1)
                {
                    children[0].GetComponent<Transform>().DOLocalMove(new Vector3(0, 0, 0), 1f);
                }
                else
                {

                }
            }
        }

        private void BodyShowComplete()
        {
            MeshRenderer header = null;
            if (children.Count == 1)
                header = children[0];
            if (header != null)
            {
                FadeMesh(header, 5f, 1.5f, null);
                StartCoroutine(ScanLightDelay(1.5f));
            }
            else
            {
                UpdateShader(header, false);
                UpdateShader(body, false);
                SwitchHighlight(true);
            }
        }

        IEnumerator ScanLightDelay(float time)
        {
            yield return new WaitForSeconds(time);
            if (children.Count == 1)
                UpdateShader(children[0], false);
            UpdateShader(body, false);
            SwitchHighlight(true);
        }

        private void FadeMesh(MeshRenderer target, float amount, float time, TweenCallback action)
        {
            if (target == null)
                return;
            target.material.DOFloat(amount, "_Disslove", time).SetDelay(0.1f).OnComplete(() => {
                if (action != null)
                    action.Invoke();
            });
        }
        
        public void Hide(float time)
        {
            UpdateShader(body, true);

            FadeMesh(body, 0, time, BodyHideComplete);
            if (children.Count == 1)
            {
                UpdateShader(children[0], true);
                FadeMesh(children[0], 0, time, null);
            }
        }

        private void BodyHideComplete()
        {
            ZAssetController.Instance.DeActivateAsset(mTransform);
        }
    }
}
