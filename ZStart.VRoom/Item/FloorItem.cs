using System.Collections;
using UnityEngine;
using ZStart.VRoom.Controller;

namespace ZStart.VRoom.Item
{
    public class FloorItem:InteractiveItem
    {
        public AudioSource audioSource;
       
        public MoveController moveCtrl;
        public Transform tipsFooter;
      
        public float time = 0.02f;
        public Animator holeAnimator;
        public Animator tipAnimator;
        protected override void Start()
        {
            gameObject.tag = "floor";
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        public override void OnGazeEnter()
        {
            tipAnimator.Play("n");
        }

        public override void OnGazeActive()
        {
            moveCtrl.SetTarget(tipsFooter);
            StartCoroutine(fadeFX());
        }

        public override void OnGazeOut()
        {
            moveCtrl.SetTarget(null);
            tipAnimator.Play("b");
        }

        IEnumerator fadeFX()
        {
            holeAnimator.Play("b");
            audioSource.Play();
            yield return new WaitForSeconds(time);
            holeAnimator.Play("n");

        }
    }
}
