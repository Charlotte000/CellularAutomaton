namespace CellularAutomaton.Interfaces
{
    using CellularAutomaton.Classes;
    using SFML.Graphics;
    using SFML.System;

    public interface IEntity
    {
        public RectangleShape CollisionBox { get; set; }

        public bool IsVisible { get; set; }

        public void Draw(RenderWindow window);

        public void Update(Scene scene);

        public int Light { get; set; }

        public void OnCollision(IEntity entity, Vector2f? contactNormal);
    }
}
