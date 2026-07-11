using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UsedSpaceManager : MonoBehaviour
{
    const float MaxRoomSize = 10f;
    const float MinRoomSize = 5f;
    const float MaxRoomHeight = 5f;
    const float MinRoomHeight = 2.5f;

    [SerializeField]
    private GameObject UsedSpacePrefab;
    [SerializeField]
    private List<GameObject> UsedSpaces = new List<GameObject>();

    public void RegisterRoom(GameObject roomCarving)
    {
        GameObject newSpace = Instantiate(UsedSpacePrefab, roomCarving.transform.position, Quaternion.identity);
        newSpace.transform.localScale = roomCarving.transform.localScale;
        newSpace.transform.SetParent(transform);

        UsedSpaces.Add(newSpace);
    }

    public (Vector3 size, Vector3 position) GetNewRoomSizeAndPosition(List<AvailableSpace> availableSpaces)
    {
        Vector3 position = GetRandomPosition(availableSpaces);
        Vector3 size = GetRandomSize(position);

        return (size, position);
    }
    private Vector3 GetRandomPosition(List<AvailableSpace> availableSpaces)
    {
        Dictionary<AvailableSpace, float> volumeMapping = availableSpaces.ToDictionary(s => s, s => s.Volume);
        float totalVolume = volumeMapping.Values.Aggregate((sum, curr) => curr + sum);
        float multiplier = 1.0f / totalVolume;

        float currentSum = 0.0f;
        float randomThreshold = Random.value;
        foreach ((AvailableSpace space, float volume) in volumeMapping)
        {
            currentSum += volume * multiplier;

            if (currentSum > randomThreshold)
            {
                return space.GetRandomPoint();
            }

            if (Mathf.Approximately(currentSum, 1.0f))
            {
                return space.GetRandomPoint();
            }
        }

        throw new System.IndexOutOfRangeException("Volume Sum does not add up to 1.0f");
    }

    private Vector3 GetRandomSize(Vector3 roomCenter)
    {
        Vector3 newMaxSize = new Vector3(
            Random.Range(MinRoomSize, MaxRoomSize),
            Random.Range(MinRoomHeight, MaxRoomHeight),
            Random.Range(MinRoomSize, MaxRoomSize)
        );

        float xNeg = GetMaxDistance(roomCenter, Vector3.left, newMaxSize.x / 2f);
        float xPos = GetMaxDistance(roomCenter, Vector3.right, newMaxSize.x / 2f);
        float yNeg = GetMaxDistance(roomCenter, Vector3.down, newMaxSize.y / 2f);
        float yPos = GetMaxDistance(roomCenter, Vector3.up, newMaxSize.y / 2f);
        float zNeg = GetMaxDistance(roomCenter, Vector3.back, newMaxSize.z / 2f);
        float zPos = GetMaxDistance(roomCenter, Vector3.forward, newMaxSize.z / 2f);

        return new Vector3(
            xNeg + xPos,
            yNeg + yPos,
            zNeg + zPos
        );
    }

    private float GetMaxDistance(Vector3 roomCenter, Vector3 direction, float maxDistance)
    {
        Ray ray = new Ray(roomCenter, direction);
        RaycastHit hit;

        int layerNum = LayerMask.NameToLayer(Layers.UsedSpace);
        int layerMask = 1 << layerNum;

        if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
        {
            return hit.distance;
        }
        return maxDistance;
    }
}
