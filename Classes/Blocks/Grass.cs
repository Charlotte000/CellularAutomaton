namespace CellularAutomaton.Classes.Blocks;

using SFML.Graphics;
using SFML.System;

public class Grass : Block
{
    private static readonly Sprite[] SpriteSource = new Sprite[]
    {
         new (Scene.Texture, new IntRect(0, 0, 20, 20)),
         new (Scene.Texture, new IntRect(20, 0, 20, 20)),
         new (Scene.Texture, new IntRect(40, 0, 20, 20)),
         new (Scene.Texture, new IntRect(40, 20, 20, 20)),
         new (Scene.Texture, new IntRect(40, 40, 20, 20)),
         new (Scene.Texture, new IntRect(20, 40, 20, 20)),
         new (Scene.Texture, new IntRect(0, 40, 20, 20)),
         new (Scene.Texture, new IntRect(0, 20, 20, 20)),

         new (Scene.Texture, new IntRect(60, 0, 20, 20)),
         new (Scene.Texture, new IntRect(60, 20, 20, 20)),
         new (Scene.Texture, new IntRect(60, 40, 20, 20)),

         new (Scene.Texture, new IntRect(0, 60, 20, 20)),
         new (Scene.Texture, new IntRect(20, 60, 20, 20)),
         new (Scene.Texture, new IntRect(40, 60, 20, 20)),

         new (Scene.Texture, new IntRect(60, 60, 20, 20)),

         new (Scene.Texture, new IntRect(20, 20, 20, 20)),

         new (Scene.Texture, new IntRect(80, 00, 20, 20)),
         new (Scene.Texture, new IntRect(100, 00, 20, 20)),
         new (Scene.Texture, new IntRect(100, 20, 20, 20)),
         new (Scene.Texture, new IntRect(80, 20, 20, 20)),
    };

    public override Sprite Sprite
    {
        get
        {
            if (this.Chunk is null)
            {
                return Grass.SpriteSource[14];
            }

            var neighborhood = new Vector2i[] { new (-1, 0), new (1, 0), new (0, -1), new (0, 1) };

            var neighbors = new bool[4];
            for (int i = 0; i < 4; i++)
            {
                var coord = this.Coord + neighborhood[i];
                var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                neighbors[i] = block is not null && block is not Empty && !block.IsTransparent;
            }

            switch (neighbors[0], neighbors[1], neighbors[2], neighbors[3])
            {
                case (false, true, false, true):
                    return Grass.SpriteSource[0];
                case (true, true, false, true):
                    return Grass.SpriteSource[1];
                case (true, false, false, true):
                    return Grass.SpriteSource[2];
                case (true, false, true, true):
                    return Grass.SpriteSource[3];
                case (true, false, true, false):
                    return Grass.SpriteSource[4];
                case (true, true, true, false):
                    return Grass.SpriteSource[5];
                case (false, true, true, false):
                    return Grass.SpriteSource[6];
                case (false, true, true, true):
                    return Grass.SpriteSource[7];
                case (false, false, false, true):
                    return Grass.SpriteSource[8];
                case (false, false, true, true):
                    return Grass.SpriteSource[9];
                case (false, false, true, false):
                    return Grass.SpriteSource[10];
                case (false, true, false, false):
                    return Grass.SpriteSource[11];
                case (true, true, false, false):
                    return Grass.SpriteSource[12];
                case (true, false, false, false):
                    return Grass.SpriteSource[13];
                case (false, false, false, false):
                    return Grass.SpriteSource[14];
                case (true, true, true, true):
                    var coord = this.Coord + new Vector2i(1, 1);
                    var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                    if (!(block is not null && block is not Empty && !block.IsTransparent))
                    {
                        return Grass.SpriteSource[16];
                    }

                    coord = this.Coord + new Vector2i(-1, 1);
                    block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                    if (!(block is not null && block is not Empty && !block.IsTransparent))
                    {
                        return Grass.SpriteSource[17];
                    }

                    coord = this.Coord + new Vector2i(-1, -1);
                    block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                    if (!(block is not null && block is not Empty && !block.IsTransparent))
                    {
                        return Grass.SpriteSource[18];
                    }

                    coord = this.Coord + new Vector2i(1, -1);
                    block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                    if (!(block is not null && block is not Empty && !block.IsTransparent))
                    {
                        return Grass.SpriteSource[19];
                    }

                    return Grass.SpriteSource[15];
            }
        }
    }

    public override void OnUpdate()
    {
        if (Scene.RandomGenerator.Next(0, 3) == 0)
        {
            foreach (var delta in Scene.ExpandedNeighborhood)
            {
                var coord = this.Coord + delta;
                var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (block is Dirt)
                {
                    if (block.IsBoundary())
                    {
                        block.Chunk.BlockMesh[block.Coord] = new Grass();
                        return;
                    }
                }
            }
        }
    }

    public override Block Copy()
        => new Grass()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
        };
}
