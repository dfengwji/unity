using UnityEngine;
namespace ZStart.Common
{
    public abstract class BaseEffect : Core.ZBehaviourBase
    {
        public ParticleSystem[] particles;

        protected virtual void Start()
        {
            particles = GetComponentsInChildren<ParticleSystem>();
        }

        public virtual void Init()
        {
            
        }

        public virtual void Play()
        {
            if (particles == null)
                return;
            for (int i = 0; i < particles.Length;i++ )
            {
                particles[i].Play();
            }
        }

        public virtual void Stop()
        {
            if (particles == null)
                return;
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i].Stop();
            }
        }

        public virtual void Clear()
        {

        }
    }
}
