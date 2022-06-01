namespace CellularAutomaton.Classes.Blocks;

using CellularAutomaton.Classes.Entities;
using SFML.Graphics;
using SFML.System;

public class Tree : Block
{
    private static readonly List<Sprite> SpriteSource = new ()
    {
        new (Scene.Texture, new IntRect(0, 40, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(40, 40, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(80, 40, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(120, 40, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(160, 40, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(200, 40, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(240, 40, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
        new (Scene.Texture, new IntRect(280, 40, 40, 200)) { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) },
    };

    private int treeType = Scene.RandomGenerator.Next(0, 8);

    public override Sprite Sprite { get => Tree.SpriteSource[this.treeType]; }

    public override int LightDiffusion { get => 15; }

    public override bool IsTransparent { get => true; }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(40, 200))
    { Origin = new Vector2f(Block.Size / 2, 200 - Block.Size) };

    public override void OnUpdate(Scene scene)
    {
        if (Scene.RandomGenerator.Next(0, 40) == 0)
        {
            scene.Entities.Add(new Leaf(this.CollisionBox.Position - this.CollisionBox.Origin + new Vector2f(this.CollisionBox.Size.X / 2, 0)));
        }
    }

    public override Block Copy()
        => new Tree()
        {
            CollisionBox = new RectangleShape(this.CollisionBox),
            Coords = this.Coords,
            Light = this.Light,
            Wall = this.Wall?.Copy(),
            WasUpdated = this.WasUpdated,
            treeType = this.treeType,
        };
}
