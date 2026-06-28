//using System.Collections.Generic;
//using UnityEngine;

///// <summary> Room Size and Max Span data used when generating a new room. </summary>
//public class RoomSizeDto
//{
//    //Both secants are recorded as positive.
//    public float MaxNegativeSecant;
//    public float MaxPositiveSecant;
//    public Vector3 RoomSize;
//}

///// <summary> Room Dimensions and Neighbor data used when generating a new room. </summary>
//public class NewRoomDataDto
//{
//    public Vector3 Size;
//    public Vector3 Position;
//    public List<Room> Neighbors;
//}

//public class RoomManager : Singleton<RoomManager>
//{
//    private const float MaxRoomSize = 20f;
//    private const float MinRoomSize = 10f;

//    [SerializeField] private GameObject Room;
//    [SerializeField] private List<Room> RoomList = new List<Room>();

//    private void Awake()
//    {
//        IntializeSingleton(this);
//    }

//    private void Start()
//    {
//        CreateRooms();
//    }

//    private void CreateRooms()
//    {
//        for (int i = 0; i < 4; i++)
//        {
//            if (i == 0)
//            {
//                CreateFirstRoom();
//            }
//            else
//            {
//                CreateRoom();
//            }
//        }
//    }

//    private void CreateFirstRoom()
//    {
//        GameObject clone = Instantiate(Room, Vector3.zero, Quaternion.identity, transform);

//        Vector3 newRoomSize = new Vector3(
//            Random.Range(MinRoomSize, MaxRoomSize),
//            1f,
//            Random.Range(MinRoomSize, MaxRoomSize)
//        );
//        clone.transform.localScale = newRoomSize;

//        Room newRoom = clone.GetComponent<Room>();
//        RoomList.Add(newRoom);
//    }

//    private void CreateRoom()
//    {
//        Room baseRoom = RoomList[Random.Range(0, RoomList.Count)];

//        AdjacentPositionDto baseAdjacentPoint = baseRoom.GetRandomAdjacentPosition();
//        (Vector3 position, Vector3 size) = GetNextPositionAndSize(baseAdjacentPoint);

//        GameObject clone = Instantiate(Room, position, Quaternion.identity, transform);
//        clone.transform.localScale = size;

//        Room newRoom = clone.GetComponent<Room>();
//        RoomList.Add(newRoom);

//        newRoom.AddNeighbor(baseRoom);
//        newRoom.GenerateWalls(baseAdjacentPoint.GetFlippedSide());
//        baseRoom.AddNeighbor(newRoom);
//        baseRoom.GenerateWalls(baseAdjacentPoint);
//    }

//    private (Vector3, Vector3) GetNextPositionAndSize(AdjacentPositionDto baseAdjacentPoint)
//    {
//        RoomSizeDto newSizeDto = GenerateNewRoomSize(baseAdjacentPoint);
//        Vector3 newOffset = GenerateNewRoomOffset(newSizeDto, baseAdjacentPoint);

//        return (baseAdjacentPoint.Point + newOffset, newSizeDto.RoomSize);
//    }

//    private RoomSizeDto GenerateNewRoomSize(AdjacentPositionDto baseAdjacentPoint)
//    {
//        Physics.SyncTransforms();

//        Vector3 direction = baseAdjacentPoint.Side.ToVector3();
//        Vector3 origin = baseAdjacentPoint.Point + (direction * Constants.WallThickness);
//        float maxRoomExtent = Random.Range(MinRoomSize, MaxRoomSize);

//        if (Physics.Raycast(origin, direction, out RaycastHit hit, MaxRoomSize))
//        {
//            maxRoomExtent = (hit.point - baseAdjacentPoint.Point).magnitude;
//        }

//        (float maxSecantNegative, float maxSecantPositive) = GetRoomSpan(origin, direction, baseAdjacentPoint.Side, maxRoomExtent);
//        float roomSpan = Random.Range(0f, maxSecantNegative + maxSecantPositive);

//        return new RoomSizeDto()
//        {
//            MaxNegativeSecant = maxSecantNegative,
//            MaxPositiveSecant = maxSecantPositive,
//            RoomSize = new Vector3(
//                baseAdjacentPoint.Side.IsHorizontalEdge() ? maxRoomExtent : roomSpan,
//                1f,
//                baseAdjacentPoint.Side.IsHorizontalEdge() ? roomSpan : maxRoomExtent
//            )
//        };
//    }

//    private (float, float) GetRoomSpan(Vector3 origin, Vector3 direction, Side side, float maxRoomExtent)
//    {
//        float maxSecantNegative = MaxRoomSize / 2f;
//        float maxSecantPositive = MaxRoomSize / 2f;

//        for (float i = 0f; i < maxRoomExtent; i += MinRoomSize)
//        {
//            Vector3 secantOrigin = origin + (direction * i);
//            (Vector3 secantDirectionNegative, Vector3 secantDirectionPositive) = side.ToAdjacentVector3();

//            RaycastHit hit;
//            if (Physics.Raycast(secantOrigin, secantDirectionNegative, out hit, maxSecantNegative))
//            {
//                maxSecantNegative = hit.distance;
//            }
//            if (Physics.Raycast(secantOrigin, secantDirectionPositive, out hit, maxSecantNegative))
//            {
//                maxSecantNegative = hit.distance;
//            }
//        }

//        float randomNegative = Random.Range(0f, MaxRoomSize / 2f);
//        float randomPositive = Random.Range(0f, MaxRoomSize / 2f);

//        return (Mathf.Min(maxSecantNegative, randomNegative), Mathf.Min(randomPositive, maxSecantPositive));
//    }

//    private Vector3 GenerateNewRoomOffset(RoomSizeDto newSizeDto, AdjacentPositionDto baseOffset)
//    {
//        bool isHorizontalEdge = baseOffset.Side == Side.Left || baseOffset.Side == Side.Right;

//        float staticOffset = isHorizontalEdge ? newSizeDto.RoomSize.x / 2f : newSizeDto.RoomSize.z / 2f;
//        if (baseOffset.Side == Side.Left || baseOffset.Side == Side.Bottom)
//        {
//            staticOffset = -staticOffset;
//        }

//        float halfSpan = newSizeDto.RoomSize.GetSpan(isHorizontalEdge) / 2f;
//        float maxPositiveOffset = Mathf.Max(0f, newSizeDto.MaxPositiveSecant - Constants.MinDoorWidth);
//        float maxNegativeOffset = Mathf.Max(0f, newSizeDto.MaxNegativeSecant - Constants.MinDoorWidth);
//        float dynamicOffset = Random.Range(-maxNegativeOffset, maxPositiveOffset);

//        float x = isHorizontalEdge ? staticOffset : dynamicOffset;
//        float z = isHorizontalEdge ? dynamicOffset : staticOffset;

//        return new Vector3(x, 0f, z);
//    }
//}
