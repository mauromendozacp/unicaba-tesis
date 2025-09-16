using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private InputActionAsset inputAsset = null;
    private InputActionMap playerMap = null;

    private Vector2 move = Vector2.zero;
    private InputAction fireAction = null;

    public Action onPause = null;
    public Action onEquipItem = null;
    public Action onUseItem = null;
    public Action onPreviousItem = null;
    public Action onNextItem = null;

    private void Awake()
    {
        inputAsset = GetComponent<PlayerInput>().actions;
        playerMap = inputAsset.FindActionMap("Player");
        fireAction = playerMap.FindAction("Fire");
    }

    void OnEnable()
    {
        playerMap.FindAction("Move").performed += OnMove;
        playerMap.FindAction("Move").canceled += OnStopMove;
        playerMap.FindAction("EquipItem").started += OnEquipItem;
        playerMap.FindAction("UseItem").started += OnUseItem;
        playerMap.FindAction("Previous").started += OnPreviousItem;
        playerMap.FindAction("Next").started += OnNextItem;
        playerMap.FindAction("Pause").started += OnPause;
        playerMap.Enable();
    }

    void OnDisable()
    {
        playerMap.FindAction("Move").performed -= OnMove;
        playerMap.FindAction("Move").canceled -= OnStopMove;
        playerMap.FindAction("EquipItem").started -= OnEquipItem;
        playerMap.FindAction("UseItem").started -= OnUseItem;
        playerMap.FindAction("Previous").started -= OnPreviousItem;
        playerMap.FindAction("Next").started -= OnNextItem;
        playerMap.FindAction("Pause").started -= OnPause;
        playerMap.Disable();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            move = ctx.ReadValue<Vector2>();
        }
    }

    private void OnStopMove(InputAction.CallbackContext ctx)
    {
        move = Vector2.zero;
    }

    private void OnEquipItem(InputAction.CallbackContext ctx)
    {
        onEquipItem?.Invoke();
    }

    private void OnUseItem(InputAction.CallbackContext ctx)
    {
        onUseItem?.Invoke();
    }

    private void OnPreviousItem(InputAction.CallbackContext ctx)
    {
        onPreviousItem?.Invoke();
    }

    private void OnNextItem(InputAction.CallbackContext ctx)
    {
        onNextItem?.Invoke();
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        onPause?.Invoke();
    }

    public Vector2 GetInputMove()
    {
        return move;
    }

    // Devuelve true mientras el botón de disparo esté presionado
    public bool GetInputFire()
    {
        return fireAction != null && fireAction.IsPressed();
    }
}
