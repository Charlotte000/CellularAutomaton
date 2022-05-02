namespace CellularAutomaton.Interfaces
{
    using SFML.System;

    public interface IWall : IEntity
    {
        public Vector2i Coords { get; set; }

        public int Light { get; set; }

        public int LightDiffusion { get; set; }

        public IWall Copy();

        public void Dispose()
        {
            this.CollisionBox.Dispose();
        }
    }
}
