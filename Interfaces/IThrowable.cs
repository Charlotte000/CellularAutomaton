namespace CellularAutomaton.Interfaces;

using SFML.System;

public interface IThrowable
{
    public float ThrowMag { get; }

    public IGameObject? ThrowOwner { get; }

    public void Throw(Vector2f mousePosition);
}
