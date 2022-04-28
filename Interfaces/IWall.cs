namespace CellularAutomaton.Interfaces
{
    using SFML.System;

    public interface IWall : IEntity
    {
        public Vector2i Coords { get; set; }

        public byte Light { get; set; }

        public byte LightDiffusion { get; set; }

        public IWall Copy();

        public void Dispose()
        {
            this.CollisionBox.Dispose();
        }
    }
}
