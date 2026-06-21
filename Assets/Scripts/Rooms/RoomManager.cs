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
        for (int i = 0; i < 5; i++)
        {
            Vector3 newRoomSize = new Vector3(
                Random.Range(MinRoomSize, MaxRoomSize),
                1f,
                Random.Range(MinRoomSize, MaxRoomSize)
            );

            if (i == 0)
            {
                CreateFirstRoom(newRoomSize);
            }
            else
            {
                CreateRoom(newRoomSize);
            }
        }
    }

    private void CreateFirstRoom(Vector3 size)
    {
        GameObject clone = Instantiate(Room, Vector3.zero, Quaternion.identity, transform);
        clone.transform.localScale = size;

        Room newRoom = clone.GetComponent<Room>();
        RoomList.Add(newRoom);
    }

    private void CreateRoom(Vector3 size)
    {
        Room baseRoom = RoomList[Random.Range(0, RoomList.Count)];
        Vector3 position = GetNextPosition(baseRoom, size);

        GameObject clone = Instantiate(Room, position, Quaternion.identity, transform);
        clone.transform.localScale = size;

        Room newRoom = clone.GetComponent<Room>();
        RoomList.Add(newRoom);

        newRoom.AddNeighbor(baseRoom);
        baseRoom.AddNeighbor(newRoom);
    }

    private Vector3 GetNextPosition(Room baseRoom, Vector3 newSize)
    {
        AdjacentPositionDto baseOffset = baseRoom.GetRandomAdjacentPosition();
        Vector3 newOffset = GenerateNewRoomOffset(newSize, baseOffset);

        return baseOffset.Point + newOffset;
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
