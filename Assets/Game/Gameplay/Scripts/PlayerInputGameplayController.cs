using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputGameplayController : PlayerInputUIController
{
    private InputActionMap playerMap = null;

    private Vector2 move = Vector2.zero;
    private InputAction fireAction = null;
    private InputAction reviveAction = null;
    private InputAction lookAction = null;
    public Action onPause = null;
    public Action onEquipItem = null;
    public Action onUseItem = null;
    public Action onPreviousItem = null;
    public Action onNextItem = null;
    public Action<bool> onRevive = null;

    private PlayerInput playerInput = null;

    public InputActionMap PlayerMap => playerMap;

    protected override void Awake()
    {
        base.Awake();

        playerInput = GetComponent<PlayerInput>();
        playerMap = inputAsset.FindActionMap("Player");
        fireAction = playerMap.FindAction("Fire");
        reviveAction = playerMap.FindAction("Revive");
        lookAction = playerMap.FindAction("Look");
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        playerMap.FindAction("Move").performed += OnMove;
        playerMap.FindAction("Move").canceled += OnStopMove;
        playerMap.FindAction("EquipItem").started += OnEquipItem;
        playerMap.FindAction("UseItem").started += OnUseItem;
        playerMap.FindAction("Previous").started += OnPreviousItem;
        playerMap.FindAction("Next").started += OnNextItem;
        playerMap.FindAction("Pause").started += OnPause;
        reviveAction.performed += OnRevive;
        reviveAction.canceled += OnReviveCanceled;
        playerMap.Enable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        playerMap.FindAction("Move").performed -= OnMove;
        playerMap.FindAction("Move").canceled -= OnStopMove;
        playerMap.FindAction("EquipItem").started -= OnEquipItem;
        playerMap.FindAction("UseItem").started -= OnUseItem;
        playerMap.FindAction("Previous").started -= OnPreviousItem;
        playerMap.FindAction("Next").started -= OnNextItem;
        playerMap.FindAction("Pause").started -= OnPause;
        reviveAction.performed -= OnRevive;
        reviveAction.canceled -= OnReviveCanceled;
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

    public bool GetInputFire()
    {
        return fireAction != null && fireAction.IsPressed();
    }

    private void OnRevive(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            onRevive?.Invoke(true);
        }
    }

    private void OnReviveCanceled(InputAction.CallbackContext ctx)
    {
        onRevive?.Invoke(false);
    }

    public Vector2 GetRightStickAim()
    {
        if (playerInput == null || playerInput.devices.Count == 0) return Vector2.zero;
        var gp = playerInput.devices.OfType<Gamepad>().FirstOrDefault();
        return gp != null ? gp.rightStick.ReadValue() : Vector2.zero;
    }

    public Vector2 GetLookInput()
    {
        return lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;
    }

    public Vector2 GetPointerScreenPosition()
    {
        if (playerInput == null || playerInput.devices.Count == 0) return Vector2.zero;
        var mouse = playerInput.devices.OfType<Mouse>().FirstOrDefault();
        return mouse != null ? mouse.position.ReadValue() : Vector2.zero;
    }
}
