namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Door : Block, IClickable
{
    private static readonly Sprite[] SpriteSource = new Sprite[]
    {
        new (Scene.Texture, new IntRect(0, 40, 20, 40)) { Origin = new Vector2f(0, Block.Size) },
        new (Scene.Texture, new IntRect(20, 40, 20, 40)) { Origin = new Vector2f(0, Block.Size) },

        new (Scene.Texture, new IntRect(40, 40, 20, 40)) { Origin = new Vector2f(0, Block.Size) },
        new (Scene.Texture, new IntRect(60, 40, 20, 40)) { Origin = new Vector2f(0, Block.Size) },
    };

    private Door? upper;

    private Door? lower;

    public Door() // ToDo: door blockHistory
    {
        this.CollisionBox = new (new Vector2f(Block.Size / 5, Block.Size))
        { Origin = this.IsLeft ? new Vector2f(0, 0) : new Vector2f(-Block.Size * 4f / 5f, 0) };
    }

    public override Sprite Sprite
    { get => Door.SpriteSource[this.IsLeft ? this.IsOpened ? 1 : 0 : this.IsOpened ? 3 : 2]; }

    public override int LightDiffusion { get => this.IsOpened ? 15 : 50; }

    public override bool IsTransparent { get => true; }

    public override bool IsCollidable { get => !this.IsOpened; }

    public bool IsOpened { get; set; } = true;

    public bool IsLeft { get; set; } = true;

    public override RectangleShape CollisionBox { get; set; }

    public override void Draw(RenderTarget target, RenderStates states)
    {
        if (this.upper is not null)
        {
            base.Draw(target, states);
        }
    }

    public override void OnCreate()
    {
        base.OnCreate();

        var upperCoord = this.Coord + new Vector2i(0, -1);
        var upperChunk = this.Chunk.Scene.ChunkMesh[upperCoord];
        if (upperChunk?.BlockMesh[upperCoord] is Empty)
        {
            this.upper = new () { lower = this, IsLeft = this.IsLeft };
            upperChunk.BlockMesh[upperCoord] = this.upper;
        }
        else
        {
            var lowerCoord = this.Coord + new Vector2i(0, 1);
            var lowerChunk = this.Chunk.Scene.ChunkMesh[lowerCoord];
            if (lowerChunk?.BlockMesh[lowerCoord] is Empty)
            {
                this.lower = new () { upper = this, IsLeft = this.IsLeft };
                lowerChunk.BlockMesh[lowerCoord] = this.lower;
            }
            else
            {
                this.Chunk.BlockMesh[this.Coord] = new Empty();
            }
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (this.upper is not null)
        {
            this.upper.lower = null;
            this.upper.Chunk.BlockMesh[this.upper.Coord] = new Empty();
        }

        if (this.lower is not null)
        {
            this.lower.upper = null;
            this.lower.Chunk.BlockMesh[this.lower.Coord] = new Empty();
        }
    }

    public override Door Copy()
        => new ()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            WasUpdated = this.WasUpdated,
            IsOpened = this.IsOpened,
            IsLeft = this.IsLeft,
            upper = this.upper,
            lower = this.lower,
        };

    public void OnClick()
    {
        if (this.IsOpened)
        {
            foreach (var entity in this.Chunk.Scene.Entities)
            {
                if (entity.CollisionBox.GetGlobalBounds().Intersects(this.CollisionBox.GetGlobalBounds()))
                {
                    return;
                }
            }
        }

        this.IsOpened = !this.IsOpened;
        if (this.upper is not null)
        {
            this.upper.IsOpened = this.IsOpened;
        }

        if (this.lower is not null)
        {
            this.lower.IsOpened = this.IsOpened;
        }
    }
}
