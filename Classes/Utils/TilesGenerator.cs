namespace CellularAutomaton.Classes.Utils;

using CellularAutomaton.Classes.Blocks;
using SFML.Graphics;
using SFML.System;

public static class TilesGenerator
{
    public static Sprite[] GenerateSource(Vector2i coord)
        => new Sprite[]
        {
            new (Scene.Texture, new IntRect(coord.X, coord.Y, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 20, coord.Y, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 40, coord.Y, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 40, coord.Y + 20, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 40, coord.Y + 40, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 20, coord.Y + 40, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X, 40, coord.Y + 20, 20)),
            new (Scene.Texture, new IntRect(coord.X, 20, coord.Y + 20, 20)),

            new (Scene.Texture, new IntRect(coord.X + 60, coord.Y, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 60, coord.Y + 20, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 60, coord.Y + 40, 20, 20)),

            new (Scene.Texture, new IntRect(coord.X, coord.Y + 60, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 20, coord.Y + 60, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 40, coord.Y + 60, 20, 20)),

            new (Scene.Texture, new IntRect(coord.X + 60, coord.Y + 60, 20, 20)),

            new (Scene.Texture, new IntRect(coord.X + 20, coord.Y + 20, 20, 20)),

            new (Scene.Texture, new IntRect(coord.X + 80, coord.Y, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 100, coord.Y, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 100, coord.Y + 20, 20, 20)),
            new (Scene.Texture, new IntRect(coord.X + 80, coord.Y + 20, 20, 20)),
        };

    public static int GetSpriteIndex(Block presenter)
    {
        if (presenter.Chunk is null)
        {
            return 14;
        }

        var neighborhood = new Vector2i[] { new (-1, 0), new (1, 0), new (0, -1), new (0, 1) };

        var neighbors = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            var coord = presenter.Coord + neighborhood[i];
            var block = presenter.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
            neighbors[i] = block is not null && block is not Empty && !block.IsTransparent;
        }

        switch (neighbors[0], neighbors[1], neighbors[2], neighbors[3])
        {
            case (false, true, false, true):
                return 0;
            case (true, true, false, true):
                return 1;
            case (true, false, false, true):
                return 2;
            case (true, false, true, true):
                return 3;
            case (true, false, true, false):
                return 4;
            case (true, true, true, false):
                return 5;
            case (false, true, true, false):
                return 6;
            case (false, true, true, true):
                return 7;
            case (false, false, false, true):
                return 8;
            case (false, false, true, true):
                return 9;
            case (false, false, true, false):
                return 10;
            case (false, true, false, false):
                return 11;
            case (true, true, false, false):
                return 12;
            case (true, false, false, false):
                return 13;
            case (false, false, false, false):
                return 14;
            case (true, true, true, true):
                var coord = presenter.Coord + new Vector2i(1, 1);
                var block = presenter.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (!(block is not null && block is not Empty && !block.IsTransparent))
                {
                    return 16;
                }

                coord = presenter.Coord + new Vector2i(-1, 1);
                block = presenter.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (!(block is not null && block is not Empty && !block.IsTransparent))
                {
                    return 17;
                }

                coord = presenter.Coord + new Vector2i(-1, -1);
                block = presenter.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (!(block is not null && block is not Empty && !block.IsTransparent))
                {
                    return 18;
                }

                coord = presenter.Coord + new Vector2i(1, -1);
                block = presenter.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
                if (!(block is not null && block is not Empty && !block.IsTransparent))
                {
                    return 19;
                }

                return 15;
        }
    }
}
