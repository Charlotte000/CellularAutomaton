namespace CellularAutomaton.Interfaces;

using SFML.System;

public interface IThrowable
{
    public float ThrowMag { get; }

    public void Throw(IGameObject owner, Vector2f mousePosition);
}
