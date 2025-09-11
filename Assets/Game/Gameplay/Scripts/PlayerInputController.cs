using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    private InputActionAsset inputAsset = null;
    private InputActionMap playerMap = null;

    private Vector2 move = Vector2.zero;
    private InputAction fireAction = null;

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
        playerMap.Enable();
    }

    void OnDisable()
    {
        playerMap.FindAction("Move").performed -= OnMove;
        playerMap.FindAction("Move").canceled -= OnStopMove;
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
