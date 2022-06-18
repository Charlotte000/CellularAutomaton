namespace CellularAutomaton.Interfaces;

using CellularAutomaton.Classes;
using SFML.System;

public interface IMovingEntity : IEntity
{
    public static Vector2f Gravity { get; set; } = new Vector2f(0, 1f);

    public Vector2f Vel { get; set; }

    public Scene Scene { get; set; }
}
