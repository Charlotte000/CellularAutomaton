namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Player : Entity
{
    private static readonly Sprite SpriteSource;

    static Player()
    {
        using var renderTexture = new RenderTexture(19, 39);
        using var shape = new RectangleShape(new Vector2f(19, 39)) { FillColor = Color.Red };
        renderTexture.Draw(shape);
        renderTexture.Display();
        Player.SpriteSource = new Sprite(new Texture(renderTexture.Texture));
    }

    public override Vector2f Gravity
    { get => new Vector2f(0, 1) * (this.IsClimbing ? 0 : 1); }

    public override float AirResistance { get => this.IsClimbing ? .7f : .87f; }

    public override Vector2f LiquidLift { get => new (0, -.7f); }

    public override float PressureOut { get => .05f; }

    public override Sprite Sprite { get => Player.SpriteSource; }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(19, 39));

    public override bool IsCollidable { get => true; }

    public bool IsClimbing { get; set; } = false;

    public override void OnUpdate()
    {
        this.Control();
        base.OnUpdate();
    }

    public override void OnCollision(IGameObject gameObject, Vector2f? contactNormal)
    {
        base.OnCollision(gameObject, contactNormal);
        this.IsClimbing |= gameObject is Block block && block.IsClimbable;
    }

    public override IGameObject Copy()
        => new Player();

    internal override void Collision()
    {
        this.IsClimbing = false;
        base.Collision();
    }

    private void Control()
    {
        if (Keyboard.IsKeyPressed(Keyboard.Key.D))
        {
            this.Vel += new Vector2f(.7f, 0);
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.A))
        {
            this.Vel += new Vector2f(-.7f, 0);
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.W) && this.IsOnGround)
        {
            this.Vel += new Vector2f(0, -15f);
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.W) && (this.IsClimbing || this.IsOnLiquid))
        {
            this.Vel += new Vector2f(0, -1.7f);
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.S) && this.IsClimbing)
        {
            this.Vel += new Vector2f(0, .7f);
        }
    }
}
