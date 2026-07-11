using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum UsedSpaceManagerState
{
    Waiting,
    ComputingPosition,
    ComputingSizeInstantiation,
    ComputingSizeCollisionDetection,
    Finished
}

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

    [Header("State")]
    [SerializeField]
    private Vector3 ComputedSize = Vector3.zero;
    [SerializeField]
    private Vector3 ComputedPosition = Vector3.zero;
    [SerializeField]
    private UsedSpaceManagerState State = UsedSpaceManagerState.Waiting;
    [SerializeField]
    private List<AvailableSpace> InputAvailableSpace = new List<AvailableSpace>();

    public UsedSpaceManagerState CurrentState => State;
    public Vector3 NewSize => ComputedSize;
    public Vector3 NewPosition => ComputedPosition;

    public void StartComputingSizeAndPosition(List<AvailableSpace> inputSpaces)
    {
        ComputedSize = Vector3.zero;
        ComputedPosition = Vector3.zero;
        State = UsedSpaceManagerState.ComputingPosition;
        InputAvailableSpace = inputSpaces;
    }

    public void ResetState()
    {
        State = UsedSpaceManagerState.Waiting;
    }

    private void Update()
    {
        if (State == UsedSpaceManagerState.Waiting || State == UsedSpaceManagerState.Finished)
        {
            return;
        }

        if (State == UsedSpaceManagerState.ComputingPosition)
        {
            ComputedPosition = ComputeRandomPosition();
            State = UsedSpaceManagerState.ComputingSizeInstantiation;
            return;
        }
        if (State == UsedSpaceManagerState.ComputingSizeInstantiation)
        {
            SpawnSizeChecker();
            State = UsedSpaceManagerState.ComputingSizeCollisionDetection;
            return;
        }
        if (State == UsedSpaceManagerState.ComputingSizeCollisionDetection)
        {
            ComputeSize();
            RegisterRoom();
            RemoveChecker();
            State = UsedSpaceManagerState.Finished;
            return;
        }

        throw new System.ArgumentOutOfRangeException($"Unknown State: {State}");
    }



    private Vector3 ComputeRandomPosition()
    {
        Dictionary<AvailableSpace, float> volumeMapping = InputAvailableSpace.ToDictionary(s => s, s => s.Volume);
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

    private void SpawnSizeChecker()
    {
        Vector3 newMaxSize = new Vector3(
            Random.Range(MinRoomSize, MaxRoomSize),
            Random.Range(MinRoomHeight, MaxRoomHeight),
            Random.Range(MinRoomSize, MaxRoomSize)
        );

        GameObject newChecker = Instantiate(UsedSpaceCheckerPrefab, ComputedPosition, Quaternion.identity);
        newChecker.transform.localScale = newMaxSize;
        newChecker.transform.SetParent(transform);

        UsedSpaceCheckerInstance = newChecker;
    }

    private void ComputeSize()
    {
        UsedSpaceChecker checker = UsedSpaceCheckerInstance.GetComponent<UsedSpaceChecker>();
        ComputedSize = checker.GetClampedSize();
    }

    private void RegisterRoom()
    {
        GameObject newSpace = Instantiate(UsedSpacePrefab, ComputedPosition, Quaternion.identity);
        newSpace.transform.localScale = ComputedSize;
        newSpace.transform.SetParent(transform);

        UsedSpaces.Add(newSpace);
    }

    private void RemoveChecker()
    {
        Destroy(UsedSpaceCheckerInstance);
        UsedSpaceCheckerInstance = null;
    }
}
