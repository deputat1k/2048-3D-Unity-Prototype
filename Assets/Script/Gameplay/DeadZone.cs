using UnityEngine;
using System;

namespace Cube2048.Gameplay
{
    public class DeadZone : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float timeToLose = 2.0f;

        public event Action OnZoneFilled;
        private int cubesInsideCount = 0;
        private float timer = 0f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Cube>() != null)
            {
                cubesInsideCount++;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<Cube>() != null)
            {
                cubesInsideCount--;
                if (cubesInsideCount < 0) cubesInsideCount = 0;

                if (cubesInsideCount == 0)
                {
                    timer = 0f;
                }
            }
        }

        private void Update()
        {

            if (cubesInsideCount > 0)
            {
                timer += Time.deltaTime;

                if (timer >= timeToLose)
                {
                    OnZoneFilled?.Invoke();

                    // Скидаємо, щоб не спамити
                    timer = 0f;
                    cubesInsideCount = 0;
                }
            }
        }
    }
}