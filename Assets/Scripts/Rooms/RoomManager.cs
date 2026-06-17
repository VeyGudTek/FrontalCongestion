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
        AdjacentPositionDto baseOffset = baseRoom.GetRandomAdjacentRelativePosition();
        Vector3 newOffset = GenerateNewOffset(newSize, baseOffset);

        return baseRoom.transform.position + baseOffset.Point + newOffset;
    }

    private Vector3 GenerateNewOffset(Vector3 size, AdjacentPositionDto baseOffset)
    {
        float staticOffset = baseOffset.IsHorizontalEdge ? size.x / 2f : size.z / 2f;
        float dynamicOffset = baseOffset.IsHorizontalEdge ?
            Random.Range(0f, (size.z / 2f) - Constants.MinDoorWidth):
            Random.Range(0f, (size.x / 2f) - Constants.MinDoorWidth);

        if (Random.value < .5f)
        {
            dynamicOffset = -dynamicOffset;
        }
        if (baseOffset.Point.x < 0f && baseOffset.IsHorizontalEdge)
        {
            staticOffset = -staticOffset;
        }
        if (baseOffset.Point.z < 0f && !baseOffset.IsHorizontalEdge)
        {
            staticOffset = -staticOffset;
        }

        float x = baseOffset.IsHorizontalEdge ? staticOffset : dynamicOffset;
        float z = baseOffset.IsHorizontalEdge ? dynamicOffset : staticOffset;

        return new Vector3(x, 0f, z);
    }
}
