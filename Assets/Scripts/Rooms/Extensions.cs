using UnityEngine;
using System;

public static class Extensions
{
    public static AdjacentPositionDto GetFlippedSide(this AdjacentPositionDto originalDto)
    {
        Side newSide;
        switch (originalDto.Side)
        {
            case Side.Left:
                newSide = Side.Right;
                break;
            case Side.Right:
                newSide = Side.Left;
                break;
            case Side.Top:
                newSide = Side.Bottom; 
                break;
            case Side.Bottom:
                newSide = Side.Top; 
                break;
            default:
                newSide = originalDto.Side;
                break;
        }

        return new AdjacentPositionDto
        {
            Side = newSide,
            Point = originalDto.Point,
        };
    }

    public static bool IsHorizontalEdge(this Side side)
    {
        return side == Side.Left || side == Side.Right;
    }

    public static (Vector3, Vector3) ToAdjacentVector3(this Side side)
    {
        switch (side)
        {
            case Side.Left:
                return (Vector3.forward, Vector3.back);
            case Side.Right:
                return (Vector3.forward, Vector3.back);
            case Side.Top:
                return (Vector3.left, Vector3.right);
            case Side.Bottom:
                return (Vector3.left, Vector3.right);
            default:
                throw new InvalidOperationException($"Invalid Side Enum: {side}");
        }
    }

    public static Vector3 ToVector3(this Side side)
    {
        switch (side)
        {
            case Side.Left:
                return Vector3.left;
            case Side.Right:
                return Vector3.right;
            case Side.Top:
                return Vector3.forward;
            case Side.Bottom:
                return Vector3.back;
            default:
                throw new InvalidOperationException($"Invalid Side Enum: {side}");
        }
    }
}
