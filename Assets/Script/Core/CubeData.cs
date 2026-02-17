using UnityEngine;

namespace Cube2048.Data
{

    public struct CubeData
    {
        public int InstanceID;  
        public Vector3 Position; 
        public int Value;       

        // Конструктор для зручності
        public CubeData(int id, Vector3 pos, int value)
        {
            InstanceID = id;
            Position = pos;
            Value = value;
        }
    }
}