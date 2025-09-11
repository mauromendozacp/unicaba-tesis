using UnityEngine;

[CreateAssetMenu(menuName = "Player")]
public class PlayerData : ScriptableObject
{
    [SerializeField] private string playerName = string.Empty;
    [SerializeField] private int maxLife = 0;
    [SerializeField] private float speed = 0f;
    [SerializeField] private Sprite icon = null;

    public string PlayerName => playerName;
    public int MaxLife => maxLife;
    public float Speed => speed;
    public Sprite Icon => icon;
}
