using Cube2048.Core.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Cube2048.Features.AutoMerge
{
    public class MergeFXController : MonoBehaviour, IMergeFX
    {
        [Header("Settings")]
        [SerializeField] private ParticleSystem explosionPrefab;
        [SerializeField] private int poolSize = 10;

        private Queue<ParticleSystem> pool = new Queue<ParticleSystem>();

        private void Awake()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            if (explosionPrefab == null)
            {
                Debug.LogError("Prefab is missing!");
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
            item.gameObject.SetActive(false);
            pool.Enqueue(item);
            return item;
        }

        public void PlayExplosion(Vector3 position)
        {
            if (pool.Count == 0)
            {
                CreateNewItem();
            }

            ParticleSystem effect = pool.Dequeue();

            effect.transform.position = position;
            effect.gameObject.SetActive(true);
            effect.Play();

            pool.Enqueue(effect);
        }
    }
}