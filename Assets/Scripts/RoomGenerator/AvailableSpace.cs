using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AvailableSpace : MonoBehaviour
{
    public bool HasCollision = false;
    public List<Vector3> Vertices;
    public List<Vector3> OtherVertices;

    private void OnTriggerEnter(Collider other)
    {
        if (!HasCollision)
        {
            HasCollision = true;
            Vertices = transform.GetVertices();
            OtherVertices = other.transform.GetVertices();
        }
    }

    public List<(Vector3 position, Vector3 size)> GetNewSpaceTransforms()
    {
        if (!HasCollision)
        {
            return new List<(Vector3 position, Vector3 size)> { (this.transform.position, this.transform.localScale) };
        }

        return GenerateTransformByVertices();
    }

    private List<(Vector3 position, Vector3 size)> GenerateTransformByVertices()
    {
        List<(Vector3 position, Vector3 size)> newTransforms = new();

        float topOtherY = OtherVertices.Max(v => v.y);
        float topY = Vertices.Max(v => v.y);
        float? bottomOfTopSlice = null;

        if (topOtherY < topY)
        {
            (Vector3 position, Vector3 size) topSlice = CreateTopSlice(topOtherY);

            bottomOfTopSlice = topSlice.position.y - topSlice.size.y / 2f;
            newTransforms.Add(topSlice);
        }

        float bottomOtherY = OtherVertices.Min(v => v.y);
        float bottomY = Vertices.Min(v => v.y);
        float? topOfBottomSlice = null;

        if (bottomOtherY > bottomY)
        {
            (Vector3 position, Vector3 size) bottomSlice = CreateBottomSlice(bottomOtherY);

            topOfBottomSlice = bottomSlice.position.y + bottomSlice.size.y / 2f;
            newTransforms.Add(bottomSlice);
        }

        return newTransforms;
    }

    private (Vector3 position, Vector3 size) CreateTopSlice(float topOtherY)
    {
        float topOfSpace = transform.position.y + transform.localScale.y / 2f;

        Vector3 newSize = new Vector3(
            transform.localScale.x,
            topOfSpace - topOtherY,
            transform.localScale.z
        );

        
        Vector3 newPosition = new Vector3(
            transform.position.x,
            topOfSpace - (newSize.y / 2f),
            transform.position.z
        );

        return ( newPosition, newSize );
    }

    private (Vector3 position, Vector3 size) CreateBottomSlice(float bottomOtherY)
    {
        float bottomOfSpace = transform.position.y - transform.localScale.y / 2f;

        Vector3 newSize = new Vector3(
            transform.localScale.x,
            bottomOtherY - bottomOfSpace,
            transform.localScale.z
        );

        Vector3 newPosition = new Vector3(
            transform.position.x,
            bottomOfSpace + (newSize.y / 2f),
            transform.position.z
        );

        return ( newPosition, newSize );
    }
}
