namespace CellularAutomaton.Classes.Entities;

using CellularAutomaton.Classes.Utils;
using CellularAutomaton.Interfaces;
using SFML.Graphics;
using SFML.System;

public class ExplosionParticle : Entity, ILightSource, ITimedEntity
{
    private static readonly Sprite SpriteSource = new (Scene.Texture, new (334, 80, 6, 6));

    public override Sprite Sprite { get => ExplosionParticle.SpriteSource; }

    public override Vector2f Gravity { get => new (0, .01f); }

    public override RectangleShape CollisionBox { get; set; } = new (new Vector2f(6, 6));

    public override bool IsCollidable { get => false; }

    public override float AirResistance { get => .99f; }

    public override float LiquidResistance { get => .87f; }

    public int Brightness { get => 300; }

    public bool IsLifeTimeActive { get; set; } = true;

    public float LifeTimeStart { get; set; }

    public float LifeTimeEnd { get; set; }

    public float LifeTime { get; set; } = 1 + (Random.Shared.NextSingle() * .5f);

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

    public void OnTimeOut()
    {
        this.Scene.RemoveEntity(this);
    }
}
