namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Entities;
using SFML.Graphics;
using SFML.System;

public class Tree : Block
{
    private static readonly List<Sprite> SpriteSource = new ()
    {
        new (Scene.Texture, new IntRect(0, 80, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(40, 80, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(80, 80, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(120, 80, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(160, 80, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(200, 80, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(240, 80, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(280, 80, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
    };

    private int treeType = Scene.RandomGenerator.Next(0, 8);

    public override Sprite Sprite { get => Tree.SpriteSource[this.treeType]; }

    public override int LightDiffusion { get => 15; }

    public override bool IsTransparent { get => true; }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(40, 200))
    { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) };

    public override void OnUpdate()
    {
        if (Scene.RandomGenerator.Next(0, 40) == 0)
        {
            this.Chunk.Scene.AddEntity(
                new Leaf(this.CollisionBox.Position - this.CollisionBox.Origin + new Vector2f(
                    this.CollisionBox.Size.X / 2,
                    0)));
        }
    }

    public override Block Copy()
        => new Tree()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            WasUpdated = this.WasUpdated,
            treeType = this.treeType,
        };
}
