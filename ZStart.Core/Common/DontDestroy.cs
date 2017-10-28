namespace ZStart.Core.Common
{
    public class DontDestroy:ZBehaviourBase
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
