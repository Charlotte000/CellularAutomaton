namespace CellularAutomaton.Classes.Blocks;

using SFML.Graphics;
using SFML.System;

public class Dynamite : Block
{
    public override void OnCreate()
    {
        int splash = 3;
        for (int x = this.Coord.X - splash; x <= this.Coord.X + splash; x++)
        {
            for (int y = this.Coord.Y - splash; y <= this.Coord.Y + splash; y++)
            {
                var delta = this.Coord - new Vector2i(x, y);
                if ((delta.X * delta.X) + (delta.Y * delta.Y) <= splash * splash)
                {
                    var chunk = this.Chunk.Scene.ChunkMesh[x, y];
                    if (chunk is not null)
                    {
                        chunk.BlockMesh[x, y] = new Empty();
                    }
                }
            }
        }
    }

    public override Block Copy()
        => new Dynamite()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
        };
}
