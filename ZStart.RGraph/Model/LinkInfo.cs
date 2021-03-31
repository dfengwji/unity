
using UnityEngine;
using ZStart.RGraph.Enum;

namespace ZStart.RGraph.Model
{
    public class LinkInfo:Core.ZDataBase
    {
        public NodeInfo from = default;
        public NodeInfo to = default;
        public float InitLength
        {
            private set;
            get;
        }
        public float offLength = 0f;
        public float Stiffness = 0f;
        public int index = 0;
        public int number = 0;
        public int repeat = 0;
        public string type = "";
        public DirectionType direction = DirectionType.FromTo;
        public bool Directed
        {
            get;
            set;
        }

        public float Length
        {
            get
            {
                return InitLength + offLength;
            }
        }

        public LinkInfo() { }

        public LinkInfo(string id, NodeInfo source, NodeInfo target, string lable, float length)
        {
            UID = id;
            from = source;
            to = target;
            Directed = false;
            name = lable;
            this.InitLength = length;
        }

        public virtual void RandomLength(float offset)
        {
            InitLength = Random.Range(260.0f, 330.0f) + offset;
        }

        public bool IsLink(string from, string to)
        {
            if ((this.from.UID == from || this.from.UID == to)&&(this.to.UID == from || this.to.UID == to))
            {
                return true;
            }
            return false;
        }

        public bool HadNode(string target)
        {
            if (from.UID == target || to.UID == target)
            {
                return true;
            }
            return false;
        }

        public string GetOther(string target)
        {
            if(this.from.UID == target)
            {
                return this.to.UID;
            }else if(this.to.UID == target)
            {
                return this.from.UID;
            }
            else
            {
                return "";
            }
        }

        public override void Dispose()
        {

        }

        public override int GetHashCode()
        {
            return UID.GetHashCode();
        }
        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            LinkInfo p = obj as LinkInfo;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (UID == p.UID);
        }

        public bool Equals(LinkInfo p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (UID == p.UID);
        }

        public static bool operator ==(LinkInfo a, LinkInfo b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.UID == b.UID;
        }

        public static bool operator !=(LinkInfo a, LinkInfo b)
        {
            return !(a == b);
        }
    }
}
