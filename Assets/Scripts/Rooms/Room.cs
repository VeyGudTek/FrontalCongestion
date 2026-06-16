using UnityEngine;

public class AdjacentPositionDto
{
    public Vector3 Point { get; set; }
    public bool IsHorizontalEdge { get; set; }
}

public class Room : MonoBehaviour
{
    private float Height => transform.localScale.y;
    private float Length => transform.localScale.x;
    private float Width => transform.localScale.z;

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
