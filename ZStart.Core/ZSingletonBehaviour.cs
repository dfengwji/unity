namespace ZStart.Core
{
    public abstract class ZSingletonBehaviour<T> : ZBehaviourBase where T : ZBehaviourBase
    {
        private static T _instance;
      
        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            _instance = (ZBehaviourBase)this as T;
        }
    }
}
