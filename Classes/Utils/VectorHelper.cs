namespace CellularAutomaton.Classes.Utils;

using SFML.System;

public static class VectorHelper
{
    public static Vector2f Mult(this Vector2f v1, Vector2f v2)
        => new (v1.X * v2.X, v1.Y * v2.Y);

    public static Vector2f Div(this Vector2f v1, Vector2f v2)
        => new (v1.X / v2.X, v1.Y / v2.Y);

    public static float MagSq(this Vector2f v)
        => (v.X * v.X) + (v.Y * v.Y);

    public static float Mag(this Vector2f v)
        => MathF.Sqrt((v.X * v.X) + (v.Y * v.Y));

    public static Vector2f Normalize(this Vector2f v)
        => v / v.Mag();

    public static Vector2f Constrain(this Vector2f v, float mag)
    {
        var currentMag = v.Mag();
        return currentMag <= mag ? v : v / currentMag * mag;
    }

    public static Vector2f Abs(this Vector2f v)
        => new (MathF.Abs(v.X), MathF.Abs(v.Y));

    public static Vector2i Floor(this Vector2f v)
        => new ((int)Math.Floor(v.X), (int)Math.Floor(v.Y));

    public static bool XYEquals(this Vector2f v, float value)
        => v.X == value && v.Y == value;

    public static Vector2f Random()
        => new (
            (float)(Scene.RandomGenerator.NextDouble() * 2) - 1,
            (float)(Scene.RandomGenerator.NextDouble() * 2) - 1);
}
