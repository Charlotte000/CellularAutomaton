namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Water : Block
{
    private static readonly Sprite[] SpriteSource = new Sprite[]
    {
        new (Scene.Texture, new IntRect(Block.Size, Block.Size * 3 / 4, Block.Size, Block.Size / 4))
        {
            Origin = new Vector2f(0, -Block.Size * 3 / 4),
        },
        new (Scene.Texture, new IntRect(Block.Size, Block.Size / 2, Block.Size, Block.Size / 2))
        {
            Origin = new Vector2f(0, -Block.Size / 2),
        },
        new (Scene.Texture, new IntRect(Block.Size, Block.Size / 4, Block.Size, Block.Size * 3 / 4))
        {
            Origin = new Vector2f(0, -Block.Size / 4),
        },
        new (Scene.Texture, new IntRect(Block.Size, 0, Block.Size, Block.Size)),
    };

    public override Sprite Sprite { get => Water.SpriteSource[this.Amount - 1]; }

    public override int LightDiffusion { get => 10; }

    public override bool IsTransparent { get => true; }

    public int Amount { get; set; } = 4;

    public static bool Push(Scene scene, Water water)
    {
        var coord = water.Coords + new Vector2i(0, -1);
        var block = scene.ChunkMesh[coord]?.BlockMesh[coord];
        if (block is Empty || block is Water)
        {
            if (Water.Push(scene, block, water.Amount))
            {
                scene.SetBlock(new Empty(), water.Coords, false, true);
                return true;
            }
        }

        return false;
    }

    public override void OnUpdate()
    {
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

    public override void OnCollision(IEntity entity, Vector2f? normal)
    {
        if (entity is ILivingEntity livingEntity)
        {
            livingEntity.IsOnWater = true;
        }
    }

    public override Block Copy()
        => new Water()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
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
            scene.SetBlock(new Water() { WasUpdated = true, Light = block.Light, Amount = amount }, block.Coords, false);
            return true;
        }

        if (block is Water waterBlock)
        {
            if (waterBlock.Amount + amount <= 4)
            {
                waterBlock.Amount += amount;
                return true;
            }

            var coord = block.Coords + new Vector2i(0, -1);
            var neighbour = scene.ChunkMesh[coord]?.BlockMesh[coord];
            if (neighbour is Water || neighbour is Empty)
            {
                if (Water.Push(scene, neighbour, amount - 4 + waterBlock.Amount))
                {
                    waterBlock.Amount = 4;
                    return true;
                }
            }
        }

        return false;
    }

    private bool FallDown()
    {
        var coord = new Vector2i(this.Coords.X, this.Coords.Y + 1);
        var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
        if (block is not null && block is not Water && block is not ICollidable)
        {
            this.Chunk.Scene.SetBlock(this.Copy(), this.Coords.X, this.Coords.Y + 1, false);
            this.Chunk.Scene.SetBlock(new Empty() { WasUpdated = true, Light = this.Light }, this.Coords, false);
            return true;
        }

        if (block is Water water && water.Amount < 4)
        {
            water.WasUpdated = true;
            water.Amount += this.Amount;
            this.Amount = 0;
            if (water.Amount > 4)
            {
                this.Amount = water.Amount - 4;
                water.Amount = 4;
            }

            if (this.Amount < 1)
            {
                this.Chunk.Scene.SetBlock(new Empty() { WasUpdated = true, Light = this.Light }, this.Coords, false);
            }

            return true;
        }

        return false;
    }

    private bool SpreadOut(int deltaX)
    {
        var coord = new Vector2i(this.Coords.X + deltaX, this.Coords.Y);
        var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
        if (block is not null && block is not Water && block is not ICollidable)
        {
            var prevAmount = this.Amount;

            this.Amount /= 2;

            this.Chunk.Scene.SetBlock(
                new Water()
                {
                    Amount = prevAmount - this.Amount,
                    Light = this.Light,
                    WasUpdated = true,
                },
                this.Coords.X + deltaX,
                this.Coords.Y,
                false);

            if (this.Amount < 1)
            {
                this.Chunk.Scene.SetBlock(new Empty() { WasUpdated = true, Light = this.Light }, this.Coords, false);
            }

            return true;
        }

        if (block is Water water && water.Amount < 4 && water.Amount < this.Amount)
        {
            water.WasUpdated = true;
            water.Amount++;
            this.Amount--;
            if (this.Amount < 1)
            {
                this.Chunk.Scene.SetBlock(new Empty() { WasUpdated = true, Light = this.Light }, this.Coords, false);
            }

            return true;
        }

        return false;
    }
}
