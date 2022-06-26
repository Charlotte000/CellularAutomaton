namespace CellularAutomaton.Classes.Meshes;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Classes.Walls;
using SFML.Graphics;
using SFML.System;

public class ChunkMesh : Mesh<Chunk, Scene>, IEnumerable<Block>, IEnumerable<(Block block, Wall wall)>
{
    public ChunkMesh(Scene scene)
        : base(scene, new Vector2i(4, 4))
    {
        for (int x = 0; x < this.Width; x++)
        {
            for (int y = 0; y < this.Height; y++)
            {
                this.Grid[x, y] = new Chunk(x * Chunk.Size.X, y * Chunk.Size.Y) { Scene = scene };
            }
        }
    }

    public override Vector2i Coord { get => this.Parent.Coord; }

    public override Chunk? this[int x, int y]
    {
        get
        {
            if (this.IsValidCoord(x, y))
            {
                var localCoord = new Vector2i(x, y) - this.Coord;
                return this.Grid[localCoord.X / Chunk.Size.X, localCoord.Y / Chunk.Size.Y];
            }

            return null;
        }
    }

    public override bool IsValidCoord(int x, int y)
    {
        var topLeftCoord = this.Coord;
        var bottomRightCoord = this.Grid[this.Width - 1, this.Height - 1].Coord + Chunk.Size;
        return x >= topLeftCoord.X && x < bottomRightCoord.X && y >= topLeftCoord.Y && y < bottomRightCoord.Y;
    }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        foreach (var chunk in this.Grid)
        {
            target.Draw(chunk, states);
        }
    }

    public override void SlowUpdate()
    {
        foreach (var chunk in this.Grid)
        {
            chunk.SlowUpdate();
        }
    }

    public override void FastUpdate()
    {
        foreach (var chunk in this.Grid)
        {
            chunk.FastUpdate();
        }
    }

    public void Move(Scene scene)
    {
        var cameraCoord = scene.Entities[0].CollisionBox.Position / Block.Size;
        if (cameraCoord.X < this.Grid[1, 0].Coord.X)
        {
            ChunkMoveHelper.MoveChunksLeft(scene);
            return;
        }

        if (cameraCoord.X > this.Grid[this.Width - 2, 0].Coord.X + Chunk.Size.X)
        {
            ChunkMoveHelper.MoveChunksRight(scene);
            return;
        }

        if (cameraCoord.Y < this.Grid[0, 1].Coord.Y)
        {
            ChunkMoveHelper.MoveChunksUp(scene);
            return;
        }

        if (cameraCoord.Y > this.Grid[0, this.Height - 2].Coord.Y + Chunk.Size.Y)
        {
            ChunkMoveHelper.MoveChunksDown(scene);
            return;
        }
    }

    IEnumerator<Block> IEnumerable<Block>.GetEnumerator()
    {
        foreach (var chunk in this.Grid)
        {
            foreach (var block in chunk.BlockMesh.Grid)
            {
                yield return block;
            }
        }
    }

    IEnumerator<(Block block, Wall wall)> IEnumerable<(Block block, Wall wall)>.GetEnumerator()
    {
        foreach (var chunk in this.Grid)
        {
            for (int x = 0; x < Chunk.Size.X; x++)
            {
                for (int y = 0; y < Chunk.Size.Y; y++)
                {
                    yield return (chunk.BlockMesh.Grid[x, y], chunk.WallMesh.Grid[x, y]);
                }
            }
        }
    }
}