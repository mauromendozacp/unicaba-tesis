using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMenuInputController : MonoBehaviour
{
    private InputActionAsset inputAsset = null;
    private InputActionMap playerMap = null;

    public Action onPreviousItem = null;
    public Action onNextItem = null;
    public Action onAccept = null;
    public Action onStart = null;
    public Action onBack = null;

    private void Awake()
    {
        inputAsset = GetComponent<PlayerInput>().actions;
        playerMap = inputAsset.FindActionMap("Player");
    }

    void OnEnable()
    {
        playerMap.FindAction("Previous").started += OnPreviousItem;
        playerMap.FindAction("Next").started += OnNextItem;
        playerMap.FindAction("Pause").started += OnStart;
        playerMap.FindAction("Accept").started += OnAccept;
        playerMap.FindAction("Back").started += OnBack;
        playerMap.Enable();
    }

    void OnDisable()
    {
        playerMap.FindAction("Previous").started -= OnPreviousItem;
        playerMap.FindAction("Next").started -= OnNextItem;
        playerMap.FindAction("Pause").started -= OnStart;
        playerMap.FindAction("Accept").started -= OnAccept;
        playerMap.FindAction("Back").started -= OnBack;
        playerMap.Disable();
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
