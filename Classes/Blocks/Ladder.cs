namespace CellularAutomaton.Classes.Blocks;

using SFML.Graphics;
using SFML.System;

public class Ladder : Block
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new IntRect(100, 0, 20, 20));

    public override Sprite Sprite { get => Ladder.SpriteSource; }

    public override int LightDiffusion { get => 15; }

    public override bool IsCollidable { get => false; }

    public override bool IsTransparent { get => true; }

    public override bool IsClimbable { get => true; }

    public override void OnCreate()
    {
        base.OnCreate();

        this.Expand(10);
    }

    public override Ladder Copy()
        => new ()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coord = this.Coord,
            WasUpdated = this.WasUpdated,
        };

    public void Expand(int length)
    {
        for (int i = 1; i < length; i++)
        {
            var coord = new Vector2i(this.Coord.X, this.Coord.Y + i);
            var block = this.Chunk.Scene.ChunkMesh[coord]?.BlockMesh[coord];
            if (block is not Empty)
            {
                return;
            }

            var newLadder = new Ladder();
            block.Chunk.BlockMesh[block.Coord] = newLadder;
            this.Chunk.Scene.BlockHistory.SaveBlock(block.Chunk, newLadder);
        }
    }
}
