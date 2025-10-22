using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawn
{
  [Tooltip("El prefab del enemigo a generar.")]
  public GameObject enemyPrefab;
  [Tooltip("La cantidad de este tipo de enemigo en la oleada.")]
  public int count;
}

[System.Serializable]
public class ItemDrop
{
  [Tooltip("El ItemData del ítem a dropear.")]
  public ItemData itemData;
  [Tooltip("La cantidad total de este ítem a dropear en la oleada.")]
  public int count;
}

[CreateAssetMenu(fileName = "newWave", menuName = "Wave/Wave")]
public class Wave : ScriptableObject
{
  [Tooltip("Lista de tipos y cantidades de enemigos para esta oleada.")]
  public List<EnemySpawn> enemiesInWave;
  [Tooltip("Tiempo de espera para la siguiente oleada después de esta.")]
  public float cooldownTime;
  //[Tooltip("Cantidad de ítems que serán dropeados durante el transcurso de la oleada.")]
  [Tooltip("Lista de items a dropear y sus cantidades para esta oleada.")]
  public List<ItemDrop> itemsToDropInWave;
}