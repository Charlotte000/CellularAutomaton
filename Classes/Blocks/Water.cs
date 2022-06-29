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

    public static bool Push(Scene scene, Water water)
    {
        var coord = water.Coord + new Vector2i(0, -1);
        var block = scene.ChunkMesh[coord]?.BlockMesh[coord];
        if (block is Empty || block is Water)
        {
            if (Water.Push(scene, block, water.Amount))
            {
                water.Chunk.BlockMesh[water.Coord] = new Empty();
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

    private static bool Push(Scene scene, Block block, int amount)
    {
        if (amount <= 0)
        {
            return true;
        }

        if (block is Empty)
        {
            block.Chunk.BlockMesh[block.Coord] = new Water() { WasUpdated = true, Amount = amount };
            return true;
        }

        if (block is Water waterBlock)
        {
            if (waterBlock.Amount + amount <= Water.MaxAmount)
            {
                waterBlock.Amount += amount;
                return true;
            }

            var coord = block.Coord + new Vector2i(0, -1);
            var neighbour = scene.ChunkMesh[coord]?.BlockMesh[coord];
            if (neighbour is Water || neighbour is Empty)
            {
                if (Water.Push(scene, neighbour, amount - Water.MaxAmount + waterBlock.Amount))
                {
                    waterBlock.Amount = Water.MaxAmount;
                    return true;
                }
            }
        }

        return false;
    }

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
