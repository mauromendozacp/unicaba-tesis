using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class EnemyManager : MonoBehaviour
{
  public event Action<int, int> OnWavesStart; // Oleada actual, Total de oleadas
  public event Action OnWaveStart;
  public event Action OnWavesEnd;

  [Header("Control de Inicio")]
  [Tooltip("Si está activado, las oleadas comenzarán automáticamente al inicio del juego. De lo contrario, se deben iniciar llamando a StartEnemyWaves().")]
  public bool autoStartWaves = false;

  [Header("General")]
  [Tooltip("Lista general de todos los prefabs de enemigos.")]
  public List<GameObject> enemyPrefabs;

  [Header("Configuración de Oleadas")]
  [Tooltip("Lista de todas las oleadas de enemigos.")]
  public List<Wave> waves;
  [Tooltip("El tamaño máximo que puede alcanzar el pool.")]
  public int poolMaxSize = 50;

  [Header("Puntos de Spawn")]
  [Tooltip("Lista de Transform de los puntos centrales para el spawn.")]
  public List<Transform> spawnCenters;
  [Tooltip("El radio máximo desde un centro donde se puede instanciar un enemigo.")]
  public float maxSpawnRadius = 5f;

  [Header("Controller to drop items")]
  [SerializeField] ItemController itemController;

  private Dictionary<string, GameObject> enemyPrefabDict;
  private Dictionary<string, IObjectPool<GameObject>> enemyPoolDict;
  private Transform poolParent;
  private int currentWaveIndex = 0;
  private int enemiesToSpawnInCurrentWave;
  private int enemiesAlive;
  private Dictionary<int, ItemData> dropMap;
  private List<int> dropIndices;

  public static EnemyManager Instance;

  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      poolParent = transform;
    }
    else
    {
      Destroy(gameObject);
    }

    enemyPrefabDict = new Dictionary<string, GameObject>();
    foreach (GameObject prefab in enemyPrefabs)
    {
      if (prefab != null)
      {
        enemyPrefabDict[prefab.name] = prefab;
      }
    }
    enemyPoolDict = new Dictionary<string, IObjectPool<GameObject>>();
  }

  void Start()
  {
    if (itemController == null)
    {
      itemController = FindFirstObjectByType<ItemController>();
    }
    InitializePool();
    if (autoStartWaves)
    {
      StartEnemyWaves();
    }
  }

  public void StartEnemyWaves()
  {
    if (waves.Count > 0)
    {
      StopCoroutine(nameof(StartWaves));
      StartCoroutine(nameof(StartWaves));
    }
  }

  void InitializePool()
  {
    foreach (GameObject prefab in enemyPrefabs)
    {
      if (!enemyPoolDict.ContainsKey(prefab.name))
      {
        // Crea un pool para cada prefab de enemigo.
        var pool = new ObjectPool<GameObject>(
            createFunc: () => CreatePooledItem(prefab),
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: false,
            defaultCapacity: 20,
            maxSize: poolMaxSize
        );
        enemyPoolDict.Add(prefab.name, pool);
      }
    }
  }


  private GameObject CreatePooledItem(GameObject prefabToInstantiate)
  {
    GameObject enemy = Instantiate(prefabToInstantiate, poolParent);

    EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
    if (enemyScript != null)
    {
      // Se modifica para que use el pool correspondiente al tipo de enemigo.
      enemyScript.SetPool(enemyPoolDict[prefabToInstantiate.name]);
    }
    return enemy;
  }
  private void OnTakeFromPool(GameObject enemy)
  {
    enemy.SetActive(true);
  }

  private void OnReturnToPool(GameObject enemy)
  {
    enemy.SetActive(false);
  }

  private void OnDestroyPoolObject(GameObject enemy)
  {
#if UNITY_EDITOR
    DestroyImmediate(enemy);
#else
    Destroy(enemy);
#endif
  }

  IEnumerator StartWaves()
  {
    for (currentWaveIndex = 0; currentWaveIndex < waves.Count; currentWaveIndex++)
    {
      OnWavesStart?.Invoke(currentWaveIndex + 1, waves.Count);
      OnWaveStart?.Invoke();

      yield return StartCoroutine(SpawnCurrentWave());

      yield return new WaitForSeconds(waves[currentWaveIndex].cooldownTime);
    }

    OnWavesEnd?.Invoke();
  }

  IEnumerator SpawnCurrentWave()
  {
    if (waves.Count == 0 || currentWaveIndex >= waves.Count)
    {
      yield break;
    }

    Wave currentWave = waves[currentWaveIndex];
    enemiesToSpawnInCurrentWave = 0;

    foreach (var waveContent in currentWave.enemiesInWave)
    {
      enemiesToSpawnInCurrentWave += waveContent.count;
    }

    enemiesAlive = enemiesToSpawnInCurrentWave;

    dropIndices = new List<int>();
    dropMap = new Dictionary<int, ItemData>();

    List<ItemData> totalItemsToDrop = new List<ItemData>();
    foreach (var itemDrop in currentWave.itemsToDropInWave)
    {
      for (int i = 0; i < itemDrop.count; i++)
      {
        if (itemDrop.itemData != null)
        {
          totalItemsToDrop.Add(itemDrop.itemData);
        }
      }
    }

    // Determinar cuáles de los N enemigos serán los que dropeen
    int totalDropsCount = totalItemsToDrop.Count;

    if (totalDropsCount > 0 && enemiesToSpawnInCurrentWave > 0)
    {
      int actualDrops = Mathf.Min(totalDropsCount, enemiesToSpawnInCurrentWave);

      List<int> availableKillIndices = new List<int>();
      for (int i = 1; i <= enemiesToSpawnInCurrentWave; i++)
      {
        availableKillIndices.Add(i);
      }

      for (int i = 0; i < actualDrops; i++)
      {
        ItemData itemToDrop = totalItemsToDrop[i];

        // Seleccionar un índice de muerte aleatorio de los disponibles
        int randomIndex = UnityEngine.Random.Range(0, availableKillIndices.Count);
        int killIndex = availableKillIndices[randomIndex];

        // Asignar el ItemData a ese índice de muerte
        dropMap.Add(killIndex, itemToDrop);

        // Remover el índice para que no se repita
        availableKillIndices.RemoveAt(randomIndex);
      }
    }

    foreach (var enemySpawn in currentWave.enemiesInWave)
    {
      string enemyPrefabName = enemySpawn.enemyPrefab.name;
      if (enemyPoolDict.ContainsKey(enemyPrefabName))
      {
        var specificEnemyPool = enemyPoolDict[enemyPrefabName];
        for (int i = 0; i < enemySpawn.count; i++)
        {
          GameObject enemyToSpawn = specificEnemyPool.Get();
          if (enemyToSpawn != null)
          {
            enemyToSpawn.transform.position = GetRandomSpawnPosition();
            enemyToSpawn.SetActive(true);
          }
        }
      }
      else
      {
        Debug.LogError($"Pool para el enemigo '{enemyPrefabName}' no encontrado. Asegúrate de que el prefab está en la lista general de prefabs del EnemyManager.");
      }
    }

    while (enemiesAlive > 0)
    {
      yield return null;
    }
  }

  Vector3 GetRandomSpawnPosition()
  {
    Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * maxSpawnRadius; ;
    if (spawnCenters.Count == 0)
    {
      Debug.LogWarning("No hay centros de spawn definidos. Usando la posición del EnemyManager como fallback.");
      return transform.position + new Vector3(randomCircle.x, 1.2f, randomCircle.y);
    }

    Transform selectedCenter = spawnCenters[UnityEngine.Random.Range(0, spawnCenters.Count)];
    return selectedCenter.position + new Vector3(randomCircle.x, 1.2f, randomCircle.y);
  }

  public void OnEnemyKilled(Vector3 enemyPosition)
  {
    enemiesAlive--;
    if (enemiesAlive >= 0)
    {
      // El índice de muerte es el número de enemigos que han muerto en la oleada (empezando en 1)
      int currentKillIndex = enemiesToSpawnInCurrentWave - enemiesAlive;

      // Chequear si este índice de muerte estaba en nuestro mapa de drops
      if (dropMap != null && dropMap.ContainsKey(currentKillIndex))
      {
        ItemData itemToDrop = dropMap[currentKillIndex];
        //dropMap.Remove(currentKillIndex); // remover la entrada si ya se usó.

        if (itemController != null)
        {
          Vector3 itemPosition = new Vector3(enemyPosition.x, 1, enemyPosition.z);
          itemController.SpawnItem(itemToDrop, itemPosition);
          //Debug.Log($"Item '{itemToDrop.name}' dropped at kill index {currentKillIndex} at position {itemPosition} and wave {currentWaveIndex + 1}");
        }
      }
    }
  }

  public void KillAllEnemies()
  {
    StopAllCoroutines();
    EnemyBase[] activeEnemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

    foreach (EnemyBase enemy in activeEnemies)
    {
      if (enemy.IsAlive)
      {
        enemy.Kill();
      }

      enemiesAlive = 0;
    }
  }
}