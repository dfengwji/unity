using DG.Tweening;
using ZStart.Common.Controller;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZStart.Common.View.Parts
{
    public class AnimatorItemParts:AppParts,IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        public Animator animator;
        public Text label;
        public int type = 0;
        public float gazeTime = 5f;
        public Vector3 roteOff = new Vector3(0,30f,0);
        public GameObject twinEffect;
        public Animator logoAnimator;
        
        private UnityAction<int> clickCallFun;
        private UnityAction<int> gazeCallFun;
        public Vector3 defaultEuler = Vector3.zero; 
        protected override void Start()
        {
            if (animator != null)
                animator.speed = 0.5f;
            defaultEuler = new Vector3(mTransform.localEulerAngles.x, mTransform.localEulerAngles.y, 0);
        }
       
        public void AddListenser(UnityAction<int> clickFun,UnityAction<int> gazeFun)
        {
            clickCallFun = clickFun;
            gazeCallFun = gazeFun;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (clickCallFun != null)
                clickCallFun.Invoke(type);
        }

        public void UpdateLabel(string tip)
        {
            if (label != null)
                label.text = tip;
        }

        protected override void OnDisable()
        {
            
        }

        public override void Clear()
        {
            StopAllCoroutines();
            twinEffect.SetActive(false);
            logoAnimator.Play("BaseLayer.Open", 0);
            if (animator != null)
                animator.speed = 0.5f;
            mTransform.localEulerAngles = new Vector3(defaultEuler.x, defaultEuler.y, 0);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (animator != null)
                animator.speed = 0.5f;
            twinEffect.SetActive(false);
            logoAnimator.Play("BaseLayer.Open", 0);
            //mTransform.localEulerAngles = new Vector3(defaultEuler.x, defaultEuler.y, 0);
            mTransform.DOLocalRotate(new Vector3(defaultEuler.x, defaultEuler.y, 0), 0.2f);
            StopAllCoroutines();
            AnchorController.Instance.Gaze(0);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (animator != null)
                animator.speed = 1;
            twinEffect.SetActive(true);
            logoAnimator.Play("BaseLayer.Close", 0);
            //mTransform.localEulerAngles = new Vector3(defaultEuler.x + roteOff.x, defaultEuler.y + roteOff.y, defaultEuler.z + roteOff.z);
            mTransform.DOLocalRotate(new Vector3(defaultEuler.x + roteOff.x, defaultEuler.y + roteOff.y, defaultEuler.z + roteOff.z), 0.2f);
            if (gazeTime > 0.01f)
            {
                AnchorController.Instance.Gaze(gazeTime);
                StartCoroutine(GazeInspector());
            }
        }

        IEnumerator GazeInspector()
        {
            yield return new WaitForSeconds(gazeTime);
            if (gazeCallFun != null)
                gazeCallFun.Invoke(type);
        }
    }
}
