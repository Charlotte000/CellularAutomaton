namespace CellularAutomaton.Classes.Blocks;

using Newtonsoft.Json;
using SFML.Graphics;
using SFML.System;

public abstract class Liquid : Block
{
    public static readonly int MaxAmount = 10;

    public override bool IsTransparent { get => true; }

    public override bool IsCollidable { get => false; }

    public override bool IsIndestructible { get => true; }

    [JsonRequired]
    public int Amount { get; set; } = Liquid.MaxAmount;

    public static Sprite[] GetSprites(int x, int y)
    {
        var sprites = new Sprite[MaxAmount];

        var step = Block.Size / Liquid.MaxAmount;
        for (int i = 1; i <= Liquid.MaxAmount; i++)
        {
            sprites[i - 1] = new (Scene.Texture, new IntRect(x, y + 20 - (step * i), 20, step * i))
            {
                Origin = new (0, -20 + (step * i)),
            };
        }

        return sprites;
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
        var deltaX = (Random.Shared.Next(0, 2) * -2) + 1;
        if (this.SpreadOut(deltaX))
        {
            return;
        }
    }

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
                block.Chunk.BlockMesh[block.Coord] = (Block)this.Copy();
                this.Chunk.BlockMesh[this.Coord] = new Empty();
                return true;
            }
        }

        return false;
    }

    private bool FallDown()
    {
        var coord = new Vector2i(this.Coord.X, this.Coord.Y + 1);
        var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
        if (block is not null && block is not Liquid && !block.IsCollidable)
        {
            block.Chunk.BlockMesh[block.Coord] = (Block)this.Copy();
            this.Chunk.BlockMesh[this.Coord] = new Empty();
            return true;
        }

        if (block is Liquid liquid && block.GetType() == this.GetType() && liquid.Amount < Liquid.MaxAmount)
        {
            liquid.WasUpdated = true;
            liquid.Amount += this.Amount;
            this.Amount = 0;
            if (liquid.Amount > Liquid.MaxAmount)
            {
                this.Amount = liquid.Amount - Liquid.MaxAmount;
                liquid.Amount = Liquid.MaxAmount;
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
        if (block is not null && block is not Liquid && !block.IsCollidable)
        {
            var prevAmount = this.Amount;

            this.Amount /= 2;

            var copy = (Liquid)this.Copy();
            copy.WasUpdated = true;
            copy.Amount = prevAmount - this.Amount;
            block.Chunk.BlockMesh[block.Coord] = copy;

            if (this.Amount < 1)
            {
                this.Chunk.BlockMesh[this.Coord] = new Empty();
            }

            return true;
        }

        if (block is Liquid liquid &&
            block.GetType() == this.GetType() &&
            liquid.Amount < Liquid.MaxAmount &&
            liquid.Amount < this.Amount)
        {
            var delta = Math.Max((int)((this.Amount - liquid.Amount) / 2f), 1);
            liquid.WasUpdated = true;
            liquid.Amount += delta;
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
