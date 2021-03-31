using System.Collections.Generic;
using UnityEngine;
using ZStart.RGraph.Common;

namespace ZStart.RGraph.Layout
{
    public class ForceDirectLayout:BaseLayout
    {
        public float Stiffness
        {
            get;
            set;
        }

        public float Repulsion
        {
            get;
            set;
        }

        public float Damping
        {
            get;
            set;
        }

        public float Threadshold
        {
            get;
            set;
        }

        public bool WithinThreashold
        {
            get;
            private set;
        }

        public float Speed
        {
            get;
            private set;
        }
       
        private float initRadius = 200f;
        private float initAngleRoll = 0f;
        private float initAngleYaw = 0f;
        public bool isPause = false;
        public Vector2 unitSize = new Vector2(500f, 500f);
        public RectTransform canvas;
        float k = 0;
        public ForceDirectLayout(float stiffness, float repulsion, float damping, float radius, RectTransform rect)
        {
            canvas = rect;
            Stiffness = stiffness;
            Repulsion = repulsion;
            Damping = damping;
            initRadius = radius;
            Speed = 10f;
            Threadshold = 0.01f;
            initAngleRoll = Mathf.PI * (3 - Mathf.Sqrt(5));
            initAngleYaw = Mathf.PI * 20 / (9 + Mathf.Sqrt(221));
        }

        public List<Vector3> InitNodePositions(int count)
        {
            List<Vector3> list = new List<Vector3>(count);
            for (int i = 0;i < count;i += 1)
            {
                var radius = initRadius * Mathf.Sqrt(i);
                var roll = i * initAngleRoll;
                var yaw = i * initAngleYaw;
                Vector3 pos = Vector3.zero;
                pos.x = radius * Mathf.Cos(roll);
                pos.y = radius * Mathf.Sin(roll);
                list.Add(pos);
            }
            return list;
        }

        // TODO: change this for group only after node grouping
        protected void ApplyCoulombsLaw()
        {
            float offset = 1f;
            foreach (var n1 in allNodes)
            {
                foreach (var n2 in allNodes)
                {
                    if (n1.Data.UID != n2.Data.UID)
                    {
                        Vector3 d = n1.mTransform.localPosition - n2.mTransform.localPosition;
                        float distance = d.magnitude;
                        //Debug.LogWarning("from = " +n1.Data.name + "; to ="+n2.Data.name+";distance = " + distance);
                        Vector3 direction = d.normalized;
                        if (n1.Pinned && n2.Pinned)
                        {
                            n1.ApplyForce(direction * 0.0f);
                            n2.ApplyForce(direction * 0.0f);
                        }
                        else if (n1.Pinned)
                        {
                            n1.ApplyForce(direction * 0.0f);
                            n2.ApplyForce((direction * Repulsion) / (distance * offset * -1));
                        }
                        else if (n2.Pinned)
                        {
                            n1.ApplyForce((direction * Repulsion) / distance * offset);
                            n2.ApplyForce(direction * 0.0f);
                        }
                        else
                        {
                            n1.ApplyForce((direction * Repulsion) / (distance * offset));
                            n2.ApplyForce((direction * Repulsion) / (distance * offset * -1));
                        }
                    }
                }
            }
        }

        protected void ApplyHookesLaw()
        {
            float offset = 1f;
            foreach (var link in allEdges)
            {
                if (link.Data.repeat == 0)
                {
                    Vector3 d = link.NodeTo.mTransform.localPosition - link.NodeFrom.mTransform.localPosition;
                    float displacement = link.Data.Length - d.magnitude;
                    Vector3 direction = d.normalized;
                    if (link.NodeFrom.Pinned && link.NodeTo.Pinned)
                    {
                        link.NodeFrom.ApplyForce(direction * 0.0f);
                        link.NodeTo.ApplyForce(direction * 0.0f);
                    }
                    else if (link.NodeFrom.Pinned)
                    {
                        link.NodeFrom.ApplyForce(direction * 0.0f);
                        link.NodeTo.ApplyForce(direction * (Stiffness * displacement * offset));
                    }
                    else if (link.NodeTo.Pinned)
                    {
                        link.NodeFrom.ApplyForce(direction * (Stiffness * displacement * offset * -1));
                        link.NodeTo.ApplyForce(direction * 0.0f);
                    }
                    else
                    {
                        link.NodeFrom.ApplyForce(direction * (Stiffness * displacement * offset * -1));
                        link.NodeTo.ApplyForce(direction * (Stiffness * displacement * offset));
                    }
                }
            }
        }

        private void AttractToCentre()
        {
            //for (int i = 0; i < graphBoxs.Count; i += 1)
            //{
            //    var center = graphBoxs[i].GetCenter();
            //    for (int j = 0; j < graphBoxs[i].nodes.Count; j += 1)
            //    {
            //        var node = graphBoxs[i].nodes[j];
            //        if (!node.Pinned && node != center)
            //        {
            //            Vector3 direction = center.mTransform.localPosition - node.mTransform.localPosition;
            //            float displacement = direction.magnitude;
            //            //float displacement = direction.magnitude;
            //            direction = direction.normalized;
            //            node.ApplyForce(direction * Stiffness * displacement * 0.8f);
            //        }
            //    }
            //    if (!center.Pinned)
            //    {
            //        Vector3 direction = center.mTransform.localPosition * -1.0f;
            //        float displacement = direction.magnitude;
            //        direction = direction.normalized;
            //        center.ApplyForce(direction * Stiffness * displacement * 0.8f);
            //    }
            //}
            for (int i = 0; i < allNodes.Count; i += 1)
            {
                if (!allNodes[i].Pinned)
                {
                    Vector3 direction = allNodes[i].mTransform.localPosition * -1.0f;
                    float displacement = direction.magnitude;
                    direction = direction.normalized;
                    allNodes[i].ApplyForce(direction * Stiffness * displacement * 10.8f);
                }
            }
        }

        private void UpdateVelocity()
        {
            var rangX = Vector2.zero;
            var rangY = Vector2.zero;
            foreach (var n in allNodes)
            {
                n.Data.Velocity = n.Data.Velocity + (n.Data.Acceleration * Time.deltaTime * Speed);
                n.Data.Velocity = n.Data.Velocity * Damping;
                n.Data.Acceleration = Vector3.zero;

                var offset = n.Data.Velocity * Time.deltaTime * Speed;
               
                //offset.x = Mathf.Clamp(offset.x, -3f, 3f);
                //offset.y = Mathf.Clamp(offset.y, -3f, 3f);
                if (offset.magnitude > 0.005f && offset.magnitude < 5f)
                {
                    var pos = n.mTransform.localPosition + offset;
                    //var pos = n.mTransform.localPosition + n.Data.Velocity;
                    pos.z = 0;
                    n.mTransform.localPosition = pos;
                }
                if (n.mTransform.localPosition.x < rangX.x)
                {
                    rangX.x = n.mTransform.localPosition.x;
                }else if (n.mTransform.localPosition.x > rangX.y)
                {
                    rangX.y = n.mTransform.localPosition.x;
                }
                if (n.mTransform.localPosition.y < rangY.x)
                {
                    rangY.x = n.mTransform.localPosition.y;
                }
                else if (n.mTransform.localPosition.y > rangY.y)
                {
                    rangY.y = n.mTransform.localPosition.y;
                }
                //Debug.LogWarning(n.Data.name + "__" + n.Data.Velocity + "___" + offset + "_" + offset.magnitude + "___");
                n.Data.Velocity = Vector3.zero;
            }
            var width = Mathf.Abs(rangX.x) > Mathf.Abs(rangX.y) ? Mathf.Abs(rangX.x) : Mathf.Abs(rangX.y);
            var height = Mathf.Abs(rangY.x) > Mathf.Abs(rangY.y) ? Mathf.Abs(rangY.x) : Mathf.Abs(rangY.y);
            canvas.sizeDelta = new Vector2(width * 2 + unitSize.x, height * 2 + unitSize.y);
        }

        private void Relax()
        {
            for (int i = 0;i < allNodes.Count;i += 1)
            {
                var node = allNodes[i];
                if (!node.IsActive)
                    continue;
                float fx = 0.0f;
                float fy = 0.0f;
               
                for (int j = 0;j < allNodes.Count; j += 1)
                {
                    Vector2 distPos = node.mTransform.localPosition - allNodes[j].mTransform.localPosition;
                    float dist = distPos.magnitude;
                    Vector2 coulomb = new Vector2(680 * distPos.x, 680 * distPos.y);
                    float rsq = distPos.x * distPos.x + distPos.y * distPos.y;
                    if (rsq < 0.1f)
                        rsq = 1f;
                    if (dist < 10)
                    {
                        fx += coulomb.x / rsq;
                        fy += coulomb.y / rsq;
                    }
                }
               
                //fx = fx * 0.01f;
                //fy = fy * 0.01f;
                //for (int k = 0;k < allEdges.Count; k += 1)
                //{
                //    Vector3 distPos = Vector3.zero;
                //    if (allEdges[k].Data.HadNode(node.Data.UID))
                //    {
                //        if (allEdges[k].NodeFrom.Data.UID == node.Data.UID)
                //        {
                //            distPos = allEdges[k].NodeTo.mTransform.localPosition - node.mTransform.localPosition;
                //        }else if (allEdges[k].NodeTo.Data.UID == node.Data.UID)
                //        {
                //            distPos = allEdges[k].NodeFrom.mTransform.localPosition - node.mTransform.localPosition;
                //        }
                //    }
                //    fx += distPos.x * 0.08f;
                //    fy += distPos.y * 0.08f;
                //}
                Vector3 pos = node.mTransform.localPosition;
                //Debug.LogError(node.Data.LastPosition + "---" + fx + "---" + fy);
                node.Data.LastPosition = new Vector3(node.Data.LastPosition.x + fx, node.Data.LastPosition.y + fy, 0f) * 0.5f;
                node.mTransform.localPosition = pos + node.Data.LastPosition;
            }
        }

        private float GetTotalEnergy()
        {
            float energy = 0.0f;
            for (int i = 0; i < allNodes.Count; i += 1)
            {
                float speed = allNodes[i].Data.Velocity.magnitude;
                energy += allNodes[i].Data.mass * speed * speed;
            }
            return energy;
        }

        public override void AddNode(RGNode node, string parent, bool add)
        {
            base.AddNode(node, parent, add);
            k = Mathf.Sqrt(1920f * 1080f / (float)allNodes.Count);
        }

        public override void Stop()
        {
            isPause = true;
        }

        public override void Begin()
        {
            isPause = false;
        }

        public override void Step() // time in second
        {
            ApplyCoulombsLaw();
            ApplyHookesLaw();
            //AttractToCentre();
            UpdateVelocity();
            // Relax();

            // CalculateRepulsive();
            // CalculateTraction();
            // UpdateCoordinates();

            UpdateLines();
        }

        // 计算两个Node的斥力产生的单位位移
        private void CalculateRepulsive()
        {
            float ejectFactor = 0.00003f;
            foreach (var n1 in allNodes)
            {
                //n1.distPos = Vector2.zero;
                foreach (var n2 in allNodes)
                {
                    var distX = n1.distPos.x - n2.distPos.x;
                    var distY = n1.distPos.y - n2.distPos.y;
                    var dist = Vector2.Distance(n1.distPos, n2.distPos);

                    if (dist < 100)
                    {
                        ejectFactor = 0.00002f;
                    }
                    if (dist > 0)
                    {
                        n1.distPos.x += distX / dist * k * k / dist * ejectFactor;
                        n1.distPos.y += distY / dist * k * k / dist * ejectFactor;
                    }
                    Debug.LogError(dist + "--;" + n1.distPos);
                }
            }
        }

        // 计算Edge的引力对两端Node产生的引力。
        private void CalculateTraction()
        {
            float condenseFactor = 0.00003f;
            foreach (var edge in allEdges)
            {
                var from = edge.NodeFrom;
                var to = edge.NodeTo;
                float dist = 0;
                Vector2 distV = Vector2.zero;
                distV.x = from.distPos.x - to.distPos.x;
                distV.y = from.distPos.y - to.distPos.y;
                dist = Vector2.Distance(from.distPos, to.distPos);
                var tx = distV.x * dist / k * condenseFactor;
                var ty = distV.y * dist / k * condenseFactor;
                from.distPos.x -= tx;
                from.distPos.y -= ty;
                to.distPos.x += tx;
                to.distPos.y += ty;
                Debug.LogWarning(from.distPos + "---" + to.distPos);
            }
        }

        private void UpdateCoordinates()
        {
            Vector2 maxDist = new Vector2(4, 3);
            foreach (var node in allNodes)
            {
                //int dx = Mathf.FloorToInt(node.distPos.x);
                //int dy = Mathf.FloorToInt(node.distPos.y);
                node.mTransform.anchoredPosition = new Vector2(node.distPos.x, node.distPos.y);
            }
        }

        //public NodeBox GetBoundingBox()
        //{
        //    NodeBox boundingBox = new NodeBox();
        //    Vector3 bottomLeft = new Vector3(-17f, -7f, 0);
        //    Vector3 topRight = new Vector3(17f, 10f, 0); 
        //    foreach (NodeItem node in nodeGroups)
        //    {
        //        Vector3 position = node.mTransform.position;
        //        if (position.x < bottomLeft.x)
        //            bottomLeft.x = position.x;
        //        if (position.y < bottomLeft.y)
        //            bottomLeft.y = position.y;
        //        if (position.z < bottomLeft.z)
        //            bottomLeft.z = position.z;
        //        if (position.x > topRight.x)
        //            topRight.x = position.x;
        //        if (position.y > topRight.y)
        //            topRight.y = position.y;
        //        if (position.z > topRight.z)
        //            topRight.z = position.z;
        //    }
        //    Vector3 padding = (topRight - bottomLeft) * 0.07f;
        //    boundingBox.bottomLeftFront = bottomLeft - padding;
        //    boundingBox.topRightBack = topRight + padding;
        //    return boundingBox;
        //}
    }
}
