using UnityEngine;
using UnityEngine.EventSystems;
using ZStart.Core.Enum;

namespace ZStart.Core.Util
{
    public class MathUtil
    {
        private MathUtil()
        {
        }

        public static MoveDirection GetDirection(Vector2 from,Vector2 to,float dist = 1)
        {
            float diffX = Mathf.Abs(to.x - from.x);
            float diffY = Mathf.Abs(to.y - from.y);
            if(diffX > diffY){
                if (diffX > dist)
                {
                    if (to.x > from.x)
                    {
                        return MoveDirection.Right;
                    }
                    else if (to.x < from.x)
                    {
                        return MoveDirection.Left;
                    }
                    else
                        return MoveDirection.None;
                }
                else
                    return MoveDirection.None;
            }else{
                if (diffY > dist)
                {
                    if (to.y > from.y)
                    {
                        return MoveDirection.Up;
                    }
                    else if (to.y < from.y)
                    {
                        return MoveDirection.Down;
                    }
                    else
                        return MoveDirection.None;
                }
                else
                    return MoveDirection.None;
            }
            
        }

        public static Vector3 GetBoundCenter(Vector3[] points)
        {
            if(points == null || points.Length < 1) return Vector3.zero;
            int leng = points.Length;
            Vector3 center = Vector3.zero;
            for (int i = 0; i < leng; i++)
            {
                center += points[i];
            }
            center /= (leng + 1);
            return center;
        }

        public static Vector3 CenterOfPoints(Vector3[] points)
        {
            if (points == null) return Vector3.zero;
            Vector3 center = Vector3.zero;
            if (points.Length == 1)
            {
                center = points[0];

            }
            else if (points.Length == 2)
            {
                center.x = (points[0].x + points[1].x) / 2;
                center.y = (points[0].y + points[1].y) / 2;
                center.z = (points[0].z + points[1].z) / 2;
            }
            else
            {

            }
            return center;
        }

        public static Vector3 CenterOfVectors(Vector3 vector1, Vector3 vector2)
        {
            Vector3[] vectors = new Vector3[2];
            vectors[0] = vector1;
            vectors[1] = vector2;
            Vector3 sum = Vector3.zero;
            if (vectors == null || vectors.Length == 0)
            {
                return sum;
            }

            foreach (Vector3 vec in vectors)
            {
                sum += vec;
            }
            return sum / vectors.Length;
        }

        public static bool Equal(Vector2 one, Vector2 two)
        {
            if (Mathf.Abs(one.x - two.x) > 0.01f)
            {
                return false;
            }
            else if (Mathf.Abs(one.y - two.y) > 0.01f)
            {
                return false;
            }
            return true;
        }

        public static bool Equal(Vector3 one, Vector3 two)
        {
            if (Mathf.Abs(one.x - two.x) > 0.01f)
            {
                return false;
            }
            else if (Mathf.Abs(one.y - two.y) > 0.01f)
            {
                return false;
            }
            else if (Mathf.Abs(one.z - two.z) > 0.01f)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src">当前 normaled vector2</param>
        /// <param name="degress">角度</param>
        /// <returns></returns>
        public static Vector2 GetDirection(Vector2 src, float degress)
        {
            Vector2 dst = new Vector2();
            //Vector2 fwd = new Vector2(Vector3.forward.x,Vector3.forward.z);
            int qd = GetQuadrant(src);
            if (Mathf.Abs(degress) > 90)
                degress = 90 * Mathf.Sign(degress);
            bool clockwise = Mathf.Sign(degress) > 0 ? true : false;
            //float offFwd = Vector2.Dot(src, fwd);

            float rad = degress * Mathf.PI / 180.0f;
            float cos = Mathf.Cos(rad);
            float v1 = cos * 1;

            float dstY = 0;
            float dstX = 0;
            float a = src.y * src.y + src.x * src.x; ;
            float b = -2 * v1 * src.x;
            float c = v1 * v1 - src.y * src.y;
            float tmp = b * b - (4 * a * c);

            float b2 = -2 * v1 * src.y;
            float c2 = v1 * v1 - src.x * src.x;
            float tmp2 = b2 * b2 - (4 * a * c2);

            if (tmp < 0)
            {
                dstX = src.x;
            }
            else
            {
                if (clockwise)
                {
                    if (qd == 1 || qd == 2)
                        dstX = (-b + Mathf.Sqrt(tmp)) / 2 * a;
                    else
                        dstX = (-b - Mathf.Sqrt(tmp)) / 2 * a;
                }
                else
                {
                    if (qd == 1 || qd == 2)
                        dstX = (-b - Mathf.Sqrt(tmp)) / 2 * a;
                    else
                        dstX = (-b + Mathf.Sqrt(tmp)) / 2 * a;
                }
            }
            if (tmp2 < 0)
            {
                dstY = src.y;
            }
            else
            {
                if (clockwise)
                {
                    if (qd == 1 || qd == 4)
                        dstY = (-b2 - Mathf.Sqrt(tmp2)) / 2 * a;
                    else
                        dstY = (-b2 + Mathf.Sqrt(tmp2)) / 2 * a;
                }
                else
                {
                    if (qd == 1 || qd == 4)
                        dstY = (-b2 + Mathf.Sqrt(tmp2)) / 2 * a;
                    else
                        dstY = (-b2 - Mathf.Sqrt(tmp2)) / 2 * a;
                }
            }

            dst.x = dstX;
            dst.y = dstY;
            return dst;
        }

        /// <summary>
        /// 根据坐标获取象限
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static int GetQuadrant(Vector2 v2)
        {
            if (v2.x > 0 && v2.y > 0)
            {
                return 1;
            }
            else if (v2.x > 0 && v2.y < 0)
            {
                return 4;
            }
            else if (v2.x < 0 && v2.y > 0)
            {
                return 2;
            }
            else if (v2.x < 0 && v2.y < 0)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 取模
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float GetVectorMode(Vector2 v2)
        {
            return Mathf.Sqrt(v2.x * v2.x + v2.y * v2.y);
        }

        /// <summary>
        /// 取模
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public static float GetVectorMode(Vector3 v3)
        {
            return Mathf.Sqrt(v3.x * v3.x + v3.y * v3.y + v3.z * v3.z);
        }

        /// <summary>
        /// 获取空间指定平面内圆形均匀随即分布点
        /// </summary>
        /// <param name="number"></param>
        /// <param name="radius"></param>
        /// <param name="offval"></param>
        /// <returns></returns>
        public static Vector3[] GetCircleRandomPoints(int number, float radius, Vector3 center, SpaceTypeEnum type)
        {
            Vector3[] array = new Vector3[number];
            if (number < 2)
            {
                array[0] = center;
                return array;
            }
            for (int i = 0; i < number; i++)
            {
                array[i] = GetCircleRandomPoint(radius,center,type);
            }
            return array;
        }

        public static Vector3 GetCircleRandomPoint(float radius,Vector3 center,SpaceTypeEnum type)
        {
            Vector3 pos = new Vector3(0, 0, 0);
            float range = Mathf.Sqrt(Random.Range(0.0f, 1.0f)) * radius;
            float angle = Random.Range(0.0f, 1.0f) * Mathf.PI * 2;
            if (type == SpaceTypeEnum.XY)
            {
                pos.x = Mathf.Cos(angle) * range + center.x;
                pos.y = Mathf.Sign(angle) * range + center.y;
                pos.z = center.z;
            }
            else if (type == SpaceTypeEnum.XZ)
            {
                pos.x = Mathf.Cos(angle) * range + center.x;
                pos.z = Mathf.Sign(angle) * range + center.z;
                pos.y = center.y;
            }
            else if (type == SpaceTypeEnum.YZ)
            {
                pos.z = Mathf.Cos(angle) * range + center.z;
                pos.y = Mathf.Sign(angle) * range + center.y;
                pos.x = center.x;
            }
            return pos;
        }

        public static string GetVectorStr(Vector3 vector)
        {
            string x = vector.x.ToString("#0.0");
            string y = vector.y.ToString("#0.0");
            string z = vector.z.ToString("#0.0");
            return x + "," + y + "," + z;
        }

        public static Vector3 ParseStrToVector(string str)
        {
            Vector3 vect = Vector3.zero;
            string[] strArr = str.Split(',');
            if (strArr == null || strArr.Length < 3)
                return vect;
            vect.x = float.Parse(strArr[0]);
            vect.y = float.Parse(strArr[1]);
            vect.z = float.Parse(strArr[2]);
            return vect;
        }

        public static float RestrictInBoundary(float num, Vector2 boundary)
        {
            float max = Mathf.Max(boundary.x, boundary.y);
            float min = Mathf.Min(boundary.y, boundary.x);
            if (num > max)
                num = max;
            else if (num < min)
                num = min;
            return num;

        }
    }
}
