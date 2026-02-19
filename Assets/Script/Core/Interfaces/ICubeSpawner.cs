using System.Collections.Generic;
using UnityEngine;
using Cube2048.Gameplay;

namespace Cube2048.Core.Interfaces
{
    public interface ICubeSpawner
    {
        Cube Spawn();
        Cube SpawnSpecific(Vector3 position, int value);
        void ReturnToPool(Cube cube);

        IReadOnlyList<Gameplay.Cube> ActiveCubes { get; }
    }
}