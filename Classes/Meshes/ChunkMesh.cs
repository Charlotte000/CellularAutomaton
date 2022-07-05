namespace CellularAutomaton.Classes.Meshes;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Classes.Walls;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class ChunkMesh : Mesh<Chunk, Scene>, IEnumerable<Block>, IEnumerable<Wall>, IEnumerable<(Block block, Wall wall)>
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

    public override void OnUpdate()
    {
        base.OnUpdate();

        this.Move();
        this.UpdateNearest();

        foreach (var chunk in this.Grid)
        {
            chunk.OnUpdate();
        }
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        foreach (var chunk in this.Grid)
        {
            chunk.OnFixedUpdate();
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        foreach (var chunk in this.Grid)
        {
            chunk.OnDestroy();
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

    IEnumerator<Wall> IEnumerable<Wall>.GetEnumerator()
    {
        foreach (var chunk in this.Grid)
        {
            foreach (var wall in chunk.WallMesh.Grid)
            {
                yield return wall;
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

    private void Move()
    {
        var cameraCoord = this.Parent.Entities[0].CollisionBox.Position / Block.Size;
        if (cameraCoord.X < this.Grid[1, 0].Coord.X)
        {
            ChunkMoveHelper.MoveChunksLeft(this.Parent);
            return;
        }

        if (cameraCoord.X > this.Grid[this.Width - 2, 0].Coord.X + Chunk.Size.X)
        {
            ChunkMoveHelper.MoveChunksRight(this.Parent);
            return;
        }

        if (cameraCoord.Y < this.Grid[0, 1].Coord.Y)
        {
            ChunkMoveHelper.MoveChunksUp(this.Parent);
            return;
        }

        if (cameraCoord.Y > this.Grid[0, this.Height - 2].Coord.Y + Chunk.Size.Y)
        {
            ChunkMoveHelper.MoveChunksDown(this.Parent);
            return;
        }
    }

    private void UpdateNearest()
    {
        var wallMode = this.Parent.WallMode;

        var origin = this.Parent.Entities[0].CollisionBox.Position + (this.Parent.Entities[0].CollisionBox.Size / 2);
        var direction = (Vector2f)(this.Parent.GetMouseCoords() * Block.Size) +
            (new Vector2f(Block.Size, Block.Size) / 2) - origin;

        if (!this.Parent.DiggerMode)
        {
            if (direction.MagSq() <= this.Parent.BuildingDistance * this.Parent.BuildingDistance)
            {
                var coord = this.Parent.GetMouseCoords();
                var block = this[coord]?.BlockMesh[coord];
                if (block is null || (wallMode && !block.IsTransparent))
                {
                    this.Parent.Nearest = null;
                    return;
                }

                IGameObject? gameObject = !wallMode ? block.Chunk?.BlockMesh[coord] : block.Chunk?.WallMesh[coord];
                this.Parent.Nearest = gameObject is null || !gameObject.IsIndestructible ? gameObject : null;
            }
            else
            {
                this.Parent.Nearest = null;
            }

            return;
        }

        direction = direction.Constrain(this.Parent.BuildingDistance);

        var minTime = float.MaxValue;
        IGameObject? selected = null;
        var buffer = new List<(float time, IGameObject gameObject)>();

        foreach (var block in this as IEnumerable<Block>)
        {
            if (wallMode && !block.IsTransparent)
            {
                continue;
            }

            var localCoord = block.Coord - block.Chunk.Coord;
            IGameObject gameObject = !wallMode ? block : block.Chunk.WallMesh.Grid[localCoord.X, localCoord.Y];
            if (block.Chunk.VisibilityMesh.Grid[localCoord.X, localCoord.Y] && !gameObject.IsIndestructible)
            {
                using var box = new RectangleShape(new Vector2f(Block.Size, Block.Size))
                { Position = gameObject.CollisionBox.Position - gameObject.CollisionBox.Origin };
                if (AABBCollision.RayVsRect(origin, direction, box, out _, out var time))
                {
                    if (time < 1 && time < minTime)
                    {
                        minTime = time;
                        selected = gameObject;
                    }
                }
            }
        }

        this.Parent.Nearest = selected;
    }
}