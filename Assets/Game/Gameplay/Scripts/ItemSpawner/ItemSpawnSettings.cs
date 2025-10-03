using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSpawnSettings
{
    public string Name = "New Item";
    public ItemData ItemData;
    public List<Vector3> SpawnPositions = new List<Vector3>();
    public float RespawnTime = 0f;
}