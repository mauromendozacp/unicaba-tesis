using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private InputActionAsset inputAsset = null;
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

    private void Awake()
    {
        inputAsset = GetComponent<PlayerInput>().actions;
        playerMap = inputAsset.FindActionMap("Player");
        fireAction = playerMap.FindAction("Fire");
        reviveAction = playerMap.FindAction("Revive");
        lookAction = playerMap.FindAction("Look");
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
        reviveAction.performed += OnRevive;
        reviveAction.canceled += OnReviveCanceled;
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

    // Devuelve true mientras el botón de disparo esté presionado
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
        var gp = Gamepad.current;
        return gp != null ? gp.rightStick.ReadValue() : Vector2.zero;
    }

    public Vector2 GetLookInput()
    {
      return lookAction != null ? lookAction.ReadValue<Vector2>() : Vector2.zero;
    }

    public Vector2 GetPointerScreenPosition()
    {
      var mouse = Mouse.current;
      return mouse != null ? mouse.position.ReadValue() : Vector2.zero;
    }

}
