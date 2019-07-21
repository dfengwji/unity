using UnityEngine;
using System.Collections;
using ZStart.Core.Controller;
using ZStart.Core.Enum;

namespace ZStart.Core
{
    public class ZEffectBase : ZBehaviourBase
    {
        public string label = "";
        public float lifeTime = 2.0f;
        public bool isLoop = false;
        public EffectType type = EffectType.Scene;
        public long id = 0;
        public bool isDestroy = false;
        public bool isStartPlay = false;

        public float passTime = 0f;
        public bool isPause = false;
        public ParticleSystem[] particleEffects;
        //BaseSpeciallyEffect[] specialEffects;
        public Animator animator;
        void Start()
        {
            if(particleEffects == null || particleEffects.Length < 1)
                particleEffects = this.GetComponentsInChildren<ParticleSystem>();
            //specialEffects = this.GetComponentsInChildren<BaseSpeciallyEffect>();
            animator = GetComponent<Animator>();
            if (isStartPlay)
                Play();
            isStartEnd = true;
        }

        IEnumerator DelayDestroy()
        {
            while (passTime < lifeTime)
            {
                passTime += Time.deltaTime;
                yield return null;
            }
            Dispose(true);

        }

        IEnumerator DelayPlay()
        {
            while (isStartEnd == false)
            {
                yield return null;
            }
            ActivePlay();
        }

        private void ActivePlay()
        {
            if (gameObject.activeInHierarchy == false)
                gameObject.SetActive(true);
            if (animator)
                animator.Play("BaseLayer.Idle",0);
            for (int i = 0; i < particleEffects.Length; i++)
            {
                //particleEffects[i].enableEmission = true;
                particleEffects[i].Play();
            }
            //for (int i = 0; i < specialEffects.Length; i++)
            //{
            //    specialEffects[i].Play();
            //}
            StopAllCoroutines();

            if (lifeTime > 0 && isLoop == false)
                StartCoroutine(DelayDestroy());
        }

        public void Pause()
        {
            if (IsStartEnd == false) return;
            if (isPause) return;
            isPause = true;
            for (int i = 0; i < particleEffects.Length; i++)
            {
                particleEffects[i].Pause();
            }
            StopAllCoroutines();
        }

        public void Play()
        {
            isPause = false;
            if (isStartEnd)
            {
                ActivePlay();
            }
            else
            {
                StartCoroutine(DelayPlay());
            }
        }

        public void Play(float life)
        {
            lifeTime = life;
            Play();
        }

        public void DeActive()
        {
            Clear();
            this.gameObject.SetActive(false);
        }

        public void Clear()
        {
            for (int i = 0; i < particleEffects.Length; i++)
            {
                particleEffects[i].Clear();
            }

            //for (int i = 0; i < specialEffects.Length; i++)
            //{
            //    specialEffects[i].Stop();
            //}
        }

        public virtual void Stop() { }

        private void Remove()
        {
            StopAllCoroutines();
            if (isDestroy)
            {
                Destroy(this.gameObject);
            }
            else
            {
                if (particleEffects != null && particleEffects.Length > 0)
                {
                    //for (int i = 0; i < particleEffects.Length; i++)
                    //{
                    //    particleEffects[i].enableEmission = false;
                    //}
                    if (gameObject.activeSelf)
                    {
                        if (isLoop)
                        {
                            if (lifeTime > 0)
                                StartCoroutine(DeActiveDelay(lifeTime));
                            else
                                ZAssetController.Instance.DeActivateEffect(mTransform);
                        }
                        else
                            ZAssetController.Instance.DeActivateEffect(mTransform);
                    }
                }
                else
                {
                    ZAssetController.Instance.DeActivateEffect(mTransform);
                }
                //if (id > 0)
                //{
                    
                //}
                //else
                //{
                //    this.gameObject.SetActive(false);
                //}
            }
        }

        public void DisposeImmediately()
        {
            passTime = 0f;
            StopAllCoroutines();
            if (isDestroy)
            {
                Destroy(this.gameObject);
            }
            else
            {
                if (particleEffects != null && particleEffects.Length > 0)
                {
                    //for (int i = 0; i < particleEffects.Length; i++)
                    //{
                    //    particleEffects[i].enableEmission = false;
                    //}
                    ZAssetController.Instance.DeActivateEffect(mTransform);
                }
                else
                {
                    ZAssetController.Instance.DeActivateEffect(mTransform);
                }
            }
        }

        IEnumerator DeActiveDelay(float time)
        {
            yield return new WaitForSeconds(time);
            ZAssetController.Instance.DeActivateEffect(mTransform);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;
            passTime = 0f;
            Remove();
        }
    }
}
