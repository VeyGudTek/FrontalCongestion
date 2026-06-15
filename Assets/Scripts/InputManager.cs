using System;
using UnityEngine;
using System.Collections.Generic;

public class InputManager : Singleton<InputManager>
{
    GeneratedInputSystem inputSystem;

    private void Awake()
    {
        IntializeSingleton(this);
        inputSystem = new GeneratedInputSystem();
    }

    private void OnEnable() => inputSystem.Enable();

    private readonly List<Action<bool>> crouchActions = new();
    private readonly List<Action<bool>> jumpActions = new();
    private readonly List<Action<Vector2>> moveActions = new();

    public void SubscribeCrouchAction(Action<bool> crouchAction)
    {
        crouchActions.Add(crouchAction);
    }

    public void SubscribeJumpAction(Action<bool> jumpAction)
    {
        jumpActions.Add(jumpAction);
    }

    public void SubscribeMoveAction(Action<Vector2> moveAction)
    {
        moveActions.Add(moveAction);
    }

    void Update()
    {
        ProcessCrouchOptions();
        ProcessJumpActions();
        ProcessMoveActions();
    }

    void ProcessCrouchOptions()
    {
        foreach (var item in crouchActions)
        {
            item.Invoke(inputSystem.Player.Crouch.IsPressed());
        }
    }

    void ProcessJumpActions()
    {
        foreach (var item in jumpActions)
        {
            item.Invoke(inputSystem.Player.Jump.IsPressed());
        }
    }

    void ProcessMoveActions()
    {
        foreach (var item in moveActions)
        {
            item.Invoke(inputSystem.Player.Move.ReadValue<Vector2>());
        }
    }
}
