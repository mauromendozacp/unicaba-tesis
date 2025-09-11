using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float damage = 10f;

    private Vector3 direction;
    private float timer;
    private ProjectileObjectPool<Projectile> pool;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    public void SetPool(ProjectileObjectPool<Projectile> poolRef)
    {
        pool = poolRef;
    }

    private void OnEnable()
    {
        timer = 0f;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            ReturnToPool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (pool != null)
        {
            pool.ReturnToPool(this);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public float GetDamage() => damage;
}
