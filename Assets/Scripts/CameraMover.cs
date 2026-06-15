using UnityEngine;

public class CameraMover : MonoBehaviour
{
    const float MoveMagnitude = 10f;
    const float LookMagnitude = 20f;

    private Vector2 CurrentLook = Vector2.zero;

    void Update()
    {
        UpdatePosition();
        UpdateRotation();
    }

    private void UpdatePosition()
    {
        InputManager inputManager = InputManager.GetInstance();

        Vector3 currPosition = transform.position;

        if (inputManager.IsCrouching)
        {
            currPosition.y -= Time.deltaTime * MoveMagnitude;
        }
        if (inputManager.IsJumping)
        {
            currPosition.y += Time.deltaTime * MoveMagnitude;
        }

        currPosition.x += inputManager.GetMove.x * MoveMagnitude * Time.deltaTime;
        currPosition.z += inputManager.GetMove.y * MoveMagnitude * Time.deltaTime;

        transform.position = currPosition;
    }

    private void UpdateRotation()
    {
        InputManager inputManager = InputManager.GetInstance();

        Vector2 temp = CurrentLook;

        temp.x -= Time.deltaTime * inputManager.GetLook.y * LookMagnitude;
        temp.y += Time.deltaTime * inputManager.GetLook.x * LookMagnitude;
        
        temp.y = Mathf.Clamp(temp.y, -90, 90);

        CurrentLook = temp;

        transform.localRotation = Quaternion.Euler(CurrentLook.x, CurrentLook.y, 0f);
    }
}
