namespace CellularAutomaton.Classes.Meshes;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Classes.Walls;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

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

    public override void OnFixedUpdate()
    {
        foreach (var chunk in this.Grid)
        {
            chunk.OnFixedUpdate();
        }
    }

    public override void OnUpdate()
    {
        this.Move();
        this.UpdateNearest(!Keyboard.IsKeyPressed(Keyboard.Key.LShift));

        foreach (var chunk in this.Grid)
        {
            chunk.OnUpdate();
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

    private void UpdateNearest(bool isBlock = true)
    {
        var origin = this.Parent.Entities[0].CollisionBox.Position + (this.Parent.Entities[0].CollisionBox.Size / 2);
        var direction = ((Vector2f)(this.Parent.GetMouseCoords() * Block.Size) +
            (new Vector2f(Block.Size, Block.Size) / 2) - origin).Constrain(50);
        var buffer = new List<(float time, IGameObject gameObject)>();

        foreach (var chunk in this)
        {
            for (int x = 0; x < Chunk.Size.X; x++)
            {
                for (int y = 0; y < Chunk.Size.Y; y++)
                {
                    var block = chunk.BlockMesh.Grid[x, y];
                    if (!isBlock && !block.IsTransparent)
                    {
                        continue;
                    }

                    IGameObject gameObject = isBlock ? block : chunk.WallMesh.Grid[x, y];
                    if (chunk.VisibilityMesh.Grid[x, y] && !gameObject.IsIndestructible)
                    {
                        using var box = new RectangleShape(new Vector2f(Block.Size, Block.Size))
                        { Position = gameObject.CollisionBox.Position - gameObject.CollisionBox.Origin };
                        if (AABBCollision.RayVsRect(origin, direction, box, out _, out var time))
                        {
                            if (time >= 0 && time < 1)
                            {
                                buffer.Add((time, gameObject));
                            }
                        }
                    }
                }
            }
        }

        buffer.Sort((item1, item2) => item1.time.CompareTo(item2.time));
        this.Parent.Nearest = buffer.Count > 0 ? buffer[0].gameObject : null;
    }
}