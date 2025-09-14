using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private GameplayUI gameplayUI = null;
    [SerializeField] private PlayerSpawn playerSpawn = null;

    private void Awake()
    {
        playerSpawn.Init(gameplayUI.GetPlayerUI);
    }
}
