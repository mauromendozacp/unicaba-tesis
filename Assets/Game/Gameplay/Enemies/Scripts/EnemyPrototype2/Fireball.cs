using UnityEngine;

public class Fireball : MonoBehaviour
{
  [SerializeField] private float damage = 20f;
  [SerializeField] private float lifetime = 5f;

  void Start()
  {
    // Destruye la bola de fuego después de 'lifetime' segundos
    Destroy(gameObject, lifetime);
  }

  void OnTriggerEnter(Collider other)
  {
    // Si choca con el jugador, le hace daño y se destruye
    if (other.CompareTag("Player"))
    {
      IDamageable player = other.GetComponent<IDamageable>();
      if (player != null)
      {
        player.TakeDamage(damage);
      }
      Destroy(gameObject);
    }
  }
}