using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UsedSpaceManager : MonoBehaviour
{
    const float MaxRoomSize = 40f;
    const float MinRoomSize = 3f;
    const float MaxRoomHeight = 10f;
    const float MinRoomHeight = 2.5f;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject UsedSpacePrefab;
    [SerializeField]
    private GameObject UsedSpaceCheckerPrefab;

    [Header("Instances")]
    [SerializeField]
    private GameObject UsedSpaceCheckerInstance;
    [SerializeField]
    private List<GameObject> UsedSpaces = new List<GameObject>();

    public (Vector3 size, Vector3 position) GetNewPositionAndSize(List<AvailableSpace> inputSpaces)
    {
        Vector3 newPosition = GetPosition(inputSpaces);
        Vector3 newSize = GetSize(newPosition);
        RegisterRoom(newPosition, newSize);

        return (newSize, newPosition);
    }

    private Vector3 GetPosition(List<AvailableSpace> inputSpaces)
    {
        Dictionary<AvailableSpace, float> volumeMapping = inputSpaces.ToDictionary(s => s, s => s.Volume);
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

    private Vector3 GetSize(Vector3 newPosition)
    {
        UsedSpaceChecker newChecker = SpawnSizeChecker(newPosition);
        Vector3 newSize = newChecker.GetClampedSize();

        Destroy(newChecker.gameObject);

        return newSize;
    }

    private UsedSpaceChecker SpawnSizeChecker(Vector3 newPosition)
    {
        Vector3 newMaxSize = new Vector3(
            Random.Range(MinRoomSize, MaxRoomSize),
            Random.Range(MinRoomHeight, MaxRoomHeight),
            Random.Range(MinRoomSize, MaxRoomSize)
        );

        GameObject newChecker = Instantiate(UsedSpaceCheckerPrefab, newPosition, Quaternion.identity);
        newChecker.transform.localScale = newMaxSize;
        newChecker.transform.SetParent(transform);

        return newChecker.GetComponent<UsedSpaceChecker>();
    }

    private void RegisterRoom(Vector3 newPosition, Vector3 newSize)
    {
        GameObject newSpace = Instantiate(UsedSpacePrefab, newPosition, Quaternion.identity);
        newSpace.transform.localScale = newSize;
        newSpace.transform.SetParent(transform);

        UsedSpaces.Add(newSpace);
    }
}
