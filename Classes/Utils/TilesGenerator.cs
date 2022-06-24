namespace CellularAutomaton.Classes.Utils;

using CellularAutomaton.Classes.Blocks;
using SFML.Graphics;
using SFML.System;

public static class TilesGenerator
{
    public static Sprite[] GenerateSource(Sprite s1, Sprite s2, Sprite s3, Sprite s4, Sprite s5, Sprite s6, Sprite s7)
    {
        return new Sprite[]
        {
            s1,
            s2,
            TilesGenerator.Rotate(s2, 90),
            s3,
            TilesGenerator.Rotate(s2, 180),
            s4,
            TilesGenerator.Rotate(s3, 90),
            s5,
            TilesGenerator.Rotate(s2, 270),
            TilesGenerator.FlipX(s3),
            TilesGenerator.Rotate(s4, 90),
            TilesGenerator.Rotate(s5, 270),
            TilesGenerator.Rotate(s3, 180),
            TilesGenerator.Rotate(s5, 180),
            TilesGenerator.Rotate(s5, 90),
            s6,

            TilesGenerator.AddCorner(s6, s7, 0, 90, 180, 270),
            TilesGenerator.AddCorner(s6, s7, 90, 180, 270),
            TilesGenerator.AddCorner(s6, s7, 0, 180, 270),
            TilesGenerator.AddCorner(s6, s7, 180, 270),
            TilesGenerator.AddCorner(s6, s7, 0, 90, 270),
            TilesGenerator.AddCorner(s6, s7, 90, 270),
            TilesGenerator.AddCorner(s6, s7, 0, 270),
            TilesGenerator.AddCorner(s6, s7, 270),
            TilesGenerator.AddCorner(s6, s7, 0, 90, 180),
            TilesGenerator.AddCorner(s6, s7, 90, 180),
            TilesGenerator.AddCorner(s6, s7, 0, 180),
            TilesGenerator.AddCorner(s6, s7, 180),
            TilesGenerator.AddCorner(s6, s7, 0, 90),
            TilesGenerator.AddCorner(s6, s7, 90),
            TilesGenerator.AddCorner(s6, s7, 0),

            TilesGenerator.AddCorner(s3, s7, 90),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s3, s7, 90), 90),
            TilesGenerator.AddCorner(s5, s7, 90),
            TilesGenerator.AddCorner(s5, s7, 180),
            TilesGenerator.AddCorner(s5, s7, 90, 180),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s3, s7, 90), 270),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s5, s7, 90), 270),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s5, s7, 180), 270),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s5, s7, 90, 180), 270),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s3, s7, 90), 180),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s5, s7, 180), 180),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s5, s7, 90), 180),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s5, s7, 90, 180), 180),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s5, s7, 90), 90),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s5, s7, 180), 90),
            TilesGenerator.Rotate(TilesGenerator.AddCorner(s5, s7, 90, 180), 90),
        };
    }

    public static int GetSpriteIndex(Block block)
    {
        if (block.Chunk is null)
        {
            return 0;
        }

        var index = TilesGenerator.GetIndex(block, new Vector2i[] { new (0, -1), new (1, 0), new (0, 1), new (-1, 0) });

        if (index == 3 && !TilesGenerator.HasNeighbour(block, new Vector2i(1, -1)))
        {
            return 31;
        }

        if (index == 6 && !TilesGenerator.HasNeighbour(block, new Vector2i(1, 1)))
        {
            return 32;
        }

        if (index == 7 && !TilesGenerator.HasNeighbour(block, new Vector2i(1, -1)) &&
            TilesGenerator.HasNeighbour(block, new Vector2i(1, 1)))
        {
            return 33;
        }

        if (index == 7 && TilesGenerator.HasNeighbour(block, new Vector2i(1, -1)) &&
            !TilesGenerator.HasNeighbour(block, new Vector2i(1, 1)))
        {
            return 34;
        }

        if (index == 7 && !TilesGenerator.HasNeighbour(block, new Vector2i(1, -1)) &&
            !TilesGenerator.HasNeighbour(block, new Vector2i(1, 1)))
        {
            return 35;
        }

        if (index == 9 && !TilesGenerator.HasNeighbour(block, new Vector2i(-1, -1)))
        {
            return 36;
        }

        if (index == 11 && !TilesGenerator.HasNeighbour(block, new Vector2i(-1, -1)) &&
            TilesGenerator.HasNeighbour(block, new Vector2i(1, -1)))
        {
            return 37;
        }

        if (index == 11 && TilesGenerator.HasNeighbour(block, new Vector2i(-1, -1)) &&
            !TilesGenerator.HasNeighbour(block, new Vector2i(1, -1)))
        {
            return 38;
        }

        if (index == 11 && !TilesGenerator.HasNeighbour(block, new Vector2i(-1, -1)) &&
            !TilesGenerator.HasNeighbour(block, new Vector2i(1, -1)))
        {
            return 39;
        }

        if (index == 12 && !TilesGenerator.HasNeighbour(block, new Vector2i(-1, 1)))
        {
            return 40;
        }

        if (index == 13 && !TilesGenerator.HasNeighbour(block, new Vector2i(-1, -1)) &&
            TilesGenerator.HasNeighbour(block, new Vector2i(-1, 1)))
        {
            return 41;
        }

        if (index == 13 && TilesGenerator.HasNeighbour(block, new Vector2i(-1, -1)) &&
            !TilesGenerator.HasNeighbour(block, new Vector2i(-1, 1)))
        {
            return 42;
        }

        if (index == 13 && !TilesGenerator.HasNeighbour(block, new Vector2i(-1, -1)) &&
            !TilesGenerator.HasNeighbour(block, new Vector2i(-1, 1)))
        {
            return 43;
        }

        if (index == 14 && !TilesGenerator.HasNeighbour(block, new Vector2i(1, 1)) &&
            TilesGenerator.HasNeighbour(block, new Vector2i(-1, 1)))
        {
            return 44;
        }

        if (index == 14 && TilesGenerator.HasNeighbour(block, new Vector2i(1, 1)) &&
            !TilesGenerator.HasNeighbour(block, new Vector2i(-1, 1)))
        {
            return 45;
        }

        if (index == 14 && !TilesGenerator.HasNeighbour(block, new Vector2i(1, 1)) &&
            !TilesGenerator.HasNeighbour(block, new Vector2i(-1, 1)))
        {
            return 46;
        }

        if (index == 15)
        {
            var index2 = TilesGenerator.GetIndex(block, new Vector2i[] { new (-1, -1), new (1, -1), new (1, 1), new (-1, 1) });

            if (index2 == 15)
            {
                return 15;
            }

            return index + index2 + 1;
        }

        return index;
    }

    private static bool HasNeighbour(Block block, Vector2i delta)
    {
        var coord = block.Coord + delta;
        var neighbour = block.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
        return neighbour is not null && !neighbour.IsTransparent;
    }

    private static Sprite AddCorner(Sprite origin, Sprite corner, params int[] angles)
    {
        var renderTexture = new RenderTexture((uint)Block.Size, (uint)Block.Size);
        renderTexture.Draw(origin);

        foreach (var angle in angles)
        {
            var renderState = RenderStates.Default;
            renderState.Transform.Rotate(angle, new Vector2f(Block.Size, Block.Size) / 2);
            renderTexture.Draw(corner, renderState);
        }

        renderTexture.Display();

        return new Sprite(new Texture(renderTexture.Texture));
    }

    private static Sprite Rotate(Sprite origin, int angle)
    {
        var renderState = RenderStates.Default;
        renderState.Transform.Rotate(angle, new Vector2f(Block.Size, Block.Size) / 2);

        var renderTexture = new RenderTexture((uint)Block.Size, (uint)Block.Size);
        renderTexture.Draw(origin, renderState);
        renderTexture.Display();

        return new Sprite(new Texture(renderTexture.Texture));
    }

    private static Sprite FlipX(Sprite origin)
    {
        var renderState = RenderStates.Default;
        renderState.Transform.Scale(new Vector2f(-1, 1), new Vector2f(Block.Size, Block.Size) / 2);

        var renderTexture = new RenderTexture((uint)Block.Size, (uint)Block.Size);
        renderTexture.Draw(origin, renderState);
        renderTexture.Display();

        return new Sprite(new Texture(renderTexture.Texture));
    }

    private static int GetIndex(Block block, Vector2i[] neighborhood)
    {
        var index = 0;
        for (int i = 0; i < 4; i++)
        {
            if (TilesGenerator.HasNeighbour(block, neighborhood[i]))
            {
                index |= 1 << i;
            }
        }

        return index;
    }
}
