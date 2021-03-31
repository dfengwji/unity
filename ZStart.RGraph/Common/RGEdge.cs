using System.Collections.Generic;
using UnityEngine;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Model;
using ZStart.RGraph.View.Parts;

namespace ZStart.RGraph.Common
{
    public class RGEdge
    {
        public LinkInfo Data
        {
            private set;
            get;
        }
        public RGNode NodeFrom
        {
            private set; get;
        }
        public RGNode NodeTo
        {
            private set; get;
        }

        public GEdgeParts viewer;
        public RectTransform mTransform
        {
            get
            {
                return viewer.mTransform;
            }
        }

        public bool IsHighlight
        {
            set
            {
                viewer.IsHighlight = value;
            }
        }

        public DirectionType Direction
        {
            set
            {
                viewer.Direction = value;
            }
        }

        public bool UpdateInfo(LinkInfo data, GEdgeParts view, RGNode from, RGNode to)
        {
            if (from == null || to == null)
            {
                Core.ZLog.Warning("create edge error that from or to is null!");
                return false;
            }
            Data = data;
            viewer = view;
            viewer.UpdateInfo(data);
            NodeFrom = from;
            NodeTo = to;
            return true;
        }

        public void Show()
        {
            viewer.Show();
        }

        public void Hide()
        {
            Data.offLength = 0;
            Data = null;
            viewer.UnShow();
        }

        public RGNode GetAnother(string target)
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

        public void DrawLine()
        {
            if (NodeFrom == null || NodeTo == null)
                return;
            var from = NodeFrom.mTransform.localPosition;
            var to = NodeTo.mTransform.localPosition;
            Vector2 v2 = (to - from).normalized;
          
            float angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
            if (angle < -90)
            {
                angle += 180;
            }
            else if (angle > 90)
            {
                angle += -180;
            }
            
            viewer.labelBox.localRotation = Quaternion.Euler(0f, 0f, angle);
            viewer.line.localRotation = Quaternion.Euler(0f, 0f, angle);

            //if (Data.number > 0)
            //{
            //    var curvePos = GetCurvePoint(from, to, Data.number);
            //    var centre = DrawCurve(from, curvePos, to);
            //    viewer.labelBox.localPosition = centre;
            //}
            //else
            //{
                viewer.labelBox.localPosition = GetCenterPoint(from, to);
                var fromPos = NodeFrom.mTransform.localPosition;
                var toPos = NodeTo.mTransform.localPosition;
                DrawLine(fromPos, toPos);
            //}
            viewer.line.anchoredPosition = viewer.labelBox.localPosition;
        }

        public void DrawLine(Vector3 from, Vector3 to)
        {
            viewer.line.sizeDelta = new Vector2(Vector2.Distance(from, to), 3f);
            //Debug.LogError("from = "+from + ";to = " + to + "; distance = " + Vector3.Distance(from, to));
            //if (viewer.line.GetPointCount() > 1)
            //{
            //    viewer.line.SetPointInfo(0, new LinePoint(from));
            //    viewer.line.SetPointInfo(1, new LinePoint(to));
            //}
            //else
            //{
            //    viewer.line.AddPoint(new LinePoint(from));
            //    viewer.line.AddPoint(new LinePoint(to));
            //}

            //List<Vector3> pointList = new List<Vector3>
            //{
            //    from,
            //    to
            //};
            //render.positionCount = pointList.Count;
            //render.SetPositions(pointList.ToArray());
        }

        public Vector3 DrawCurve(Vector3 from, Vector3 center, Vector3 to)
        {
            int count = 30;//采样点数量
            Vector3 centre = center;
            List<Vector3> pointList = new List<Vector3>(count);
            for (int i = 0; i < count; i += 1)
            {
                var ratio = i * (1.0f / count);
                Vector3 vertex1 = Vector3.Lerp(from, center, ratio);
                Vector3 vectex2 = Vector3.Lerp(center, to, ratio);
                Vector3 bezier = Vector3.Lerp(vertex1, vectex2, ratio);
                pointList.Add(bezier);
                if (i == 15)
                {
                    centre = bezier;
                }
            }
            //render.positionCount = pointList.Count;
            //render.SetPositions(pointList.ToArray());
            return centre;
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
            center.z = mTransform.localPosition.z;
            return center;
        }

        private Vector2 GetCenterPoint(Vector2 from, Vector2 to)
        {
            Vector2 center = Vector2.zero;
            center.x = (from.x + to.x) * 0.5f;
            center.y = (from.y + to.y) * 0.5f;
            return center;
        }

    }
}
