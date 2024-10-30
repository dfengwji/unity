using System;

namespace ZStart.Core
{
    public abstract class ZDataBase :ICloneable, IDisposable
    {
        public string UID = "";
        public long ID = 0;
        public string name = "";

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        public virtual void Dispose()
        {

        }
    }
}
