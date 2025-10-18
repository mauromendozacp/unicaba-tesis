using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputUIController : MonoBehaviour
{
    protected InputActionAsset inputAsset = null;
    private InputActionMap uiMap = null;

    public Action<Vector2> onNavigate = null;
    public Action onClick = null;

    protected virtual void Awake()
    {
        inputAsset = GetComponent<PlayerInput>().actions;
        uiMap = inputAsset.FindActionMap("UI");
    }

    protected virtual void OnEnable()
    {
        uiMap.FindAction("Navigate").performed += OnNavigate;
        uiMap.FindAction("Click").started += OnClick;

        uiMap.Enable();
    }

    protected virtual void OnDisable()
    {
        uiMap.FindAction("Navigate").performed -= OnNavigate;
        uiMap.FindAction("Click").started -= OnClick;

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
}
