
using UnityEngine;
using ZStart.Core.Model;
namespace ZStart.Core.View
{
    public interface IZUIPanel
    {
        int depth
        {
            get;
        }

        int echelon
        {
            get;
        }

        string definition
        {
            get;
        }
        bool isOpen
        {
            get;
        }
        void WakenUp(Transform parent);
       
        void Open(Transform parent, UIParamInfo info);
        void Open(UIParamInfo info);
        void Close(Transform parent);
    }
}
