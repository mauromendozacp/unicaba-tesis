using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMenuInputController : MonoBehaviour
{
    private InputActionAsset inputAsset = null;
    private InputActionMap playerMap = null;
    private InputActionMap uiMap = null;

    public Action onPreviousItem = null;
    public Action onNextItem = null;
    public Action onAccept = null;
    public Action onStart = null;
    public Action onBack = null;

    public Action<Vector2> onNavigate = null;
    public Action onClick = null;

    private void Awake()
    {
        inputAsset = GetComponent<PlayerInput>().actions;
        playerMap = inputAsset.FindActionMap("Player");
        uiMap = inputAsset.FindActionMap("UI");
    }

    void OnEnable()
    {
        /*playerMap.FindAction("Previous").started += OnPreviousItem;
        playerMap.FindAction("Next").started += OnNextItem;
        playerMap.FindAction("Pause").started += OnStart;
        playerMap.FindAction("Fire").started += OnAccept;
        playerMap.FindAction("Back").started += OnBack;*/

        uiMap.FindAction("Navigate").performed += OnNavigate;
        uiMap.FindAction("Click").started += OnClick;

        playerMap.Enable();
        uiMap.Enable();
    }

    void OnDisable()
    {
        /*playerMap.FindAction("Previous").started -= OnPreviousItem;
        playerMap.FindAction("Next").started -= OnNextItem;
        playerMap.FindAction("Pause").started -= OnStart;
        playerMap.FindAction("Fire").started -= OnAccept;
        playerMap.FindAction("Back").started -= OnBack;*/

        uiMap.FindAction("Navigate").performed -= OnNavigate;
        uiMap.FindAction("Click").started -= OnClick;

        playerMap.Disable();
        uiMap.Disable();
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        onNavigate?.Invoke(ctx.ReadValue<Vector2>());
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        onClick?.Invoke();
    }

    private void OnPreviousItem(InputAction.CallbackContext ctx)
    {
        onPreviousItem?.Invoke();
    }

    private void OnNextItem(InputAction.CallbackContext ctx)
    {
        onNextItem?.Invoke();
    }

    private void OnStart(InputAction.CallbackContext ctx)
    {
        onStart?.Invoke();
    }

    private void OnAccept(InputAction.CallbackContext ctx)
    {
        onAccept?.Invoke();
    }

    private void OnBack(InputAction.CallbackContext ctx)
    {
        onBack?.Invoke();
    }
}
