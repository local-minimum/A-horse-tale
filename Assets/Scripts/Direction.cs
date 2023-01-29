using System.Collections.Generic;
using UnityEngine;

public enum Direction { North, NorthWest, West, SouthWest, South, SouthEast, East, NorthEast };

public static class DirectionClassExtensions
{
    private static readonly List<Direction> directions = new List<Direction>
    {
        Direction.North,
        Direction.NorthEast,
        Direction.East,
        Direction.SouthEast,
        Direction.South,
        Direction.SouthWest,
        Direction.West,
        Direction.NorthWest,
    };

    private static readonly int nDirections = directions.Count;

    public static Direction RotateCW(this Direction direction, int steps = 2)
    {
        var index = (directions.IndexOf(direction) + steps) % nDirections;
        if (index < 0) index += nDirections;
        return directions[index];
    }

    public static Direction RotateCCW(this Direction direction, int steps = 2)
    {
        return direction.RotateCW(-steps);
    }
    
    public static Vector3 AsDirectionVector3(this Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return new Vector3(0, 0, 1);
            case Direction.NorthEast:
                return new Vector3(1, 0, 1);
            case Direction.East:
                return new Vector3(1, 0, 0);
            case Direction.SouthEast:
                return new Vector3(1, 0, -1);
            case Direction.South:
                return new Vector3(0, 0, -1);
            case Direction.SouthWest:
                return new Vector3(-1, 0, -1);
            case Direction.West:
                return new Vector3(-1, 0, 0);
            case Direction.NorthWest:
                return new Vector3(-1, 0, 1);
            default:
                throw new System.ArgumentException($"{direction} not known as a vector");
        }
    }
}
