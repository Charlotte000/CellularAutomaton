namespace CellularAutomaton.Interfaces
{
    using SFML.Graphics;
    using SFML.System;

    public interface IBlock : IEntity
    {
        public const int Size = 20;

        public Vector2i Coords { get; set; }

        public int Light { get; set; }

        public int LightDiffusion { get; }

        public IWall Wall { get; set; }

        public bool WasUpdated { get; set; }

        public Sprite Sprite { get; }

        public bool IsTransparent { get; }

        public IBlock Copy();

        public void Dispose()
        {
            this.Wall.Dispose();
            this.CollisionBox.Dispose();
        }
    }
}
