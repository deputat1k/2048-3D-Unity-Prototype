using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Cube2048.Core
{
    public class ObjectPool<T> where T : MonoBehaviour
    {
        private T prefab;
        private Transform parent;
        private Queue<T> pool = new Queue<T>();
        private DiContainer container;

        public ObjectPool(T prefab, int initialSize, Transform parent, DiContainer container)
        {
            this.prefab = prefab;
            this.parent = parent;
            this.container = container;

            for (int i = 0; i < initialSize; i++)
            {
                T obj = CreateObject();
                pool.Enqueue(obj);
            }
        }

        public T GetElement()
        {
            if (pool.Count > 0)
            {
                T element = pool.Dequeue();
                element.gameObject.SetActive(true);
                return element;
            }

            T newElement = CreateObject();
            newElement.gameObject.SetActive(true);
            return newElement;
        }

        public void ReturnElement(T element)
        {
            element.gameObject.SetActive(false);
            pool.Enqueue(element);
        }

        private T CreateObject()
        {
            T newObj = container.InstantiatePrefabForComponent<T>(prefab, parent);
            newObj.gameObject.SetActive(false);
            return newObj;
        }
    }
}