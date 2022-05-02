namespace CellularAutomaton.Interfaces
{
    using SFML.System;

    public interface IMovingEntity : IEntity, ICollidable
    {
        public static Vector2f Gravity { get; set; } = new Vector2f(0, 1f);

        public Vector2f Vel { get; set; }

        public bool IsOnGround { get; set; }

        public bool IsOnWater { get; set; }

        public void OnCollision(IEntity entity, Vector2f normal);
    }
}
