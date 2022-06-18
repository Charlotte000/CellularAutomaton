namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Walls;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class Liana : Block, IClimbable
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(Block.Size * 5, 0, Block.Size, Block.Size));

    public override Sprite Sprite { get => Liana.SpriteSource; }

    public override int LightDiffusion { get => 15; }

    public override bool IsTransparent { get => true; }

    public override void OnCreate()
        => this.Expand(Scene.RandomGenerator.Next(3, 10));

    public override Block Copy()
        => new Liana()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
        };

    public void Expand(int length) // HACK: one length liana
    {
        for (int i = 1; i < length; i++)
        {
            var coord = new Vector2i(this.Coords.X, this.Coords.Y + i);
            var chunk = this.Chunk.Scene.ChunkMesh[coord];
            var block = chunk?.BlockMesh[coord];
            var wall = chunk?.WallMesh[coord];
            if (block is null || block is not Empty || wall is EmptyWall)
            {
                return;
            }

            this.Chunk.Scene.SetBlock(new Liana(), coord, false, true);
        }
    }
}
