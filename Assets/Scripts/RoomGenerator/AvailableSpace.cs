using UnityEngine;

public class AvailableSpace : MonoBehaviour
{
    public bool HasCollision = false;

    private void OnTriggerEnter(Collider other)
    {
        HasCollision = true;
    }
}
