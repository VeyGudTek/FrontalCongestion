using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static Vector3 GetHalfExtents(this Transform transform)
    {
        return new Vector3(
            transform.localScale.x / 2f,
            transform.localScale.y / 2f,
            transform.localScale.z / 2f
        );
    }

    public static float GetTopBound(this Transform transform)
    {
        return transform.position.y + transform.localScale.y / 2f;
    }

    public static float GetBottomBound(this Transform transform)
    {
        return transform.position.y - transform.localScale.y / 2f;
    }

    public static float GetRightBound(this Transform transform)
    {
        return transform.position.x + transform.localScale.x / 2f;
    }

    public static float GetLeftBound(this Transform transform)
    {
        return transform.position.x - transform.localScale.x / 2f;
    }

    public static float GetForwardBound(this Transform transform)
    {
        return transform.position.z + transform.localScale.z / 2f;
    }

    public static float GetBackBound(this Transform transform)
    {
        return transform.position.z - transform.localScale.z / 2f;
    }

    public static List<Vector3> GetVertices(this Transform transform)
    {
        Vector3 size = transform.localScale;
        Vector3 position = transform.position;

        List<Vector3> verticess = new List<Vector3>();

        float xOffset = size.x / 2f;
        float yOffset = size.y / 2f;
        float zOffset = size.z / 2f;

        foreach(float x in new[] { xOffset, -xOffset })
        {
            foreach(float y in new[] { yOffset, -yOffset })
            {
                foreach(float z in new[] { zOffset, -zOffset })
                {
                    verticess.Add(new Vector3(position.x + x, position.y + y, position.z + z));
                }
            }
        }

        return verticess;
    }

    public static float GetVolume(this Transform transform)
    {
        return transform.localScale.x * transform.localScale.y * transform.localScale.z;
    }

    public static Vector3 GetRandomOffset(this Transform transform)
    {
        return new Vector3(
            Random.Range(-(transform.localScale.x / 2f), transform.localScale.x / 2f),
            Random.Range(-(transform.localScale.y / 2f), transform.localScale.y / 2f),
            Random.Range(-(transform.localScale.z / 2f), transform.localScale.z / 2f)
        );
    }

    public static List<Vector3> GetInsideVertices(this Transform transform, List<Vector3> vertices)
    {
        Vector3 center = transform.position;
        Vector3 halfScale = transform.localScale / 2f;
        List<Vector3> insideVertices = new List<Vector3>();

        foreach (Vector3 v in vertices)
        {
            bool xIsBounded = v.x.PointIsInsideBound(center.x - halfScale.x, center.x + halfScale.x);
            bool yIsBounded = v.y.PointIsInsideBound(center.y - halfScale.y, center.y + halfScale.y);
            bool zIsBounded = v.z.PointIsInsideBound(center.z - halfScale.z, center.z + halfScale.z);

            if (xIsBounded && yIsBounded && zIsBounded)
            {
                insideVertices.Add(v);
            }
        }

        return insideVertices;
    }

    public static bool PointIsInsideBound(this float point, float min, float max)
    {
        return (point >= min && point <= max);
    }
}
