using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class JoystickCursor : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform cursorRect;
    public RectTransform canvasRect;
    public Image cursorIcon;
    public float speed = 800f;
    public float clickRadius = 15f;

    private Vector2 moveInput;
    private Vector2 cursorPos;
    private Vector2 startAnchoredPos;

    private Camera uiCamera;

    private int playerIndex = -1;
    private bool isEnabled = false;

    private void Start()
    {
        if (canvasRect == null)
            canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        var canvas = canvasRect.GetComponent<Canvas>();
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;

        cursorPos = new Vector2(Screen.width / 2f, Screen.height / 2f);

        startAnchoredPos = cursorRect.anchoredPosition;
    }

    private void Update()
    {
        if (!isEnabled) return;

        MoveCursor();
    }

    public void Init(PlayerInput playerInput, int playerIndex)
    {
        PlayerInputUIController inputController = playerInput.gameObject.GetComponent<PlayerInputUIController>();
        inputController.onNavigate += SetMoveInput;
        inputController.onClick += TryClick;

        this.playerIndex = playerIndex;
    }

    private void SetMoveInput(Vector2 move)
    {
        moveInput = move;
    }

    private void MoveCursor()
    {
        cursorPos += moveInput * speed * Time.unscaledDeltaTime;
        cursorPos.x = Mathf.Clamp(cursorPos.x, 0, Screen.width);
        cursorPos.y = Mathf.Clamp(cursorPos.y, 0, Screen.height);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, cursorPos, uiCamera, out Vector2 anchoredPos);
        cursorRect.anchoredPosition = anchoredPos;
    }

    private void TryClick()
    {
        if (!isEnabled) return;

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = cursorPos
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            var buttonUI = result.gameObject.GetComponent<ButtonUI>();
            if (buttonUI && buttonUI.PlayerOwnerIndex == playerIndex)
            {
                buttonUI.Button.onClick.Invoke();
                break;
            }
        }
    }

    public void Toggle(bool status)
    {
        cursorRect.anchoredPosition = startAnchoredPos;
        cursorIcon?.gameObject.SetActive(status);
        isEnabled = status;
    }
}
