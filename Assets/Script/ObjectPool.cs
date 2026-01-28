using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : MonoBehaviour
{
    private T prefab;              
    private Transform parent;     
    private Queue<T> pool = new Queue<T>();

   
    public ObjectPool(T prefab, int initialSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;

     
        for (int i = 0; i < initialSize; i++)
        {
            CreateObject();
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
        T newObj = Object.Instantiate(prefab, parent);
        newObj.gameObject.SetActive(false); 
        pool.Enqueue(newObj);
        return newObj;
    }
}