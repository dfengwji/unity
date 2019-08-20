namespace ZStart.Common.Enum
{
    public enum ServerErrorStaus
    {
        Unknown = -1,
        Success = 0,
        StatusNoAdmin = 1,

        StatusBindError = 2,

        StatusNotExisted = 3,

        StatusAddException = 4,

        StatusRemoveException = 5,

        StatusUpdateException = 6,

        StatusApplyError = 7,

        StatusApplyNotMatch = 8,

        StatusCopyFailed = 9,

        StatusPasswordNotEqual = 10,

        StatusAddRepeat = 11,

        StatusAuthorError = 12,

        StatusPhoneUsed = 13,

        StatusUploadException = 14,

        StatusNumberMax = 15,

        StatusEmailUsed = 16,

        StatusEmpty = 17,

        StatusPermissionDenied = 18,

        StatusValidateFailed = 19,

        StatusTimeExpire = 20,

        StatusLogout = 21,

        StatusParamError = 22,

        StatusFormatNoSupport = 23,

        StatusOptionError = 24,
    }
}
