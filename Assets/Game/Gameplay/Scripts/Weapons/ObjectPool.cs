using System.Collections.Generic;
using UnityEngine;

public class ProjectileObjectPool : MonoBehaviour
{
    [SerializeField] private Projectile prefab;
    [SerializeField] private int initialSize = 16;

    private readonly Queue<Projectile> pool = new();
    private bool initialized = false;

    private void Awake()
    {
        if (prefab != null && !initialized)
            Init(prefab, initialSize);
    }

    public void SetPrefab(Projectile p, int size)
    {
        if (initialized) return;
        prefab = p;
        initialSize = size;
        Init(prefab, initialSize);
    }

    private void Init(Projectile p, int size)
    {
        if (p == null) return;
        initialized = true;
        for (int i = 0; i < size; i++)
        {
            var inst = Create();
            inst.gameObject.SetActive(false);
            pool.Enqueue(inst);
        }
    }

    private Projectile Create()
    {
        var p = Instantiate(prefab, transform);
        p.SetPool(this);
        return p;
    }

    public Projectile Get()
    {
        if (!initialized) Init(prefab, initialSize);
        Projectile p = pool.Count > 0 ? pool.Dequeue() : Create();
        p.gameObject.SetActive(true);
        return p;
    }

    public void ReturnToPool(Projectile p)
    {
        if (p == null) return;
        p.gameObject.SetActive(false);
        pool.Enqueue(p);
    }
}
