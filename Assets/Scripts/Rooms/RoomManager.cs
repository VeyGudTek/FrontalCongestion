using System.Collections.Generic;
using UnityEngine;

public class RoomManager : Singleton<RoomManager>
{
    private const float MaxRoomSize = 20f;
    private const float MinRoomSize = 10f;

    [SerializeField] private GameObject Room;
    [SerializeField] private List<Room> RoomList = new List<Room>();

    private void Awake()
    {
        IntializeSingleton(this);
    }

    private void Start()
    {
        CreateRooms();
    }

    private void CreateRooms()
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
            {
                CreateFirstRoom();
            }
            else
            {
                CreateRoom();
            }
        }
    }

    private void CreateFirstRoom()
    {
        GameObject clone = Instantiate(Room, Vector3.zero, Quaternion.identity, transform);

        Vector3 newRoomSize = new Vector3(
            Random.Range(MinRoomSize, MaxRoomSize),
            1f,
            Random.Range(MinRoomSize, MaxRoomSize)
        );
        clone.transform.localScale = newRoomSize;

        Room newRoom = clone.GetComponent<Room>();
        RoomList.Add(newRoom);
    }

    private void CreateRoom()
    {
        Room baseRoom = RoomList[Random.Range(0, RoomList.Count)];

        AdjacentPositionDto baseAdjacentPoint = baseRoom.GetRandomAdjacentPosition();
        (Vector3 position, Vector3 size) = GetNextPositionAndSize(baseAdjacentPoint);

        GameObject clone = Instantiate(Room, position, Quaternion.identity, transform);
        clone.transform.localScale = size;

        Room newRoom = clone.GetComponent<Room>();
        RoomList.Add(newRoom);

        newRoom.AddNeighbor(baseRoom);
        newRoom.GenerateWalls(baseAdjacentPoint.GetFlippedSide());
        baseRoom.AddNeighbor(newRoom);
        baseRoom.GenerateWalls(baseAdjacentPoint);
    }

    private (Vector3, Vector3) GetNextPositionAndSize(AdjacentPositionDto baseAdjacentPoint)
    {
        Vector3 newSize = GenerateNewRoomSize();
        Vector3 newOffset = GenerateNewRoomOffset(newSize, baseAdjacentPoint);

        return (baseAdjacentPoint.Point + newOffset, newSize);
    }

    private Vector3 GenerateNewRoomSize()
    {
        Physics.SyncTransforms();
        return new Vector3(
            Random.Range(MinRoomSize, MaxRoomSize),
            1f,
            Random.Range(MinRoomSize, MaxRoomSize)
        );
    }

    private Vector3 GenerateNewRoomOffset(Vector3 newRoomSize, AdjacentPositionDto baseOffset)
    {
        bool isHorizontalEdge = baseOffset.Side == Side.Left || baseOffset.Side == Side.Right;

        float staticOffset = isHorizontalEdge ? newRoomSize.x / 2f : newRoomSize.z / 2f;
        float dynamicOffset = isHorizontalEdge ?
            Random.Range(0f, (newRoomSize.z / 2f) - Constants.MinDoorWidth):
            Random.Range(0f, (newRoomSize.x / 2f) - Constants.MinDoorWidth);

        if (Random.value < .5f)
        {
            dynamicOffset = -dynamicOffset;
        }
        if (baseOffset.Side == Side.Left || baseOffset.Side == Side.Bottom)
        {
            staticOffset = -staticOffset;
        }

        float x = isHorizontalEdge ? staticOffset : dynamicOffset;
        float z = isHorizontalEdge ? dynamicOffset : staticOffset;

        return new Vector3(x, 0f, z);
    }
}
