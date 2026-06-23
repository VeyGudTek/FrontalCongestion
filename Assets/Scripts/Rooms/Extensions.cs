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
}
