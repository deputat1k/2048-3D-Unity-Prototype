using UnityEngine;

namespace Cube2048.Data
{
    public struct CubeData
    {
        public Vector3 Position;
        public int Value;
        public int InstanceID;

        public CubeData(Vector3 pos, int val, int id)
        {
            Position = pos;
            Value = val;
            InstanceID = id;
        }
    }
}