using System.Collections.Generic;
using ZStart.Core.Enum;

namespace ZStart.Core.Model
{
    public struct ResponseInfo
    {
        public HttpStateType state;
        public int code;
        public string cookie;
        public string error;
        public string data;
        public string param;
        public Dictionary<string, object> jsonDic;
    }
}
