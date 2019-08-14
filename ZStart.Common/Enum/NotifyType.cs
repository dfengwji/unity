namespace ZStart.Common.Enum
{
    public enum NotifyType : int
    {     
        OnLogin = 1,

        OnSceneInit,
        OnAllMenuInit,
        OnAllADInit,
        /// <summary>
        /// param:int total
        /// </summary>
        OnAllAppsInit,

        OnLocalAppsUpdate,
        OnLocalFreeSpace,

        OnOnlineAppsUpdate,

        /// <summary>
        /// param:List<AppInfo> list
        /// 刷新总页面数据
        /// </summary>
        OnAppPagesUpdate,

        /// <summary>
        /// param:List<AppInfo> list
        /// </summary>
        OnSearchUpdate,

        OnSearchClear,

        /// <summary>
        /// 
        /// </summary>
        OnAppBuySuccess,

        /// <summary>
        /// 单个App信息发生变化（string）
        /// </summary>
        OnAppInfoUpdate,

        /// <summary>
        /// 当移除一个APP的时候
        /// </summary>
        OnAppRemoved,

        /// <summary>
        /// 
        /// </summary>
        OnAppBehaviour,

        OnAppIconLoad,

        /// <summary>
        /// param:AppImageInfo info
        /// </summary>
        OnImageUpdate,

        /// <summary>
        /// 刷新下载数据
        /// </summary>
        OnDownloadUpdate,
       
        OnSceneChanged,

        OnGuideComplete,

        OnShowPassword,

        OnShowStateBar,

        OnUpdateTitle,

        OnAnimatorUpdate,

        OnHotareaActive,

        OnLaguageUpdate,

        OnTaskUpdate,

        OnTaskReady,
        OnNoticeUpdate,
        OnResourceCheck,

        OnBrowerInit,
        OnBrowerFlyIn,
        OnBrowerFlyOut,
        
        OnPlayerUpdate,

        OnAudioPlay,
        OnAudioRead,
        OnAudioLoadError
    }
}
