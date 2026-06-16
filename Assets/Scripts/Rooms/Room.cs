using UnityEngine;

public class Room : MonoBehaviour
{
    private float Height => transform.localScale.y;
    private float Length => transform.localScale.x;
    private float Width => transform.localScale.z;

    public Vector3 GetRandomAdjacentRelativePosition()
    {
        bool staticHorizontal = Random.value > .5f;
        bool flipSide = Random.value > .5f;

        float staticOffset = staticHorizontal ? Length / 2f : Width / 2f;
        float dynamicOffset = staticHorizontal ? Random.Range(0f, Width / 2f) : Random.Range(0f, Length / 2f);

        float x = staticHorizontal ? staticOffset : dynamicOffset;
        float z = staticHorizontal ? dynamicOffset : staticOffset;

        if (flipSide)
        {
            x = -x;
            z = -z;
        }

        return new Vector3(x, 0f, z);
    }
}
