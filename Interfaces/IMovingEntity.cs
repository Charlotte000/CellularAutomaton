namespace CellularAutomaton.Interfaces;

using CellularAutomaton.Classes;
using SFML.System;

public interface IMovingEntity : IEntity
{
    public Vector2f Vel { get; set; }

    public Scene Scene { get; set; }
}
