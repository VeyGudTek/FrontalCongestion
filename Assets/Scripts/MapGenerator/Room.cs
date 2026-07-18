using UnityEngine;
using System.Collections.Generic;

public enum Edge
{
    Left,
    Right,
    Forward,
    Back
}

public class Room : MonoBehaviour
{
    [System.Serializable]
    private class AvailableEdge
    {
        public Edge Edge;
        public List<(float start, float end)> AvailableLines;
    }

    [SerializeField]
    Transform Template;

    [SerializeField]
    private float Level;
    [SerializeField]
    private float Height;

    [SerializeField]
    private float LeftBound;
    [SerializeField]
    private float RightBound;
    [SerializeField]
    private float ForwardBound;
    [SerializeField]
    private float BackBound;

    List<AvailableEdge> AvailableEdges = new List<AvailableEdge>();

    private float CenterX => (LeftBound + RightBound) / 2f;
    private float CenterZ => (ForwardBound + BackBound) / 2f;
    private float CenterY => (Level + Height / 2f);

    private float ScaleX => RightBound - LeftBound;
    private float ScaleZ => ForwardBound - BackBound;

    public void SetBounds(float left, float right, float forward, float back, float level, float height)
    {
        Level = level;
        Height = height;

        LeftBound = left;
        RightBound = right;
        ForwardBound = forward;
        BackBound = back;

        AvailableEdges.Add(new AvailableEdge() { Edge = Edge.Left, AvailableLines =    new List<(float start, float end)> { (back, forward) } });
        AvailableEdges.Add(new AvailableEdge() { Edge = Edge.Right, AvailableLines =   new List<(float start, float end)> { (back, forward) } });
        AvailableEdges.Add(new AvailableEdge() { Edge = Edge.Forward, AvailableLines = new List<(float start, float end)> { (left, right) } });
        AvailableEdges.Add(new AvailableEdge() { Edge = Edge.Back, AvailableLines =    new List<(float start, float end)> { (left, right) } });

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Template.transform.position = new Vector3(CenterX, CenterY, CenterZ);
        Template.localScale = new Vector3(ScaleX, Height, ScaleZ);
    }

    public (Edge edge, Vector3 point) GetRandomAvailablePoint()
    {
        float totalPerimeter = GetAvailablePerimeter();
        float randomThreshold = Random.value * totalPerimeter;
        float currentThreshold = 0f;

        foreach (AvailableEdge ae in AvailableEdges)
        {
            foreach ((float start, float end) in ae.AvailableLines)
            {
                currentThreshold += (end - start);

                if (currentThreshold > randomThreshold || Mathf.Approximately(currentThreshold, randomThreshold))
                {
                    float randomValue = Random.Range(start, end);
                    float staticValue = GetBoundForEdge(ae.Edge);

                    bool isHorizontalEdge = ae.Edge == Edge.Left || ae.Edge == Edge.Right;
                    Vector3 newVector = new Vector3(
                        isHorizontalEdge ? staticValue : randomValue,
                        Level,
                        isHorizontalEdge ? randomValue : staticValue
                    );

                    return (ae.Edge, newVector);
                }
            }
        }

        throw new System.ArgumentOutOfRangeException("RandomThreshold Greater than total perimeter.");
    }

    private float GetAvailablePerimeter()
    {
        float totalAvailablePerimeter = 0f;
        foreach(AvailableEdge ae in AvailableEdges)
        {
            foreach ((float start, float end) in ae.AvailableLines)
            {
                totalAvailablePerimeter += end - start;
            }
        }

        return totalAvailablePerimeter;
    }

    private float GetBoundForEdge(Edge edge)
    {
        switch (edge)
        {
            case Edge.Left:
                return LeftBound;
            case Edge.Right:
                return RightBound;
            case Edge.Forward:
                return ForwardBound;
            case Edge.Back:
                return BackBound;
            default:
                throw new System.ArgumentOutOfRangeException("Undefined Edge");
        }
    }

    public (float left, float right, float forward, float back) GetClampedDimensions(float left, float right, float forward, float back, Edge originalEdge)
    {
        int collisions = 0;
        bool collidedLeft = false;
        bool collidedRight = false;
        bool collidedForward = false;
        bool collidedBack = false;

        if (left < RightBound && right > RightBound && originalEdge != Edge.Right)
        {
            collisions++;
            collidedLeft = true;
        }
        if (right > LeftBound && left < LeftBound && originalEdge != Edge.Left)
        {
            collisions++;
            collidedRight = true;
        }
        if (forward > BackBound && back < BackBound && originalEdge != Edge.Back)
        {
            collisions++;
            collidedForward = true;
        }
        if (back < ForwardBound && forward > ForwardBound && originalEdge != Edge.Forward)
        {
            collisions++;
            collidedBack = true;
        }

        if (collisions != 0 && collisions > 2)
        {
            throw new System.ArgumentOutOfRangeException($"Unexpected number of collisions: {collisions}");
        }

        bool alreadyCollided = false;
        bool ignoreCollision = collisions == 2 && Random.value > .5f;

        float newLeft = left;
        float newRight = right;
        float newForward = forward;
        float newBack = back;

        if (collidedLeft)
        {
            if (ignoreCollision)
            {
                ignoreCollision = false;
            }
            else
            {
                newLeft = RightBound;
                alreadyCollided = true;
            }
        }
        if (collidedRight && !alreadyCollided)
        {
            if (ignoreCollision)
            {
                ignoreCollision = false;
            }
            else
            {
                newLeft = RightBound;
                alreadyCollided = true;
            }
        }
        if (collidedForward && !alreadyCollided)
        {
            if (ignoreCollision)
            {
                ignoreCollision = false;
            }
            else
            {
                newLeft = RightBound;
                alreadyCollided = true;
            }
        }
        if (collidedBack && !alreadyCollided)
        {
            if (!ignoreCollision)
            {
                newLeft = RightBound;
                alreadyCollided = true;
            }
        }

        return (newLeft, newRight, newForward, newBack);
    }
}
