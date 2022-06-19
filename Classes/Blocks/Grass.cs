namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Grass : Block, ICollidable
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

            var ns = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                var coord = this.Coords + Scene.Neighborhood[i];
                var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                ns[i] = block is not null && block is not Empty && !block.IsTransparent;
            }

            if (!ns[0] && ns[1] && !ns[2] && ns[3])
            {
                return Grass.SpriteSource[0];
            }
            else if (ns[0] && ns[1] && !ns[2] && ns[3])
            {
                return Grass.SpriteSource[1];
            }
            else if (ns[0] && !ns[1] && !ns[2] && ns[3])
            {
                return Grass.SpriteSource[2];
            }
            else if (ns[0] && !ns[1] && ns[2] && ns[3])
            {
                return Grass.SpriteSource[3];
            }
            else if (ns[0] && !ns[1] && ns[2] && !ns[3])
            {
                return Grass.SpriteSource[4];
            }
            else if (ns[0] && ns[1] && ns[2] && !ns[3])
            {
                return Grass.SpriteSource[5];
            }
            else if (!ns[0] && ns[1] && ns[2] && !ns[3])
            {
                return Grass.SpriteSource[6];
            }
            else if (!ns[0] && ns[1] && ns[2] && ns[3])
            {
                return Grass.SpriteSource[7];
            }
            else if (!ns[0] && !ns[1] && !ns[2] && ns[3])
            {
                return Grass.SpriteSource[8];
            }
            else if (!ns[0] && !ns[1] && ns[2] && ns[3])
            {
                return Grass.SpriteSource[9];
            }
            else if (!ns[0] && !ns[1] && ns[2] && !ns[3])
            {
                return Grass.SpriteSource[10];
            }
            else if (!ns[0] && ns[1] && !ns[2] && !ns[3])
            {
                return Grass.SpriteSource[11];
            }
            else if (ns[0] && ns[1] && !ns[2] && !ns[3])
            {
                return Grass.SpriteSource[12];
            }
            else if (ns[0] && !ns[1] && !ns[2] && !ns[3])
            {
                return Grass.SpriteSource[13];
            }
            else if (!ns[0] && !ns[1] && !ns[2] && !ns[3])
            {
                return Grass.SpriteSource[14];
            }
            else if (ns[0] && ns[1] && ns[2] && ns[3])
            {
                var coord = this.Coords + new Vector2i(1, 1);
                var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (!(block is not null && block is not Empty && !block.IsTransparent))
                {
                    return Grass.SpriteSource[16];
                }

                coord = this.Coords + new Vector2i(-1, 1);
                block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (!(block is not null && block is not Empty && !block.IsTransparent))
                {
                    return Grass.SpriteSource[17];
                }

                coord = this.Coords + new Vector2i(-1, -1);
                block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (!(block is not null && block is not Empty && !block.IsTransparent))
                {
                    return Grass.SpriteSource[18];
                }

                coord = this.Coords + new Vector2i(1, -1);
                block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (!(block is not null && block is not Empty && !block.IsTransparent))
                {
                    return Grass.SpriteSource[19];
                }

                return Grass.SpriteSource[15];
            }

            return Grass.SpriteSource[15];
        }
    }

    public override int LightDiffusion { get => 50; }

    public override bool IsTransparent { get => false; }

    public override void OnUpdate()
    {
        if (Scene.RandomGenerator.Next(0, 3) == 0)
        {
            foreach (var delta in Scene.ExpandedNeighborhood)
            {
                var coord = this.Coords + delta;
                var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (block is Dirt)
                {
                    if (block.IsBoundary())
                    {
                        block.Chunk.BlockMesh[block.Coords] = new Grass();
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
            Coords = this.Coords,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
        };
}
