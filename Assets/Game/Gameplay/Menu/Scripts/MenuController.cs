using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private SectionPanel[] panels = null;
    [SerializeField] private Button playBtn = null;
    [SerializeField] private Button optionsBtn = null;
    [SerializeField] private Button creditsBtn = null;
    [SerializeField] private Button tutorialBtn = null;
    [SerializeField] private Button controlBtn = null;
    [SerializeField] private Button quitBtn = null;
    [SerializeField] private Button[] backMenuBtns = null;
    [SerializeField] private Button[] backOptionsBtns = null;
    [SerializeField] private OptionSound musicOption = null;
    [SerializeField] private OptionSound sfxOption = null;
    [SerializeField] private AudioEvent menuMusic = null;

    [SerializeField] private CharacterSelectionController selectionController = null;

    private PANEL_TYPE currentPanel = default;

    private List<PlayerInput> players = new List<PlayerInput>();

    private void Start()
    {
        playBtn.onClick.AddListener(OnPlay);
        optionsBtn.onClick.AddListener(() => ShowPanel(PANEL_TYPE.OPTIONS));
        creditsBtn.onClick.AddListener(() => ShowPanel(PANEL_TYPE.CREDITS));
        tutorialBtn.onClick.AddListener(() => ShowPanel(PANEL_TYPE.TUTORIAL));
        controlBtn.onClick.AddListener(() => ShowPanel(PANEL_TYPE.CONTROLS));
        quitBtn.onClick.AddListener(OnQuit);

        for (int i = 0; i < backMenuBtns.Length; i++)
        {
            backMenuBtns[i].onClick.AddListener(() => ShowPanel(PANEL_TYPE.MENU));
        }

        for (int i = 0; i < backMenuBtns.Length; i++)
        {
            backOptionsBtns[i].onClick.AddListener(() => ShowPanel(PANEL_TYPE.OPTIONS));
        }

        selectionController.Init(GoToGameplay, OpenMenu);

        AudioManager audioManager = GameManager.Instance.AudioManager;
        musicOption.Init(audioManager.MusicEnabled, audioManager.MusicVolume, audioManager.UpdateMusicVolume, audioManager.ToggleMusic);
        sfxOption.Init(audioManager.SfxEnabled, audioManager.SfxVolume, audioManager.UpdateSfxVolume, audioManager.ToggleSFX);

        GameManager.Instance.AudioManager.PlayAudio(menuMusic);

        ShowPanel(PANEL_TYPE.MENU);
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

                if (currentPanel == PANEL_TYPE.MENU)
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

    private void ShowPanel(PANEL_TYPE type)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].Toggle(panels[i].Type == type);
        }

        currentPanel = type;
    }

    public void InitSelectionButtons()
    {
        selectionController.InitButtons();
    }

    private void OnPlay()
    {
        ShowPanel(PANEL_TYPE.SELECTION);

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

    private void OpenMenu()
    {
        GameManager.Instance.CursorManager.ToggleAllCursors(false);
        if (players[0].currentControlScheme == "Gamepad")
        {
            GameManager.Instance.CursorManager.ToggleCursor(0, true);
        }

        ShowPanel(PANEL_TYPE.MENU);
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

        for (int i = 0; i < backOptionsBtns.Length; i++)
        {
            backOptionsBtns[i].onClick.RemoveAllListeners();
        }

        backOptionsBtns[0].onClick.AddListener(() => ShowPanel(PANEL_TYPE.CONTROLS));
        backOptionsBtns[1].onClick.AddListener(() => GameManager.Instance.ChangeScene(SceneGame.Gameplay));

        ShowPanel(PANEL_TYPE.TUTORIAL);
    }
}
