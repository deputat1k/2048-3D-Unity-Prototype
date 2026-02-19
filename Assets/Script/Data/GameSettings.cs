using UnityEngine;
namespace Cube2048.Data
{
    [CreateAssetMenu(fileName = "NewGameSettings", menuName = "Game Settings", order = 51)]
    public class GameSettings : ScriptableObject
    {
        [Header("Movement Settings")]
        public float MoveSpeed = 0.2f;


        [Header("Mechanics")]
        public float PushForce = 20f;
        public float MergePushForce = 3f;

        [Header("Colors")]
        public Color[] CubeColors;

        [Range(1f, 50f)]
        public float SmoothTime = 15f;
    }
}