using ZStart.Core;
namespace ZStart.Common.View
{
    public abstract class AppUIGroup:ZUIBehaviour
    {
        public virtual bool isOpened
        {
            get
            {
                return isActiveAndEnabled;
            }
        }

        protected override void Start()
        {
            
        }

        public virtual void Init() { }

        public virtual void AddListeners()
        {

        }

        public virtual void RemoveListeners()
        {

        }

        public virtual void Appear()
        {
            if (gameObject.activeInHierarchy == false)
                gameObject.SetActive(true);
        }
        public virtual void Disappear()
        {
            Clear();
            if (gameObject.activeInHierarchy)
                gameObject.SetActive(false);
        }
    }
}
