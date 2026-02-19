using UnityEngine;

namespace Cube2048.Core.Interfaces
{
    public interface IMergeService
    {
        void ProcessMerge(Gameplay.Cube cubeA, Gameplay.Cube cubeB, Vector3 spawnPosition);
    }
}