using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ZStart.Core.Controller;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Model;

namespace ZStart.RGraph.View.Item
{
    public class EdgeItem:Core.ZBehaviourBase
    {
        public LineRenderer render = null;
        public SpriteRenderer sprite;
        public SpriteRenderer arrowLeft;
        public SpriteRenderer arrowRight;

        public Vector2 BGDefaultSzie = new Vector2(0.3f, 0.35f);
        public float DefaultX = 0.7f;
        public Transform labelBox;
        public TextMeshPro label;
        //public Material prefab;

        public Sprite normalSprite;
        public Sprite highSprite;

        public LinkInfo Data
        {
           private set;
           get;
        }
        public NodeItem NodeFrom
        {
            private set; get;
        }
        public NodeItem NodeTo
        {
            private set; get;
        }

        public bool IsHighlight
        {
            set
            {
                if (sprite != null)
                {
                    sprite.sprite = value ? highSprite : normalSprite;
                    //sprite.color = new Color(1f, 1f, 1f, value ? 1f : 0.5f);
                }
                if (render != null && render.material.HasProperty("_TintColor"))
                {
                    Color color = render.material.GetColor("_TintColor");
                    if (value)
                    {
                        render.material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 0.1f));
                    }
                    else
                    {
                        render.material.SetColor("_TintColor", new Color(color.r, color.g, color.b, 0.05f));
                    }
                }
            }
        }

        public DirectionType Direction
        {
            set
            {
                if(value == DirectionType.ToFrom)
                {
                    arrowLeft.enabled = false;
                    arrowRight.enabled = true;
                }else if (value == DirectionType.FromTo)
                {
                    arrowLeft.enabled = true;
                    arrowRight.enabled = false;
                }
                else
                {
                    arrowLeft.enabled = true;
                    arrowRight.enabled = true;
                }
            }
        }

        private void Start()
        {
            //render.material = new Material(prefab)
            //{
            //    name = "EdgeMat" + Time.realtimeSinceStartup
            //};
            render.startColor = Color.white;
            render.endColor = Color.white;
            render.startWidth = 0.02f;
            IsHighlight = false;
        }

        public bool UpdateInfo(LinkInfo data, NodeItem from, NodeItem to)
        {
            if (from == null || to == null)
            {
                Core.ZLog.Warning("create edge error that from or to is null!");
                return false;
            }
            Data = data;
            if (string.IsNullOrEmpty(data.name))
            {
                labelBox.gameObject.SetActive(false);
            }
            else
            {
                label.text = data.name;
                var num = data.name.Length;
                sprite.size = new Vector2(BGDefaultSzie.x + num * 0.1f, BGDefaultSzie.y);
                arrowLeft.transform.localPosition = new Vector3(-DefaultX - num * 0.1f, 0f, 0f);
                arrowRight.transform.localPosition = new Vector3(DefaultX + num * 0.1f, 0f, 0f);
                labelBox.gameObject.SetActive(true);
            }
           
            NodeFrom = from;
            NodeTo = to;
            return true;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            Data.offLength = 0;
            Data = null;
            //NodeFrom = null;
            //NodeTo = null;
            ZAssetController.Instance.DeActivateAsset(mTransform);
        }

        public void DrawLine()
        {
            if (NodeFrom == null || NodeTo == null)
                return;
            var from = NodeFrom.mTransform.position;
            var to = NodeTo.mTransform.position;
            Vector2 v2 = (to - from).normalized;
            float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
            if (angle < -90)
            {
                angle += 180;
            }else if(angle > 90)
            {
                angle += -180;
            }
            labelBox.localRotation = Quaternion.Euler(0f, 0f, angle);
            
            if(Data.number > 0)
            {
                var curvePos = GetCurvePoint(from, to, Data.number);
                var centre = DrawCurve(from, curvePos, to);
                labelBox.position = centre;
            }
            else
            {
                labelBox.position = GetCenterPoint(from, to);
                DrawLine(NodeFrom.mTransform.position, NodeTo.mTransform.position);
            }
        }

        private Vector3 GetCurvePoint(Vector3 from, Vector3 to, int index)
        {
            var center = GetCenterPoint(from, to);
            var dir = (to - from).normalized;
            var distance = Vector3.Distance(from, to);
            var nor = new Vector3(-dir.y / dir.x, 1, 0).normalized;
            var segment = distance / 6.0f;
            //Debug.DrawLine(center, center + nor * 2.1f, Color.red);
            //Debug.DrawLine(center, center - nor * 2.1f, Color.red);
            //Debug.DrawLine(from, from + dir * distance, Color.green);
            int n = Mathf.CeilToInt(index / 2.0f);
            if (index % 2 == 0)
            {
                return center + nor * segment * n;
            }
            else
            {
                return center - nor * segment * n;
            }
        }

        private Vector3 GetCenterPoint(Vector3 from, Vector3 to)
        {
            Vector3 center = Vector3.zero;
            center.x = (from.x + to.x) * 0.5f;
            center.y = (from.y + to.y) * 0.5f;
            center.z = mTransform.position.z;
            return center;
        }

        private Vector2 GetCenterPoint(Vector2 from, Vector2 to)
        {
            Vector2 center = Vector2.zero;
            center.x = (from.x + to.x) * 0.5f;
            center.y = (from.y + to.y) * 0.5f;
            return center;
        }

        public void DrawLine(Vector3 from, Vector3 to)
        {
            List<Vector3> pointList = new List<Vector3>
            {
                from,
                to
            };
            render.positionCount = pointList.Count;
            render.SetPositions(pointList.ToArray());
        }

        public Vector3 DrawCurve(Vector3 from, Vector3 center, Vector3 to)
        {
            int count = 30;//采样点数量
            Vector3 centre = center;
            List<Vector3> pointList = new List<Vector3>();
            for (int i  = 0;i < count;i += 1)
            {
                var ratio = i * (1.0f / count);
                Vector3 vertex1 = Vector3.Lerp(from, center, ratio);
                Vector3 vectex2 = Vector3.Lerp(center, to, ratio);
                Vector3 bezier = Vector3.Lerp(vertex1, vectex2, ratio);
                pointList.Add(bezier);
                if(i == 15)
                {
                    centre = bezier;
                }
            }
            render.positionCount = pointList.Count;
            render.SetPositions(pointList.ToArray());
            return centre;
        }

        public NodeItem GetAnother(string target)
        {
            if (NodeFrom.Data.UID == target)
            {
                return NodeTo;
            }
            else if (NodeTo.Data.UID == target)
            {
                return NodeFrom;
            }
            else
            {
                return null;
            }
        }

        public void Clear()
        {
            Data.Dispose();
            Data = null;
            NodeFrom = null;
            NodeTo = null;
        }
    }
}
