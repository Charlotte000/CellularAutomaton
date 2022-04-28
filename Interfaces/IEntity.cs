namespace CellularAutomaton.Interfaces
{
    using CellularAutomaton.Classes;
    using SFML.Graphics;

    public interface IEntity
    {
        public RectangleShape CollisionBox { get; set; }

        public bool IsCollidable { get; set; }

        public void Draw(RenderWindow window);

        public void Update(Scene scene);

    }
}
