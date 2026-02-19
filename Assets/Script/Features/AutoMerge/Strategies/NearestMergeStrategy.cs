using System.Collections.Generic;
using UnityEngine;
using Cube2048.Core.Interfaces;
using Cube2048.Data;

namespace Cube2048.Features.AutoMerge.Strategies
{
    public class NearestMergeStrategy : IMergeStrategy
    {
        public (int indexA, int indexB) FindBestPair(List<CubeData> cubes)
        {
            float minDistance = float.MaxValue;
            int bestA = -1;
            int bestB = -1;

            for (int i = 0; i < cubes.Count; i++)
            {
                for (int j = i + 1; j < cubes.Count; j++)
                {
                    if (cubes[i].Value == cubes[j].Value)
                    {
                        float dist = Vector3.Distance(cubes[i].Position, cubes[j].Position);

                        if (dist < minDistance)
                        {
                            minDistance = dist;
                            bestA = i;
                            bestB = j;
                        }
                    }
                }
            }

            return (bestA, bestB);
        }
    }
}