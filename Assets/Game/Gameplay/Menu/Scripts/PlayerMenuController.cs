using UnityEngine;
using UnityEngine.InputSystem;

public enum ControlScheme
{
    None,
    Keyboard,
    Gamepad
}

public class PlayerMenuController : MonoBehaviour
{
    private PlayerMenuInputController inputController = null;
    private PlayerInput playerInput = null;

    private CharacterSelectionController selectionController = null;
    private int playerIndex = -1;
    private bool isMainPlayer = false;

    private void Awake()
    {
        inputController = GetComponent<PlayerMenuInputController>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Start()
    {
        SlotPlayer slotPlayer = selectionController.GetSlotByIndex(playerIndex);

        ControlScheme control = GetControl();
        switch (control)
        {
            case ControlScheme.None:
                break;
            case ControlScheme.Keyboard:
                slotPlayer.InitButtons();

                if (isMainPlayer)
                {
                    selectionController.InitButtons();
                }
                break;
            case ControlScheme.Gamepad:
                inputController.onNextItem += slotPlayer.OnNextCharacter;
                inputController.onPreviousItem += slotPlayer.OnPreviousCharacter;
                inputController.onAccept += slotPlayer.OnConfirm;

                if (isMainPlayer)
                {
                    inputController.onStart += selectionController.onStart;
                    inputController.onAccept += selectionController.onBack;
                }
                break;
            default:
                break;
        }

        slotPlayer.OnJoinPlayer();
    }

    public void Init(CharacterSelectionController selectionController, int playerIndex, bool isMainPlayer = false)
    {
        this.selectionController = selectionController;
        this.playerIndex = playerIndex;
        this.isMainPlayer = isMainPlayer;
    }

    private ControlScheme GetControl()
    {
        ControlScheme control = ControlScheme.None;

        switch (playerInput.currentControlScheme)
        {
            case "Keyboard&Mouse":
                control = ControlScheme.Keyboard;
                break;
            case "Gamepad":
                control = ControlScheme.Gamepad;
                break;
            default:
                control = ControlScheme.None;
                break;
        }

        return control;
    }
}
