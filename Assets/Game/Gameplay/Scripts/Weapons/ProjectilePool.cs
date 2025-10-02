using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    [SerializeField] private ProjectileObjectPool internalPool;

    public Projectile Get()
    {
        return internalPool.Get();
    }

    public void ReturnToPool(Projectile p)
    {
        internalPool.ReturnToPool(p);
    }
}
