﻿
using ZStart.Core.Enum;
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
        void WakenUp();

        void UpdateDepth(int depth);

        void UpdateDepth(PanelDepthType depth);
        void DelayOpen(UIParamInfo info);
        void Open(UIParamInfo info);
        void Close();
    }
}