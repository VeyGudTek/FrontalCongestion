using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum Edge
{
    Left,
    Right,
    Forward,
    Back
}

public class Room : MonoBehaviour
{
    [SerializeField]
    Transform Template;

    [SerializeField]
    private float Level;
    [SerializeField]
    private float Height;

    [Header("Bounds")]
    [SerializeField]
    private float LeftBound;
    [SerializeField]
    private float RightBound;
    [SerializeField]
    private float ForwardBound;
    [SerializeField]
    private float BackBound;

    [Header("Peripheral")]
    public float AvailablePerimeter = 0f;
    [SerializeField]
    List<AvailableEdgeDto> AvailableEdges = new List<AvailableEdgeDto>();
    [SerializeField]
    List<NeighborDto> Neighbors = new List<NeighborDto>();

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

        AvailableEdges.Add(new AvailableEdgeDto() { Edge = Edge.Left, AvailableLines =    new List<(float start, float end)> { (back, forward) } });
        AvailableEdges.Add(new AvailableEdgeDto() { Edge = Edge.Right, AvailableLines =   new List<(float start, float end)> { (back, forward) } });
        AvailableEdges.Add(new AvailableEdgeDto() { Edge = Edge.Forward, AvailableLines = new List<(float start, float end)> { (left, right) } });
        AvailableEdges.Add(new AvailableEdgeDto() { Edge = Edge.Back, AvailableLines =    new List<(float start, float end)> { (left, right) } });
        foreach(AvailableEdgeDto ae in AvailableEdges)
        {
            ae.UpdateDebug();
        }

        UpdateAvailablePerimeter();
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        Template.transform.position = new Vector3(CenterX, CenterY, CenterZ);
        Template.localScale = new Vector3(ScaleX, Height, ScaleZ);
    }

    public (Edge edge, Vector3 point) GetRandomAvailablePoint()
    {
        float randomThreshold = Random.value * AvailablePerimeter;
        float currentThreshold = 0f;

        foreach (AvailableEdgeDto ae in AvailableEdges)
        {
            foreach ((float start, float end) in ae.AvailableLines)
            {
                currentThreshold += (end - start);

                if (currentThreshold > randomThreshold || Mathf.Approximately(currentThreshold, randomThreshold))
                {
                    float randomValue = Random.Range(start, end);
                    float staticValue = GetBoundForEdge(ae.Edge);

                    Vector3 newVector = new Vector3(
                        ae.Edge.IsLateral() ? staticValue : randomValue,
                        Level,
                        ae.Edge.IsLateral() ? randomValue : staticValue
                    );

                    return (ae.Edge, newVector);
                }
            }
        }

        throw new System.ArgumentOutOfRangeException("RandomThreshold Greater than total perimeter.");
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

    public (float left, float right, float forward, float back, Edge sharedEdge) GetClampedDimensions(float left, float right, float forward, float back, Edge originalEdge, Vector3 startingPoint)
    {
        List<Edge> updatedEdges = new List<Edge>();
        float clampedLeft = left;
        float clampedRight = right;
        float clampedForward = forward;
        float clampedBack = back;

        //Partially Enveloped Collision
        if (originalEdge != Edge.Right && RightBound < startingPoint.x)
        {
            clampedLeft = RightBound;
            updatedEdges.Add(Edge.Left);
        }
        if (originalEdge != Edge.Left && LeftBound > startingPoint.x)
        {
            clampedRight = LeftBound;
            updatedEdges.Add(Edge.Right);
        }
        if (originalEdge != Edge.Back && BackBound > startingPoint.z)
        {
            clampedForward = BackBound;
            updatedEdges.Add(Edge.Forward);
        }
        if (originalEdge != Edge.Forward && ForwardBound < startingPoint.z)
        {
            clampedBack = ForwardBound;
            updatedEdges.Add(Edge.Back);
        }

        //Fully Enveloped Collision
        if (updatedEdges.Count == 0)
        {
            if (originalEdge == Edge.Left)
            {
                clampedLeft = RightBound;
                updatedEdges.Add(Edge.Left);
            }
            if (originalEdge == Edge.Right)
            {
                clampedRight = LeftBound;
                updatedEdges.Add(Edge.Right);
            }
            if (originalEdge == Edge.Forward)
            {
                clampedForward = BackBound;
                updatedEdges.Add(Edge.Forward);
            }
            if (originalEdge == Edge.Back)
            {
                clampedBack = ForwardBound;
                updatedEdges.Add(Edge.Back);
            }
        }

        if (updatedEdges.Count != 1 && updatedEdges.Count != 2)
        {
            throw new System.InvalidOperationException($"Unexpected number of clamped values: {updatedEdges.Count}");
        }

        Edge randomUpdatedEdge = updatedEdges[Random.Range(0, updatedEdges.Count)];

        if (randomUpdatedEdge == Edge.Left)
            return (clampedLeft, right, forward, back, Edge.Left);
        if (randomUpdatedEdge == Edge.Right)
            return (left, clampedRight, forward, back, Edge.Right);
        if (randomUpdatedEdge == Edge.Forward)
            return (left, right, clampedForward, back, Edge.Forward);
        if (randomUpdatedEdge == Edge.Back)
            return (left, right, forward, clampedBack, Edge.Back);

        throw new System.InvalidOperationException("Random Updated Value did not match any clamped values.");
    }

    public void AddNeighbor(Edge sharedEdge, Room neighbor)
    {
        Neighbors.Add(new() { Neighbor = neighbor, SharedEdge = sharedEdge });

        float neighborStart = sharedEdge.IsLateral() ? neighbor.BackBound : neighbor.LeftBound;
        float neighborEnd = sharedEdge.IsLateral() ? neighbor.ForwardBound : neighbor.RightBound;

        AvailableEdgeDto edgeToShrink = AvailableEdges.Where(e => e.Edge == sharedEdge).First();

        List<(float start, float end)> newAvailableLines = new List<(float start, float end)>();
        foreach((float start, float end) in edgeToShrink.AvailableLines)
        {
            if (neighborStart >= end || neighborEnd <= start)
            {
                newAvailableLines.Add((start, end));
                continue;
            }

            if (neighborStart <= start && neighborEnd >= end)
            {
                continue;
            }

            if (neighborStart > start && neighborEnd < end)
            {
                newAvailableLines.Add((start, neighborStart));
                newAvailableLines.Add((neighborEnd, end));
                continue;
            }

            if (neighborEnd >= end)
            {
                newAvailableLines.Add((start, neighborStart));
            }
            else
            {
                newAvailableLines.Add((neighborEnd, end));
            }
        }

        edgeToShrink.AvailableLines = newAvailableLines;
        edgeToShrink.UpdateDebug();
        UpdateAvailablePerimeter();
    }

    private void UpdateAvailablePerimeter()
    {
        float newAvailablePerimeter = 0f;
        foreach (AvailableEdgeDto ae in AvailableEdges)
        {
            foreach ((float start, float end) in ae.AvailableLines)
            {
                newAvailablePerimeter += end - start;
            }
        }

        AvailablePerimeter = newAvailablePerimeter;
    }
}
