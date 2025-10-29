using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PoolableItem))]
public class ExplosionLifetime : MonoBehaviour
{
  [SerializeField] private float lifetime = 2f;
  private PoolableItem poolableItem;

  private void Awake()
  {
    poolableItem = GetComponent<PoolableItem>();
    if (poolableItem == null)
    {
      Debug.LogError("ExplosionLifetime requiere un componente PoolableItem.", this);
    }
  }

  private void OnEnable()
  {
    // Cada vez que se saca del pool y se activa, iniciamos la cuenta atrás
    StartCoroutine(ReturnToPoolAfterDelay());
  }

  private void OnDisable()
  {
    // Limpiamos la corrutina por si se desactiva antes de tiempo
    StopAllCoroutines();
  }

  private IEnumerator ReturnToPoolAfterDelay()
  {
    yield return new WaitForSeconds(lifetime);

    // Usamos el PoolableItem para devolverlo al pool correcto
    if (poolableItem != null)
    {
      poolableItem.ReturnToPool();
    }
    else
    {
      // Fallback por si algo salió mal
      Destroy(gameObject);
    }
  }
}