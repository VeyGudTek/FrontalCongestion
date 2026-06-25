using System.Collections.Generic;
using UnityEngine;

public class RoomLimitationDto
{
    public float MaxNegativeSecant;
    public float MaxPositiveSecant;
    public Vector3 RoomSize;
}

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
        for (int i = 0; i < 4; i++)
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
        RoomLimitationDto newRoomLimits = GenerateNewRoomSize(baseAdjacentPoint);
        Vector3 newOffset = GenerateNewRoomOffset(newRoomLimits, baseAdjacentPoint);

        return (baseAdjacentPoint.Point + newOffset, newRoomLimits.RoomSize);
    }

    private RoomLimitationDto GenerateNewRoomSize(AdjacentPositionDto baseAdjacentPoint)
    {
        Physics.SyncTransforms();

        Vector3 direction = baseAdjacentPoint.Side.ToVector3();
        Vector3 origin = baseAdjacentPoint.Point + (direction * Constants.WallThickness);
        float maxRoomExtent;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, MaxRoomSize))
        {
            maxRoomExtent = (hit.point - baseAdjacentPoint.Point).magnitude;
        }
        else
        {
            maxRoomExtent = MaxRoomSize;
        }

        (float maxSecantNegative, float maxSecantPositive) = GetRoomSpan(origin, direction, baseAdjacentPoint.Side, maxRoomExtent);
        float roomSpan = Random.Range(maxSecantNegative, maxSecantPositive);

        return new RoomLimitationDto()
        {
            MaxNegativeSecant = maxSecantNegative,
            MaxPositiveSecant = maxSecantPositive,
            RoomSize = new Vector3(
                baseAdjacentPoint.Side.IsHorizontalEdge() ? maxRoomExtent : roomSpan,
                1f,
                baseAdjacentPoint.Side.IsHorizontalEdge() ? roomSpan : maxRoomExtent
            )
        };
    }

    private (float, float) GetRoomSpan(Vector3 origin, Vector3 direction, Side side, float maxRoomExtent)
    {
        float maxSecantNegative = MaxRoomSize;
        float maxSecantPositive = MaxRoomSize;

        for (float i = 0f; i < maxRoomExtent; i += MinRoomSize)
        {
            Vector3 secantOrigin = origin + (direction * i);
            (Vector3 secantDirectionNegative, Vector3 secantDirectionPositive) = side.ToAdjacentVector3();

            RaycastHit hit;
            if (Physics.Raycast(secantOrigin, secantDirectionNegative, out hit, maxSecantNegative))
            {
                maxSecantNegative = hit.distance;
            }
            if (Physics.Raycast(secantOrigin, secantDirectionPositive, out hit, maxSecantNegative))
            {
                maxSecantNegative = hit.distance;
            }
        }

        return (maxSecantNegative, maxSecantPositive);
    }

    private Vector3 GenerateNewRoomOffset(RoomLimitationDto newRoomLimits, AdjacentPositionDto baseOffset)
    {
        bool isHorizontalEdge = baseOffset.Side == Side.Left || baseOffset.Side == Side.Right;

        float staticOffset = isHorizontalEdge ? newRoomLimits.RoomSize.x / 2f : newRoomLimits.RoomSize.z / 2f;
        float dynamicOffset = isHorizontalEdge ?
            Random.Range(0f, (newRoomLimits.RoomSize.z / 2f) - Constants.MinDoorWidth):
            Random.Range(0f, (newRoomLimits.RoomSize.x / 2f) - Constants.MinDoorWidth);

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
