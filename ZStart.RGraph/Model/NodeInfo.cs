using UnityEngine;
using ZStart.RGraph.Enum;

namespace ZStart.RGraph.Model
{
    public class NodeInfo: Core.ZDataBase
    {
        public string parent = "";
        public string avatar = "";
        // 个人详情信息文件绝对路径
        public string fullPath = "";
        public string type = "";
        public string vName = "";
        public float mass = 1.0f;
        public bool isDrag = false;
        private bool pinned = false;
        public bool Pinned
        {
            get
            {
                if (isDrag)
                    return true;
                return pinned;
            }
            set
            {
                pinned = value;
                if (value)
                {
                    vx = 0f;
                    vy = 0f;
                }
            }
        }

        public ExpendStatus Expended
        {
            get;
            set;
        }
        public int index = 0;
        public double X
        {
            set;get;
        }
        public double Y {
            set;get;
        }

        public bool IsVirtual
        {
            get
            {
                return !string.IsNullOrEmpty(vName);
            }
        }

        private double vx = 0.0f;
        public double VX
        {
            set
            {
                if (pinned)
                    return;
                vx = value;
            }
            get
            {
                return vx;
            }
        }

        private double vy = 0f;
        public double VY
        {
            set {
                if (pinned)
                    return;
                vy = value;
            }
            get
            {
                return vy;
            }
        }
        public double fx = double.MaxValue, fy = double.MaxValue;
        public int level = 2;
        public Vector3 LastPosition { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
        public Vector2 Screen { get; set; }
        public Vector3 Position
        {
            get
            {
                return new Vector3((float)X, (float)Y, 0.0f);
            }
        }

        public EntityInfo entity = null;
        public NodeInfo() {
            mass = 1.0f;
            LastPosition = Vector3.zero;
            Velocity = Vector3.zero;
            Acceleration = Vector3.zero;
            Pinned = false;
        }

        public NodeInfo(string iId, string label, float mass, Vector3 pos)
        {
            UID = iId;
            this.name = label;
            this.mass = mass;
            this.LastPosition = pos;
            Pinned = false;
            Expended = ExpendStatus.Closed;
            LastPosition = Vector3.zero;
            Velocity = Vector3.zero;
            Acceleration = Vector3.zero;
        }

        public override void Dispose()
        {

        }

        public void ClearForce()
        {
            Acceleration = Vector3.zero;
            Velocity = Vector3.zero;
        }

        public override string ToString()
        {
            return "UID = " +UID + ";xy = (x:" + X + ", y:" + Y + "); vxy = (x:" + VX + ", y:" + VY + "); fxy = (x:"+fx+",y:"+fy+")";
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
            NodeInfo p = obj as NodeInfo;
            if ((System.Object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (UID == p.UID);
        }

        public bool Equals(NodeInfo p)
        {
            // If parameter is null return false:
            if ((object)p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (UID == p.UID);
        }

        public static bool operator ==(NodeInfo a, NodeInfo b)
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

        public static bool operator !=(NodeInfo a, NodeInfo b)
        {
            return !(a == b);
        }
    }
}
