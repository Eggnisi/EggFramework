#region

//文件创建者：Egg
//创建时间：09-13 01:46

#endregion

using UnityEngine;

namespace EggFramework.Util
{
    public static class MathUtil
    {
        public static Vector3 Get2DOffsetByAngleAndLength(float angle, float length)
        {
            return new Vector3(Mathf.Cos(angle / Mathf.PI) * length, Mathf.Sin(angle / Mathf.PI) * length);
        }

        public static Vector2 Get2DDirByAngle(float angle)
        {
            return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        }

        public static float DistancePointToSegment(Vector3 a, Vector3 b, Vector3 p)
        {
            var AP = p - a; // 向量AP
            var AB = b - a; // 向量AB

            var magnitudeAB = AB.sqrMagnitude;     // 线段AB的长度平方
            var dotProduct  = Vector3.Dot(AP, AB); // 向量AP和向量AB的点积

            // 参数t表示投影点D在线段AB上的位置比例，0 <= t <= 1
            var t = dotProduct / magnitudeAB;

            switch (t)
            {
                // 如果t不在[0, 1]范围内，那么最近点不是线段内部的点
                case < 0:
                    return Vector3.Distance(p, a); // 最近点是A
                case > 1:
                    return Vector3.Distance(p, b); // 最近点是B
                default:
                {
                    // 投影点D的位置
                    Vector3 D = a + t * AB;
                    return Vector3.Distance(p, D); // 计算点P到点D的距离
                }
            }
        }

        public static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 p, float t)
        {
            t = Mathf.Clamp01(t);
            var ap = Vector3.Lerp(a, p, t);
            var pb = Vector3.Lerp(p, b, t);
            return Vector3.Lerp(ap, pb, t);
        }
    }
}