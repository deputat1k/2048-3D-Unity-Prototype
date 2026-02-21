using UnityEngine;
using DigitalRuby.LightningBolt;

namespace Cube2048.Features.AutoMerge
{
    public class MergeVisuals : MonoBehaviour
    {
        [Header("Lightning Asset")]
        [SerializeField] private LightningBoltScript lightningBolt;

        public void ShowLightning(Transform startObj, Transform endObj)
        {
            if (lightningBolt == null || startObj == null || endObj == null) return;

            lightningBolt.gameObject.SetActive(true);
            lightningBolt.StartObject = startObj.gameObject;
            lightningBolt.EndObject = endObj.gameObject;
            lightningBolt.StartPosition = Vector3.zero;
            lightningBolt.EndPosition = Vector3.zero;

        }

        public void HideLightning()
        {
            if (lightningBolt != null)
            {
                lightningBolt.gameObject.SetActive(false);
                lightningBolt.StartObject = null;
                lightningBolt.EndObject = null;
            }
        }
    }
}