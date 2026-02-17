using System.Collections.Generic;
using UnityEngine;
using Cube2048.Data;

namespace Cube2048.Core 
{
    public static class MathUtils
    {
        public static void FindBestPair(List<CubeData> data, out int bestIdA, out int bestIdB)
        {
            bestIdA = -1;
            bestIdB = -1;
            float minDistance = float.MaxValue;

            for (int i = 0; i < data.Count; i++)
            {
                for (int j = i + 1; j < data.Count; j++)
                {
                    if (data[i].Value == data[j].Value)
                    {
                        float dist = Vector3.Distance(data[i].Position, data[j].Position);
                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            bestIdA = data[i].InstanceID;
                            bestIdB = data[j].InstanceID;
                        }
                    }
                }
            }
        }

        public static Vector3 GetBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            return (uu * p0) + (2 * u * t * p1) + (tt * p2);
        }
    }
}