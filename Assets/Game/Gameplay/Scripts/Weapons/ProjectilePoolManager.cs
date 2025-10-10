using System.Collections.Generic;
using UnityEngine;

public class ProjectilePoolManager : MonoBehaviour
{
    public static ProjectilePoolManager Instance { get; private set; }

    [System.Serializable]
    private class PreloadEntry
    {
        public Projectile prefab;
        public int initialSize = 16;
    }

    [SerializeField] private List<PreloadEntry> preload = new();

    private readonly Dictionary<Projectile, ProjectileObjectPool> pools = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Preload opcional
        foreach (var e in preload)
        {
            if (e.prefab == null || pools.ContainsKey(e.prefab)) continue;
            CreatePool(e.prefab, e.initialSize);
        }
    }

    private ProjectileObjectPool CreatePool(Projectile prefab, int size)
    {
        var go = new GameObject("Pool_" + prefab.name);
        go.transform.SetParent(transform);
        var pool = go.AddComponent<ProjectileObjectPool>();
        pool.SetPrefab(prefab, size);
        pools[prefab] = pool;
        return pool;
    }

    public ProjectileObjectPool GetPool(Projectile prefab, int initialSize = 8)
    {
        if (prefab == null) return null;
        if (pools.TryGetValue(prefab, out var pool)) return pool;
        return CreatePool(prefab, initialSize);
    }
}