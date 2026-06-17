using UnityEngine;
using System.Collections.Generic;

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
public class Neighbor
{
    public Room Room;
    public Side SharedEdge;
}

public class Room : MonoBehaviour
{
    [SerializeField] private List<Neighbor> Neighbors = new List<Neighbor>();

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
}
