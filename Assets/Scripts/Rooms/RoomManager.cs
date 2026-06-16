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
            CreateRoom();
        }
    }

    private void CreateRoom()
    {
        Vector3 size = new Vector3(
            Random.Range(MinRoomSize, MaxRoomSize),
            1f,
            Random.Range(MinRoomSize, MaxRoomSize)
        );
        Vector3 position = GetNextPosition(size);

        GameObject clone = Instantiate(Room, position, Quaternion.identity, transform);
        clone.transform.localScale = size;

        Room newRoom = clone.GetComponent<Room>();
        RoomList.Add(newRoom);
    }

    private Vector3 GetNextPosition(Vector3 newSize)
    {
        if (RoomList.Count == 0)
        {
            return Vector3.zero;
        }

        Room baseRoom = RoomList[Random.Range(0, RoomList.Count)];
        Vector3 baseOffset = baseRoom.GetRandomAdjacentRelativePosition();

        Vector3 newOffset = new Vector3(newSize.x / 2f, 0f, newSize.z / 2f);

        if (baseOffset.x < 0)
        {
            newOffset.x = -newOffset.x;
        }
        if (baseOffset.z < 0)
        {
            newOffset.z = -newOffset.z;
        }

        return baseRoom.transform.position + baseOffset + newOffset;
    }
}
