namespace ZStart.Common.Enum
{
    public enum DownloadStatus
    {
        /// <summary>
        /// 
        /// </summary>
        Pending = 1,
        Running = 2,
        Pause = 4,
        Success = 8,
        Failed = 16,
        Connecting = 32,
        Queue = 64,
    }
}
