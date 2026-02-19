using UnityEngine;

namespace Cube2048.Data
{
    
    public class LightningSettings : ScriptableObject
    {
        [Header("Visuals")]
        public Color Color = Color.cyan;
        public float Width = 0.1f;

        [Header("Settings")]
        public float ArcHeight = 2.0f; //Висота 
        public int ArcSegments = 20;   // кількість сегментів (чим більше плавніше)

        [Header("Timing")]
        public float CheckInterval = 0.2f;

        [Header("Animation")]
        public float MergeAnimDuration = 1.0f;
        public float LiftHeight = 3.0f;
    }
}