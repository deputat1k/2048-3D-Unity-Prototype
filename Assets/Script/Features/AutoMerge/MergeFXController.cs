using Cube2048.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Cube2048.Features.AutoMerge
{
    public class MergeFXController : MonoBehaviour, IMergeFX
    {
        [Header("Settings")]
        [SerializeField] private ParticleSystem explosionPrefab; // Префаб ефекту
        [SerializeField] private int poolSize = 10; // Скільки тримати в пам'яті

        // Черга готових ефектів
        private Queue<ParticleSystem> pool = new Queue<ParticleSystem>();

        private void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            if (explosionPrefab == null)
            {
                Debug.LogError("MergeFXController: Prefab is missing!");
                return;
            }

            for (int i = 0; i < poolSize; i++)
            {
                CreateNewItem();
            }
        }

        private ParticleSystem CreateNewItem()
        {
            ParticleSystem item = Instantiate(explosionPrefab, transform);
            item.gameObject.SetActive(false); // Ховаємо одразу
            pool.Enqueue(item);
            return item;
        }

        public void PlayExplosion(Vector3 position)
        {
            if (pool.Count == 0)
            {
                // Якщо пул пустий - створюємо додатковий (резервний)
                CreateNewItem();
            }

            // Дістаємо з черги
            ParticleSystem effect = pool.Dequeue();

            // Активуємо
            effect.transform.position = position;
            effect.gameObject.SetActive(true);
            effect.Play();

            // Повертаємо в кінець черги (для наступного разу)
            // Оскільки ми поставили "Stop Action: Disable" в Unity, 
            // він сам вимкнеться, а ми його просто знову використаємо.
            pool.Enqueue(effect);
        }
    }
}