using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZStart.Common.Model
{
    public struct PairInfo
    {
        public string key;
        public string value;
        public PairInfo(string k, string val)
        {
            key = k;
            value = val;
        }
    }
}
