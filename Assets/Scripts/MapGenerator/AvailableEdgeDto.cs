using System.Collections.Generic;

[System.Serializable]
public class AvailableEdgeDto
{
    public Edge Edge;
    public List<(float start, float end)> AvailableLines;

    public List<AvailableLineDto> DebugLines = new List<AvailableLineDto>();
    public void UpdateDebug()
    {
        DebugLines.Clear();
        foreach ((float start, float end) in AvailableLines)
        {
            DebugLines.Add(new() { Start = start, End = end });
        }
    }
}

[System.Serializable]
public class AvailableLineDto
{
    public float Start;
    public float End;
}