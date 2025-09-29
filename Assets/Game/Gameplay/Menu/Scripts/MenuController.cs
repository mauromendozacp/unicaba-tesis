using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private PlayerInputManager inputManager = null;

    [SerializeField] private GameObject menuPanel = null;
    [SerializeField] private Button playBtn = null;
    [SerializeField] private Button quitBtn = null;

    [SerializeField] private CharacterSelectionController selectionController = null;

    private List<PlayerInput> players = new List<PlayerInput>();

    private void Start()
    {
        playBtn.onClick.AddListener(OnPlay);
        quitBtn.onClick.AddListener(OnQuit);

        selectionController.Init(GoToGameplay, OpenMenu, GetPlayersCount);
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerMenuController player = playerInput.GetComponent<PlayerMenuController>();
        if (player != null)
        {
            player.Init(selectionController, players.Count, players.Count == 0);
        }

        players.Add(playerInput);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        players.Remove(playerInput);
    }

    public void InitSelectionButtons()
    {
        selectionController.InitButtons();
    }

    private void OnPlay()
    {
        selectionController.Toggle(true);
        Toggle(false);
    }

    private void OnQuit()
    {
        Application.Quit();
    }

    private void Toggle(bool status)
    {
        menuPanel.gameObject.SetActive(status);
    }

    private void OpenMenu()
    {
        selectionController.Toggle(false);
        Toggle(transform);
    }

    private int GetPlayersCount()
    {
        return players.Count;
    }

    private void GoToGameplay()
    {
        GameManager.Instance.ChangeScene(SceneGame.Gameplay);
    }
}
