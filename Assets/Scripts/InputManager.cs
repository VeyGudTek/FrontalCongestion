using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    private GeneratedInputSystem inputSystem { get; set; }
    private void Awake()
    {
        IntializeSingleton(this);
        inputSystem = new GeneratedInputSystem();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable() => inputSystem.Enable();

    public bool IsJumping => inputSystem.Player.Jump.IsPressed();
    public bool IsCrouching => inputSystem.Player.Crouch.IsPressed();
    public Vector2 GetMove => inputSystem.Player.Move.ReadValue<Vector2>();
    public Vector2 GetLook => inputSystem.Player.Look.ReadValue<Vector2>();
}
