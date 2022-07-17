namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Walls;
using SFML.Graphics;
using SFML.System;

public class Liana : Block
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(120, 0, 20, 20));

    public override Sprite Sprite { get => Liana.SpriteSource; }

    public override int LightDiffusion { get => 15; }

    public override bool IsTransparent { get => true; }

    public override bool IsCollidable { get => false; }

    public override bool IsClimbable { get => true; }

    public override void OnCreate()
    {
        base.OnCreate();
        this.Expand(Application.RandomGenerator.Next(3, 10));
    }

    public override Liana Copy()
        => new ()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            WasUpdated = this.WasUpdated,
        };

    public void Expand(int length) // HACK: one length liana
    {
        for (int i = 1; i < length; i++)
        {
            var coord = new Vector2i(this.Coord.X, this.Coord.Y + i);
            var chunk = this.Chunk.Scene.ChunkMesh[coord];
            var block = chunk?.BlockMesh[coord];
            var wall = chunk?.WallMesh[coord];
            if (block is null || block is not Empty || wall is EmptyWall)
            {
                return;
            }

            var newLiana = new Liana();
            chunk!.BlockMesh[coord] = newLiana;
            this.Chunk.Scene.History.SaveBlock(newLiana);
        }
    }
}
