using UnityEngine;

namespace Cube2048.Data
{
    
    public class LightningSettings : ScriptableObject
    {
        [Header("Visuals")]
        public Color Color = Color.cyan;
        public float Width = 0.1f;

        [Tooltip("Наскільки високо піднімати дугу")]
        public float ArcHeight = 2.0f; // 🔥 НОВЕ: Висота дуги
        [Tooltip("Скільки точок у лінії (більше = плавніше)")]
        public int ArcSegments = 20;   // 🔥 НОВЕ: Кількість сегментів

        [Header("Timing")]
        public float CheckInterval = 0.2f;

        [Header("Animation")]
        public float MergeAnimDuration = 1.0f;
        public float LiftHeight = 3.0f;
    }
}