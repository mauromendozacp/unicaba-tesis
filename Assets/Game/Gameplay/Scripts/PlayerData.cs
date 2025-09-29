using UnityEngine;

[CreateAssetMenu(menuName = "Player")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private string playerName = string.Empty;
    [SerializeField] private int maxLife = 0;
    [SerializeField] private float speed = 0f;
    [SerializeField] private Sprite icon = null;
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private Vector3 positionOffset = Vector3.zero;

    public string PlayerName => playerName;
    public int MaxLife => maxLife;
    public float Speed => speed;
    public Sprite Icon => icon;
    public GameObject Prefab => prefab;
    public Vector3 PositionOffset => positionOffset;
}
