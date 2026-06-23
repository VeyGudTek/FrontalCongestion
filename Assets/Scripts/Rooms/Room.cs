using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum Side
{
    Left,
    Right,
    Top,
    Bottom
}

public class AdjacentPositionDto
{
    public Vector3 Point { get; set; }
    public Side Side { get; set; }
}

[System.Serializable]
public class EdgeAvailabilityDto
{
    public Side Side;
    public List<Line> AvailableEdges;
}

[System.Serializable]
public class Line
{
    public Line(float start, float end)
    {
        if (start > end)
        {
            throw new System.InvalidOperationException("Line start shouldn't be greater than line end.");
        }

        Start = start;
        End = end;
    }

    public float Start;
    public float End;
}

[System.Serializable]
public class Neighbor
{
    public Room Room;
    public Side SharedEdge;
}

public class Room : MonoBehaviour
{
    const float TempRoomHeight = 10f;

    [SerializeField] private List<Neighbor> Neighbors = new List<Neighbor>();
    [SerializeField] private List<EdgeAvailabilityDto> Edges = new List<EdgeAvailabilityDto>();
    [SerializeField] private List<GameObject> Walls = new List<GameObject>();
    [SerializeField] private GameObject Wall;
    [SerializeField] private GameObject DebugObject;

    public float PositionX => transform.position.x;
    public float PositionZ => transform.position.z;
    public float Length => transform.localScale.x;
    public float Width => transform.localScale.z;

    private void Awake()
    {
        SetEdgeAvailability();
    }

    public void AddNeighbor(Room parentRoom)
    {
        float xDiff = Mathf.Abs(parentRoom.PositionX - PositionX);
        float combinedHalfLengths = (parentRoom.Length + Length) / 2f;

        Side sharedEdge;
        if (Mathf.Abs(xDiff - combinedHalfLengths) < Constants.ZeroThreshold)
        {
            bool parentIsGreater = parentRoom.PositionX > PositionX;
            sharedEdge = parentIsGreater ? Side.Right : Side.Left;
        }
        else
        {
            bool parentIsGreater = parentRoom.PositionZ > PositionZ;
            sharedEdge = parentIsGreater ? Side.Top : Side.Bottom;
        }

        Neighbors.Add(new Neighbor()
        {
            Room = parentRoom,
            SharedEdge = sharedEdge,
        });

        SetEdgeAvailability();
    }

    //DoorLocation will be used later
    public void GenerateWalls(AdjacentPositionDto doorLocation)
    {
        foreach (GameObject wall in Walls)
        {
            Destroy(wall);
        }
        Walls.Clear();

        GenerateWall(new Vector3(PositionX - (Length / 2f), TempRoomHeight / 2f, PositionZ), new Vector3(Constants.WallThickness, TempRoomHeight, Width));
        GenerateWall(new Vector3(PositionX + (Length / 2f), TempRoomHeight / 2f, PositionZ), new Vector3(Constants.WallThickness, TempRoomHeight, Width));
        GenerateWall(new Vector3(PositionX, TempRoomHeight / 2f, PositionZ - (Width / 2f)), new Vector3(Length, TempRoomHeight, Constants.WallThickness));
        GenerateWall(new Vector3(PositionX, TempRoomHeight / 2f, PositionZ + (Width / 2f)), new Vector3(Length, TempRoomHeight, Constants.WallThickness));
    }

    private void GenerateWall(Vector3 position, Vector3 size)
    {
        GameObject wall = Instantiate(Wall, position, Quaternion.identity, transform);

        Vector3 roomScale = transform.lossyScale;
        wall.transform.localScale = new Vector3(
            size.x / roomScale.x,
            size.y / roomScale.y,
            size.z / roomScale.z
        );

        Walls.Add(wall);
    }

    public AdjacentPositionDto GetRandomAdjacentPosition()
    {
        float totalAvailableEdgeDistance = 0f;
        List<(Side, float)> sideToDistance = new List<(Side, float)>();

        foreach (EdgeAvailabilityDto dto in Edges)
        {
            float edgeTotal = 0f;

            foreach(Line line in dto.AvailableEdges)
            {
                edgeTotal += line.End - line.Start;
            }

            sideToDistance.Add((dto.Side, edgeTotal));
            totalAvailableEdgeDistance += edgeTotal;
        }

        float seed = Random.Range(0f, totalAvailableEdgeDistance);
        foreach((Side side, float distance) in sideToDistance)
        {
            seed -= distance;
            if (seed < 0f)
            {
                return GenerateAdjacentPositionDto(side); 
            }
        }

        throw new System.InvalidOperationException("Generated a Seed beyond the available distance.");
    }

    private AdjacentPositionDto GenerateAdjacentPositionDto(Side side)
    {
        bool isHorizontalSide = side == Side.Left || side == Side.Right;
        bool flipStatic = side == Side.Left || side == Side.Bottom;
        float staticPoint;

        if (isHorizontalSide)
        {
            staticPoint = flipStatic ? PositionX - (Length / 2f) : PositionX + (Length / 2f);
        }
        else
        {
            staticPoint = flipStatic ? PositionZ - (Width / 2f) : PositionZ + (Width / 2f);
        }

        EdgeAvailabilityDto edge = Edges.Where(e => e.Side == side).First();
        int lineIndex = Random.Range(0, edge.AvailableEdges.Count);
        Line line = edge.AvailableEdges[lineIndex];
        float dynamicPoint = Random.Range(line.Start, line.End);

        float x = isHorizontalSide ? staticPoint : dynamicPoint;
        float z = isHorizontalSide ? dynamicPoint : staticPoint;

        Vector3 point = new Vector3(x, 0f, z);

        Instantiate(DebugObject, point, Quaternion.identity, transform);
        return new AdjacentPositionDto()
        {
            Point = point,
            Side = side
        };
    }

    private void SetEdgeAvailability()
    {
        Edges = new List<EdgeAvailabilityDto>();

        Edges.Add(new EdgeAvailabilityDto() { Side = Side.Left, AvailableEdges = GetEdgeAvailability(Side.Left) });
        Edges.Add(new EdgeAvailabilityDto() { Side = Side.Right, AvailableEdges = GetEdgeAvailability(Side.Right) });
        Edges.Add(new EdgeAvailabilityDto() { Side = Side.Top, AvailableEdges = GetEdgeAvailability(Side.Top) });
        Edges.Add(new EdgeAvailabilityDto() { Side = Side.Bottom, AvailableEdges = GetEdgeAvailability(Side.Bottom) });
    }

    private List<Line> GetEdgeAvailability(Side edge)
    {
        bool isHorizontalEdge = edge == Side.Left || edge == Side.Right;

        float halfTotal = isHorizontalEdge ? Width / 2f : Length / 2f;
        float center = isHorizontalEdge ? PositionZ : PositionX;

        List<Line> availableSpots = new List<Line>()
        {
            new Line(center - halfTotal, center + halfTotal)
        };


        foreach (Neighbor neighbor in Neighbors.Where(n => n.SharedEdge == edge))
        {
            float neighborHalfTotal = isHorizontalEdge ? neighbor.Room.Width / 2f : neighbor.Room.Length / 2f;
            float neighborCenter = isHorizontalEdge ? neighbor.Room.PositionZ : neighbor.Room.PositionX;

            float min = neighborCenter - neighborHalfTotal;
            float max = neighborCenter + neighborHalfTotal;

            foreach (Line availableSpot in availableSpots.ToList())
            {
                if ((min > availableSpot.End) || (max < availableSpot.Start)){
                    continue;
                }

                int currentIndex = availableSpots.IndexOf(availableSpot);

                if ((min < availableSpot.Start) && (max > availableSpot.End))
                {
                    availableSpots.RemoveAt(currentIndex);
                    continue;
                }

                if ((min > availableSpot.Start) && (max < availableSpot.End))
                {
                    List<Line> newSpots = new List<Line>()
                    {
                        new Line(availableSpot.Start, min),
                        new Line(max, availableSpot.End)
                    };

                    availableSpots.RemoveAt(currentIndex);
                    availableSpots.InsertRange(currentIndex, newSpots);
                    continue;
                }

                availableSpots[currentIndex].Start = max < availableSpot.End ? max : availableSpot.Start;
                availableSpots[currentIndex].End = min > availableSpot.Start ? min : availableSpot.End;
            }
        }

        return availableSpots;
    }
}
