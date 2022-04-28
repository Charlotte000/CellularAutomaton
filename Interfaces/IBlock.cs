namespace CellularAutomaton.Interfaces
{
    using SFML.System;

    public interface IBlock : IEntity
    {
        public const int Size = 20;

        public Vector2i Coords { get; set; }

        public byte Light { get; set; }

        public byte LightDiffusion { get; set; }

        public IWall Wall { get; set; }

        public IBlock Copy();

        public void Dispose()
        {
            this.Wall.Dispose();
            this.CollisionBox.Dispose();
        }
    }
}
