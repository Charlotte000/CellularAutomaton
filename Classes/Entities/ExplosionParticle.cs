namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class ExplosionParticle : Entity, ILightSource
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new (334, 80, 6, 6));

    public override Sprite Sprite { get => ExplosionParticle.SpriteSource; }

    public override Vector2f Gravity { get => new (0, .2f); }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(6, 6));

    public override bool IsCollidable { get => false; }

    public override float AirResistance { get => .99f; }

    public override float LiquidResistance { get => .87f; }

    public int Brightness { get => 300; }

    public override void OnCollision(IGameObject gameObject, Vector2f? contactNormal)
    {
        base.OnCollision(gameObject, contactNormal);

        if (gameObject.IsCollidable)
        {
            this.Vel *= 0;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (this.Vel.XYEquals(0))
        {
            this.Scene.RemoveEntity(this);
        }
    }

    public override ExplosionParticle Copy()
        => new ();
}
