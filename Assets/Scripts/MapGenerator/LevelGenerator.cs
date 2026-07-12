using UnityEngine;
using System.Collections.Generic;

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
    private int TotalRooms = 0;
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
        CreateRoom(left, right, forward, back);

        TotalRooms = 2;
    }

    private void GenerateRandomRoom()
    {
        int randomIndex = Random.Range(0, RoomList.Count);
        Room randomRoom = RoomList[randomIndex];

        (Edge availableEdge, Vector3 availablePoint) = randomRoom.GetRandomAvailablePoint();
        (float left, float right, float forward, float back) = GetNewRoomDimensions(availableEdge, availablePoint);

        CreateRoom(left, right, forward, back);
    }

    private (float left, float right, float forward, float back) GetNewRoomDimensions(Edge startingEdge, Vector3 startingPoint)
    {
        CreateDebugPoint(startingPoint);

        Vector3 newRoomSize = new Vector3(Random.Range(MinRoomSize, MaxRoomSize), Height, Random.Range(MinRoomSize, MaxRoomSize));
        Vector3 halfExtents = newRoomSize / 2f;
        Vector3 center = GetCenter(startingEdge, startingPoint, halfExtents);
        (float left, float right, float forward, float back) = GetNewStartingBounds(center, halfExtents);

        CreateDebugPoint(center);

        int layerNum = LayerMask.NameToLayer(Layers.Room);
        int layerMask = 1 << layerNum;

        Collider[] collisions = Physics.OverlapBox(center, halfExtents - FloatConstants.OverLapThreshold, Quaternion.identity, layerMask);
        foreach (Collider collision in collisions)
        {
            Room collidedRoom = collision.GetComponentInParent<Room>();
            (float newLeft, float newRight, float newForward, float newBack) = collidedRoom.GetClampedDimensions(left, right, forward, back, startingEdge);

            left = newLeft;
            right = newRight;
            forward = newForward;
            back = newBack;
        }

        return (left, right, forward, back);
    }

    private Vector3 GetCenter(Edge startingEdge, Vector3 startingPoint, Vector3 halfExtents)
    {
        float x = startingPoint.x;
        float y = startingPoint.y;
        float z = startingPoint.z;

        switch(startingEdge)
        {
            case Edge.Left:
                x -= halfExtents.x;
                break;
            case Edge.Right:
                x += halfExtents.x;
                break;
            case Edge.Forward:
                z += halfExtents.z;
                break;
            case Edge.Back:
                z -= halfExtents.z;
                break;
            default:
                throw new System.ArgumentOutOfRangeException("Undefined Edge");
        }

        return new Vector3(x, y, z);
    }

    private (float left, float right, float forward, float back) GetNewStartingBounds(Vector3 center, Vector3 halfExtents)
    {
        float left = center.x - halfExtents.x;
        float right = center.x + halfExtents.x;
        float forward = center.z + halfExtents.z;
        float back = center.z - halfExtents.z;

        return new (left, right, forward, back);
    }

    private void CreateRoom(float left, float right, float forward, float back)
    {
        GameObject newObject = Instantiate(RoomPrefab, transform);
        Room newRoom = newObject.GetComponent<Room>();

        newRoom.SetBounds(left, right, forward, back, CurrentLevel, Height);

        RoomList.Add(newRoom.GetComponent<Room>());
    }

    private void CreateDebugPoint(Vector3 point)
    {
        Instantiate(DebugPoint, point, Quaternion.identity, transform);
    }
}
