namespace CellularAutomaton.Interfaces;

using SFML.System;

public interface IThrowable : IMovingEntity
{
    public float ThrowMag { get; }

    public void Throw(IEntity owner, Vector2f mousePosition)
    {
        var ownerPosition = owner.CollisionBox.Position + (owner.CollisionBox.Size / 2);

        this.CollisionBox.Position = ownerPosition;

        var direction = mousePosition - ownerPosition;
        var directionMag = MathF.Sqrt((direction.X * direction.X) + (direction.Y * direction.Y));

        this.Vel = direction / directionMag * this.ThrowMag;
    }
}
