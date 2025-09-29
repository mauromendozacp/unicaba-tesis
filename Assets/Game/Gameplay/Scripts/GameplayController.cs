using UnityEngine;

public class GameplayController : MonoBehaviour
{
    [SerializeField] private GameplayUI gameplayUI = null;
    [SerializeField] private PlayerSpawn playerSpawn = null;

    private void Awake()
    {
        playerSpawn.Init(gameplayUI.GetPlayerUI, () => 
        { 
            TogglePause(true);
            gameplayUI.TogglePause(true);
        });
    }

    private void Start()
    {
        gameplayUI.Init(() => { TogglePause(false); }, GoToMenu);
    }

    private void TogglePause(bool status)
    {
        Time.timeScale = status ? 0f : 1f;
    }

    private void GoToMenu()
    {
        GameManager.Instance.ChangeScene(SceneGame.Menu);
    }
}
