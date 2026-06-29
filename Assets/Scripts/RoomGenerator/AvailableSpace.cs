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
        if (!HasCollision && other.TryGetComponent<RoomCarving>(out _))
        {
            HasCollision = true;
            Vertices = transform.GetVertices();
            OtherVertices = other.transform.GetVertices();
        }
    }

    public void ResetCollisions()
    {
        HasCollision = false;
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
        Vector3? topSliceSize = null;

        if (topOtherY < topY)
        {
            (Vector3 position, Vector3 size) topSlice = CreateTopSlice(topOtherY);

            topSliceSize = topSlice.size;
            newTransforms.Add(topSlice);
        }

        float bottomOtherY = OtherVertices.Min(v => v.y);
        float bottomY = Vertices.Min(v => v.y);
        Vector3? bottomSliceSize = null;

        if (bottomOtherY > bottomY)
        {
            (Vector3 position, Vector3 size) bottomSlice = CreateBottomSlice(bottomOtherY);

            bottomSliceSize = bottomSlice.size;
            newTransforms.Add(bottomSlice);
        }

        float leftOtherX = OtherVertices.Min(v => v.x);
        float leftX = Vertices.Min(v => v.x);
        Vector3? leftSliceSize = null;

        if (leftOtherX > leftX)
        {
            (Vector3 position, Vector3 size) leftSlice = CreateLeftSlice(leftOtherX, topSliceSize, bottomSliceSize);

            leftSliceSize = leftSlice.size;
            newTransforms.Add(leftSlice);
        }

        float rightOtherX = OtherVertices.Max(v => v.x);
        float rightX = Vertices.Max(v => v.x);
        Vector3? rightSliceSize = null;

        if (rightOtherX < rightX)
        {
            (Vector3 position, Vector3 size) rightSlice = CreateRightSlice(rightOtherX, topSliceSize, bottomSliceSize);

            rightSliceSize = rightSlice.size;
            newTransforms.Add(rightSlice);
        }

        float frontOtherZ = OtherVertices.Max(v => v.z);
        float frontZ = Vertices.Max(v => v.z);
        
        if (frontOtherZ < frontZ)
        {
            (Vector3 position, Vector3 size) frontSlice = CreateFrontSlice(frontOtherZ, topSliceSize, bottomSliceSize, leftSliceSize, rightSliceSize);

            newTransforms.Add(frontSlice);
        }

        float backOtherZ = OtherVertices.Min(v => v.z);
        float backZ = Vertices.Min(v => v.z);

        if (backOtherZ > backZ)
        {
            (Vector3 position, Vector3 size) backSlice = CreateBackSlice(backOtherZ, topSliceSize, bottomSliceSize, leftSliceSize, rightSliceSize);

            newTransforms.Add(backSlice);
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

    private (Vector3 position, Vector3 size) CreateLeftSlice(float leftOtherX, Vector3? topSliceSize, Vector3? bottomSliceSize)
    {
        float height = GetHeight(topSliceSize, bottomSliceSize);
        
        float leftOfSpace = transform.position.x - (transform.localScale.x / 2f);
        float bottomOfSpace = transform.position.y - (transform.localScale.y / 2f);
        float bottomSliceHeight = bottomSliceSize.HasValue ? bottomSliceSize.Value.y : 0f;

        Vector3 newSize = new Vector3(
            leftOtherX - leftOfSpace,
            height,
            transform.localScale.z
        );

        Vector3 newPosition = new Vector3(
            leftOfSpace + newSize.x / 2f,
            bottomOfSpace + (height / 2f) + bottomSliceHeight,
            transform.position.z
        );

        return ( newPosition, newSize );
    }

    private (Vector3 position, Vector3 size) CreateRightSlice(float rightOtherX, Vector3? topSliceSize, Vector3? bottomSliceSize)
    {
        float height = GetHeight(topSliceSize, bottomSliceSize);

        float rightOfSpace = transform.position.x + (transform.localScale.x / 2f);
        float bottomOfSpace = transform.position.y - (transform.localScale.y / 2f);
        float bottomSliceHeight = bottomSliceSize.HasValue ? bottomSliceSize.Value.y : 0f;

        Vector3 newSize = new Vector3(
            rightOfSpace - rightOtherX,
            height,
            transform.localScale.z
        );

        Vector3 newPosition = new Vector3(
            rightOfSpace - newSize.x / 2f,
            bottomOfSpace + (height / 2f) + bottomSliceHeight,
            transform.position.z
        );

        return (newPosition, newSize);
    }

    private (Vector3 position, Vector3 size) CreateFrontSlice(float frontOtherX, Vector3? topSliceSize, Vector3? bottomSliceSize, Vector3? leftSliceSize, Vector3? rightSliceSize)
    {
        float height = GetHeight(topSliceSize, bottomSliceSize);
        float length = GetLength(leftSliceSize, rightSliceSize);

        float frontOfSpace = transform.position.z + (transform.localScale.z / 2f);

        float leftOfSpace = transform.position.x - (transform.localScale.x / 2f);
        float leftSliceLength = leftSliceSize.HasValue ? leftSliceSize.Value.x : 0f;
        float bottomOfSpace = transform.position.y - (transform.localScale.y / 2f);
        float bottomSliceHeight = bottomSliceSize.HasValue ? bottomSliceSize.Value.y : 0f;

        Vector3 newSize = new Vector3(
            length,
            height,
            frontOfSpace - frontOtherX
        );

        Vector3 newPosition = new Vector3(
            leftOfSpace + leftSliceLength + (length / 2f),
            bottomOfSpace + bottomSliceHeight + (height / 2f),
            frontOfSpace - (newSize.z / 2f)
        );

        return (newPosition, newSize);
    }

    private (Vector3 position, Vector3 size) CreateBackSlice(float backOtherX, Vector3? topSliceSize, Vector3? bottomSliceSize, Vector3? leftSliceSize, Vector3? rightSliceSize)
    {
        float height = GetHeight(topSliceSize, bottomSliceSize);
        float length = GetLength(leftSliceSize, rightSliceSize);

        float backOfSpace = transform.position.z - (transform.localScale.z / 2f);

        float leftOfSpace = transform.position.x - (transform.localScale.x / 2f);
        float leftSliceLength = leftSliceSize.HasValue ? leftSliceSize.Value.x : 0f;
        float bottomOfSpace = transform.position.y - (transform.localScale.y / 2f);
        float bottomSliceHeight = bottomSliceSize.HasValue ? bottomSliceSize.Value.y : 0f;

        Vector3 newSize = new Vector3(
            length,
            height,
            backOtherX - backOfSpace
        );

        Vector3 newPosition = new Vector3(
            leftOfSpace + leftSliceLength + (length / 2f),
            bottomOfSpace + bottomSliceHeight + (height / 2f),
            backOfSpace + (newSize.z / 2f)
        );

        return (newPosition, newSize);
    }

    private float GetHeight(Vector3? topSliceSize, Vector3? bottomSliceSize)
    {
        float height = transform.localScale.y;

        if (topSliceSize.HasValue)
        {
            height -= topSliceSize.Value.y;
        }
        if (bottomSliceSize.HasValue)
        {
            height -= bottomSliceSize.Value.y;
        }

        return height;
    }

    private float GetLength(Vector3? leftSliceSize, Vector3? rightSliceSize)
    {
        float length = transform.localScale.x;

        if (leftSliceSize.HasValue)
        {
            length -= leftSliceSize.Value.x;
        }
        if (rightSliceSize.HasValue)
        {
            length -= rightSliceSize.Value.x;
        }

        return length;
    }
}
