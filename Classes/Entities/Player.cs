namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Blocks;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

public class Player : Entity, ILivingEntity
{
    private static readonly Sprite SpriteSource;

    static Player()
    {
        using var renderTexture = new RenderTexture(19, 39);
        renderTexture.Draw(new RectangleShape(new Vector2f(19, 39)) { FillColor = Color.Red });
        renderTexture.Display();
        Player.SpriteSource = new Sprite(new Texture(renderTexture.Texture));
    }

    public Player(float x, float y)
    {
        this.CollisionBox.Position = new Vector2f(x, y);
    }

    public override Vector2f Gravity
    { get => new Vector2f(0, 1) * (this.IsOnWater ? .2f : 1) * (this.IsClimbing ? 0 : 1); }

    public override float AirResistance { get => this.IsOnWater || this.IsClimbing ? .8f : .87f; }

    public override float PressureOut { get => .05f; }

    public override Sprite Sprite { get => Player.SpriteSource; }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(19, 39));

    public override bool IsCollidable { get => true; }

    public bool IsOnGround { get; set; } = false;

    public bool IsOnWater { get; set; } = false;

    public bool IsClimbing { get; set; } = false;

    public override void OnUpdate()
    {
        this.Control();
        base.OnUpdate();
    }

    public override void OnCollision(IGameObject entity, Vector2f? contactNormal)
    {
        base.OnCollision(entity, contactNormal);
        this.IsOnGround |= contactNormal?.Y == -1 && entity.IsCollidable;
        this.IsClimbing |= entity is Block block && block.IsClimbable;
    }

    public override IGameObject Copy()
        => new Player(this.CollisionBox.Position.X, this.CollisionBox.Position.Y);

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

        if (Keyboard.IsKeyPressed(Keyboard.Key.W) && (this.IsClimbing || this.IsOnWater))
        {
            this.Vel += new Vector2f(0, -.7f);
        }

        if (Keyboard.IsKeyPressed(Keyboard.Key.S) && (this.IsClimbing || this.IsOnWater))
        {
            this.Vel += new Vector2f(0, .7f);
        }
    }
}
