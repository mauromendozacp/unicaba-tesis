using System.Collections;
using UnityEngine;

// Asegura que el GameObject tenga un BoxCollider
[RequireComponent(typeof(BoxCollider))]
public class GrowCollider : MonoBehaviour
{
  private BoxCollider boxCollider;

  // --- Variables de configuración ---
  public float targetZSize = 11.5f;
  public float growDuration = 1.0f;
  // ---

  private Vector3 initialSize;
  private Vector3 initialCenter;

  void Awake()
  {
    // Obtener la referencia al BoxCollider
    boxCollider = GetComponent<BoxCollider>();

    // Guardar los valores iniciales de X e Y para no modificarlos
    initialSize = new Vector3(boxCollider.size.x, boxCollider.size.y, 0f);
    initialCenter = new Vector3(boxCollider.center.x, boxCollider.center.y, 0f);
  }

  void OnEnable()
  {
    StartGrowing();
  }

  void Start()
  {
    // Asegurarse de que el collider esté desactivado al empezar
    if (boxCollider != null)
    {
      boxCollider.enabled = false;
    }
  }

  /// <summary>
  /// Método público para iniciar la animación del collider.
  /// </summary>
  public void StartGrowing()
  {
    // Detiene cualquier corrutina anterior para evitar solapamientos
    StopCoroutine("GrowAndShrink");
    // Inicia la nueva corrutina
    StartCoroutine(GrowAndShrink());
  }

  private IEnumerator GrowAndShrink()
  {
    // 1. Activar el collider y resetear su tamaño a 0 en Z
    boxCollider.enabled = true;
    boxCollider.size = initialSize;
    boxCollider.center = initialCenter;

    float elapsedTime = 0f;

    // Definir los estados iniciales y finales
    Vector3 startSize = initialSize;
    Vector3 endSize = new Vector3(initialSize.x, initialSize.y, targetZSize);

    Vector3 startCenter = initialCenter;
    // El centro debe moverse la mitad del tamaño en Z para que crezca desde el origen
    Vector3 endCenter = new Vector3(initialCenter.x, initialCenter.y, targetZSize / 2f);

    // 2. Bucle de crecimiento durante el tiempo especificado
    while (elapsedTime < growDuration)
    {
      // Incrementar el tiempo transcurrido
      elapsedTime += Time.deltaTime;

      // Calcular el progreso (de 0.0 a 1.0)
      float t = elapsedTime / growDuration;
      t = Mathf.Clamp01(t); // Asegurar que t no pase de 1

      // Interpolar (Lerp) el tamaño y el centro
      boxCollider.size = Vector3.Lerp(startSize, endSize, t);
      boxCollider.center = Vector3.Lerp(startCenter, endCenter, t);

      // Esperar al siguiente frame
      yield return null;
    }

    // Asegurarse de que los valores finales sean exactos
    boxCollider.size = endSize;
    boxCollider.center = endCenter;

    // 3. Desactivar el collider
    boxCollider.enabled = false;
  }
}