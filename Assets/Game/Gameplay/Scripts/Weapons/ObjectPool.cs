using System.Collections.Generic;
using UnityEngine;

public class ProjectileObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private T prefab;
    [SerializeField] private int initialSize = 10;

    private Queue<T> pool = new Queue<T>();

    private void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            T obj = Instantiate(prefab, transform);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        if (pool.Count > 0)
        {
            T obj = pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        else
        {
            T obj = Instantiate(prefab, transform);
            return obj;
        }
    }

    public void ReturnToPool(T obj)
    {
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
