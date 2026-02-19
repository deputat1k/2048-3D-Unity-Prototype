using UnityEngine;
using Cube2048.Data;
using Cube2048.Core;

namespace Cube2048.Features.AutoMerge
{
    public class MergeVisuals : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private LineRenderer lightningRenderer;

        [Header("Settings")]
        [SerializeField] private LightningSettings settings;
        [SerializeField] private Material lightningMaterial;

        private void Start()
        {
            SetupLineRenderer();
        }

        private void SetupLineRenderer()
        {
            if (lightningRenderer == null || settings == null) return;

            lightningRenderer.startColor = settings.Color;
            lightningRenderer.endColor = settings.Color;
            lightningRenderer.startWidth = settings.Width;
            lightningRenderer.endWidth = settings.Width;
            lightningRenderer.positionCount = settings.ArcSegments;
            lightningRenderer.enabled = false;

            if (lightningMaterial != null)
            {
                lightningRenderer.material = lightningMaterial;
            }
        }

        public void ShowLightning(Vector3 startPos, Vector3 endPos)
        {
            if (lightningRenderer == null || settings == null) return;

            lightningRenderer.enabled = true;
            lightningRenderer.startColor = settings.Color;
            lightningRenderer.endColor = settings.Color;

            Vector3 midPoint = (startPos + endPos) / 2f + Vector3.up * settings.ArcHeight;

            for (int i = 0; i < settings.ArcSegments; i++)
            {
                float t = i / (float)(settings.ArcSegments - 1);
                Vector3 pos = MathUtils.GetBezierPoint(t, startPos, midPoint, endPos);
                lightningRenderer.SetPosition(i, pos);
            }
        }

        public void HideLightning()
        {
            if (lightningRenderer != null) lightningRenderer.enabled = false;
        }
    }
}