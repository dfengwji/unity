using System.Collections.Generic;
using System.Text;

namespace ZStart.RGraph.Model
{
    public class AffairInfo
    {
        public string uid;
        public int year;
        public int month;
        public string time;
        public string name;
        public string description;
        public string[] images;
        public PairInfo[] keywords;
    }

    public class PropertyInfo
    {
        public string key;
        public PairInfo[] entities;
    }

    public struct PairInfo
    {
        public string key;
        public string value;

        public PairInfo(string k, string v)
        {
            key = k;
            value = v;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var pair = (PairInfo)obj;
            if ((pair.key == key || pair.value == key) && (pair.key == value || pair.value == value))
                return true;
            else
                return false;
        }
    }

    public class EntityInfo
    {
        public string uid = "";
        public string name = "";
        public string remark = "";
        public string type = "";
        public List<string> tags = new List<string>(5);
        public List<PropertyInfo> properties = new List<PropertyInfo>(5);
        public List<AffairInfo> experiences = new List<AffairInfo>(5);

        public bool HadEvents
        {
            get
            {
                return experiences.Count > 0 ? true : false;
            }
        }

        public bool HadRecord
        {
            get
            {
                if (!string.IsNullOrEmpty(remark))
                    return true;
                return properties.Count > 1 ? true : false;
            }
        }

        public AffairInfo GetAffair(string uid)
        {
            return new AffairInfo();
        }

        public string CombineProperties()
        {
            var builder = new StringBuilder();
           
            return builder.ToString();
        }
    }
}
