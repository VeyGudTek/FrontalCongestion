using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject RoomPrefab;
    [SerializeField]
    private GameObject DebugPoint;

    [SerializeField]
    private List<Room> RoomList = new List<Room>();

    [Header("Settings")]
    [SerializeField]
    private int TotalRooms = 10;
    [SerializeField]
    private float CurrentLevel = 0f;
    [SerializeField]
    private float MinRoomSize = 50f;
    [SerializeField]
    private float MaxRoomSize = 5f;
    [SerializeField]
    private float Height = 5f;

    private void Start()
    {
        //Replace this with manual call later
        StartGeneration(-10f, 10f, 10f, -10f, 0f, 5f);
    }

    void Update()
    {
        if (RoomList.Count < TotalRooms)
        {
            GenerateRandomRoom();
        }
    }

    private void StartGeneration(float left, float right, float forward, float back, float level, float height)
    {
        CurrentLevel = level;
        Height = height;
        CreateRoom(left, right, forward, back, new());
    }

    private void GenerateRandomRoom()
    {
        List<Room> roomsWithAvailablePerimeter = RoomList.Where(r => r.AvailablePerimeter > FloatConstants.MinDoorToWallDistance * 2f).ToList();
        int randomIndex = Random.Range(0, roomsWithAvailablePerimeter.Count);
        Room randomRoom = roomsWithAvailablePerimeter[randomIndex];

        (Edge availableEdge, Vector3 availablePoint) = randomRoom.GetRandomAvailablePoint();
        (float left, float right, float forward, float back, List<NeighborDto> neighbors) = GetNewRoomDimensions(availableEdge, availablePoint);

        neighbors.Add(new() { Neighbor = randomRoom, SharedEdge = availableEdge.GetOpposite()});
        CreateRoom(left, right, forward, back, neighbors);
    }

    private (float left, float right, float forward, float back, List<NeighborDto> neighbors) GetNewRoomDimensions(Edge startingEdge, Vector3 startingPoint)
    {
        CreateDebugPoint(startingPoint);

        Vector3 newRoomSize = new Vector3(Random.Range(MinRoomSize, MaxRoomSize), Height, Random.Range(MinRoomSize, MaxRoomSize));
        (float left, float right, float forward, float back) = GetNewStartingBounds(startingEdge, startingPoint, newRoomSize);

        int layerNum = LayerMask.NameToLayer(Layers.Room);
        int layerMask = 1 << layerNum;

        List<NeighborDto> neighbors = new List<NeighborDto>();
        int breaker = 0;
        while (true)
        {
            breaker++;
            if (breaker > 250)
            {
                throw new System.InvalidOperationException("Infinite Loop Reached while collision detecting.");
            }

            Vector3 halfExtents = (new Vector3(
                right - left,
                1f,
                forward - back
            ) / 2f) - FloatConstants.OverLapThreshold;
            Vector3 center = new Vector3(
                (right + left) / 2f,
                startingPoint.y,
                (forward + back) / 2f
            );

            Physics.SyncTransforms();
            Collider[] collisions = Physics.OverlapBox(center, halfExtents, Quaternion.identity, layerMask);
            if (collisions.Length > 0)
            {
                Room collidedRoom = collisions[0].GetComponentInParent<Room>();
                (float newLeft, float newRight, float newForward, float newBack, Edge sharedEdge) = collidedRoom.GetClampedDimensions(left, right, forward, back, startingEdge, startingPoint);

                left = newLeft;
                right = newRight;
                forward = newForward;
                back = newBack;

                neighbors.Add(new() { Neighbor = collidedRoom, SharedEdge = sharedEdge });
            }
            else
            {
                break;
            }
            
        }

        return (left, right, forward, back, neighbors);
    }

    private (float left, float right, float forward, float back) GetNewStartingBounds(Edge startingEdge, Vector3 startingPoint, Vector3 newRoomSize)
    {
        float left = startingPoint.x;
        float right = startingPoint.x;
        float forward = startingPoint.z;
        float back = startingPoint.z;

        float xPartitionOne = Random.Range(0f, newRoomSize.x - FloatConstants.MinDoorToWallDistance);
        float xPartitionTwo = newRoomSize.x - xPartitionOne;
        float zPartitionOne = Random.Range(0f, newRoomSize.z - FloatConstants.MinDoorToWallDistance);
        float zPartitionTwo = newRoomSize.z - zPartitionOne;

        switch (startingEdge)
        {
            case Edge.Left:
                left -= newRoomSize.x;
                forward += zPartitionOne;
                back -= zPartitionTwo;
                break;
            case Edge.Right:
                right += newRoomSize.x;
                forward += zPartitionOne;
                back -= zPartitionTwo;
                break;
            case Edge.Forward:
                left -= xPartitionOne;
                right += xPartitionTwo;
                forward += newRoomSize.z;
                break;
            case Edge.Back:
                left -= xPartitionOne;
                right += xPartitionTwo;
                back -= newRoomSize.z;
                break;
            default:
                throw new System.ArgumentOutOfRangeException("Undefined Edge");
        }

        return new (left, right, forward, back);
    }

    private void CreateRoom(float left, float right, float forward, float back, List<NeighborDto> neighbors)
    {
        GameObject newObject = Instantiate(RoomPrefab, transform);
        Room newRoom = newObject.GetComponent<Room>();

        newRoom.SetBounds(left, right, forward, back, CurrentLevel, Height);

        RoomList.Add(newRoom.GetComponent<Room>());

        foreach (NeighborDto dto in neighbors)
        {
            newRoom.AddNeighbor(dto.SharedEdge, dto.Neighbor);
            dto.Neighbor.AddNeighbor(dto.SharedEdge.GetOpposite(), newRoom);
        }
    }

    private void CreateDebugPoint(Vector3 point)
    {
        Instantiate(DebugPoint, point, Quaternion.identity, transform);
    }
}
