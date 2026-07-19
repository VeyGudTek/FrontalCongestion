public static class EdgeExtensions
{
    public static bool IsLateral(this Edge edge)
    {
        return edge == Edge.Left || edge == Edge.Right;
    }

    public static Edge GetOpposite(this Edge edge)
    {
        switch (edge)
        {
            case Edge.Left:
                return Edge.Right;
            case Edge.Right:
                return Edge.Left;
            case Edge.Forward:
                return Edge.Back;
            case Edge.Back:
                return Edge.Forward;
            default:
                throw new System.ArgumentOutOfRangeException($"Unknown Edge: {edge}");
        }
    }
}