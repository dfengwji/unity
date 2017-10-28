using UnityEngine;

namespace ZStart.Core
{
    public abstract class ZAnimationBase:ZBehaviourBase
    {
        public Transform target;
        public AnimationClip[] aniClips;
        protected Animation animationObj;

        protected virtual void Start()
        {
            
            if (aniClips != null && aniClips.Length > 0)
            {
                animationObj = target.gameObject.AddComponent<Animation>();
            }

            _isStartEnd = true;
        }

        public virtual void Play() { }

        public virtual void Pause() { }

        public virtual void Stop() { }

        public virtual void RePlay() { }
    }
}
