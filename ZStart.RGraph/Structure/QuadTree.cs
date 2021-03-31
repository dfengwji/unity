using System;
using System.Collections.Generic;
using UnityEngine;
using ZStart.RGraph.Model;
using static ZStart.RGraph.Structure.QuadTree;

namespace ZStart.RGraph.Structure
{
    public delegate bool VisitCallback(TreeNode node, double x0, double y0, double x1, double y1);
    public delegate double NodeCalculateCall(NodeInfo node);
    public delegate double LinkCalculateCall(LinkInfo node);
    public class QuadTree
    {
        public NodeCalculateCall xCall, yCall;
        public double tlX, tlY; // top-left
        public double brX, brY; // bottom-right

        TreeNode root;

        public static QuadTree Create(NodeInfo[] nodes, NodeCalculateCall x, NodeCalculateCall y)
        {
            QuadTree tree = new QuadTree(x, y, Double.NaN, Double.NaN, Double.NaN, Double.NaN);
            if (nodes != null)
            {
                tree.AddAll(nodes);
            }
            return tree;
        }

        private QuadTree(NodeCalculateCall x, NodeCalculateCall y, double x0, double y0, double x1, double y1)
        {
            this.xCall = x;
            this.yCall = y;
            this.tlX = x0;
            this.tlY = y0;
            this.brX = x1;
            this.brY = y1;
        }

        public QuadTree AddAll(NodeInfo[] nodes)
        {
            double[] xz = new double[nodes.Length], yz = new double[nodes.Length];
            double x0 = Double.PositiveInfinity, y0 = Double.PositiveInfinity;
            double x1 = Double.NegativeInfinity, y1 = Double.NegativeInfinity;
            double x, y;
            for (int i = 0; i < nodes.Length; i++)
            {
                NodeInfo node = nodes[i];
                x = node.X;
                y = node.Y;
                xz[i] = x;
                yz[i] = y;
                if (x < x0) x0 = x;
                if (x > x1) x1 = x;
                if (y < y0) y0 = y;
                if (y > y1) y1 = y;
            }

            if (x1 < x0)
            {
                x0 = this.tlX;
                x1 = this.brX;
            }
            if (y1 < y0)
            {
                y0 = this.tlY;
                y1 = this.brY;
            }
            

            Cover(x0, y0).Cover(x1, y1);

            for (int i = 0; i < nodes.Length; i++)
            {
                Add(xz[i], yz[i], nodes[i]);
            }

            return this;
        }

        public void Add(double x, double y, NodeInfo d)
        {
            Debug.LogError("add = " + d.UID);
            if (Double.IsNaN(x) || Double.IsNaN(y))
            {
                return;
            }
           
            TreeNode node = root;
            TreeNode leaf = TreeNode.CreateLeaf(d);
            if (node == null)
            {
                root = leaf;
                return;
            }
           
            double x0 = this.tlX, y0 = this.tlY;
            double x1 = this.brX, y1 = this.brY;

            int right, bottom;
            double xm, ym;
            TreeNode parent = null;
            int i = 0, j = 0;
            while (!node.IsLeaf())
            {
                xm = (x0 + x1) / 2;
                ym = (y0 + y1) / 2;

                if (x >= xm)
                {
                    right = 1;
                    x0 = xm;
                }
                else
                {
                    right = 0;
                    x1 = xm;
                }

                if (y >= ym)
                {
                    bottom = 1;
                    y0 = ym;
                }
                else
                {
                    bottom = 0;
                    y1 = ym;
                }

                parent = node;
                i = bottom << 1 | right;
                node = node.quadrants[i];
                if (node == null)
                {
                    parent.quadrants[i] = leaf;
                    return;
                }
            }

            double xp = X(node.data), yp = Y(node.data);
            if (x == xp && y == yp)
            {
                leaf.next = node;
                if (parent != null)
                {
                    parent.quadrants[i] = leaf;
                }
                else
                {
                    root = leaf;
                }
                return;
            }

            do
            {
                if (parent != null)
                {
                    parent = parent.quadrants[i] = TreeNode.CreateInternal();
                }
                else
                {
                    parent = root = TreeNode.CreateInternal();
                }

                xm = (x0 + x1) / 2;
                ym = (y0 + y1) / 2;

                if (x >= xm)
                {
                    right = 1;
                    x0 = xm;
                }
                else
                {
                    right = 0;
                    x1 = xm;
                }

                if (y >= ym)
                {
                    bottom = 1;
                    y0 = ym;
                }
                else
                {
                    bottom = 0;
                    y1 = ym;
                }

                i = bottom << 1 | right;
                j = (yp >= ym ? 1 : 0) << 1 | (xp >= xm ? 1 : 0);
            } while (i == j);

            parent.quadrants[j] = node;
            parent.quadrants[i] = leaf;

            return;
        }

        public void Add(NodeInfo d)
        {
            double xz = 0f, yz = 0f;
            double x0 = Double.PositiveInfinity, y0 = Double.PositiveInfinity;
            double x1 = Double.NegativeInfinity, y1 = Double.NegativeInfinity;
            double x, y;
            NodeInfo node = d;
            x = node.X;
            y = node.Y;
            xz = x;
            yz = y;
            if (x < x0) x0 = x;
            if (x > x1) x1 = x;
            if (y < y0) y0 = y;
            if (y > y1) y1 = y;

            if (x1 < x0)
            {
                x0 = this.tlX;
                x1 = this.brX;
            }
            if (y1 < y0)
            {
                y0 = this.tlY;
                y1 = this.brY;
            }

            Cover(x0, y0).Cover(x1, y1);

            Add(xz, yz, node);
        }

        public QuadTree Cover(double x, double y)
        {
            if (Double.IsNaN(x) || Double.IsNaN(y))
            {
                return this;
            }

            double x0 = this.tlX, y0 = this.tlY;
            double x1 = this.brX, y1 = this.brY;

            if (Double.IsNaN(x0))
            {
                x1 = (x0 = Mathf.Floor((float)x)) + 1;
                y1 = (y0 = Mathf.Floor((float)y)) + 1;
            }
            else if (x0 > x || x > x1 || y0 > y || y > y1)
            {
                double z = x1 - x0;
                TreeNode node = root;
                TreeNode parent;

                int i = (y < (y0 + y1) / 2 ? 1 : 0) << 1 | (x < (x0 + x1) / 2 ? 1 : 0);
                switch (i)
                {
                    case 0:
                        do
                        {
                            parent = TreeNode.CreateInternal();
                            parent.quadrants[i] = node;
                            node = parent;

                            z *= 2;
                            x1 = x0 + z;
                            y1 = y0 + z;
                        } while (x > x1 || y > y1);
                        break;
                    case 1:
                        do
                        {
                            parent = TreeNode.CreateInternal();
                            parent.quadrants[i] = node;
                            node = parent;

                            z *= 2;
                            x0 = x1 - z;
                            y1 = y0 + z;
                        } while (x0 > x || y > y1);
                        break;
                    case 2:
                        do
                        {
                            parent = TreeNode.CreateInternal();
                            parent.quadrants[i] = node;
                            node = parent;

                            z *= 2;
                            x1 = x0 + z;
                            y0 = y1 - z;
                        } while (x > x1 || y0 > y);
                        break;
                    case 3:
                        do
                        {
                            parent = TreeNode.CreateInternal();
                            parent.quadrants[i] = node;
                            node = parent;

                            z *= 2;
                            x0 = x1 - z;
                            y0 = y1 - z;
                        } while (x0 > x || y0 > y);
                        break;
                }

                if (root != null && !root.IsLeaf())
                {
                    root = node;
                }
            }
            else
            {
                return this;
            }

            this.tlX = x0;
            this.tlY = y0;
            this.brX = x1;
            this.brY = y1;

            return this;
        }

        public QuadTree Visit(VisitCallback callback)
        {
            Queue<Quad> quads = new Queue<Quad>(5);
            TreeNode node = root;

            if (node != null)
            {
                quads.Enqueue(new Quad(node, tlX, tlY, brX, brY));
            }

            Quad q;
            TreeNode child;
            while (quads.Count > 0)
            {
                q = quads.Dequeue();
                node = q.node;
                double x0 = q.x0, y0 = q.y0, x1 = q.x1, y1 = q.y1;
                if (!callback.Invoke(node, x0, y0, x1, y1) && !node.IsLeaf())
                {
                    double xm = (x0 + x1) / 2, ym = (y0 + y1) / 2;
                    if ((child = node.quadrants[3]) != null)
                    {
                        quads.Enqueue(new Quad(child, xm, ym, x1, y1));
                    }
                    if ((child = node.quadrants[2]) != null)
                    {
                        quads.Enqueue(new Quad(child, x0, ym, xm, y1));
                    }
                    if ((child = node.quadrants[1]) != null)
                    {
                        quads.Enqueue(new Quad(child, xm, y0, x1, ym));
                    }
                    if ((child = node.quadrants[0]) != null)
                    {
                        quads.Enqueue(new Quad(child, x0, y0, xm, ym));
                    }
                }
            }
            return this;
        }

        public QuadTree VisitAfter(VisitCallback callback)
        {
            Queue<Quad> quads = new Queue<Quad>(10);
            Queue<Quad> next = new Queue<Quad>(10);

            if (root != null)
            {
                quads.Enqueue(new Quad(root, tlX, tlY, brX, brY));
            }

            Quad q;
            TreeNode child;
            while (quads.Count > 0)
            {
                q = quads.Dequeue();
                TreeNode node = q.node;
                if (!node.IsLeaf())
                {
                    double x0 = q.x0, y0 = q.y0, x1 = q.x1, y1 = q.y1;
                    double xm = (x0 + x1) / 2, ym = (y0 + y1) / 2;
                    if ((child = node.quadrants[0]) != null)
                    {
                        quads.Enqueue(new Quad(child, x0, y0, xm, ym));
                    }
                    if ((child = node.quadrants[1]) != null)
                    {
                        quads.Enqueue(new Quad(child, xm, y0, x1, ym));
                    }
                    if ((child = node.quadrants[2]) != null)
                    {
                        quads.Enqueue(new Quad(child, x0, ym, xm, y1));
                    }
                    if ((child = node.quadrants[3]) != null)
                    {
                        quads.Enqueue(new Quad(child, xm, ym, x1, y1));
                    }
                }
                next.Enqueue(q);
            }

            while (next.Count > 0)
            {
                q = next.Dequeue();
                callback.Invoke(q.node, q.x0, q.y0, q.x1, q.y1);
            }

            return this;
        }

        NodeInfo Find(double x, double y, double radius)
        {
            NodeInfo data = null;
            List<Quad> quads = new List<Quad>(20);
            TreeNode node = root;
            double x0 = this.tlX, y0 = this.tlY, x1, y1, x2, y2, x3 = this.brX, y3 = this.brY;

            if (node != null)
            {
                quads.Add(new Quad(node, x0, y0, x3, y3));
            }

            if (radius <= 0)
            {
                radius = Double.PositiveInfinity;
            }
            else
            {
                x0 = x - radius;
                y0 = y - radius;
                x3 = x + radius;
                y3 = y + radius;
                radius *= radius;
            }

            Quad q;
            while (quads.Count > 0)
            {
                q = quads[quads.Count - 1];
                quads.RemoveAt(quads.Count - 1);
                if ((node = q.node) == null
                        || (x1 = q.x0) > x3
                        || (y1 = q.y0) > y3
                        || (x2 = q.x1) < x0
                        || (y2 = q.y1) < y0)
                {
                    continue;
                }

                if (!node.IsLeaf())
                {
                    double xm = (x1 + x2) / 2, ym = (y1 + y2) / 2;
                    quads.Add(new Quad(node.quadrants[3], xm, ym, x2, y2));
                    quads.Add(new Quad(node.quadrants[2], x1, ym, xm, y2));
                    quads.Add(new Quad(node.quadrants[1], xm, y1, x2, ym));
                    quads.Add(new Quad(node.quadrants[0], x1, y1, xm, ym));

                    int i = (y >= ym ? 1 : 0) << 1 | (x >= xm ? 1 : 0);
                    if (i > 0)
                    {
                        q = quads[quads.Count - 1];
                        quads[quads.Count - 1] = quads[quads.Count - 1 - i];
                        quads[quads.Count - 1 - i] = q;
                    }
                }
                else
                {
                    double dx = x - X(node.data), dy = y - Y(node.data);
                    double d2 = dx * dx + dy * dy;
                    if (d2 < radius)
                    {
                        radius = d2;
                        float d = Mathf.Sqrt((float)radius);
                        x0 = x - d;
                        y0 = y - d;
                        x3 = x + d;
                        y3 = y + d;
                        data = node.data;
                    }
                }
            }

            return data;
        }

        public int Size()
        {
            //int[] size = { 0 };
            //Visit((node, x0, y0, x1, y1)-> {
            //    if (node.IsLeaf())
            //    {
            //        do
            //        {
            //            size[0]++;
            //        } while ((node = node.next) != null);
            //    }
            //    return false;
            //});
            //return size[0];
            return 0;
        }

        private double X(NodeInfo node)
        {
            if (xCall != null)
            {
                return xCall.Invoke(node);
            }
            return 0;
        }

        private double Y(NodeInfo node)
        {
            if (yCall != null)
            {
                return yCall.Invoke(node);
            }
            return 0;
        }

        //public interface VisitCallback
        //{
        //    /**
        //     * If the callback returns true for a given node, then the children of that node are not visited;
        //     * otherwise, all child nodes are visited. This can be used to quickly visit only parts of the tree.
        //     * <p>
        //     * ⟨x0, y0⟩ are the lower bounds of the node, and ⟨x1, y1⟩ are the upper bounds,
        //     * Assuming that positive x is right and positive y is down, as is typically the case in
        //     * Canvas, ⟨x0, y0⟩ is the top-left corner and ⟨x1, y1⟩ is the lower-right corner.
        //     */
        //    bool Visit(TreeNode node, double x0, double y0, double x1, double y1);
        //}

        public class TreeNode
        {

            public TreeNode[] quadrants;

            public NodeInfo data; // only leaf node has data
            public TreeNode next;

            public double strength;
            public double x, y;
            public double r;
            public int Index
            {
                get
                {
                    if (data == null)
                        return 0;
                    return data.index;
                }
            }

            /** Leaf node */
            public static TreeNode CreateLeaf(NodeInfo data)
            {
                TreeNode node = new TreeNode
                {
                    data = data
                };
                return node;
            }

            /**
             * Internal nodes of the quadtree are represented as four-element arrays in left-to-right,
             * top-to-bottom order:
             * <li>0 - the top-left quadrant, if any.</li>
             * <li>1 - the top-right quadrant, if any.</li>
             * <li>2 - the bottom-left quadrant, if any.</li>
             * <li>3 - the bottom-right quadrant, if any.</li>
             */
            public static TreeNode CreateInternal()
            {
                TreeNode node = new TreeNode
                {
                    quadrants = new TreeNode[4]
                };
                return node;
            }

            private TreeNode() { }

            public bool IsLeaf()
            {
                return quadrants == null;
            }
        }

        public class Quad
        {
            public TreeNode node;
            public double x0, y0;
            public double x1, y1;

            public Quad(TreeNode node, double x0, double y0, double x1, double y1)
            {
                this.node = node;
                this.x0 = x0;
                this.y0 = y0;
                this.x1 = x1;
                this.y1 = y1;
            }
        }
    }
    //public class QuadTree
    //{
    //    private int MAX_OBJECTS = 1;
    //    private int MAX_LEVELS = 3;

    //    private int level;
    //    private List<object> objects;
    //    private Rect bounds;
    //    private QuadTree[] nodes;

    //    public QuadTree(int pLevel, Rect pBounds)
    //    {
    //        level = pLevel;
    //        objects = new List<object>(10);
    //        bounds = pBounds;
    //        nodes = new QuadTree[4];
    //    }

    //    public void Clear()
    //    {
    //        objects.Clear();

    //        for (int i = 0; i < nodes.Length; i++)
    //        {
    //            if (nodes[i] != null)
    //            {
    //                nodes[i].Clear();
    //                nodes[i] = null;
    //            }
    //        }
    //    }

    //    private void Split()
    //    {
    //        int subWidth = (int)(bounds.width / 2);
    //        int subHeight = (int)(bounds.height / 2);
    //        int x = (int)bounds.x;
    //        int y = (int)bounds.y;

    //        nodes[0] = new QuadTree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight));
    //        nodes[1] = new QuadTree(level + 1, new Rect(x, y, subWidth, subHeight));
    //        nodes[2] = new QuadTree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight));
    //        nodes[3] = new QuadTree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight));
    //    }

    //    private List<int> GetIndexes(Rect pRect)
    //    {
    //        List<int> indexes = new List<int>(5);

    //        double verticalMidpoint = bounds.x + (bounds.width / 2);
    //        double horizontalMidpoint = bounds.y + (bounds.height / 2);

    //        bool topQuadrant = pRect.y >= horizontalMidpoint;
    //        bool bottomQuadrant = (pRect.y - pRect.height) <= horizontalMidpoint;
    //        bool topAndBottomQuadrant = pRect.y + pRect.height + 1 >= horizontalMidpoint && pRect.y + 1 <= horizontalMidpoint;

    //        if (topAndBottomQuadrant)
    //        {
    //            topQuadrant = false;
    //            bottomQuadrant = false;
    //        }

    //        // Check if object is in left and right quad
    //        if (pRect.x + pRect.width + 1 >= verticalMidpoint && pRect.x - 1 <= verticalMidpoint)
    //        {
    //            if (topQuadrant)
    //            {
    //                indexes.Add(2);
    //                indexes.Add(3);
    //            }
    //            else if (bottomQuadrant)
    //            {
    //                indexes.Add(0);
    //                indexes.Add(1);
    //            }
    //            else if (topAndBottomQuadrant)
    //            {
    //                indexes.Add(0);
    //                indexes.Add(1);
    //                indexes.Add(2);
    //                indexes.Add(3);
    //            }
    //        }

    //        // Check if object is in just right quad
    //        else if (pRect.x + 1 >= verticalMidpoint)
    //        {
    //            if (topQuadrant)
    //            {
    //                indexes.Add(3);
    //            }
    //            else if (bottomQuadrant)
    //            {
    //                indexes.Add(0);
    //            }
    //            else if (topAndBottomQuadrant)
    //            {
    //                indexes.Add(3);
    //                indexes.Add(0);
    //            }
    //        }
    //        // Check if object is in just left quad
    //        else if (pRect.x - pRect.width <= verticalMidpoint)
    //        {
    //            if (topQuadrant)
    //            {
    //                indexes.Add(2);
    //            }
    //            else if (bottomQuadrant)
    //            {
    //                indexes.Add(1);
    //            }
    //            else if (topAndBottomQuadrant)
    //            {
    //                indexes.Add(2);
    //                indexes.Add(1);
    //            }
    //        }
    //        else
    //        {
    //            indexes.Add(-1);
    //        }

    //        return indexes;
    //    }

    //    public void Insert(object sprite)
    //    {
    //        object fSprite = sprite;
    //        //Rect pRect = fSprite.GetTextureRectRelativeToContainer();

    //        if (nodes[0] != null)
    //        {
    //            //List<int> indexes = GetIndexes(pRect);
    //            //for (int ii = 0; ii < indexes.Count; ii++)
    //            //{
    //            //    int index = indexes[ii];
    //            //    if (index != -1)
    //            //    {
    //            //        nodes[index].Insert(fSprite);
    //            //        return;
    //            //    }
    //            //}

    //        }

    //        objects.Add(fSprite);

    //        if (objects.Count > MAX_OBJECTS && level < MAX_LEVELS)
    //        {
    //            if (nodes[0] == null)
    //            {
    //                Split();
    //            }

    //            int i = 0;
    //            while (i < objects.Count)
    //            {
    //                object sqaureOne = objects[i];
    //                //Rect oRect = sqaureOne.GetTextureRectRelativeToContainer();
    //                //List<int> indexes = GetIndexes(oRect);
    //                //for (int ii = 0; ii < indexes.Count; ii++)
    //                //{
    //                //    int index = indexes[ii];
    //                //    if (index != -1)
    //                //    {
    //                //        nodes[index].Insert(sqaureOne);
    //                //        objects.Remove(sqaureOne);
    //                //    }
    //                //    else
    //                //    {
    //                //        i++;
    //                //    }
    //                //}
    //            }
    //        }
    //    }

    //    private List<object> Retrieve(List<object> fSpriteList, Rect pRect)
    //    {
    //        List<int> indexes = GetIndexes(pRect);
    //        for (int ii = 0; ii < indexes.Count; ii++)
    //        {
    //            int index = indexes[ii];
    //            if (index != -1 && nodes[0] != null)
    //            {
    //                nodes[index].Retrieve(fSpriteList, pRect);
    //            }

    //            fSpriteList.AddRange(objects);
    //        }

    //        return fSpriteList;
    //    }
    //}
}
