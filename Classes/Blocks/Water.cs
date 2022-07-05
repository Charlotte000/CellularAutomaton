namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Entities;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Water : Block
{
    private static readonly Sprite[] SpriteSource;

    private static readonly int MaxAmount = 10;

    static Water()
    {
        Water.SpriteSource = new Sprite[Water.MaxAmount];

        var step = Block.Size / Water.MaxAmount;
        for (int i = 1; i <= Water.MaxAmount; i++)
        {
            Water.SpriteSource[i - 1] = new (Scene.Texture, new IntRect(80, 20 - (step * i), 20, step * i))
            {
                Origin = new (0, -20 + (step * i)),
            };
        }
    }

    public override Sprite Sprite { get => Water.SpriteSource[this.Amount - 1]; }

    public override int LightDiffusion { get => 25; }

    public override bool IsTransparent { get => true; }

    public override bool IsCollidable { get => false; }

    public override bool IsIndestructible { get => true; }

    public int Amount { get; set; } = Water.MaxAmount;

    public bool Push()
    {
        for (var dy = -1; dy > -10; dy--)
        {
            var coord = this.Coord + new Vector2i(0, dy);
            var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];

            if (block is null || block.IsCollidable)
            {
                return false;
            }

            if (block is Empty)
            {
                block.Chunk.BlockMesh[block.Coord] = this.Copy();
                this.Chunk.BlockMesh[this.Coord] = new Empty();
                return true;
            }
        }

        return false;
    }

    public override void OnFixedUpdate()
    {
        base.OnFixedUpdate();

        // Fall down
        if (this.FallDown())
        {
            return;
        }

        // Spread Out
        var deltaX = (Scene.RandomGenerator.Next(0, 2) * -2) + 1;
        if (this.SpreadOut(deltaX))
        {
            return;
        }
    }

    public override void OnCollision(IGameObject gameObject, Vector2f? normal)
    {
        base.OnCollision(gameObject, normal);

        if (gameObject is Entity entity)
        {
            entity.IsOnWater = true;
        }
    }

    public override Water Copy()
        => new ()
        {
            Coord = this.Coord,
            Amount = this.Amount,
            WasUpdated = this.WasUpdated,
        };

    private bool FallDown()
    {
        var coord = new Vector2i(this.Coord.X, this.Coord.Y + 1);
        var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
        if (block is not null && block is not Water && !block.IsCollidable)
        {
            block.Chunk.BlockMesh[block.Coord] = this.Copy();
            this.Chunk.BlockMesh[this.Coord] = new Empty();
            return true;
        }

        if (block is Water water && water.Amount < Water.MaxAmount)
        {
            water.WasUpdated = true;
            water.Amount += this.Amount;
            this.Amount = 0;
            if (water.Amount > Water.MaxAmount)
            {
                this.Amount = water.Amount - Water.MaxAmount;
                water.Amount = Water.MaxAmount;
            }

            if (this.Amount < 1)
            {
                this.Chunk.BlockMesh[this.Coord] = new Empty() { WasUpdated = true };
            }

            return true;
        }

        return false;
    }

    private bool SpreadOut(int deltaX)
    {
        var coord = new Vector2i(this.Coord.X + deltaX, this.Coord.Y);
        var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
        if (block is not null && block is not Water && !block.IsCollidable)
        {
            var prevAmount = this.Amount;

            this.Amount /= 2;

            block.Chunk.BlockMesh[block.Coord] = new Water() { WasUpdated = true, Amount = prevAmount - this.Amount };

            if (this.Amount < 1)
            {
                this.Chunk.BlockMesh[this.Coord] = new Empty();
            }

            return true;
        }

        if (block is Water water && water.Amount < Water.MaxAmount && water.Amount < this.Amount)
        {
            var delta = Math.Max((int)((this.Amount - water.Amount) / 2f), 1);
            water.WasUpdated = true;
            water.Amount += delta;
            this.Amount -= delta;
            if (this.Amount < 1)
            {
                this.Chunk.BlockMesh[this.Coord] = new Empty();
            }

            return true;
        }

        return false;
    }
}
