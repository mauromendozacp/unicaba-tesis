using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel = null;
    [SerializeField] private Button playBtn = null;
    [SerializeField] private Button quitBtn = null;
    [SerializeField] private AudioEvent menuMusic = null;

    [SerializeField] private CharacterSelectionController selectionController = null;

    private List<PlayerInput> players = new List<PlayerInput>();

    private void Start()
    {
        playBtn.onClick.AddListener(OnPlay);
        quitBtn.onClick.AddListener(OnQuit);

        selectionController.Init(GoToGameplay, OpenMenu, GetPlayersCount);

        GameManager.Instance.AudioManager.PlayAudio(menuMusic);
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        PlayerMenuController player = playerInput.GetComponent<PlayerMenuController>();
        if (player != null)
        {
            bool isFirstPlayer = GetPlayersCount() == 0;

            player.Init(selectionController, GetPlayersCount(), isFirstPlayer);

            if (playerInput.currentControlScheme == "Gamepad")
            {
                GameManager.Instance.CursorManager.InitCursor(GetPlayersCount(), playerInput);

                if (menuPanel.gameObject.activeSelf)
                {
                    if (isFirstPlayer)
                    {
                        GameManager.Instance.CursorManager.ToggleCursor(0, true);
                    }
                }
                else
                {
                    GameManager.Instance.CursorManager.ToggleCursor(GetPlayersCount(), true);
                }
            }
        }

        players.Add(playerInput);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        if (playerInput.currentControlScheme == "Gamepad")
        {
            int index = players.IndexOf(playerInput);
            GameManager.Instance.CursorManager.ToggleCursor(index, false);
        }

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

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].currentControlScheme == "Gamepad")
            {
                GameManager.Instance.CursorManager.ToggleCursor(i, true);
            }
        }
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
        GameManager.Instance.CursorManager.ToggleAllCursors(false);
        if (players[0].currentControlScheme == "Gamepad")
        {
            GameManager.Instance.CursorManager.ToggleCursor(0, true);
        }

        selectionController.Toggle(false);
        Toggle(transform);
    }

    private int GetPlayersCount()
    {
        return players.Count;
    }

    private void GoToGameplay()
    {
        GameManager.Instance.CursorManager.ToggleAllCursors(false);

        List<PlayerSelectionData> playersData = new List<PlayerSelectionData>();
        for (int i = 0; i < players.Count; i++)
        {
            PlayerSelectionData data = new PlayerSelectionData();
            int index = selectionController.Slots[i].CurrentCharacterIndex;
            data.device = players[i].devices[0];
            data.controlScheme = players[i].currentControlScheme;
            data.playerData = selectionController.PlayerDatas[index];

            GameManager.Instance.GameDataManager.playersData.Add(data);
        }

        GameManager.Instance.ChangeScene(SceneGame.Gameplay);
    }
}
