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
    public bool IsHorizontalEdge { get; set; }
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
    [SerializeField] private List<Neighbor> Neighbors = new List<Neighbor>();
    [SerializeField] private List<EdgeAvailabilityDto> AvailableEdges = new List<EdgeAvailabilityDto>();

    public float PositionX => transform.position.x;
    public float PositionZ => transform.position.z;
    public float Length => transform.localScale.x;
    public float Width => transform.localScale.z;

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

    public AdjacentPositionDto GetRandomAdjacentRelativePosition()
    {
        bool staticHorizontal = Random.value > .5f;

        float staticOffset = staticHorizontal ? Length / 2f : Width / 2f;
        float dynamicOffset = staticHorizontal ? 
            Random.Range(0f, (Width  / 2f) - Constants.MinDoorWidth): 
            Random.Range(0f, (Length / 2f) - Constants.MinDoorWidth);

        float x = staticHorizontal ? staticOffset : dynamicOffset;
        float z = staticHorizontal ? dynamicOffset : staticOffset;

        if (Random.value > .5f)
        {
            x = -x;
        }
        if (Random.value > .5f)
        {
            z = -z;
        }

        return new AdjacentPositionDto(){
            Point = new Vector3(x, 0f, z),
            IsHorizontalEdge = staticHorizontal
        };
    }

    private void SetEdgeAvailability()
    {
        AvailableEdges = new List<EdgeAvailabilityDto>();

        AvailableEdges.Add(new EdgeAvailabilityDto() { Side = Side.Left, AvailableEdges = GetEdgeAvailability(Side.Left) });
        AvailableEdges.Add(new EdgeAvailabilityDto() { Side = Side.Right, AvailableEdges = GetEdgeAvailability(Side.Right) });
        AvailableEdges.Add(new EdgeAvailabilityDto() { Side = Side.Top, AvailableEdges = GetEdgeAvailability(Side.Top) });
        AvailableEdges.Add(new EdgeAvailabilityDto() { Side = Side.Bottom, AvailableEdges = GetEdgeAvailability(Side.Bottom) });
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
