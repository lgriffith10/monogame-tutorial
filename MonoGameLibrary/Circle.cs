using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary;

public readonly struct Circle : IEquatable<Circle>
{
    public Circle(int x, int y, int radius)
    {
        X = x;
        Y = y;
        Radius = radius;
    }

    public Circle(Point location, int radius)
    {
        X = location.X;
        Y = location.Y;
        Radius = radius;
    }

    public static Circle Empty { get; } = new();

    public readonly int X;
    public readonly int Y;
    public readonly int Radius;

    public readonly bool IsEmpty => X == 0 && Y == 0 && Radius == 0;

    public readonly Point Location => new(x: X, y: Y);

    public readonly int Top => Y - Radius;
    public readonly int Bottom => Y + Radius;
    public readonly int Left => X - Radius;
    public readonly int Right => X + Radius;

    public bool Intersects(Circle other)
    {
        var radiusSquared = (Radius + other.Radius) * (Radius + other.Radius);
        var distanceSquared = Vector2.DistanceSquared(value1: Location.ToVector2(), value2: other.Location.ToVector2());

        return distanceSquared < radiusSquared;
    }

    public bool Equals(Circle other)
    {
        return X == other.X
               && Y == other.Y
               && Radius == other.Radius;
    }

    public override bool Equals(object obj)
    {
        return obj is Circle other && Equals(other: other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(value1: X, value2: Y, value3: Radius);
    }

    public static bool operator ==(Circle lhs, Circle rhs)
    {
        return lhs.Equals(other: rhs);
    }

    public static bool operator !=(Circle lhs, Circle rhs)
    {
        return !lhs.Equals(other: rhs);
    }
}